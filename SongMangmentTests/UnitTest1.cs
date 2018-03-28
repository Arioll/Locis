using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess.Application;
using FileAccess.Application;
using Moq;
using SongMangment.Domain.SongFingerprintingSystem;
using Common.Entities;
using SongMangment.Domain;

namespace SongMangmentTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize]
        public void Setup()
        {
            _songRepository = new Mock<ISongRepository>();
            _fileRepository = new Mock<IFileRepository>();
            _playlistRepository = new Mock<IPlaylistRepository>();
            _songsFingerprinter = new Mock<SongsFingerprinter>();
        }

        private Mock<ISongRepository> _songRepository;
        private Mock<IFileRepository> _fileRepository;
        private Mock<IPlaylistRepository> _playlistRepository;
        private Mock<SongsFingerprinter> _songsFingerprinter;
    }
}
