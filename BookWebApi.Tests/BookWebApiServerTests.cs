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

	[TestCaseOrderer("BookWebApi.Tests.PriorityOrderer", "BookWebApi.Tests")]
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
                new Book("CLR via C#", "Jeffrey Richter", "Microsoft Press"),
                new Book("C How to Program", "Paul Deitel, Harvey Deitel",  "Pearson International")
			};
		}

		[Fact, TestPriority(0)]
		public void BookEqualsTest()
		{
            Assert.Equal(
                new Book("CLR via C#", "Jeffrey Richter", "Microsoft Press"), 
                new Book("CLR via C#", "Jeffrey Richter", "Microsoft Press")
            );
        }

		[Fact, TestPriority(1)]
		public async Task ReturnUnauthorizedWhenStart()
		{
			// Act
			this._client.DefaultRequestHeaders.Authorization = null;
			var response = await this._client.GetAsync("/api/book");

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact, TestPriority(2)]
		public async Task ReturnNotFoundWhenAuthenticated()
		{
			// Act
			this._client.DefaultRequestHeaders.Authorization = this._authenticationHeaderValue;

			var response = await this._client.GetAsync("/api/book");

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}

		[Fact, TestPriority(3)]
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

		[Fact, TestPriority(4)]
		public async Task ReturnOkWhenBookDeleted()
		{
			// Act
			this._client.DefaultRequestHeaders.Authorization = this._authenticationHeaderValue;
			var response = await this._client.DeleteAsync("/api/book/1");

			// Assert
			response = response.EnsureSuccessStatusCode();
			var responseContent = await response.Content.ReadAsStringAsync();
			Assert.Equal(JsonConvert.SerializeObject(this.Books[0]), responseContent);
		}

		[Fact, TestPriority(5)]
		public async Task ReturnOkWhenPutBook()
		{
			// Act
			this._client.DefaultRequestHeaders.Authorization = this._authenticationHeaderValue;

			var response = await this._client.PostAsync("/api/book",
				new StringContent(JsonConvert.SerializeObject(this.Books[0]), Encoding.UTF8, "application/json"));

			// Assert
			response.EnsureSuccessStatusCode();
			var responseContent = await response.Content.ReadAsStringAsync();

			response = await this._client.PutAsync($"/api/book/{responseContent}",
				new StringContent(JsonConvert.SerializeObject(this.Books[1]), Encoding.UTF8, "application/json"));
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			response = await this._client.GetAsync($"/api/book/{responseContent}");
			response = response.EnsureSuccessStatusCode();
			responseContent = await response.Content.ReadAsStringAsync();
			var item = JsonConvert.DeserializeObject<Book>(responseContent);
			Assert.Equal(this.Books[1].Title, item.Title);
		}

	}

}