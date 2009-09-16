using System;
using System.Xml;
using urakawa.media.data.audio;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.media.data
{
    /// <summary>
    /// Default implementation of a <see cref="MediaDataManager"/>
    /// </summary>
    public sealed class MediaDataManager : XukAbleManager<MediaData>
    {

        public override string GetTypeNameFormatted()
        {
            return XukStrings.MediaDataManager;
        }

        public MediaDataManager(Presentation pres) : base(pres, "MD")
        {
            mEnforceSinglePCMFormat = true;
        }

        private bool mEnforceSinglePCMFormat;

        private bool isNewDefaultPCMFormatOk(PCMFormatInfo newDefault)
        {
            foreach (MediaData md in ManagedObjects.ContentsAs_YieldEnumerable)
            {
                AudioMediaData amd = md as AudioMediaData;
                if (amd != null)
                {
                    if (!amd.PCMFormat.ValueEquals(newDefault))
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        private PCMFormatInfo mDefaultPCMFormat;
        /// <summary>
        /// Gets (copy of) the default <see cref="audio.PCMFormatInfo"/> for <see cref="audio.AudioMediaData"/> managed by the manager 
        /// </summary>
        /// <returns>The default PCM format</returns>
        public PCMFormatInfo DefaultPCMFormat
        {
            get
            {
                if (mDefaultPCMFormat == null)
                {
                    mDefaultPCMFormat = new PCMFormatInfo();
                }
                return mDefaultPCMFormat;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The default PCM format of the manager can not be null");
                }
                if (!value.ValueEquals(mDefaultPCMFormat))
                {
                    if (EnforceSinglePCMFormat)
                    {
                        if (!isNewDefaultPCMFormatOk(value))
                        {
                            throw new exception.InvalidDataFormatException(
                                "Cannot change the default PCMFormat, since single PCM format is enforced by the DataProviderManager "
                                + "and since at least one AudioMediaData is currently managed");
                        }
                    }
                    mDefaultPCMFormat = value;
                }
            }
        }

        /// <summary>
        /// Sets the number of channels of the default <see cref="audio.PCMFormatInfo"/> for <see cref="audio.AudioMediaData"/> managed by the manager
        /// </summary>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the new value is less than <c>1</c>
        /// </exception>
        /// <exception cref="exception.InvalidDataFormatException">
        /// Thrown when the manager is enforcing single PCM format and a managed <see cref="audio.AudioMediaData"/> has a different number of channels
        /// </exception>
        //public ushort DefaultNumberOfChannels
        //{
        //    set
        //    {
        //        PCMFormatInfo newFormat = DefaultPCMFormat;
        //        newFormat.Data.NumberOfChannels = value;
        //        DefaultPCMFormat = newFormat;
        //    }
        //}

        /// <summary>
        /// Sets the sample rate of the default <see cref="audio.PCMFormatInfo"/> for <see cref="audio.AudioMediaData"/> managed by the manager
        /// </summary>
        /// <exception cref="exception.InvalidDataFormatException">
        /// Thrown when the manager is enforcing single PCM format and a managed <see cref="audio.AudioMediaData"/> has a different sample rate
        /// </exception>
        //public uint DefaultSampleRate
        //{
        //    set
        //    {
        //        PCMFormatInfo newFormat = DefaultPCMFormat;
        //        newFormat.Data.SampleRate = value;
        //        DefaultPCMFormat = newFormat;
        //    }
        //}

        /// <summary>
        /// Sets the number of channels of the default <see cref="audio.PCMFormatInfo"/> for <see cref="audio.AudioMediaData"/> managed by the manager
        /// </summary>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the new value is less than <c>1</c>
        /// </exception>
        /// <exception cref="exception.InvalidDataFormatException">
        /// Thrown when the manager is enforcing single PCM format and a managed <see cref="audio.AudioMediaData"/> has a different bit depth
        /// </exception>
        //public ushort DefaultBitDepth
        //{
        //    set
        //    {
        //        PCMFormatInfo newFormat = DefaultPCMFormat;
        //        newFormat.Data.BitDepth = value;
        //        DefaultPCMFormat = newFormat;
        //    }
        //}

        /// <summary>
        /// Sets the default PCM format by number of channels, sample rate and bit depth
        /// </summary>
        /// <param name="numberOfChannels">The number of channels</param>
        /// <param name="sampleRate">The sample rate</param>
        /// <param name="bitDepth">The bit depth</param>
        //public void SetDefaultPCMFormat(ushort numberOfChannels, uint sampleRate, ushort bitDepth)
        //{
        //    PCMFormatInfo newDefault = new PCMFormatInfo();
        //    newDefault.Data.NumberOfChannels = numberOfChannels;
        //    newDefault.Data.SampleRate = sampleRate;
        //    newDefault.Data.BitDepth = bitDepth;
        //    DefaultPCMFormat = newDefault;
        //}

        /// <summary>
        /// Gets a <see cref="bool"/> indicating if a single 
        /// PCMFormat is enforced for all managed <see cref="audio.AudioMediaData"/>
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public bool EnforceSinglePCMFormat
        {
            get { return mEnforceSinglePCMFormat; }
            set
            {
                if (value)
                {
                    if (!isNewDefaultPCMFormatOk(DefaultPCMFormat))
                    {
                        throw new exception.InvalidDataFormatException(
                            "Cannot enforce single PCM format, since at least one of the managed AudioMediaData "
                            + "has a PCMFormat that is different from the manager default");
                    }
                }
                mEnforceSinglePCMFormat = value;
            }
        }

        public override bool CanAddManagedObject(MediaData data)
        {
            if (EnforceSinglePCMFormat)
            {
                if (data is AudioMediaData)
                {
                    AudioMediaData amdata = (AudioMediaData)data;
                    if (!amdata.PCMFormat.ValueEquals(DefaultPCMFormat))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a copy of a given media data
        /// </summary>
        /// <param name="data">The media data to copy</param>
        /// <returns>The copy</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="data"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.IsNotManagerOfException">
        /// Thrown when <paramref name="data"/> is not managed by <c>this</c>
        /// </exception>
        public MediaData CopyMediaData(MediaData data)
        {
            if (data == null)
            {
                throw new exception.MethodParameterIsNullException("Can not copy a null AudioMediaData");
            }
            if (data.Presentation.MediaDataManager != this)
            {
                throw new exception.IsNotManagerOfException(
                    "Can not copy a AudioMediaData that is not managed by this");
            }
            return data.Copy();
        }

        /// <summary>
        /// Creates a copy of the media data with a given UID
        /// </summary>
        /// <param name="uid">The given UID</param>
        /// <returns>The copy</returns>
        /// <exception cref="exception.IsNotManagerOfException">
        /// Thrown when <c>this</c> does not manage a media data with the given UID
        /// </exception>
        public MediaData CopyMediaData(string uid)
        {
            MediaData data = GetManagedObject(uid);
            if (data == null)
            {
                throw new exception.IsNotManagerOfException(String.Format(
                                                                "The media data manager does not manage a media data with UID {0}",
                                                                uid));
            }
            return CopyMediaData(data);
        }


        #region IXukAble Members

        /// <summary>
        /// Clears the <see cref="MediaDataManager"/> disassociating any linked <see cref="MediaData"/>
        /// </summary>
        protected override void Clear()
        {
            foreach(MediaData md in ManagedObjects.ContentsAs_ListCopy)
            {
                ManagedObjects.Remove(md);
            }
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a MediaDataManager xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);

            string attr = source.GetAttribute(XukStrings.enforceSinglePCMFormat);
            if (attr == "true" || attr == "1")
            {
                EnforceSinglePCMFormat = true;
            }
            else
            {
                EnforceSinglePCMFormat = false;
            }
        }

        /// <summary>
        /// Reads a child of a MediaDataManager xuk element. 
        /// More specifically the <see cref="MediaData"/> managed by <c>this</c>
        /// is read from the mMediaData child.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukNamespaceUri)
            {
                readItem = true;
                if (source.LocalName == XukStrings.DefaultPCMFormat)
                {
                    XukInDefaultPCMFormat(source, handler);
                }
                else if (source.LocalName == XukStrings.MediaDatas)
                {
                    XukInMediaDatas(source, handler);
                }
                else if (true || !Presentation.Project.IsPrettyFormat()
                    // && source.LocalName == XukStrings.MediaDataItem
                    )
                {
                    //XukInMediaDataItem(source, handler);
                    XukInMediaData(source, handler);
                }
                else
                {
                    readItem = false;
                }
            }
            if (!(readItem || source.IsEmptyElement))
            {
                source.ReadSubtree().Close();
            }
        }

        private void XukInDefaultPCMFormat(XmlReader source, ProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == XukStrings.PCMFormatInfo
                            && source.NamespaceURI == XukNamespaceUri)
                        {
                            PCMFormatInfo newInfo = new PCMFormatInfo();
                            newInfo.XukIn(source, handler);
                            bool enf = EnforceSinglePCMFormat;
                            if (enf) EnforceSinglePCMFormat = false;
                            DefaultPCMFormat = newInfo;
                            if (enf) EnforceSinglePCMFormat = true;
                        }
                        else if (!source.IsEmptyElement)
                        {
                            source.ReadSubtree().Close();
                        }
                    }
                    else if (source.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }

        private void XukInMediaDatas(XmlReader source, ProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == XukStrings.MediaDataItem && source.NamespaceURI == XukNamespaceUri)
                        {
                            XukInMediaDataItem(source, handler);
                        }
                        else
                        {
                            XukInMediaData(source, handler);
                        }
                    
                    //else if (!source.IsEmptyElement)
                        //{
                        //    source.ReadSubtree().Close();
                        //}
                    }
                    else if (source.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }


        private void XukInMediaData(XmlReader source, ProgressHandler handler)
        {
            if (source.NodeType == XmlNodeType.Element)
            {
                MediaData data = null;
                data = Presentation.MediaDataFactory.Create(source.LocalName, source.NamespaceURI);
                if (data != null)
                {
                    data.XukIn(source, handler);

                    //string uid = source.GetAttribute(XukStrings.Uid);

                    if (string.IsNullOrEmpty(data.Uid))
                    {
                        throw new exception.XukException(
                            "uid attribute is missing from mMediaDataItem attribute");
                    }
                    if (IsManagerOf(data.Uid))
                    {
                        if (GetManagedObject(data.Uid) != data)
                        {
                            throw new exception.XukException(
                                String.Format("Another MediaData exists in the manager with uid {0}", data.Uid));
                        }
                    }
                    else
                    {
                        SetUidOfManagedObject(data, data.Uid);
                    }

                }
                else if (!source.IsEmptyElement)
                {
                    source.ReadSubtree().Close();
                }
            }
        }

        private void XukInMediaDataItem(XmlReader source, ProgressHandler handler)
        {
            MediaData data = null;
            if (!source.IsEmptyElement)
            {
                string uid = source.GetAttribute(XukStrings.Uid);

                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        data = Presentation.MediaDataFactory.Create(source.LocalName, source.NamespaceURI);
                        if (data != null)
                        {
                            string uid_ = source.GetAttribute(XukStrings.Uid);

                            data.XukIn(source, handler);

                            if (string.IsNullOrEmpty(uid_) && !string.IsNullOrEmpty(uid))
                            {
                                data.Uid = uid;
                            }

                            if (IsManagerOf(data.Uid))
                            {
                                if (GetManagedObject(data.Uid) != data)
                                {
                                    throw new exception.XukException(
                                        String.Format("Another MediaData exists in the manager with uid {0}", data.Uid));
                                }
                            }
                            else
                            {
                                SetUidOfManagedObject(data, data.Uid);
                            }
                        }

                    }
                    else if (source.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                }
            }
        }

        /// <summary>
        /// Writes the attributes of a MediaDataManager element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            base.XukOutAttributes(destination, baseUri);

            destination.WriteAttributeString(XukStrings.enforceSinglePCMFormat, EnforceSinglePCMFormat ? "true" : "false");
            
        }

        /// <summary>
        /// Write the child elements of a MediaDataManager element.
        /// Mode specifically the <see cref="MediaData"/> of <c>this</c> is written to a mMediaData element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            destination.WriteStartElement(XukStrings.DefaultPCMFormat, XukNamespaceUri);
            DefaultPCMFormat.XukOut(destination, baseUri, handler);
            destination.WriteEndElement();

            if (Presentation.Project.IsPrettyFormat())
            {
                destination.WriteStartElement(XukStrings.MediaDatas, XukNamespaceUri);
            }
            //foreach (string uid in mMediaDataDictionary.Keys)
            foreach (MediaData md in ManagedObjects.ContentsAs_YieldEnumerable)
            {
                if (false && Presentation.Project.IsPrettyFormat())
                {
                    destination.WriteStartElement(XukStrings.MediaDataItem, XukNamespaceUri);
                    //destination.WriteAttributeString(XukStrings.Uid, uid);
                }

                //mMediaDataDictionary[uid].XukOut(destination, baseUri, handler);
                md.XukOut(destination, baseUri, handler);

                if (false && Presentation.Project.IsPrettyFormat())
                {
                    destination.WriteEndElement();
                }
            }
            if (Presentation.Project.IsPrettyFormat())
            {
                destination.WriteEndElement();
            }

            base.XukOutChildren(destination, baseUri, handler);
        }

        #endregion

        #region IValueEquatable<MediaDataManager> Members

        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }
            MediaDataManager otherManager = other as MediaDataManager;

            if (otherManager == null)
            {
                return false;
            }


            if (otherManager.EnforceSinglePCMFormat != EnforceSinglePCMFormat)
            {
                return false;
            }
            if (!otherManager.DefaultPCMFormat.ValueEquals(DefaultPCMFormat))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}