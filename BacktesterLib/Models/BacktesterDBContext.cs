using BrokerLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BacktesterLib.Models
{
    public class BacktesterDBContext : DbContext
    {
        public DbSet<BacktesterTransaction> BacktesterTransactions { get; set; }

        public DbSet<BacktesterScore> BacktesterScores { get; set; }
        public DbSet<BacktesterEquity> BacktesterEquitys { get; set; }

        public BacktesterDBContext() : base()
        {

        }

        public BacktesterDBContext(DbContextOptions<BacktesterDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<UserBotRelation>()
            //    .HasOne(p => p.Blog)
            //    .WithMany(b => b.Posts)
            //    .HasForeignKey(p => p.BlogUrl)
            //    .HasPrincipalKey(b => b.Url);

            //modelBuilder.Entity<Candle>()
            //    .HasKey(c => new { c.TimeFrame, c.Timestamp, c.Symbol });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }


        ///////////////////////////////////////////////////////////////////
        //////////////////////// STATIC FUNCTIONS /////////////////////////
        ///////////////////////////////////////////////////////////////////

        private static string connectionString = null;
        public static DbContextOptionsBuilder optionsBuilder = null;

        public static string GetConnectionString()
        {
            try
            {
                if (connectionString == null)
                {
                    var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    IConfigurationRoot Configuration = builder.Build();
                    connectionString = Configuration.GetConnectionString("BacktesterConnection");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return connectionString;
        }

        public static DbContextOptionsBuilder GetOptionsbuilder()
        {
            if (optionsBuilder == null)
            {
                optionsBuilder = new DbContextOptionsBuilder<BacktesterDBContext>();
                optionsBuilder.UseSqlServer(GetConnectionString());
            }
            return optionsBuilder;
        }

        public static T Execute<T>(Func<BacktesterDBContext, T> func, bool insertOrUpdate = false)
        {
            //using (BacktesterDBContext backtesterContext = BacktesterDBContext.newDBContext())
            //{
                BacktesterDBContext backtesterContext = BacktesterDBContext.newDBContext();
                if (insertOrUpdate)
                {
                    func.Invoke(backtesterContext);
                }
                else
                {
                    try
                    {
                        var result = func.Invoke(backtesterContext);
                        if (typeof(T) == typeof(Array))
                        {
                            var resultCast = result as Array;
                            if (resultCast.Length > 0)
                            {
                                return result;
                            }
                        }
                        else if (result != null)
                        {
                            return result;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            //}
            return default(T);
        }

        //public static BacktesterDBContext newDBContext()
        //{
        //    return new BacktesterDBContext((DbContextOptions<BacktesterDBContext>)GetOptionsbuilder().Options);
        //}

        private static BacktesterDBContext _context = new BacktesterDBContext((DbContextOptions<BacktesterDBContext>)GetOptionsbuilder().Options);
        public static BacktesterDBContext newDBContext()
        {
            return _context;
        }
    }
}
