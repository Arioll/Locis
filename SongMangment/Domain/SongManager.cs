using System;
using System.Collections.Generic;
using System.IO;
using Common.Entities;
using Journalist;
using SongMangment.Application;
using DataAccess.Application;
using System.Net.Http;
using System.Threading.Tasks;
using FileAccess.Application;
using SongMangment.Domain.SongFingerprintingSystem;
using SongMangment.Domain.TagLibFiles;
using TagLib;

namespace SongMangment.Domain
{
    public class SongManager : ISongManager
    {
        public SongManager(
            ISongRepository songRepository,
            IFileRepository fileRepository,
            SongsFingerprinter songsFingerprinter)
        {
            _songRepository = songRepository;
            _fileRepository = fileRepository;
            _songsFingerprinter = songsFingerprinter;
            FileSavingProcessMarker = new object();
        }
        
        private object FileSavingProcessMarker { get; }
        private readonly ISongRepository _songRepository;
        private readonly IFileRepository _fileRepository;
        private readonly SongsFingerprinter _songsFingerprinter;

        public IEnumerable<Song> GetSongsByQuery(string query)
        {
            Require.NotEmpty(query, nameof(query));

            return _songRepository.GetSongsByQuery(query) ?? new List<Song>();
        }

        public Song GetSongById(uint songId)
        {
            Require.Positive(songId, nameof(songId));

            return _songRepository.GetSongById(songId);
        }

        public Stream GetSongStream(uint songId)
        {
            Require.Positive(songId, nameof(songId));
            var song = _songRepository.GetSongById(songId);
            var stream = _fileRepository.GetFileStream(song.Path);
            if (stream == null) _songRepository.DeleteSong(song);
            return stream;
        }

        public async Task<IEnumerable<string>> SaveSongFiles(IEnumerable<HttpContent> files)
        {
            Require.NotNull(files, nameof(files));

            var wrongFiles = new List<string>();
            foreach (var file in files)
            {
                var fileName = file.Headers.ContentDisposition.FileName;
                var fileArray = await file.ReadAsByteArrayAsync();
                var fileAbstraction = new FileStreamAbstraction(fileName, fileArray);
                TagLib.File taglibFile;
                try
                {
                    taglibFile = TagLib.File.Create(fileAbstraction);
                }
                catch (UnsupportedFormatException)
                {
                    wrongFiles.Add(fileName);
                    continue;
                }
                if (taglibFile.Properties.MediaTypes != MediaTypes.Audio)
                {
                    wrongFiles.Add(fileName);
                    continue;
                }
                lock (FileSavingProcessMarker)
                {
                    if (_songsFingerprinter.SongIsInDataBase(fileArray))
                    {
                        wrongFiles.Add(fileName);
                        continue;
                    }

                    var song = new Song(taglibFile.Properties.Duration, taglibFile.Tag.Title, fileName,
                        taglibFile.Tag.Performers, taglibFile.Tag.Album, taglibFile.Tag.Genres,
                        taglibFile.Tag.Year, null);
                    var fileId = _songRepository.SaveSong(song);
                    string path;
                    try
                    {
                        path = _fileRepository.SaveFile(fileArray, fileId.ToString());
                    }
                    catch (Exception)
                    {
                        wrongFiles.Add(fileName);
                        _songRepository.DeleteSong(song);
                        continue;
                    }
                    var newSong = _songRepository.GetSongById(fileId);
                    newSong.Path = path;
                    _songRepository.UpdateSong(newSong);

                    _songsFingerprinter.AddAudioFileFingerprintsInStorage(newSong);
                }
            }
            return wrongFiles;
        }
    }
}