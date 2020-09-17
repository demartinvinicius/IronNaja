using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel.Composition;
using NLog;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using System.Data.Entity.Core.EntityClient;

namespace IronNajaUiPathActivities
{
    public class ProcessDocuments : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> FileUrl { get; set; }

        [Category("Database")]
        [RequiredArgument]
        public InArgument<string> ServerName { get; set; }

        [Category("Database")]
        [RequiredArgument]
        public InArgument<string> DatabaseName { get; set; }

        [Category("Database")]
        [RequiredArgument]
        public InArgument<string> UserName { get; set; }

        [Category("Database")]
        [RequiredArgument]
        public InArgument<string> Password { get; set; }

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected override void Execute(CodeActivityContext context)
        {
            logger.Info("Starting the activity process");

            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();

            sqlBuilder.DataSource = ServerName.Get(context);
            sqlBuilder.InitialCatalog = DatabaseName.Get(context);
            sqlBuilder.IntegratedSecurity = false;
            sqlBuilder.UserID = UserName.Get(context);
            sqlBuilder.Password = Password.Get(context);
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";
            sqlBuilder.Encrypt = true;

            EntityConnectionStringBuilder entityConnection = new EntityConnectionStringBuilder();

            entityConnection.Provider = "System.Data.SqlClient";
            entityConnection.Metadata = @"res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl";

            entityConnection.ProviderConnectionString = sqlBuilder.ToString();

            logger.Info(entityConnection.ConnectionString);

            Configuration config;
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.GetSection("connectionStrings") != null)
            {
                logger.Info("Found config section. It will be deleted");
                config.Sections.Remove("connectionStrings");
                config.Save(ConfigurationSaveMode.Full);
            }

            ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings();
            //connectionStringSettings.ConnectionString = "metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SqlClient;provider connection string=\";data source=mindsforai.database.windows.net;initial catalog=Mouseion;persist security info=True;user id=vinicius;password=M#str@d0;MultipleActiveResultSets=True;App=EntityFramework\"";

            connectionStringSettings.ConnectionString = entityConnection.ConnectionString;
            connectionStringSettings.ProviderName = "System.Data.EntityClient";
            connectionStringSettings.Name = "MouseionEntities";
            config.ConnectionStrings.ConnectionStrings.Add(connectionStringSettings);

            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("connectionStrings");
            string fileurl = FileUrl.Get(context);

            logger.Info($"We're gonna process {fileurl}");
            var db = new MouseionEntities();

            var amzdata = new tb_amz_arquivos_armazenados
            {
                amz_filename = fileurl,
                amz_full_path = "fullpath",
                amz_guid = "guid",
                amz_hash = "hash",
                amz_store_datetime = System.DateTime.Now
            };
            db.tb_amz_arquivos_armazenados.Add(amzdata);
            db.SaveChanges();

            //throw new NotImplementedException();
        }
    }
}