using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Workshop.Areas.Identity.Data;
using Workshop.Models;

namespace Workshop.Data
{
    public class WorkshopContext : IdentityDbContext<WorkshopUser>
    {
        public WorkshopContext (DbContextOptions<WorkshopContext> options)
            : base(options)
        {
        }

        public DbSet<Workshop.Models.Author> Author { get; set; } = default!;
        public DbSet<Workshop.Models.Book> Book { get; set; } = default!;
        public DbSet<Workshop.Models.Genre> Genre { get; set; } = default!;
        public DbSet<Workshop.Models.Review> Review { get; set; } = default!;
        public DbSet<Workshop.Models.UserBooks> UserBooks { get; set; } = default!;
        public DbSet<Workshop.Models.BookGenre> BookGenre { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
