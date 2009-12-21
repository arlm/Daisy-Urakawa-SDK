using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa.data;
using urakawa.exception;
using urakawa.xuk;

namespace urakawa.media.data.image
{
    public abstract class ImageMediaData : MediaData
    {
        public abstract string MimeType { get; }

        private DataProvider m_DataProvider;
        public DataProvider DataProvider
        {
            get
            {
                return m_DataProvider;
            }
            private set
            {
                m_DataProvider = value;
            }
        }

        public override IEnumerable<DataProvider> UsedDataProviders
        {
            get
            {
                yield return m_DataProvider;
                yield break;
            }
        }

        private string m_OriginalRelativePath;
        public string OriginalRelativePath
        {
            get
            {
                return m_OriginalRelativePath;
            }
            private set
            {
                m_OriginalRelativePath = value;
            }
        }

        public new ImageMediaData Copy()
        {
            return CopyProtected() as ImageMediaData;
        }

        public new ImageMediaData Export(Presentation destPres)
        {
            return ExportProtected(destPres) as ImageMediaData;
        }

        protected override MediaData CopyProtected()
        {
            ImageMediaData copyImageMediaData = (ImageMediaData)Presentation.MediaDataFactory.Create(GetType());
            // We do not Copy the FileDataProvider,
            // it is shared amongst ImageMediaData instances without concurrent access problems
            // because the file stream is read-only by design.
            copyImageMediaData.InitializeImage(m_DataProvider, OriginalRelativePath);
            return copyImageMediaData;
        }

        protected override MediaData ExportProtected(Presentation destPres)
        {
            ImageMediaData expImgData = (ImageMediaData)Presentation.MediaDataFactory.Create(GetType());
            expImgData.InitializeImage(m_DataProvider.Export(destPres), OriginalRelativePath);
            return expImgData;
        }


        public void InitializeImage(string path, string originalRelativePath)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new MethodParameterIsNullException("The path of a image can not be null");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found at " + path);
            }

            DataProvider imgDataProvider = Presentation.DataProviderFactory.Create(MimeType);
            ((FileDataProvider)imgDataProvider).InitByCopyingExistingFile(path);

            InitializeImage(imgDataProvider, originalRelativePath);
        }

        public void InitializeImage(DataProvider dataProv, string imgOriginalName)
        {
            if (dataProv == null)
            {
                throw new MethodParameterIsNullException("The data provider of a image can not be null");
            }

            if (dataProv.MimeType != MimeType)
            {
                throw new OperationNotValidException("The mime type of the given DataProvider is not correct !");
            }

            if (string.IsNullOrEmpty(imgOriginalName))
            {
                throw new MethodParameterIsEmptyStringException("original name of image cannot be null!");
            }

            DataProvider = dataProv;
            OriginalRelativePath = imgOriginalName;
        }

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            ImageMediaData otherz = other as ImageMediaData;
            if (otherz == null)
            {
                return false;
            }

            if (OriginalRelativePath != otherz.OriginalRelativePath)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }

            if (!DataProvider.ValueEquals(otherz.DataProvider))
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            return true;
        }

            public Stream OpenInputStream ()
                {
                if ( m_DataProvider == null )
                    {
                    throw new exception.IsNotInitializedException ("ImageMediaData is not initialized with an image!") ;
                    }


            return m_DataProvider.OpenInputStream () ;
                }

            

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            if (String.IsNullOrEmpty(OriginalRelativePath))
            {
                throw new XukException("The OriginalRelativePath of an ImageMediaData cannot be null or empty !");
            }
            destination.WriteAttributeString(XukStrings.OriginalRelativePath, OriginalRelativePath);

            if (DataProvider == null || String.IsNullOrEmpty(DataProvider.Uid))
            {
                throw new XukException("The DataProvider of an ImageMediaData cannot be null or empty !");
            }
            destination.WriteAttributeString(XukStrings.DataProvider, DataProvider.Uid);

            //if (!Presentation.Project.IsPrettyFormat())
            //{
            //    //destination.WriteAttributeString(XukStrings.Uid, Uid);
            //}
        }

        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string path = source.GetAttribute(XukStrings.OriginalRelativePath);
            if (String.IsNullOrEmpty(path))
            {
                throw new XukException("The OriginalRelativePath of an ImageMediaData cannot be null or empty !");
            }
            
            string uid = source.GetAttribute(XukStrings.DataProvider);
            if (String.IsNullOrEmpty(uid))
            {
                throw new XukException("The DataProvider of an ImageMediaData cannot be null or empty !");
            }
            DataProvider prov = Presentation.DataProviderManager.GetManagedObject(uid);

            if (prov == null)
            {
                throw new XukException(
                        String.Format("DataProvider cannot be found {0}", uid));
            }

            InitializeImage(prov, path);
        }

        /*
        {            
            FileInfo mImgFile = new FileInfo(path);

            // We are not using derived class but switch case to avoid complexities.We will be implementing the classes
           //  in future if needed.
            switch (mImgFile.Extension)
            {
                case ".jpg":
                    {
                        imgDataProvider = Presentation.DataProviderFactory.Create(DataProviderFactory.IMAGE_JPG_MIME_TYPE);
                        break;
                    }
                case ".png":
                    {
                        imgDataProvider = Presentation.DataProviderFactory.Create(DataProviderFactory.IMAGE_PNG_MIME_TYPE);
                        break;
                    }
                case ".svg":
                    {
                        imgDataProvider = Presentation.DataProviderFactory.Create(DataProviderFactory.IMAGE_SVG_MIME_TYPE);
                        break;
                    }
                default:
                        break;
            }
          
          FileStream originalImageStream = new FileStream ( path, FileMode.Open, FileAccess.Read );

            try
            {
               imgDataProvider.AppendData ( originalImageStream, originalImageStream.Length );
            }
            finally
            {
                originalImageStream.Close ();
                originalImageStream = null;
            }
            return imgDataProvider;
          }
         */
    }//Class
}//namespace