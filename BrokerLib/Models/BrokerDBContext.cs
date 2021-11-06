using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace BrokerLib.Models
{
    public class BrokerDBContext : DbContext
    {
        public DbSet<Point> Points { get; set; }
        public DbSet<Candle> Candles { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<AccessPoint> AccessPoints { get; set; }
        public DbSet<Equity> Equitys { get; set; }
        public DbSet<ActiveMarket> ActiveMarkets { get; set; }

        public string _connectionString = null;

        public BrokerDBContext() : base()
        {
        }

        public BrokerDBContext(string connectionString, DbContextOptions<BrokerDBContext> options) : base(options)
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

            //modelBuilder.Entity<UserBotRelation>()
            //    .HasKey(c => new { c.UserId, c.BotId });

            modelBuilder.Entity<Candle>()
                .HasKey(c => new { c.TimeFrame, c.Symbol, c.Timestamp });

            modelBuilder.Entity<Point>()
                .HasKey(c => new { c.TimeFrame, c.Symbol, c.Timestamp, c.Line });

            modelBuilder.Entity<ActiveMarket>()
                .Property(c => c.id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Equity>()
                .Property(c => c.id)
                .ValueGeneratedNever();


        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (string.IsNullOrEmpty(_connectionString) || _connectionString == GetConnectionString("BrokerConnection"))
            {
                optionsBuilder.UseSqlServer(GetConnectionString("BrokerConnection"));
            }
            else if (_connectionString == GetConnectionString("BrokerConnectionClient"))
            {
                optionsBuilder.UseSqlServer(GetConnectionString("BrokerConnectionClient"));
            }
        }

        public List<ActiveMarket> GetActiveMarketsFromDB()
        {
            return ActiveMarkets.ToList();
        }

        public int GetActiveMarketsCountFromDB()
        {
            return ActiveMarkets.Count();
        }

        public void DeleteActiveMarketsFromDB()
        {
             ActiveMarkets.RemoveRange(ActiveMarkets);
            SaveChanges();
        }

        public List<Trade> GetTradesFromDB()
        {
            return Trades.ToList();
        }

        public List<Equity> GetEquitiesFromDB()
        {
            return Equitys.ToList();
        }
        public List<Transaction> GetTransactionsFromDB()
        {
            return Transactions.ToList();
        }

        public List<AccessPoint> GetAccessPointsFromDB()
        {
            return AccessPoints.ToList();
        }

        public void DeleteAccessPointsFromDB()
        {
            AccessPoints.RemoveRange(AccessPoints);
            SaveChanges();
        }

        public int GetAccessPointsCountFromDB()
        {
            return AccessPoints.Count();
        }


        public List<Candle> GetCandlesFromDB(TimeFrames timeFrame, string market, DateTime fromDate, DateTime toDate)
        {
            try
            {
                List<Candle> candles = new List<Candle>();
                DateTime _fromDate = fromDate;
                DateTime _toDate = fromDate.AddDays(10) > toDate ? toDate : fromDate.AddDays(10);

                while (_fromDate < toDate)
                {
                    BrokerLib.DebugMessage(String.Format("BrokerDBContext::GetCandlesFromDB() : Getting candles from {0} to {1}.", _fromDate, _toDate));
                    List<Candle> candlesAux = Candles.Where(m => m.TimeFrame == timeFrame && m.Timestamp >= _fromDate && m.Timestamp <= _toDate && m.Symbol.Equals(market)).ToList();
                    _fromDate = _toDate.AddMinutes((int) timeFrame);
                    _toDate = _toDate > toDate ? toDate : _toDate.AddDays(10) > toDate ? toDate: _toDate.AddDays(10);
                    if (candlesAux.Count > 0)
                    {
                        candles.AddRange(candlesAux);
                    }
                }

                BrokerLib.DebugMessage("BrokerDBContext::GetCandlesFromDB() : fetched " + candles.Count + " candles from DB.");
                return candles;

            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }

        ///////////////////////////////////////////////////////////////////
        //////////////////////// STATIC FUNCTIONS /////////////////////////
        ///////////////////////////////////////////////////////////////////

        public static Dictionary<string, string> connectionStringMap = new Dictionary<string, string>();

        public static DbContextOptionsBuilder optionsBuilder = null;

        public static BrokerBDProvider[] providers;

        public static void InitProviders() 
        {
            var provider1 = new BrokerBDProvider(new DBSettings("BrokerConnection"));
            var provider2 = new BrokerBDProvider(new DBSettings("BrokerConnectionClient"));
            List<BrokerBDProvider> providersList = new List<BrokerBDProvider>();

            if (CanAddProvider(provider1))
            {
                providersList.Add(provider1);
            }
            if (CanAddProvider(provider2))
            {
                providersList.Add(provider2);
            }

            providers = new BrokerBDProvider[providersList.Count];
            for (int i = 0; i < providersList.Count; i++)
            {
                providers[i] = providersList[i];
                //connectionStringMap.Add(providers[i].settings.connectionStringName, providers[i].GetConnectionString());
            }

        }

        public static bool CanAddProvider(BrokerBDProvider provider) 
        {
            try
            {
                using (BrokerDBContext context = (BrokerDBContext) provider.GetDBContext())
                {
                    context.GetAccessPointsCountFromDB();//dummy call, just to test connection
                }
                return true;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(String.Format("Connection {0} failed.", provider.GetConnectionString()));
                BrokerLib.DebugMessage(e);

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
                optionsBuilder = new DbContextOptionsBuilder<BrokerDBContext>();
                optionsBuilder.UseSqlServer(GetConnectionString(connectionName));
            }
            return optionsBuilder;
        }

        public static BrokerDBContext newDBContext()
        {
            return new BrokerDBContext(GetConnectionString("BrokerConnection"), (DbContextOptions<BrokerDBContext>)GetOptionsbuilder("BrokerConnection").Options);
        }

        public static BrokerDBContext newDBContextClient()
        {
            return new BrokerDBContext(GetConnectionString("BrokerConnectionClient"), (DbContextOptions<BrokerDBContext>)GetOptionsbuilder("BrokerConnectionClient").Options);
        }
    }
}
