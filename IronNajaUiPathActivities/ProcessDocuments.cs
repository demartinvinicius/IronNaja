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

namespace IronNajaUiPathActivities
{
    public class SqlHelper
    {
        private SqlConnection cn;

        public SqlHelper(string connectionString)
        {
            cn = new SqlConnection(connectionString);
        }

        public bool IsConnection
        {
            get
            {
                if (cn.State == System.Data.ConnectionState.Closed)
                {
                    cn.Open();
                }
                return true;
            }
        }
    }

    public class ProcessDocuments : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> FileUrl { get; set; }

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected override void Execute(CodeActivityContext context)
        {
            logger.Info("Starting the activity process");

            Configuration config;
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings();
            connectionStringSettings.ConnectionString = "metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SqlClient;provider connection string=\";data source=mindsforai.database.windows.net;initial catalog=Mouseion;persist security info=True;user id=vinicius;password=M#str@d0;MultipleActiveResultSets=True;App=EntityFramework\"";
            connectionStringSettings.ProviderName = "System.Data.EntityClient";
            connectionStringSettings.Name = "MouseionEntities";
            config.ConnectionStrings.ConnectionStrings.Add(connectionStringSettings);
            //config.ConnectionStrings.ConnectionStrings["MouseionEntities"].ConnectionString = "metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=mindsforai.database.windows.net;initial catalog=Mouseion;persist security info=True;user id=vinicius;password=M#str@d0;MultipleActiveResultSets=True;App=EntityFramework&quot;";
            //config.ConnectionStrings.ConnectionStrings["MouseionEntities"].ProviderName = "System.Data.SqlClient";

            //ConfigurationSection section = new ConfigurationSection();
            //
            //config.Sections.Add()

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