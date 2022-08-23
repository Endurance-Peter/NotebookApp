using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notebook.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Infrastructure.Configurations
{
    //public class AppDbContext : IdentityDbContext
    //{
    //    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    //    public DbSet<User>? Users { get; set; }
    //}

    public class ApplicationContext : IdentityDbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) 
        {
            
        }

        public virtual DbSet<User>? Users { get; set; }
        public virtual DbSet<RefreshToken>? RefreshTokens { get; set; }
    }
}
