using Microsoft.EntityFrameworkCore;
using UtilsLib.Utils;

namespace BotLib.Models
{
    public class BotBDProvider : DBContextProvider
    {
        public BotBDProvider(DBSettings settings)
        : base(settings)
        {

        }

        public override DbContextOptionsBuilder GetDbContextOptionsBuilder()
        {
            if (optionsBuilder == null)
            {
                optionsBuilder = new DbContextOptionsBuilder<BotDBContext>();
                optionsBuilder.UseSqlServer(settings.GetConnectionString());
            }
            return optionsBuilder;
        }

        public override DbContext GetDBContext()
        {
            return new BotDBContext(settings.GetConnectionString(), (DbContextOptions<BotDBContext>)GetDbContextOptionsBuilder().Options);
        }

        public override string GetConnectionString()
        {
            return settings.GetConnectionString();
        }
    }
}
