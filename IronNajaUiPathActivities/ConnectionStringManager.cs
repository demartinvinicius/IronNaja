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
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();

            sqlBuilder.DataSource = Server;
            sqlBuilder.InitialCatalog = DatabaseName;
            sqlBuilder.IntegratedSecurity = false;
            sqlBuilder.UserID = UserName;
            sqlBuilder.Password = Password;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";
            sqlBuilder.Encrypt = true;

            EntityConnectionStringBuilder entityConnection = new EntityConnectionStringBuilder();

            entityConnection.Provider = "System.Data.SqlClient";
            entityConnection.Metadata = @"res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl";

            entityConnection.ProviderConnectionString = sqlBuilder.ToString();

            logger.Info($"String Connection Being Inserted = {entityConnection.ConnectionString}");

            Configuration config;
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.GetSection("connectionStrings") != null)
            {
                logger.Info("Found config section. It will be deleted");
                config.Sections.Remove("connectionStrings");
                config.Save(ConfigurationSaveMode.Full);
            }

            ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings();

            connectionStringSettings.ConnectionString = entityConnection.ConnectionString;
            connectionStringSettings.ProviderName = "System.Data.EntityClient";
            connectionStringSettings.Name = "MouseionEntities";
            config.ConnectionStrings.ConnectionStrings.Add(connectionStringSettings);

            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("connectionStrings");
        }
    }
}