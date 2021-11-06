using Microsoft.Extensions.Configuration;

namespace UtilsLib.Utils
{
    public class DBSettings
    {

        public string connectionStringName = null;
        private string connectionString = null;

        public DBSettings(string connectionStringName) 
        {
            this.connectionStringName = connectionStringName;
            GetConnectionString();
        }

        public string GetConnectionString()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfigurationRoot Configuration = builder.Build();
                connectionString = Configuration.GetConnectionString(connectionStringName);
            }
            return connectionString;
        }
    }
}
