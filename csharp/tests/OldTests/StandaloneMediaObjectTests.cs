using System;
using NUnit.Framework;
using urakawa.media;
using urakawa.media.timing;
using urakawa.xuk;

namespace urakawa.oldTests
{
    /// <summary>
    /// Test media objects
    /// </summary>
    [TestFixture]
    public class StandaloneMediaObjectTests
    {
        private Presentation pres;
        private MediaFactory factory;

        [SetUp]
        public void Init()
        {
            Project proj = new Project();
            pres = proj.AddNewPresentation();
            factory = pres.MediaFactory;
        }

        /// <summary>
        /// Assign clip begin and end values and see if the duration is correct
        /// Use milliseconds (long)
        /// </summary>
        [Test]
        public void CheckAudioDuration_SimpleMS()
        {
            ExternalAudioMedia audio = factory.Create<ExternalAudioMedia>();

            audio.ClipBegin = new Time(0);
            audio.ClipEnd = new Time(1000);

            TimeDelta td = (TimeDelta) audio.Duration;

            Assert.AreEqual(1000, td.TimeDeltaAsMillisecondLong);
        }

        /// <summary>
        /// Split the audio node and check the new end time
        /// Use milliseconds (long)
        /// </summary>
        [Test]
        public void SplitAudioObjectCheckNewTimes_SimpleMS()
        {
            ExternalAudioMedia obj = factory.Create<ExternalAudioMedia>();

            obj.ClipBegin = new Time(0);
            obj.ClipEnd = new Time(1000);

            ExternalAudioMedia new_obj = obj.Split(new Time(600));

            //check begin/end times for original node
            Time t = (Time) obj.ClipBegin;
            Assert.AreEqual(0, t.TimeAsMillisecondLong);

            t = (Time) obj.ClipEnd;
            Assert.AreEqual(600, t.TimeAsMillisecondLong);

            //check begin/end times for newly created node
            t = (Time) new_obj.ClipBegin;
            Assert.AreEqual(600, t.TimeAsMillisecondLong);

            t = (Time) new_obj.ClipEnd;
            Assert.AreEqual(1000, t.TimeAsMillisecondLong);
        }

        /// <summary>
        /// Split the audio node and check the new duration
        /// Use milliseconds (long)
        /// </summary>
        [Test]
        public void SplitVideoObjectCheckNewDuration_SimpleMS()
        {
            ExternalVideoMedia obj = factory.Create<ExternalVideoMedia>();

            obj.ClipBegin = new Time(0);
            obj.ClipEnd = new Time(1000);

            AbstractVideoMedia new_obj = (AbstractVideoMedia) obj.Split(new Time(600));

            TimeDelta td_1 = obj.Duration;
            TimeDelta td_2 = new_obj.Duration;

            Assert.AreEqual(600, td_1.TimeDeltaAsMillisecondLong);
            Assert.AreEqual(400, td_2.TimeDeltaAsMillisecondLong);
        }

        /// <summary>
        /// 1. set the src location and check that it has been set correctly
        /// 2. set to a different src location, using a different string,
        /// and see that a. it is correct and b. it is not the same as the previous location
        /// </summary>
        [Test]
        public void setAndGetImageMediaLocation()
        {
            string src = "myfile.ext";
            string src2 = "myotherfile.ext";

            ExternalImageMedia obj = factory.Create<ExternalImageMedia>();

            obj.Src = src;

            Assert.AreEqual(obj.Src, src);

            obj.Src = src2;

            Assert.AreNotEqual(src, src2);
            Assert.AreEqual(obj.Src, src2);
        }

        [Test]
        public void checkTypeAfterCopy()
        {
            AbstractAudioMedia audio = factory.Create<ExternalAudioMedia>();

            AbstractAudioMedia audio_copy = (AbstractAudioMedia) audio.Copy();

            Assert.AreEqual(audio_copy.GetType(), audio.GetType());
        }

        [Test]
        public void checkAudioMediaCopy()
        {
            ExternalAudioMedia audio = factory.Create<ExternalAudioMedia>();
            bool exceptionOccured = false;
            try
            {
                Media audio_copy = ((Media) audio).Copy();
            }
            catch (exception.OperationNotValidException)
            {
                exceptionOccured = true;
            }
            Assert.IsFalse(
                exceptionOccured,
                "Media.copy() method was probably not overridden in AbstractAudioMedia subclass of ExternalMedia");
        }

