using System.IO;

namespace FileAccess.Application
{
    public interface IFileRepository
    {
        Stream GetFileStream(string filePath);
        string SaveFile(byte[] fileContent, string fileId);
    }
}
