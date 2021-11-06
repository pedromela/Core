using Microsoft.EntityFrameworkCore;
using UtilsLib.Utils;

namespace BrokerLib.Models
{
    public class BrokerBDProvider : DBContextProvider
    {
        public BrokerBDProvider(DBSettings settings)
        : base(settings)
        {

        }

        public override DbContextOptionsBuilder GetDbContextOptionsBuilder()
        {
            if (optionsBuilder == null)
            {
                optionsBuilder = new DbContextOptionsBuilder<BrokerDBContext>();
                optionsBuilder.UseSqlServer(settings.GetConnectionString());
            }
            return optionsBuilder;
        }

        public override DbContext GetDBContext()
        {
            return new BrokerDBContext(settings.GetConnectionString(), (DbContextOptions<BrokerDBContext>)GetDbContextOptionsBuilder().Options);
        }

        public override string GetConnectionString()
        {
            return settings.GetConnectionString();
        }
    }
}
