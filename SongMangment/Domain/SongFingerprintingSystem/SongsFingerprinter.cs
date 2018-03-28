using System;
using System.IO;
using System.Threading.Tasks;
using Common.Entities;
using Journalist;
using SoundFingerprinting;
using SoundFingerprinting.Audio;
using SoundFingerprinting.Audio.NAudio;
using SoundFingerprinting.Builder;
using SoundFingerprinting.DAO.Data;
using SoundFingerprinting.Query;
using SoundFingerprinting.SQL;

namespace SongMangment.Domain.SongFingerprintingSystem
{
    public class SongsFingerprinter
    {
        public SongsFingerprinter()
        {
            _modelService = new SqlModelService();
            _audioService = new NAudioService();
            _fingerprintCommandBuilder = new FingerprintCommandBuilder();
            _queryCommandBuilder = new QueryCommandBuilder();
        }

        private readonly IModelService _modelService;
        private readonly IAudioService _audioService;
        private readonly IFingerprintCommandBuilder _fingerprintCommandBuilder;
        private readonly IQueryCommandBuilder _queryCommandBuilder;

        internal bool SongIsInDataBase(byte[] fileArray)
        {
            Require.NotNull(fileArray, nameof(fileArray));
            
            CheckCasheDirectory();
            var path = SaveFileToCasheDirectory(fileArray);
            try
            {
                var entry = GetBestMatchForSong(path);
                var res = entry != null && entry.Confidence >= 0.5;
                return res;
            }
            catch (Exception exception)
            {
                return true;
            }
            finally
            {
                File.Delete(path);
            }
        }

        internal void AddAudioFileFingerprintsInStorage(Song song)
        {
            Require.NotNull(song, nameof(song));

            var track = new TrackData(song.SongId.ToString(), 
                string.Join(", ", song.Performers), 
                song.Title, 
                song.Album, 
                0, song.Length.Seconds);
            var trackReference = _modelService.InsertTrack(track);
            var hashedFingerprints = _fingerprintCommandBuilder
                                        .BuildFingerprintCommand()
                                        .From(song.Path)
                                        .UsingServices(_audioService)
                                        .Hash()
                                        .Result;

            _modelService.InsertHashDataForTrack(hashedFingerprints, trackReference);
        }

        private ResultEntry GetBestMatchForSong(string pathToAudioFile)
        {
            const int secondsToAnalyze = 10;
            const int startAtSecond = 0;

            var queryResult = _queryCommandBuilder.BuildQueryCommand()
                                                 .From(pathToAudioFile, secondsToAnalyze, startAtSecond)
                                                 .UsingServices(_modelService, _audioService)
                                                 .Query()
                                                 .Result;

            return queryResult.BestMatch;
        }

        private string SaveFileToCasheDirectory(byte[] fileArray)
        {
            Require.NotNull(fileArray, nameof(fileArray));

            var saveTask = new Task<string>(array =>
            {
                var fileName = fileArray.GetHashCode();
                File.WriteAllBytes("E:/Locis/Tracks/Cashe/" + fileName + ".mp3", fileArray);
                return "E:/Locis/Tracks/Cashe/" + fileName + ".mp3";
            }, fileArray);
            saveTask.Start();
            var path = saveTask.Result;
            return path;
        }

        private void CheckCasheDirectory()
        {
            if (!Directory.Exists("E:/Locis/Tracks/Cashe"))
            {
                Directory.CreateDirectory("E:/Locis/Tracks/Cashe");
            }
        }
    }
}
