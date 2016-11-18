using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;

namespace Spotlight2BG
{
  internal class Program
  {
    public static void Main(string[] args)
    {
      if (!Environment.UserInteractive)
      {
        StartService();
      }
      else
      {
        if (args.Length == 0)
        {
          RunFileMover();
        }
        else if (args.Length == 1)
        {
          HandleArgs(args);
        }
      }
    }

    private static void HandleArgs(string[] args)
    {
      switch (args[0])
      {
        case "-i":
        case "-install":
        {
          SetupService(true);
          break;
        }
        case "-u":
        case "-uninstall":
        {
          SetupService(false);
          break;
        }
        default:
          return;
      }
    }

    private static void SetupService(bool install)
    {
      var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
      if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
      {
        Console.WriteLine("Start with admin-rights");
        return;
      }

      const string serviceName = "Spotlight2BG";

      var scCommad = (install ? "sc create " : "sc delete ") + serviceName;
      if (install)
      {
        scCommad += " binPath= \"" + Assembly.GetEntryAssembly().Location + "\" start= delayed-auto";
      }

      var cmd = new Process
      {
        StartInfo =
        {
          FileName = "cmd.exe",
          Arguments = "/C " + scCommad,
          RedirectStandardOutput = true,
          CreateNoWindow = true,
          UseShellExecute = false
        }
      };
      cmd.Start();

      cmd.WaitForExit();
      Console.WriteLine(cmd.StandardOutput.ReadToEnd());
    }

    private static void StartService()
    {
      var spotlight2BgService = new Spotlight2BgService();
      ServiceBase.Run(spotlight2BgService);
    }

    private static void RunFileMover()
    {
      var fileMover = new FileMover();
      fileMover.Run();
    }
  }
}