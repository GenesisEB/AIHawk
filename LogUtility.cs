using BizHawk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIHawk
{
    internal static class LogUtility
    {
        internal static void Log(string s)
        {
            File.AppendAllText("./log.txt", s);
        }
    }
}
