using CommerceBuilder.Common;
using CommerceBuilder.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceBuilder.Utility;
using CommerceBuilder.Payments;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace RandomQuotesPlugin
{
    public class RQPlugin : PluginBase
    {
        public RQPlugin(PluginManifest manifest)
            :base(manifest)
        {
        }

        public override bool Install()
        {
            bool installed = false;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["AbleCommerce"].ConnectionString;
                var errors = RunScript(connectionString, Properties.Resources.create_custom_quotes);
                if (errors.Count > 0)
                {
                    Logger.Error(string.Format("There are errors when trying to install '{0}', please change log level to info for error details.", this.Manifest.Name));
                    errors.ForEach(e => Logger.Info(e));
                }
                else
                {
                    installed = true;
                }
                
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }

            return installed;
        }

        public override bool UnInstall()
        {
            bool uninstalled = false;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["AbleCommerce"].ConnectionString;
                var errors = RunScript(connectionString, Properties.Resources.drop_custom_quotes);
                if (errors.Count > 0)
                {
                    Logger.Error(string.Format("There are errors when trying to uninstall '{0}', please change log level to info for error details.", this.Manifest.Name));
                    errors.ForEach(e => Logger.Info(e));
                }
                else
                {
                    uninstalled = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }

            return uninstalled;
        }

        private List<string> RunScript(string connectionString, string sqlScript)
        {
            // initialize the error list
            List<string> errorList = new List<string>();

            // REMOVE SCRIPT COMMENTS
            sqlScript = Regex.Replace(sqlScript, @"/\*.*?\*/", string.Empty);

            // SPLIT INTO COMMANDS
            string[] commands = StringHelper.Split(sqlScript, "\r\nGO\r\n");

            // SETUP DATABASE CONNECTION
            int errorCount = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Run each command on the database
                conn.Open();
                foreach (string sql in commands)
                {
                    if (!string.IsNullOrEmpty(sql.Trim().Trim('\r', '\n')))
                    {
                        try
                        {
                            SqlCommand command = new SqlCommand(sql, conn);
                            command.CommandTimeout = 0;
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            errorList.Add("<b>SQL:</b> " + sql);
                            errorList.Add("<b>Error:</b> " + ex.Message);
                        }
                    }
                }
                conn.Close();
            }
            if (errorCount > 0) errorList.Add("<b>Errors Count:</b> " + errorCount);

            return errorList;
        }
    }
}
