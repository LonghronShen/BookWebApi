using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BookWebApi.Models
{

	public class Book
	{

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("author")]
		public string Author { get; set; }

		[JsonProperty("publisher")]
		public string Publisher { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as Book;
			if (other != null)
			{
				return this.Id == other.Id &&
					this.Author == other.Author &&
					this.Title == other.Title &&
					this.Publisher == other.Publisher;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode() ^
				this.Author.GetHashCode() ^
				this.Title.GetHashCode() ^
				this.Publisher.GetHashCode();
		}

		public static bool operator ==(Book a, Book b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(Book a, Book b)
		{
			return !object.Equals(a, b);
		}

	}

}