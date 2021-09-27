using Id3.Frames;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Id3.Net.Tests
{
    public class CommonTests
    {
        private readonly string testFilesPath = Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.IndexOf("Id3") + 3), "NUnitTestFiles"));
        private readonly string[] testFiles = new[]
        {
            "whitenoise.mp3",
            //"brownian.ogg" //TODO: Make ogg Tags readable
        };
        [SetUp]
        public void SetUp()
        {
            for (int i = 0; i < this.testFiles.Length; i++)
            {
                this.testFiles[i] = Path.Combine(this.testFilesPath, this.testFiles[i]);
            }
        }

        [Test]
        public void ConstructorTest()
        {
            Mp3 m = null;

            Assert.Throws<ArgumentException>(() => { m = new("", Mp3Permissions.Read); });
            Assert.DoesNotThrow(() => { m = new(this.testFiles[0], Mp3Permissions.Read); m?.Dispose(); });
            using (FileStream f = File.Open(this.testFiles[0], FileMode.Open, FileAccess.Read))
            {
                Assert.DoesNotThrow(() => { m = new(f, Mp3Permissions.Read); });
            }
            m?.Dispose();
            FileInfo fileInfo = new(this.testFiles[0]);
            Assert.DoesNotThrow(() => { m = new(fileInfo, Mp3Permissions.Read); });
        }

        [Test]
        public void FileIdTest()
        {
            Mp3 m = null;
            Assert.DoesNotThrow(() => { m = new(this.testFiles[0], Mp3Permissions.Read); });

            Assert.IsTrue(m.HasTags);

            Assert.AreEqual(320, m.Audio.Bitrate);
            Assert.AreEqual(new TimeSpan(0, 0, 30), m.Audio.Duration);
            Assert.AreEqual(AudioMode.JointStereo, m.Audio.Mode);
            Assert.AreEqual(44100, m.Audio.Frequency);

            Id3Tag[] tags = m.GetAllTags().ToArray();

            Assert.AreEqual(1, tags.Length);
            Assert.AreEqual(0, tags[0].Track.TrackCount);
            Assert.AreEqual(3, tags[0].Track.Value);
            Assert.AreEqual(Id3TextEncoding.Iso8859_1, tags[0].Track.EncodingType);
            Assert.AreEqual(null, tags[0].Track.Padding);
            Assert.IsTrue(tags[0].Track.IsAssigned);

            Assert.AreEqual(1, tags[0].ArtistUrls.ToArray().Length);
            Assert.AreEqual("http://none.com", tags[0].ArtistUrls.ToArray()[0].Url);
            Assert.IsTrue(tags[0].ArtistUrls.ToArray()[0].IsAssigned);

            Assert.AreEqual(2, tags[0].Artists.Value.Count);
            Assert.AreEqual("electronic", tags[0].Artists.Value[0]);
            Assert.AreEqual("volt", tags[0].Artists.Value[1]);
            Assert.AreEqual(new AlbumFrame("Noise"), tags[0].Album);
            Assert.AreEqual("Noise", tags[0].Album.Value);
            Assert.AreEqual(new BandFrame("No Album Artist"), tags[0].Band);
            Assert.AreEqual("No Album Artist", tags[0].Band.Value);
            Assert.AreEqual(new Frames.BeatsPerMinuteFrame(137), tags[0].BeatsPerMinute);
            Assert.AreEqual(137, tags[0].BeatsPerMinute.Value);
            Assert.AreEqual(1, tags[0].Comments.Count);
            Assert.AreEqual("This is a comment", tags[0].Comments[0].Comment);
            Assert.AreEqual(Id3TextEncoding.Iso8859_1, tags[0].Comments[0].EncodingType);
            Assert.AreEqual(Id3Language.deu, tags[0].Comments[0].Language);
            Assert.AreEqual("None", tags[0].Conductor.Value);
            Assert.AreEqual(new ConductorFrame("None"), tags[0].Conductor);
            Assert.AreEqual("TheGroup", tags[0].ContentGroupDescription.Value);
            Assert.AreEqual(new ContentGroupDescriptionFrame("TheGroup"), tags[0].ContentGroupDescription);
            Assert.AreEqual(new EncoderFrame("None"), tags[0].Encoder);
            Assert.AreEqual("None", tags[0].Encoder.Value);
            Assert.AreEqual(Id3TagFamily.Version2X, tags[0].Family);
            Assert.AreEqual(new GenreFrame("Electro"), tags[0].Genre);
            Assert.AreEqual("Electro", tags[0].Genre.Value);
            Assert.AreEqual("Electro", tags[0].Genre.TextValue);
            Assert.AreEqual("Some Publisher", tags[0].Publisher.Value);
            Assert.AreEqual(new PublisherFrame("Some Publisher"), tags[0].Publisher);
            Assert.AreEqual(new SubtitleFrame("some Subtitle"), tags[0].Subtitle);
            Assert.AreEqual("some Subtitle", tags[0].Subtitle.Value);
            Assert.AreEqual("WhiteNoise", tags[0].Title.Value);
            Assert.AreEqual(new TitleFrame("WhiteNoise"), tags[0].Title);
            Assert.AreEqual(new YearFrame(1970), tags[0].Year);
            Assert.AreEqual(1970, tags[0].Year.Value);
            Assert.AreEqual(Id3Version.V23, tags[0].Version);

            m?.Dispose();
        }

        [Test]
        public void DeleteTags()
        {
            for (int i = 0; i < this.testFiles.Length; i++)
            {
                string testFileCopy = Path.Combine(Path.GetDirectoryName(this.testFiles[i]), Path.GetFileNameWithoutExtension(this.testFiles[i]) + "_copy" + Path.GetExtension(this.testFiles[i]));
                File.Copy(this.testFiles[i], testFileCopy, true);
                using (Mp3 m = new(testFileCopy, Mp3Permissions.ReadWrite))
                {
                    Assert.DoesNotThrow(() => { m.DeleteAllTags(); });
                    Assert.AreEqual(0, m.GetAllTags().ToArray().Length);
                }
                File.Delete(testFileCopy);
            }
        }

        [Test]
        public void GetTags()
        {
            for (int i = 0; i < this.testFiles.Length; i++)
            {
                using (Mp3 m = new(this.testFiles[i], Mp3Permissions.Read))
                {
                    Id3Tag id = m.GetTag(Id3TagFamily.Version2X);
                    Assert.AreEqual(17, id.Count());

                    id = m.GetTag(Id3Version.V23);
                    Assert.AreEqual(17, id.Count());
                }
            }
        }

        [Test]
        public void NewTag()
        {
            for (int i = 0; i < this.testFiles.Length; i++)
            {
                string testFileCopy = Path.Combine(Path.GetDirectoryName(this.testFiles[i]), Path.GetFileNameWithoutExtension(this.testFiles[i]) + "_copy" + Path.GetExtension(this.testFiles[i]));
                File.Copy(this.testFiles[i], testFileCopy, true);
                using (Mp3 m = new(testFileCopy, Mp3Permissions.ReadWrite))
                {
                    Id3Tag id3s = new();
                    id3s.Title = "This is a testtitle! :)";
                    m.WriteTag(id3s, WriteConflictAction.Replace);

                    Assert.AreEqual(id3s.Title, m.GetAllTags().ToArray().Select(x => x.Title).ToArray()[0]);
                    Assert.AreEqual(1, m.GetAllTags().ToArray().Length);
                }
                File.Delete(testFileCopy);
            }
        }

        [Test]
        public void EditValue()
        {
            for (int i = 0; i < this.testFiles.Length; i++)
            {
                string testFileCopy = Path.Combine(Path.GetDirectoryName(this.testFiles[i]), Path.GetFileNameWithoutExtension(this.testFiles[i]) + "_copy" + Path.GetExtension(this.testFiles[i]));
                File.Copy(this.testFiles[i], testFileCopy, true);
                using (Mp3 m = new(testFileCopy, Mp3Permissions.ReadWrite))
                {
                    for (int iii = 0; iii < 5; iii++)
                    {
                        Random rand = new(BitConverter.ToInt32(Guid.NewGuid().ToByteArray()));
                        StringBuilder sb = new();

                        for (int ii = 0; ii < rand.Next(8, 32); ii++)
                        {
                            sb.Append((char)rand.Next(1, 255));
                        }

                        Id3Tag s = m.GetTag(Id3Version.V23);
                        s.Album = sb.ToString();
                        m.UpdateTag(s);

                        Assert.AreEqual(sb.ToString(), m.GetAllTags().ToArray().Select(x => x.Album).ToArray()[0].Value);
                    }
                }
                File.Delete(testFileCopy);
            }
        }

        [Test(Description = "Write Audio stream and clone tags -> A 1:1 copy by manually writing audio stream and cloning tags, test method by length of file")]
        public void AudioStream()
        {
            for (int i = 0; i < this.testFiles.Length; i++)
            {
                string testFileCopy = Path.Combine(Path.GetDirectoryName(this.testFiles[i]), Path.GetFileNameWithoutExtension(this.testFiles[i]) + "_copy" + Path.GetExtension(this.testFiles[i]));
                File.Copy(this.testFiles[i], testFileCopy, true);
                using (Mp3 m = new(testFileCopy, Mp3Permissions.Read))
                {
                    string writeFileCopy = Path.Combine(Path.GetDirectoryName(testFileCopy), "sometest");
                    byte[] audio = m.GetAudioStream();

                    using (FileStream f = File.Create(writeFileCopy))
                    {
                        f.Write(audio, 0, audio.Length);
                    }

                    FileInfo fTest = new(testFileCopy);
                    FileInfo fClone = new(writeFileCopy);

                    Assert.Greater(fClone.Length, 0);
                    Assert.AreNotEqual(fTest.Length, fClone.Length);

                    using (Mp3 mClone = new(writeFileCopy, Mp3Permissions.ReadWrite))
                    {
                        mClone.WriteTag(m.GetTag(Id3TagFamily.Version2X));
                    }

                    fClone.Refresh();

                    //TODO: Some information is not included in Id3Tag class, like rating, cannot create a 1:1 copy by writing audio and cloning tags
                    //Assert.AreEqual(fTest.Length, fClone.Length);

                    File.Delete(writeFileCopy);
                }
                File.Delete(testFileCopy);
            }
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}