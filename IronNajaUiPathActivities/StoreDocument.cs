using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.ComponentModel;
using System.Security.Cryptography;
using System.IO;

namespace IronNajaUiPathActivities
{
    public class StoreDocument : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> FilePath { get; set; }

        [Category("Output")]
        public OutArgument<string> FileGuid { get; set; }

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
            string filepath;
            byte[] hashValue;
            string sHash = "";

            filepath = FilePath.Get(context);
            logger.Info($"Storing file {filepath}");

            logger.Info($"Starting the hash calc");

            using (SHA256 mySHA256 = SHA256.Create())
            {
                try
                {
                    FileStream fileStream = new FileStream(filepath, FileMode.Open)
                    {
                        Position = 0
                    };

                    hashValue = mySHA256.ComputeHash(fileStream);
                    sHash = BitConverter.ToString(hashValue).Replace("-", "");
                }
                catch (IOException e)
                {
                    logger.Error($"I/O Exception: {e.Message}");
                }
                catch (UnauthorizedAccessException e)
                {
                    logger.Error($"Access exception: {e.Message}");
                }
            }

            logger.Info($"Hash value = {sHash}");

            logger.Info("Connect database");
            ConnectionStringManager connectionStringManager = new ConnectionStringManager();
            connectionStringManager.InsertConnectionString(ServerName.Get(context),
                DatabaseName.Get(context),
                UserName.Get(context),
                Password.Get(context));

            logger.Info("Verify if this file is already stored");

            using (var db = new MouseionEntities())
            {
                var query = from b in db.tb_amz_arquivos_armazenados
                            where b.amz_hash == sHash
                            select b;

                if (query.Count() > 0)
                {
                    logger.Info("Found a file with the same hash");
                    logger.Info($"GUID = {query.First().amz_guid}");
                }
                else
                {
                    logger.Info("File not stored yet. Generating a new GUID");
                    Guid guid = Guid.NewGuid();
                    logger.Info($"New GUID = {guid}");

                    var amzdata = new tb_amz_arquivos_armazenados();
                    amzdata.tpo_sigla = "file";
                    amzdata.amz_filename = System.IO.Path.GetFileName(filepath);
                    amzdata.amz_full_path = filepath;
                    amzdata.amz_guid = guid.ToString();
                    amzdata.amz_hash = sHash;
                    amzdata.amz_store_datetime = System.DateTime.UtcNow;

                    db.tb_amz_arquivos_armazenados.Add(amzdata);
                    db.SaveChanges();
                }
            }

            //throw new NotImplementedException();
        }
    }
}