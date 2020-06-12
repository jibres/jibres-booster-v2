using System;
using System.Diagnostics;
using System.Security.Principal;

namespace JibresBooster.lib
{
    internal class manage
    {
        public static void RestartAsAdmin()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("JibresBooster.exe") { Verb = "runas" };
            Process.Start(startInfo);
            Environment.Exit(0);
        }

        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                      .IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
