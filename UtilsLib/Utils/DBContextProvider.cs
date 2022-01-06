using Microsoft.EntityFrameworkCore;
using System;

namespace UtilsLib.Utils
{
    public class DBContextProvider
    {
        public DBSettings settings = null;
        public DbContextOptionsBuilder optionsBuilder = null;

        public DBContextProvider(DBSettings settings) 
        {
            this.settings = settings;
        }

        public virtual DbContextOptionsBuilder GetDbContextOptionsBuilder()
        {
            throw new NotImplementedException();
        }

        public virtual MyDbContext GetDBContext() 
        {
            throw new NotImplementedException();
        }

        public string GetConnectionString()
        {
            return settings.GetConnectionString();
        }
    }
}