        /// <summary>
        /// Check that the media node is reporting its static property correctly
        /// </summary>
        [Test]
        public void checkAudioMediaStaticProperties()
        {
            ExternalAudioMedia obj = factory.Create<ExternalAudioMedia>();
            Assert.AreEqual(obj.IsContinuous, true);
            Assert.AreEqual(obj.IsDiscrete, false);
            Assert.AreEqual(obj.IsSequence, false);
        }

        /// <summary>
        /// see if a new empty sequence is indeed empty and has the right type
        /// </summary>
        [Test]
        public void isEmptySequenceReallyEmpty()
        {
            SequenceMedia obj = factory.CreateSequenceMedia();

            Assert.AreEqual(true, obj.IsSequence);
            Assert.AreEqual(0, obj.ChildMedias.Count);
            Assert.AreEqual(false, obj.IsContinuous);
            Assert.AreEqual(false, obj.IsDiscrete);
        }

        /// <summary>
        /// See if the SequenceMedia object will throw an illegal type exception
        /// when we try to add different media types to it
        /// Also check that it contains the correct number of objects after the append fails
        /// And see that its type is correctly reported.
        /// </summary>
        [Test]
        [ExpectedException(typeof (exception.MediaNotAcceptable))]
        public void canSequenceMediaHoldOnlyOneMediaType()
        {
            SequenceMedia obj = factory.CreateSequenceMedia();

            AbstractAudioMedia audio_obj = factory.Create<ExternalAudioMedia>();
            AbstractTextMedia text_obj = factory.CreateTextMedia();

            obj.InsertItem(obj.ChildMedias.Count, audio_obj);

            obj.InsertItem(obj.ChildMedias.Count, text_obj);
            Assert.Fail("The previous should have thrown an MediaNotAcceptable exception");
        }

        /// <summary>
        /// This test checks that if you change the text on a TextMedia copy
        /// that the original (source) TextMedia object does not have its
        /// text changed as well.
        /// </summary>
        [Test]
        public void CopyTextMediaRenameAndCheckAgain()
        {
            TextMedia text_obj = factory.Create<TextMedia>();
            text_obj.Text = "original media object";

            AbstractTextMedia copy_obj = (AbstractTextMedia) text_obj.Copy();

            copy_obj.Text = "copied media object";

            Assert.AreNotEqual(text_obj.Text, copy_obj.Text);
        }

        //[Test, Ignore("Needs connection and sometimes takes a long time")]
        //public void PlainTextMediaGetTextHttpTest()
        //{
        //  TestPlainTextMediaGetText("http://www.daisy.org/z3986/2005/ncx-2005-1.dtd", "<!-- NCX 2005-1 DTD  2005-06-26");
        //}

        //[Test]
        //public void PlainTextMediaGetTextFileTest()
        //{
        //  TestPlainTextMediaGetText("../XukWorks/xuk.xsd", "<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        //}

        //private void TestPlainTextMediaGetText(string uri, string expectedStartOfFile)
        //{
        //  ExternalTextMedia text_obj = (ExternalTextMedia)factory.CreateMedia(
        //    typeof(ExternalTextMedia).Name, XukAble.XUK_NS);
        //  text_obj.setSrc(uri);
        //  string text = text_obj.GetText();
        //  Assert.IsTrue(text.StartsWith(expectedStartOfFile), "The file at uri {0} did not start with '{1}'", uri, expectedStartOfFile);
        //}

        //[Test]
        //public void PlainTextMediaSetTextFileTest()
        //{
        //  ExternalTextMedia text_obj = (ExternalTextMedia)factory.CreateMedia(
        //    typeof(ExternalTextMedia).Name, XukAble.XUK_NS);
        //  text_obj.setSrc("temp.txt");
        //  string text = "Test textual content\n������@��";
        //  text_obj.SetText(text);
        //  TestPlainTextMediaGetText(text_obj.getSrc(), text);
        //  text = text + "\nAppended this";
        //  text_obj.SetText(text);
        //  TestPlainTextMediaGetText(text_obj.getSrc(), text);
        //  Uri tempFileUri = new Uri(factory.getPresentation().getRootUri(), text_obj.getSrc());
        //  System.IO.File.Delete(tempFileUri.LocalPath);
        //}

        //[Test]
        //[ExpectedException(typeof(exception.OperationNotValidException))]
        //public void PlainTextMediaSetTextHttpTest()
        //{
        //  ExternalTextMedia text_obj = (ExternalTextMedia)factory.CreateMedia(
        //    typeof(ExternalTextMedia).Name, XukAble.XUK_NS);
        //  string src = "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd";
        //  text_obj.setSrc(src);
        //  text_obj.SetText("Oops, I replaced the Z39.86-2005 version 1 NCX DTD");
        //}
    }
}