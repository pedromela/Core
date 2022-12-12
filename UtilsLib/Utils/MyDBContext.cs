using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilsLib.Utils
{
    public abstract partial class MyDbContext : DbContext
    {
        private static Dictionary<string, int> myStaticFieldDict = new Dictionary<string, int>();
        private static Dictionary<string, ConcurrentHashSet<MyDbContext>> StaticCurrentContexts = new Dictionary<string, ConcurrentHashSet<MyDbContext>>();
        private static Dictionary<string, Dictionary<string, ConcurrentHashSet<DBModelBase>>> StaticRetryDict = new Dictionary<string, Dictionary<string, ConcurrentHashSet<DBModelBase>>>();
        private static Dictionary<string, string> connectionStringMap = new Dictionary<string, string>();

        public int ContextCount
        {
            get
            {
                return myStaticFieldDict.ContainsKey(this.GetType().Name)
                       ? myStaticFieldDict[this.GetType().Name]
                       : default(int);
            }

            set
            {
                myStaticFieldDict[this.GetType().Name] = value;
            }
        }

        public Dictionary<string, ConcurrentHashSet<DBModelBase>> RetryDict
        {
            get
            {
                if (StaticRetryDict.ContainsKey(GetType().Name))
                {
                    return StaticRetryDict[GetType().Name];
                }
                else
                {
                    var dict = new Dictionary<string, ConcurrentHashSet<DBModelBase>>();
                    StaticRetryDict.Add(GetType().Name, dict);
                    return dict;
                }
            }

            set
            {
                StaticRetryDict[this.GetType().Name] = value;
            }
        }

        public ConcurrentHashSet<MyDbContext> CurrentContexts
        {
            get
            {
                if (StaticCurrentContexts.ContainsKey(GetType().Name))
                {
                    return StaticCurrentContexts[GetType().Name];
                }
                else
                {
                    var list = new ConcurrentHashSet<MyDbContext>();
                    StaticCurrentContexts.Add(GetType().Name, list);
                    return list;
                }
            }

            set
            {
                StaticCurrentContexts[this.GetType().Name] = value;
            }
        }

        public MyDbContext() : base()
        {
            IncrementCountContext();
        }

        public MyDbContext(DbContextOptions options)
        : base(options)
        {
            IncrementCountContext();

        }

        public override void Dispose() 
        {
            DecrementCountContext();
            base.Dispose();
        }

        public override ValueTask DisposeAsync()
        {
            DecrementCountContext();
            return base.DisposeAsync();
        }

        public void IncrementCountContext() 
        {
            ContextCount++;
            if (CurrentContexts.Contains(this))
            {
                return;
            }
            CurrentContexts.Add(this);
        }

        public void DecrementCountContext()
        {
            try
            {
                ContextCount--;
                if (CurrentContexts.Contains(this))
                {
                    CurrentContexts.Remove(this);
                }
            }
            catch (Exception e)
            {
                UtilsLib.DebugMessage(e);
            }
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
    }
} 