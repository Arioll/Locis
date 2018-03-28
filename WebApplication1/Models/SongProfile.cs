using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class SongProfile
    {
        public SongProfile(uint songId,
            string title, 
            IEnumerable<string> performers, 
            IEnumerable<string> genres, 
            TimeSpan length, 
            string album)
        {
            SongId = songId;
            Title = title;
            Length = length;
            Performers = performers;
            Genres = genres;
            Album = album;
        }

        public uint SongId { get; set; }
        public string Title { get; protected set; }
        public IEnumerable<string> Performers { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public string Album { get; set; }
        public TimeSpan Length { get; protected set; }
    }
}