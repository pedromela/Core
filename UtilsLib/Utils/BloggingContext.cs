using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace UtilsLib.Utils
{
    public class BloggingContext : DbContext
    {
        public static readonly ILoggerFactory LoggerF = LoggerFactory.Create(builder => {  });

        //public DbSet<Blog> Blogs { get; set; }
        //public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=SQL-INSTANCE-NAME;Database=dbname1;Trusted_Connection=True;MultipleActiveResultSets=true");
            options.UseLoggerFactory(LoggerF);
        }
    }  
}
