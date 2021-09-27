using Id3.Frames;
using NUnit.Framework;
using System.IO;

namespace Id3.Net.Tests
{
    [TestFixture]
    public sealed class DebuggingTests
    {
        private Mp3 _mp3;

        [SetUp]
        public void SetUp()
        {
            var stream = new MemoryStream();
            this._mp3 = new Mp3(stream, Mp3Permissions.ReadWrite);
        }

        [Test]
        public void DebugTest()
        {
            var tag1 = new Id3Tag
            {
                Track = new TrackFrame(3, 10) { Padding = 3 },
            };
            _mp3.WriteTag(tag1, Id3Version.V23);

            Id3Tag tag2 = _mp3.GetTag(Id3Version.V23);
            tag2.Track.Padding = 4;
            Assert.AreEqual(Id3Version.V23, tag2.Version);
            Assert.AreEqual(Id3TagFamily.Version2X, tag2.Family);
            Assert.AreEqual("0003/0010", tag2.Track.TextValue);
        }

        [TearDown]
        public void TearDown()
        {
            this._mp3?.Dispose();
        }
    }
}
