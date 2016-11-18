using System;
using System.Collections.Generic;
using System.Data.HashFunction.CRCStandards;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Spotlight2BG
{
  public class FileMover
  {
    public readonly string ContentDeliveryManagerPath;

    public readonly string PictureFolderPath;

    public readonly CRC32 Crc;

    public FileMover()
    {
      var userPofile = Environment.GetEnvironmentVariable("USERPROFILE");

      if (userPofile == null) throw new ArgumentNullException();

      PictureFolderPath = Path.Combine(userPofile, @"Pictures\Spotlight2BG");
      ContentDeliveryManagerPath = Path.Combine(userPofile,
        @"AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets");
      Crc = new CRC32();
    }

    public void Run()
    {
      Console.WriteLine("Copy new Files");

      var files = GetFileNames();
      if (files.Any())
      {
        files = FilterFiles(files);
        CopyFiles(files);
      }
    }

    private List<string> GetFileNames()
    {
      if (Directory.Exists(ContentDeliveryManagerPath))
      {
        return new List<string>(Directory.GetFiles(ContentDeliveryManagerPath));
      }
      return null;
    }

    private List<string> FilterFiles(List<string> files)
    {
      var filteredFiles = new List<string>();

      var targetSize = new Size(1920, 1080);
      foreach (var file in files)
      {
        try
        {
          using (var image = Image.FromFile(file))
          {
            if (image.Size.Equals(targetSize))
            {
              filteredFiles.Add(file);
            }
          }
        }
        catch (Exception) { /* ignored */ }
      }
      return filteredFiles;
    }

    private void CopyFiles(IEnumerable<string> files)
    {
      if (!Directory.Exists(PictureFolderPath))
      {
        Console.WriteLine("Create Directory \"" + PictureFolderPath + "\"");
        Directory.CreateDirectory(PictureFolderPath);
      }

      foreach (var file in files)
      {
        var destFilePath = Path.Combine(PictureFolderPath, GetFileHash(file) + ".jpg");

        if (File.Exists(destFilePath)) continue;

        Console.WriteLine("Copy new File \"" + destFilePath + "\"");
        File.Copy(file, destFilePath);
      }
    }

    private string GetFileHash(string file)
    {
      var crc = Crc.ComputeHash(File.OpenRead(file));
      return crc.Aggregate(string.Empty, (current, b) => current + b.ToString("x2"));
    }
  }
}