using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Templarbit.Core;

namespace Templarbit.Test
{
    public class Logger : ITemplarbitLogger
    {
        public void Log(string exception)
        {
            string path = Directory.GetCurrentDirectory();

            using (StreamWriter sw = File.AppendText(path + "/LogFile.txt"))
            {
                string logLine = String.Format("\nTemplarbitMiddlewareError: {0}\n", exception);
                sw.WriteLine(logLine);
            }
        }
    }
}
