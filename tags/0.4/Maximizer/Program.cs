using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Maximizer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
                return;

            string processName = args[0];

            if (!File.Exists(processName))
                return;

            StringBuilder sb = new StringBuilder();

            for (int x = 1; x < args.Length; x++)
            {
                sb.Append("\"" + args[x] + "\"");
                if (x != args.Length - 1)
                    sb.Append(" ");
            }

            Process process = new Process();
            process.StartInfo.FileName = processName;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

            if (sb.Length != 0)
                process.StartInfo.Arguments = sb.ToString();

            // hide the calling window
            WindowHandler handler = new WindowHandler();
            int mediaCenter = handler.GetWnd("ehshell");

            if (mediaCenter > 0)
                handler.Minimize(mediaCenter);

            process.Start();

            process.WaitForExit();

            if (mediaCenter > 0)
                handler.Maximize(mediaCenter);
        }
    }
}
