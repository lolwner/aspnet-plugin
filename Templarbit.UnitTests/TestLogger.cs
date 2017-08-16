using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Templarbit.Core;

namespace Templarbit.UnitTests
{
    public class TestLogger : ITemplarbitLogger
    {
        public List<string> Logs { get; set; } = new List<string>();
        public void Log(string exception)
        {
            string path = Directory.GetCurrentDirectory();

            Logs.Add(String.Format("\nTemplarbitMiddlewareError: {0}\n", exception));
        }
    }
}
