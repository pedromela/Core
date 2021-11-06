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

        public virtual DbContext GetDBContext() 
        {
            throw new NotImplementedException();
        }

        public virtual string GetConnectionString()
        {
            throw new NotImplementedException();
        }
    }
}
