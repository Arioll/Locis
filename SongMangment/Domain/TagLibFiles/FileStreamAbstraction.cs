using System;
using System.IO;
using File = TagLib.File;

namespace SongMangment.Domain.TagLibFiles
{
    internal class FileStreamAbstraction : File.IFileAbstraction
    {
        internal FileStreamAbstraction(string name, byte[] bytes)
        {
            Name = name;
            ReadStream = new MemoryStream(bytes);
        }

        public void CloseStream(Stream stream)
        {
            stream.Close();
        }

        public string Name { get; }
        public Stream ReadStream { get; }

        public Stream WriteStream
        {
            get { throw new Exception("Can not write to this file"); }
        }
    }
}
