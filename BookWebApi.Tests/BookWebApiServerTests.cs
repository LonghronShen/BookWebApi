using BookWebApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BookWebApi.Tests
{

	public class BookWebApiServerTests
		: IClassFixture<TestFixture<BookWebApi.Startup>>
	{

		private readonly HttpClient _client;
		private readonly AuthenticationHeaderValue _authenticationHeaderValue;

		private List<Book> Books { get; }

		public BookWebApiServerTests(TestFixture<BookWebApi.Startup> fixture)
		{
			this._client = fixture.Client;
			var byteArray = Encoding.ASCII.GetBytes("user:mypass");
			this._authenticationHeaderValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

			this.Books = new List<Book>()
			{
				new Book()
				{
					Id = 1,
					Title = "CLR via C#",
					Author = "Jeffrey Richter",
					Publisher = "Microsoft Press"
				},
				new Book()
				{
					Id = 2,
					Title = "C How to Program",
					Author = "Paul Deitel, Harvey Deitel",
					Publisher = "Pearson International"
				}
			};
		}

		[Fact]
		public async Task ReturnUnauthorizedWhenStart()
		{
			// Act
			this._client.DefaultRequestHeaders.Authorization = null;
			var response = await this._client.GetAsync("/api/book");

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task ReturnNotFoundWhenAuthenticated()
		{
			// Act
			this._client.DefaultRequestHeaders.Authorization = this._authenticationHeaderValue;

			var response = await this._client.GetAsync("/api/book");

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}

		[Fact]
		public async Task Return1WhenBookAdded()
		{
			// Act
			this._client.DefaultRequestHeaders.Authorization = this._authenticationHeaderValue;

			var response = await this._client.PostAsync("/api/book",
				new StringContent(JsonConvert.SerializeObject(this.Books[0]), Encoding.UTF8, "application/json"));

			// Assert
			response = response.EnsureSuccessStatusCode();
			var responseContent = await response.Content.ReadAsStringAsync();
			Assert.Equal("1", responseContent);
		}

		[Fact]
		public async Task ReturnOkWhenBookDeleted()
		{
			// Act
			this._client.DefaultRequestHeaders.Authorization = this._authenticationHeaderValue;

			var response = await this._client.PostAsync("/api/book",
				new StringContent(JsonConvert.SerializeObject(this.Books[0]), Encoding.UTF8, "application/json"));

			// Assert
			response = response.EnsureSuccessStatusCode();
			var responseContent = await response.Content.ReadAsStringAsync();
			Assert.Equal("1", responseContent);

			response = await this._client.DeleteAsync("/api/book/1");
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		[Fact]
		public async Task ReturnOkWhenPutBook()
		{
			// Act
			this._client.DefaultRequestHeaders.Authorization = this._authenticationHeaderValue;

			var response = await this._client.PostAsync("/api/book",
				new StringContent(JsonConvert.SerializeObject(this.Books[0]), Encoding.UTF8, "application/json"));

			// Assert
			response = response.EnsureSuccessStatusCode();
			var responseContent = await response.Content.ReadAsStringAsync();
			Assert.Equal("1", responseContent);

			response = await this._client.PutAsync("/api/book/1",
				new StringContent(JsonConvert.SerializeObject(this.Books[1]), Encoding.UTF8, "application/json"));
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			response = await this._client.GetAsync("/api/book/1");
			response = response.EnsureSuccessStatusCode();
			responseContent = await response.Content.ReadAsStringAsync();
			var item = JsonConvert.DeserializeObject<Book>(responseContent);
			Assert.Equal(this.Books[1].Title, item.Title);
		}

	}

}