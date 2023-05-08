using BookStoreMVC.Areas.Identity.Data;
using BookStoreMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStoreMVC.Data
{
    public class BookStoreMVCContext : IdentityDbContext<BookStoreMVCUser>
    {
        public BookStoreMVCContext(DbContextOptions<BookStoreMVCContext> options)
            : base(options)
        {
        }

        public DbSet<BookStoreMVC.Models.Book>? Book { get; set; }

        public DbSet<BookStoreMVC.Models.Author>? Author { get; set; }

        public DbSet<BookStoreMVC.Models.Review>? Review { get; set; }

        public DbSet<BookStoreMVC.Models.Genre>? Genre { get; set; }

        public DbSet<BookStoreMVC.Models.BookGenre>? bookGenres { get; set; }

        public DbSet<BookStoreMVC.Models.UserBooks>? userBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<BookGenre>()
                .HasOne<Book>(g => g.Book)
                .WithMany(g => g.BookGenres)
                .HasForeignKey(g => g.BookId);

            builder.Entity<BookGenre>()
                .HasOne<Genre>(g => g.Genre)
                .WithMany(g => g.bookGenres)
                .HasForeignKey(g => g.GenreId);

            builder.Entity<Book>()
                .HasOne<Author>(a => a.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(a => a.AuthorId);

            builder.Entity<Review>()
                .HasOne<Book>(r => r.Book)
                .WithMany(r => r.Reviews)
                .HasForeignKey(r => r.BookId);

            builder.Entity<UserBooks>()
                .HasOne<Book>(u => u.Book)
                .WithMany(u => u.Users)
                .HasForeignKey(u => u.BookId);
        }
    }
}
