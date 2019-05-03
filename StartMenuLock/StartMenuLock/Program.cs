using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using Microsoft.Win32;

namespace StartMenuLock
{
    class Program
    {
        static void Main(string[] args)
        {
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = principal.IsInRole(WindowsBuiltInRole.Administrator);
            if (!hasAdministrativeRight)
            {
                // relaunch the application with admin rights
                string fileName = Assembly.GetExecutingAssembly().Location;
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    Verb = "runas",
                    Arguments = "",
                    FileName = fileName
                };
                try
                {
                    Process.Start(processInfo);
                    Environment.Exit(0);
                }
                catch
                {
                    // This will be thrown if the user cancels the prompt
                    //adminApp.Kill();
                    return;
                }
            }

            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Policies\\Microsoft\\Windows\\Explorer");
            Console.WriteLine("[Start Menu Locker]");
            Console.WriteLine("Please enter '1' (without the quotes) to lock the start menu or '0' to unlock it (default behavior).");
            char setting = Console.ReadLine()[0];
            if(setting != '0' && setting != '1')
            {
                Console.WriteLine("Please press either the #1 key on your keyboard, or the #2 key.");
                Main(new []{""});
                return;
            }
            key.SetValue("LockedStartLayout", setting.ToString(), RegistryValueKind.DWord);
            key.Close();


            Console.WriteLine("Start menu has been " + (setting == '1' ? "locked" : "unlocked") + "!");
            Console.WriteLine("Quitting in 3 seconds.");
            Thread.Sleep(3000);
            Environment.Exit(0);
        }
    }
}
