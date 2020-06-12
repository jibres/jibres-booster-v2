using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JibresBooster.lib
{
    class manage
    {
        static void RestartAsAdmin()
        {
            var startInfo = new ProcessStartInfo("JibresBooster.exe") { Verb = "runas" };
            Process.Start(startInfo);
            Environment.Exit(0);
        }
    }
}
