using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookWebApi.Controllers
{

    [Authorize]
    [Route("api/book")]
    public class BooksController
        : Controller
    {

        private readonly BookDbContext _dbContext;

        public BooksController(BookDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await this._dbContext.Books.AsNoTracking().ToListAsync();
            if (result.Count > 0)
            {
                return this.Json(result);
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpGet("filter:query")]
        public async Task<IActionResult> Get(string title, string author, string publisher, int startIndex, int count)
        {
            var query = this._dbContext.Books.AsNoTracking();
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(x => EF.Functions.Like(x.Title, title));
            }
            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(x => EF.Functions.Like(x.Author, author));
            }
            if (!string.IsNullOrEmpty(publisher))
            {
                query = query.Where(x => EF.Functions.Like(x.Publisher, publisher));
            }
            if (startIndex > 0)
            {
                query = query.Skip(startIndex);
            }
            if (count > 0)
            {
                query = query.Take(startIndex);
            }
            var result = await query.ToListAsync();
            if (result.Count > 0)
            {
                return this.Json(result);
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await this._dbContext.Books.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                return this.Json(item);
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Book model)
        {
            model.Id = 0;
            var book = await this._dbContext.Books.AddAsync(model);
            var r = await this._dbContext.SaveChangesAsync();
            if (r > 0)
            {
                return this.Json(book.Entity.Id);
            }
            else
            {
                return this.StatusCode(500);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Book model)
        {
            var item = await this._dbContext.Books.FindAsync(id);
            if (item == null)
            {
                return this.NotFound();
            }
            item.Title = model.Title;
            item.Author = model.Author;
            item.Publisher = model.Publisher;
            var r = await this._dbContext.SaveChangesAsync();
            if (r > 0)
            {
                return this.Json(item);
            }
            else
            {
                return this.StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await this._dbContext.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                this._dbContext.Books.Remove(item);
                return this.Json(item);
            }
            else
            {
                return this.NotFound();
            }
        }

    }

}