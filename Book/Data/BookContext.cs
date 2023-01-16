using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Book.Models;

namespace Book.Data
{
    public class BookContext : DbContext
    {
        public BookContext (DbContextOptions<BookContext> options)
            : base(options)
        {
        }

        public DbSet<Book.Models.book> book { get; set; } = default!;

        public DbSet<Book.Models.usersaccounts> usersaccounts { get; set; } = default!;

        public DbSet<Book.Models.Orders> Orders { get; set; } = default!;
        public DbSet<Book.Models.report> report { get; set; } = default!;
    }
}
