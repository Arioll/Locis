using System;
using System.Collections.Generic;

namespace Common.Entities
{
    public class Song
    {
        public Song(TimeSpan length, 
            string title,
            string fileName,
            IEnumerable<string> performers, 
            string album, 
            IEnumerable<string> genres,
            uint year,
            string path)
        {
            Length = length;
            Title = title;
            FileName = fileName;
            Performers = performers;
            Album = album;
            Genres = genres;
            Year = year;
            Path = path;
        }

        protected Song() { }

        public virtual uint SongId { get; protected set; }
        public virtual uint Year { get; protected set; }
        public virtual string Title { get; protected set; }
        public virtual string FileName { get; protected set; }
        public virtual IEnumerable<string> Performers { get; protected set; }
        public virtual string Album { get; protected set; }
        public virtual string Path { get; set; }
        public virtual IEnumerable<string> Genres { get; protected set; }
        public virtual TimeSpan Length { get; protected set; }
    }
}