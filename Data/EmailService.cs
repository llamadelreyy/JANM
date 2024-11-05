using MySqlConnector;
using System.Data;
using System.Net;
using System.Net.Mail;
using PBT.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Policy;
using DevExpress.Blazor.Internal.Grid;
using System.Text.RegularExpressions;

namespace PBT.Data
{
    public class EmailService
    {

        private List<EmailProp> _Email { get; set; }

        public IConfiguration Configuration { get; }
        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
            _Email = CreateEmail();
        }

        public List<EmailProp> CreateEmail()
        {
            //////MySqlConnection? myConn;
            //////MySqlCommand? myCmd;
            //////MySqlDataReader? myReader;

            List<EmailProp> arrItem = new List<EmailProp>();
            EmailProp _item;

            try
            {
                ////////Set SQL statement
                //////string strSQL = "SELECT * FROM tbemail WHERE rekStatus='A'";

                //////myConn = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection"));
                //////myCmd = new MySqlCommand(strSQL, myConn);

                //////myConn.Open();
                //////myReader = myCmd.ExecuteReader();

                using (MySqlConnection? myConn = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                {
                    using (MySqlCommand? myCmd = new MySqlCommand("ListEmail", myConn))
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;
                        myConn.Open();
                        using (MySqlDataReader? myReader = myCmd.ExecuteReader())
                        {
                            //Loop every data
                            while (myReader.Read())
                            {
                                _item = new EmailProp();
                                _item.emailID = myReader.GetInt32("emailID");
                                _item.smtpEmail = myReader.GetString("smtpEmail");
                                _item.smtpHost = myReader.GetString("smtpHost");
                                _item.smtpPort = myReader.IsDBNull("smtpPort") ? "" : myReader.GetString("smtpPort");
                                _item.smtpUser = myReader.GetString("smtpUser");
                                _item.smtpPassword = myReader.GetString("smtpPassword");
                                _item.smtpSender = myReader.IsDBNull("smtpSender") ? "" : myReader.GetString("smtpSender");
                                _item.smtpProtocol = myReader.IsDBNull("smtpProtocol") ? false : myReader.GetBoolean("smtpProtocol");
                                _item.smtpDefault = myReader.IsDBNull("smtpDefault") ? false : myReader.GetBoolean("smtpDefault");
                                _item.rekCipta = myReader.IsDBNull("rekCipta") ? null : myReader.GetDateTime("rekCipta");
                                _item.rekCiptaUserID = myReader.IsDBNull("rekCiptaUserID") ? 0 : myReader.GetInt32("rekCiptaUserID");
                                _item.rekUbah = myReader.IsDBNull("rekUbah") ? null : myReader.GetDateTime("rekUbah");
                                _item.rekUbahUserID = myReader.IsDBNull("rekUbahUserID") ? 0 : myReader.GetInt32("rekUbahUserID");
                                _item.rekStatus = myReader.IsDBNull("rekStatus") ? "" : myReader.GetString("rekStatus");

                                //Add item into list
                                arrItem.Add(_item);
                            }
                        }
                    }
                }

                //////myReader.Close();
                //////myCmd.Dispose();
                //////myConn.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught : CreateEmail - {0}", ex);
                return arrItem;
            }
            finally
            {
            }

            return arrItem;
        }

