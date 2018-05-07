using Microsoft.EntityFrameworkCore;

namespace BookWebApi.Models
{

    public class BookDbContext
        : DbContext
    {

        public DbSet<Book> Books { get; set; }

        public BookDbContext(DbContextOptions<BookDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(c =>
            {
                c.HasKey(x => x.Id);
                c.Property(x => x.Id).UseNpgsqlSerialColumn();
            });
            base.OnModelCreating(modelBuilder);
        }

    }

}