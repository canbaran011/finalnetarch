using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
    public class LogHelper
    {
        public static void doLog(string message)
        {
            try
            {
                if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\Log\\"))
                    Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "\\Log\\");
                using (StreamWriter sw = new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "\\Log\\" + DateTime.Today.ToString("yyyyMMdd") + ".txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString() + " - " + message);
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Error while logging...", "Logging Error");
            }
        }
    }
}
