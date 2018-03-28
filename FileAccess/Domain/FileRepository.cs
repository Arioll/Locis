using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileAccess.Application;
using Journalist;

namespace FileAccess.Domain
{
    public class FileRepository : IFileRepository
    {
        public Stream GetFileStream(string filePath)
        {
            Require.NotEmpty(filePath, nameof(filePath));

            try
            {
                return File.OpenRead(filePath);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public string SaveFile(byte[] fileContent, string fileId)
        {
            var curentPart = CheckDirectoryLimit();
            var path = FileAccessSettings.Default.Path +
                       curentPart +
                       "/" +
                       fileId +
                       FileAccessSettings.Default.Extension;
            Task.Factory.StartNew(() =>
            {
                File.WriteAllBytes(path, fileContent);
            });
            return path;
        }

        private int CheckDirectoryLimit()
        {
            var path = FileAccessSettings.Default.Path;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var currentPart = Directory.GetFiles(path).Length;
            if (currentPart == 0 || Directory.GetFiles(path + currentPart).Length >= 1000)
            {
                Directory.CreateDirectory(path + ++currentPart);
            }
            return currentPart;
        }
    }
}
