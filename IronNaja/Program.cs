using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel;
using IronNajaUiPathActivities;

namespace IronNaja
{
    internal class Program
    {
        private static void Main(string[] args)

        {
            Dictionary<string, object> arguments = new Dictionary<string, object>()
            {
                { "FileUrl", "teste carlinha"},
                { "ServerName", "mindsforai.database.windows.net" },
                { "DatabaseName", "mouseion" },
                { "UserName", "vinicius" },
                { "Password", "M#str@d0" }
            };
            _ = WorkflowInvoker.Invoke(new IronNajaUiPathActivities.ProcessDocuments(), arguments);
        }
    }
}