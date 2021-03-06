using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using urakawa.xuk;

namespace urakawa.media.data
{
    [TestFixture]
    public class FileDataProviderTests : IDataProviderTests
    {
        protected FileDataProvider mFileDataProvider1
        {
            get { return mDataProvider1 as FileDataProvider; }
        }

        protected FileDataProvider mFileDataProvider2
        {
            get { return mDataProvider2 as FileDataProvider; }
        }

        protected FileDataProvider mFileDataProvider3
        {
            get { return mDataProvider3 as FileDataProvider; }
        }

        public FileDataProviderTests()
            : base(typeof (FileDataProvider).Name, XukAble.XUK_NS)
        {
        }

        #region DataProvider tests

        [Test]
        public override void OpenInputStream_InitialState()
        {
            base.OpenInputStream_InitialState();
        }

        [Test]
        public override void OpenInputStream_CanGetMultiple()
        {
            base.OpenInputStream_CanGetMultiple();
        }

        [Test]
        public override void OpenOutputStream_InitialState()
        {
            base.OpenOutputStream_InitialState();
        }

        [Test]
        [ExpectedException(typeof (exception.OutputStreamOpenException))]
        public override void OpenOutputStream_CannotGetMultiple()
        {
            base.OpenOutputStream_CannotGetMultiple();
        }

        [Test]
        public override void OpenOutputStream_RetrieveDataWritten()
        {
            base.OpenOutputStream_RetrieveDataWritten();
        }

        [Test]
        public override void GetUid_Basics()
        {
            base.GetUid_Basics();
        }

        [Test]
        public override void Delete_Basics()
        {
            base.Delete_Basics();
        }

        [Test]
        public void Delete_DataFilesDeleted()
        {
            DataProviderManager mngr = mPresentation.DataProviderManager as DataProviderManager;
            string path = mngr.DataFileDirectoryFullPath;
            Assert.Greater(mngr.ManagedObjects.Count, 0, "The manager does not manage any DataProviders");
            foreach (DataProvider prov in mngr.ManagedObjects.ContentsAs_YieldEnumerable)
            {
                Stream outStm = prov.OpenOutputStream();
                try
                {
                    outStm.WriteByte(0xAA); //Ensure that files are created
                }
                finally
                {
                    outStm.Close();
                }
            }
            foreach (DataProvider prov in mngr.ManagedObjects.ContentsAs_ListCopy)
            {
                prov.Delete();
            }
            Assert.AreEqual(
                0, mngr.ManagedObjects.Count,
                "The manager still contains DataProviders after they are all deleted");
            Assert.IsTrue(
                Directory.Exists(path),
                "The data file directory of the FileDataManager does not exist");
            Assert.AreEqual(
                0, Directory.GetFiles(path).Length,
                "The data file directory of the FileDataManager is not empty: " + path);
        }

        [Test]
        [ExpectedException(typeof (exception.InputStreamsOpenException))]
        public override void Delete_OpenInputStream()
        {
            base.Delete_OpenInputStream();
        }

        [Test]
        [ExpectedException(typeof (exception.OutputStreamOpenException))]
        public override void Delete_OpenOutputStream()
        {
            base.Delete_OpenOutputStream();
        }

        [Test]
        public override void Copy_Basics()
        {
            base.Copy_Basics();
        }

        #endregion

        #region IValueEquatable tests

        [Test]
        public override void ValueEquals_Basics()
        {
            base.ValueEquals_Basics();
        }

        [Test]
        public override void ValueEquals_MimeType()
        {
            base.ValueEquals_MimeType();
        }

        #endregion
    }
}