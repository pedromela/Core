using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilsLib.Utils;

namespace BotLib.Models
{
    public class BotDBContext : MyDbContext
    {
        public DbSet<BotParameters> BotsParameters { get; set; }
        public DbSet<BotParametersChanges> BotParametersChanges { get; set; }
        public DbSet<BotParametersRanking> BotParametersRankings { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<UserBotRelation> UserBotRelations { get; set; }
        public DbSet<ConditionStrategyData> ConditionStrategiesData { get; set; }
        public DbSet<Profit> Profits { get; set; }

        public string _connectionString = null;


        public BotDBContext() : base()
        {

        }

        public BotDBContext(string connectionString, DbContextOptions<BotDBContext> options) : base(options)
        {
            _connectionString = connectionString;

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<UserBotRelation>()
            //    .HasOne(p => p.Blog)
            //    .WithMany(b => b.Posts)
            //    .HasForeignKey(p => p.BlogUrl)
            //    .HasPrincipalKey(b => b.Url);

            modelBuilder.Entity<UserBotRelation>()
                .HasKey(c => new { c.UserId, c.BotId });

            modelBuilder.Entity<Profit>()
                .HasKey(c => new { c.BotId, c.Timestamp });

            modelBuilder.Entity<BotParametersRanking>()
                .Property(c => c.Rank)
                .ValueGeneratedNever();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (string.IsNullOrEmpty(_connectionString) || _connectionString == GetConnectionString("BotConnection"))
            {
                optionsBuilder.UseSqlServer(GetConnectionString("BotConnection"));
            }
            else if (_connectionString == GetConnectionString("BotConnectionClient"))
            {
                optionsBuilder.UseSqlServer(GetConnectionString("BotConnectionClient"));
            }
        }

        public List<BotParameters> GetBotsFromDB()
        {
            return BotsParameters.AsNoTracking().ToList();
        }

        public List<ConditionStrategyData> GetStrategiesFromDB()
        {
            return ConditionStrategiesData.AsNoTracking().ToList();
        }

        public int GetStrategiesCountFromDB()
        {
            return ConditionStrategiesData.AsNoTracking().Count();
        }

        public List<Score> GetScoresFromDB()
        {
            return Scores.AsNoTracking().ToList();
        }

        public int GetBotRankingsCountFromDB()
        {
            return BotParametersRankings.Count();
        }

        public void SaveBotRankings(List<BotParametersRanking> rankingBots) 
        {
            if (GetBotRankingsCountFromDB() > 0)
            {
                RemoveBotRankingsFromDB();
            }
            BotParametersRankings.AddRange(rankingBots);
            SaveChanges();
        }

        public void RemoveBotRankingsFromDB()
        {
            BotParametersRankings.RemoveRange(BotParametersRankings);
        }

        public List<BotParametersRanking> GetBotRankingsFromDB()
        {
            return BotParametersRankings.AsNoTracking().ToList();
        }

        public static T Execute<T>(Func<BotDBContext, T> func, bool executeAll = false)
        {
            foreach (var provider in providers)
            {
                using (BotDBContext brokerContext = (BotDBContext)provider.GetDBContext())
                {
                    if (executeAll)
                    {
                        func.Invoke(brokerContext);
                    }
                    else
                    {
                        try
                        {
                            return func.Invoke(brokerContext);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            return default(T);
        }


        ///////////////////////////////////////////////////////////////////
        //////////////////////// STATIC FUNCTIONS /////////////////////////
        ///////////////////////////////////////////////////////////////////

        public static Dictionary<string, string> connectionStringMap = new Dictionary<string, string>();

        public static DbContextOptionsBuilder optionsBuilder = null;

        public static BotBDProvider[] providers = null;

        public static void InitProviders()
        {
            providers = new BotBDProvider[2];
            var provider1 = new BotBDProvider(new DBSettings("BotConnection"));
            var provider2 = new BotBDProvider(new DBSettings("BotConnectionClient"));
            List<BotBDProvider> providersList = new List<BotBDProvider>();

            if (CanAddProvider(provider1))
            {
                providersList.Add(provider1);
            }
            if (CanAddProvider(provider2))
            {
                providersList.Add(provider2);
            }

            providers = new BotBDProvider[providersList.Count];
            for (int i = 0; i < providersList.Count; i++)
            {
                providers[i] = providersList[i];
                //connectionStringMap.Add(providers[i].settings.connectionStringName, providers[i].GetConnectionString());
            }
        }


        public static bool CanAddProvider(BotBDProvider provider)
        {
            try
            {
                using (BotDBContext context = (BotDBContext)provider.GetDBContext())
                {
                    context.GetBotRankingsCountFromDB();//dummy call, just to test connection
                }
                return true;
            }
            catch (Exception e)
            {
                BotLib.DebugMessage(String.Format("Connection {0} failed.", provider.GetConnectionString()));
                BotLib.DebugMessage(e);
            }
            return false;
        }

        public static string GetConnectionString(string connectionName)
        {
            if (!connectionStringMap.ContainsKey(connectionName))
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfigurationRoot Configuration = builder.Build();
                connectionStringMap.Add(connectionName, Configuration.GetConnectionString(connectionName));
            }
            return connectionStringMap[connectionName];
        }

        public static DbContextOptionsBuilder GetOptionsbuilder(string connectionName) 
        {
            if (optionsBuilder == null)
            {
                optionsBuilder = new DbContextOptionsBuilder<BotDBContext>();
                optionsBuilder.UseSqlServer(GetConnectionString(connectionName));
            }
            return optionsBuilder;
        }

        public static BotDBContext newDBContext() 
        {
            return new BotDBContext(GetConnectionString("BotConnection"), (DbContextOptions<BotDBContext>)GetOptionsbuilder("BotConnection").Options);
        }

        public static BotDBContext newDBContextClient()
        {
            return new BotDBContext(GetConnectionString("BotConnectionClient"), (DbContextOptions<BotDBContext>)GetOptionsbuilder("BotConnectionClient").Options);
        }


    }

}
