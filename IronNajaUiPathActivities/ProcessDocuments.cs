using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel.Composition;
using NLog;
using System.ComponentModel;

namespace IronNajaUiPathActivities
{
    public class ProcessDocuments : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> FileUrl { get; set; }

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected override void Execute(CodeActivityContext context)
        {
            logger.Info("Starting the activity process");

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