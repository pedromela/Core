using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TelegramLib.Models
{
    public class TelegramDBContext : DbContext
    {

        public DbSet<Messages> Messages { get; set; }


        public DbSet<TelegramEquity> TelegramEquities { get; set; }

        public DbSet<TelegramUserBotRelation> TelegramUserBotRelations { get; set; }
        public DbSet<TelegramTransaction> TelegramTransactions { get; set; }
        public DbSet<ChannelScore> ChannelScores { get; set; }
        public DbSet<TelegramParameters> TelegramParameters { get; set; }
        public DbSet<TelegramScore> TelegramScores { get; set; }

        public TelegramDBContext() : base()
        {

        }

        public TelegramDBContext(DbContextOptions<TelegramDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TelegramUserBotRelation>()
                .HasKey(c => new { c.UserId, c.BotId });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }

        public List<TelegramParameters> GetBotsFromDB()
        {
            return TelegramParameters.ToList();
        }

        public List<ChannelScore> GetScoresFromDB()
        {
            return ChannelScores.ToList();
        }

        public List<TelegramTransaction> GetTransactionsFromDB()
        {
            return TelegramTransactions.ToList();
        }

        ///////////////////////////////////////////////////////////////////
        //////////////////////// STATIC FUNCTIONS /////////////////////////
        ///////////////////////////////////////////////////////////////////
        ///
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
                    connectionString = Configuration.GetConnectionString("TelegramConnection");
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
                optionsBuilder = new DbContextOptionsBuilder<TelegramDBContext>();
                optionsBuilder.UseSqlServer(GetConnectionString());
            }
            return optionsBuilder;
        }

        public static TelegramDBContext newDBContext()
        {
            return new TelegramDBContext((DbContextOptions<TelegramDBContext>)GetOptionsbuilder().Options);
        }
    }
}

