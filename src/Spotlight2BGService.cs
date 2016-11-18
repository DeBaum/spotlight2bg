using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace Spotlight2BG
{
  public class Spotlight2BgService : ServiceBase
  {
    private FileMover _fileMover;
    private FileSystemWatcher _fileSystemWatcher;

    protected override void OnStart(string[] args)
    {
      _fileMover = new FileMover();
      _fileSystemWatcher = new FileSystemWatcher(_fileMover.ContentDeliveryManagerPath);

      _fileSystemWatcher.Changed += OnFileChange;
      _fileSystemWatcher.Created += OnFileChange;

      _fileSystemWatcher.EnableRaisingEvents = true;
      base.OnStart(args);
    }

    protected override void OnStop()
    {
      _fileSystemWatcher.EnableRaisingEvents = false;
      base.OnStop();
    }

    protected override void OnPause()
    {
      _fileSystemWatcher.EnableRaisingEvents = false;
      base.OnPause();
    }

    protected override void OnContinue()
    {
      _fileSystemWatcher.EnableRaisingEvents = true;
      base.OnContinue();
    }

    protected override void Dispose(bool disposing)
    {
      _fileSystemWatcher.Dispose();
      base.Dispose(disposing);
    }

    private void OnFileChange(object source, FileSystemEventArgs e)
    {
      try
      {
        Console.WriteLine("FileWatcher Event started");

        Thread.Sleep(1000 * 60);
        _fileMover.Run();
      }
      catch (Exception ex)
      {
        Console.Write(ex.ToString());
      }
    }
  }
}