using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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
                c.Property(x => x.Id).ValueGeneratedOnAdd()
                    .UseMySqlIdentityColumn()
                    .UseNpgsqlSerialColumn();
                c.Property(x => x.CreationTime).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            });
            base.OnModelCreating(modelBuilder);
        }

    }

}