using NLog;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace IronNajaUiPathActivities
{
    internal class ConnectionStringManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void InsertConnectionString(string Server, string DatabaseName, string UserName, string Password)
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder
            {
                DataSource = Server,
                InitialCatalog = DatabaseName,
                IntegratedSecurity = false,
                UserID = UserName,
                Password = Password,
                MultipleActiveResultSets = true,
                ApplicationName = "EntityFramework",
                Encrypt = true
            };

            EntityConnectionStringBuilder entityConnection = new EntityConnectionStringBuilder
            {
                Provider = "System.Data.SqlClient",
                Metadata = @"res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl",

                ProviderConnectionString = sqlBuilder.ToString()
            };

            logger.Info($"String Connection Being Inserted = {entityConnection.ConnectionString}");

            Configuration config;
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.GetSection("connectionStrings") != null)
            {
                logger.Info("Found config section. It will be deleted");
                config.Sections.Remove("connectionStrings");
                config.Save(ConfigurationSaveMode.Full);
            }

            ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings
            {
                ConnectionString = entityConnection.ConnectionString,
                ProviderName = "System.Data.EntityClient",
                Name = "MouseionEntities"
            };
            config.ConnectionStrings.ConnectionStrings.Add(connectionStringSettings);

            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("connectionStrings");
        }
    }
}