        public Task<List<EmailProp>> GetEmailAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_Email);
        }


        public Task<bool> InsertEmailAsync(EmailProp changed, int intCurrentUserId)
        {
            //////MySqlConnection? myConn;
            //////MySqlCommand? myCmd;

            try
            {
                ////////Insert SQL statement
                //////string strSQL = "INSERT INTO tbemail (smtpEmail, smtpHost, smtpPort, smtpUser, smtpPassword, smtpSender, smtpProtocol, smtpDefault, rekCipta, rekCiptaUserID, rekStatus) VALUES (";
                //////strSQL += Jfunc.Cstring(changed.smtpEmail) + "," + Jfunc.Cstring(changed.smtpHost) + "," + Jfunc.Cstring(changed.smtpPort) + "," + Jfunc.Cstring(changed.smtpUser) + "," + Jfunc.Cstring(changed.smtpPassword) + ",";
                //////strSQL += Jfunc.Cstring(changed.smtpSender) + "," + changed.smtpProtocol + "," + changed.smtpDefault + "," + Jfunc.DBdate(DateTime.Now) + "," + intCurrentUserId + ",'A')";

                //////myConn = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection"));
                //////myConn.Open();

                ////////Update the default value first
                //////if (changed.smtpDefault)
                //////{
                //////    MySqlCommand? myCmdUpdate;

                //////    string SQLUpdate = "UPDATE tbemail SET smtpDefault=0";
                //////    myCmdUpdate = new MySqlCommand(SQLUpdate, myConn);
                //////    myCmdUpdate.ExecuteNonQuery();
                //////    myCmdUpdate.Dispose();
                //////}

                //////myCmd = new MySqlCommand(strSQL, myConn);
                //////myCmd.ExecuteNonQuery();

                //////myCmd.Dispose();
                //////myConn.Close();
                ///
				using (MySqlConnection? myConn = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                {
                    using (MySqlCommand? myCmd = new MySqlCommand("InsertEmail", myConn))
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;
                        myCmd.Parameters.AddWithValue("_smtpEmail", Jfunc.Tstring(changed.smtpEmail));
                        myCmd.Parameters.AddWithValue("_smtpHost", Jfunc.Tstring(changed.smtpHost));
                        myCmd.Parameters.AddWithValue("_smtpPort", Jfunc.Tstring(changed.smtpPort));
                        myCmd.Parameters.AddWithValue("_smtpUser", Jfunc.Tstring(changed.smtpUser));
                        myCmd.Parameters.AddWithValue("_smtpPassword", Jfunc.Tstring(changed.smtpPassword));
                        myCmd.Parameters.AddWithValue("_smtpSender", Jfunc.Tstring(changed.smtpSender));
                        myCmd.Parameters.AddWithValue("_smtpProtocol", changed.smtpProtocol);
                        myCmd.Parameters.AddWithValue("_smtpDefault", changed.smtpDefault);
                        myCmd.Parameters.AddWithValue("_rekCiptaUserID", intCurrentUserId);

                        myConn.Open();
                        myCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught : InsertEmailAsync - {0}", ex);
                return Task.FromResult(false);
            }
            finally
            {
            }

            return Task.FromResult(true);
        }

        public Task<bool> UpdateEmailAsync(EmailProp changed, int intCurrentUserId)
        {

            //////MySqlConnection? myConn;
            //////MySqlCommand? myCmd;

            try
            {
                ////////Select SQL statement
                //////string strSQL = "UPDATE tbemail SET smtpEmail=" + Jfunc.Cstring(changed.smtpEmail) + ", smtpPort=" + Jfunc.Cstring(changed.smtpPort) + ", ";
                //////strSQL += "smtpHost = " + Jfunc.Cstring(changed.smtpHost) + ", smtpUser=" + Jfunc.Cstring(changed.smtpUser) + ", smtpPassword=" + Jfunc.Cstring(changed.smtpPassword) + ", ";
                //////strSQL += "smtpSender = " + Jfunc.Cstring(changed.smtpSender) + ", smtpProtocol = " + changed.smtpProtocol + ", smtpDefault = " + changed.smtpDefault + ", rekUbah=" + Jfunc.DBdate(DateTime.Now) + ", rekUbahUserID=" + intCurrentUserId;
                //////strSQL += " WHERE emailID=" + changed.emailID;

                //////myConn = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection"));
                //////myConn.Open();

                ////////Update the default value first
                //////if (changed.smtpDefault)
                //////{
                //////    MySqlCommand? myCmdUpdate;

                //////    string SQLUpdate = "UPDATE tbemail SET smtpDefault=0";
                //////    myCmdUpdate = new MySqlCommand(SQLUpdate, myConn);
                //////    myCmdUpdate.ExecuteNonQuery();
                //////    myCmdUpdate.Dispose();
                //////}

                //////myCmd = new MySqlCommand(strSQL, myConn);
                //////myCmd.ExecuteNonQuery();

                //////myCmd.Dispose();
                //////myConn.Close();
                ///
				using (MySqlConnection? myConn = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                {
                    using (MySqlCommand? myCmd = new MySqlCommand("UpdateEmail", myConn))
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;
                        myCmd.Parameters.AddWithValue("_emailID", changed.emailID);
                        myCmd.Parameters.AddWithValue("_smtpEmail", Jfunc.Tstring(changed.smtpEmail));
                        myCmd.Parameters.AddWithValue("_smtpHost", Jfunc.Tstring(changed.smtpHost));
                        myCmd.Parameters.AddWithValue("_smtpPort", Jfunc.Tstring(changed.smtpPort));
                        myCmd.Parameters.AddWithValue("_smtpUser", Jfunc.Tstring(changed.smtpUser));
                        myCmd.Parameters.AddWithValue("_smtpPassword", Jfunc.Tstring(changed.smtpPassword));
                        myCmd.Parameters.AddWithValue("_smtpSender", Jfunc.Tstring(changed.smtpSender));
                        myCmd.Parameters.AddWithValue("_smtpProtocol", changed.smtpProtocol);
                        myCmd.Parameters.AddWithValue("_smtpDefault", changed.smtpDefault);
                        myCmd.Parameters.AddWithValue("_rekUbahUserID", intCurrentUserId);

                        myConn.Open();
                        myCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught : UpdateEmailAsync - {0}", ex);
                return Task.FromResult(false);
            }
            finally
            {
            }

            return Task.FromResult(true);
        }

        public Task<bool> RemoveEmailAsync(EmailProp dtDataItem, int intCurrentUserId)
        {
            //////MySqlConnection? myConn;
            //////MySqlCommand? myCmd;

            try
            {
                ////////Select SQL statement
                //////string strSQL = "UPDATE tbemail SET rekStatus='T', rekUbah=" + Jfunc.DBdate(DateTime.Now) + ", rekUbahUserID=" + intCurrentUserId;
                //////strSQL = strSQL + " WHERE emailID=" + dtDataItem.emailID;

                //////myConn = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection"));
                //////myCmd = new MySqlCommand(strSQL, myConn);

                //////myConn.Open();
                //////myCmd.ExecuteNonQuery();

                //////myCmd.Dispose();
                //////myConn.Close();
                ///
				using (MySqlConnection? myConn = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                {
                    using (MySqlCommand? myCmd = new MySqlCommand("DeleteEmail", myConn))
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;
                        myCmd.Parameters.AddWithValue("_emailID", dtDataItem.emailID);
                        myCmd.Parameters.AddWithValue("_smtpDefault", dtDataItem.smtpDefault);
                        myCmd.Parameters.AddWithValue("_rekUbahUserID", intCurrentUserId);

                        myConn.Open();
                        myCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught : RemoveEmailAsync - {0}", ex);
                return Task.FromResult(false);
            }
            finally
            {
            }

            return Task.FromResult(true);
        }

        public Task<List<EmailProp>> RefreshEmailAsync()
        {
            //////MySqlConnection? myConn;
            //////MySqlCommand? myCmd;
            //////MySqlDataReader? myReader;

            List<EmailProp> arrItem = new List<EmailProp>();
            EmailProp _item;

            try
            {
                ////////Select SQL statement
                //////string strSQL = "SELECT * FROM tbemail WHERE rekStatus='A'";

                //////myConn = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection"));
                //////myCmd = new MySqlCommand(strSQL, myConn);

                //////myConn.Open();
                //////myReader = myCmd.ExecuteReader();

                using (MySqlConnection? myConn = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                {
                    using (MySqlCommand? myCmd = new MySqlCommand("ListEmail", myConn))
                    {
                        myCmd.CommandType = CommandType.StoredProcedure;
                        myConn.Open();
                        using (MySqlDataReader? myReader = myCmd.ExecuteReader())
                        {
                            //Loop every data
                            while (myReader.Read())
                            {
                                _item = new EmailProp();
                                _item.emailID = myReader.GetInt32("emailID");
                                _item.smtpEmail = myReader.GetString("smtpEmail");
                                _item.smtpHost = myReader.GetString("smtpHost");
                                _item.smtpPort = myReader.IsDBNull("smtpPort") ? "" : myReader.GetString("smtpPort");
                                _item.smtpUser = myReader.GetString("smtpUser");
                                _item.smtpPassword = myReader.GetString("smtpPassword");
                                _item.smtpSender = myReader.IsDBNull("smtpSender") ? "" : myReader.GetString("smtpSender");
                                _item.smtpProtocol = myReader.IsDBNull("smtpProtocol") ? false : myReader.GetBoolean("smtpProtocol");
                                _item.smtpDefault = myReader.IsDBNull("smtpDefault") ? false : myReader.GetBoolean("smtpDefault");
                                _item.rekCipta = myReader.IsDBNull("rekCipta") ? null : myReader.GetDateTime("rekCipta");
                                _item.rekCiptaUserID = myReader.IsDBNull("rekCiptaUserID") ? 0 : myReader.GetInt32("rekCiptaUserID");
                                _item.rekUbah = myReader.IsDBNull("rekUbah") ? null : myReader.GetDateTime("rekUbah");
                                _item.rekUbahUserID = myReader.IsDBNull("rekUbahUserID") ? 0 : myReader.GetInt32("rekUbahUserID");
                                _item.rekStatus = myReader.IsDBNull("rekStatus") ? "" : myReader.GetString("rekStatus");

                                //Add item into list
                                arrItem.Add(_item);
                            }
                        }
                    }
                }

                //////myReader.Close();
                //////myCmd.Dispose();
                //////myConn.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught : RefreshEmailAsync - {0}", ex);
                return Task.FromResult(arrItem);
            }
            finally
            {
            }

            return Task.FromResult(arrItem);
        }


        public async Task<string> TestEmailConfig(EmailProp entity)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.To.Add(new MailAddress(entity.smtpEmail, "Administrator"));
                    message.From = new MailAddress(entity.smtpEmail, entity.smtpSender);
                    message.Subject = "Email Testing";
                    message.Body = "Hooray! Email setting is working.";
                    message.IsBodyHtml = false;

                    using (var client = new SmtpClient(entity.smtpHost))
                    {
                        client.UseDefaultCredentials = false;
                        client.Port = int.Parse(entity.smtpPort);
                        client.Credentials = new NetworkCredential(entity.smtpUser, entity.smtpPassword);
                        client.EnableSsl = entity.smtpProtocol;
                        await client.SendMailAsync(message);
                    }

                    return "";
                }
            }
            catch (Exception e)
            {
                var message = $"Error testing email config. Error: {e.Message}";
                Console.WriteLine("Exception caught : RemoveEmailAsync - {0}", message);
                ////return BadRequest(message);
                return "MSG=" + e.Message + "; SOURCE=" + e.Source + "; STACK TRACE=" + e.StackTrace + "; EXCEPTION=" + e.InnerException;
            }
        }

        public async Task<bool> SendResetPwdEmail(string strUserEmail)
        {
            MySqlConnection? myConn;
            MySqlCommand? myCmd;
            MySqlDataReader? myReader;
            int intUserID = 0;
            string strFullName = "", strUserKey = "", strUserPassword = "";

            try
            {
                //Need to get default email config
                //Set SQL statement
                string strSQL = "SELECT userID, userFullName, userKey, userPassword FROM tbuser WHERE userEmail='" + strUserEmail + "' AND rekStatus='A'";

                myConn = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection"));
                myCmd = new MySqlCommand(strSQL, myConn);

                myConn.Open();
                myReader = myCmd.ExecuteReader();

                if (myReader.Read())
                {
                    intUserID = myReader.IsDBNull("userID") ? 0 : myReader.GetInt32("userID");
                    strFullName = myReader.IsDBNull("userFullName") ? "" : myReader.GetString("userFullName");
                    strUserKey = myReader.IsDBNull("userKey") ? "" : myReader.GetString("userKey").Trim();
                    strUserPassword = myReader.IsDBNull("userPassword") ? "" : myReader.GetString("userPassword").Trim();
                    
                }

                myReader.Close();
                myConn.Close();
                myCmd.Dispose();

                //Check if record has any record
                if (strFullName.Trim() != "")
                {
                    //MySqlDataReader? myReaderEmail;
                    //string _Email = "", _Host = "", _User = "", _Password = "", _Sender = "", _Port = "0", _NewPassword = "";
                    //bool _Protocol = false;
                    string _NewPassword = "";

                    ////Get the default email setting
                    //strSQL = "SELECT * FROM tbemail WHERE smtpDefault=1 AND rekStatus='A'";
                    //myCmd = new MySqlCommand(strSQL, myConn);

                    //myReaderEmail = myCmd.ExecuteReader();

                    //if (myReaderEmail.Read())
                    //{
                    //    _Email = myReaderEmail.GetString("smtpEmail");
                    //    _Host = myReaderEmail.GetString("smtpHost");
                    //    _Port = myReaderEmail.IsDBNull("smtpPort") ? "" : myReaderEmail.GetString("smtpPort");
                    //    _User = myReaderEmail.GetString("smtpUser");
                    //    _Password = myReaderEmail.GetString("smtpPassword");
                    //    _Sender = myReaderEmail.IsDBNull("smtpSender") ? "" : myReaderEmail.GetString("smtpSender");
                    //    _Protocol = myReaderEmail.IsDBNull("smtpProtocol") ? false : myReaderEmail.GetBoolean("smtpProtocol");
                    //}

                    //myReaderEmail.Close();
                    //myCmd.Dispose();
                    //myConn.Close();

                    ////Default email exist
                    //if (_Email.Trim() != "")
                    //{
                    //Generate default password
                    //Create a unique password =================================================
                    Random rn = new Random();
                    string charsToUse = "AzByCxDwEvFuGtHsIrJqKpLoMnNmOlPkQjRiShTgUfVeWdXcYbZa1234567890";

                    MatchEvaluator RandomChar = delegate (Match m)
                    {
                        return charsToUse[rn.Next(charsToUse.Length)].ToString();
                    };

                    _NewPassword = Regex.Replace("XXXXXXXX", "X", RandomChar);

                    //Create key to encrypted password
                    string strKey = "";
                    strFullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => strKey += i[0].ToString());
                    strKey = strKey.Trim().Length > 7 ? strKey.Substring(0, 7) : strKey;
                    strKey = strKey + "_" + DateTime.Now.ToString("yyMMddhhmmss");

                    //Encrypt the password
                    string strEncPassword = Crypto.Encrypt(_NewPassword, strKey, "custommedia");

                        //=== Send the email ===
       //////                 using (var message = new MailMessage())
       //////                 {
							////////message.To.Add(new MailAddress(_Email, "Administrator"));
							//////message.To.Add(new MailAddress(strUserEmail, strFullName));
							//////message.From = new MailAddress(_Email, _Sender);
       //////                     message.Subject = "Request to reset password for JAINJ system.";
       //////                     message.Body = "Your password has been reset. The new password is -->" + _NewPassword;
       //////                     message.IsBodyHtml = false;

       //////                     using (var client = new SmtpClient(_Host))
       //////                     {
       //////                         client.UseDefaultCredentials = false;
       //////                         client.Port = int.Parse(_Port);
       //////                         client.Credentials = new NetworkCredential(_User, _Password);
       //////                         client.EnableSsl = _Protocol;
       //////                         await client.SendMailAsync(message);
       //////                     }
       //////                 }

                        //Send email here
                    EmailNotificationService.EnqueueEmail(new EmailQueueData
                    {
                        ReceiverEmail = strUserEmail,
                        ReceiverName = strFullName,
                        Subject = "Permohonan Untuk Reset Kata Laluan Dari Sistem i-BinaPulihMasjidSurau",
                        Body = "Kata Laluan anda telah direset. Kata Laluan yang baru adalah " + _NewPassword
                    }); 



                    //=== Update the new password with the new password
                    //Decrypt back password to be save as old password
                    strUserPassword = Crypto.DecryptToString(strUserPassword, strUserKey, "custommedia");

                    //Select connection string from every tenant
                    strSQL = "UPDATE tbuser SET userPassword='" + strEncPassword + "', userOldPassword='" + strUserPassword + "', userKey='" + strKey + "',";
                    strSQL += "rekUbah = '" + Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd") + "', rekUbahUserID=0 WHERE userID=" + intUserID;

                    myCmd = new MySqlCommand(strSQL, myConn);
                    myConn.Open();
                    myCmd.ExecuteNonQuery();

                    myCmd.Dispose();
                    myConn.Close();

                    return true;
                    //}
                    //else
                    //{
                    //    return false;
                    //}

                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                var message = $"Error testing email config. Error: {e.Message}";
                Console.WriteLine("Exception caught : RemoveEmailAsync - {0}", message);
                ////return BadRequest(message);
                return false;
            }
            finally
            {
            }
        }

    }
}
