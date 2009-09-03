﻿using System;
using System.IO;
using urakawa.core;
using urakawa.core.visitor;
using urakawa.property.channel;

namespace urakawa.publish
{
    public abstract class AbstractBasePublishAudioVisitor : ITreeNodeVisitor
    {
        protected AbstractBasePublishAudioVisitor()
        {
            mCurrentAudioFileNumber = 0;
        }

        private const int BUFFER_SIZE = 6 * 1024 * 1024; // 6 MB
        protected void copyStreamData(Stream source, Stream dest)
        {
            if (source.Length <= BUFFER_SIZE)
            {
                byte[] buffer = new byte[source.Length];
                int read = source.Read(buffer, 0, (int)source.Length);
                dest.Write(buffer, 0, read);
            }
            else
            {
                byte[] buffer = new byte[BUFFER_SIZE];
                int bytesRead = 0;
                while ((bytesRead = source.Read(buffer, 0, BUFFER_SIZE)) > 0)
                {
                    dest.Write(buffer, 0, bytesRead);
                }
            }
        }

        protected int mCurrentAudioFileNumber;
        public int CurrentAudioFileNumber
        {
            get { return mCurrentAudioFileNumber; }
        }

        private string mAudioFileBaseNameFormat = "aud{0:000}.wav";
        public string AudioFileNameFormat
        {
            get { return mAudioFileBaseNameFormat; }
        }

        protected Uri GetCurrentAudioFileUri()
        {
            Uri res = DestinationDirectory;
            res = new Uri(res, String.Format(AudioFileNameFormat, CurrentAudioFileNumber));
            return res;
        }


        public abstract bool PreVisit(TreeNode node);
        public abstract void PostVisit(TreeNode node);

        public abstract bool TreeNodeTriggersNewAudioFile(TreeNode node);
        public abstract bool TreeNodeMustBeSkipped(TreeNode node);


        private Channel mSourceChannel;
        public Channel SourceChannel
        {
            get
            {
                if (mSourceChannel == null)
                {
                    throw new exception.IsNotInitializedException(
                        "The AbstractPublishManagedAudioVisitor has not been inistalized with a source Channel");
                }
                return mSourceChannel;
            }
            set
            {
                if (value == null)
                    throw new exception.MethodParameterIsNullException("The source Channel can not be null");
                mSourceChannel = value;
            }
        }
        
        private Channel mDestinationChannel;
        public Channel DestinationChannel
        {
            get
            {
                if (mDestinationChannel == null)
                {
                    throw new exception.IsNotInitializedException(
                        "The AbstractPublishManagedAudioVisitor has not been inistalized with a destination Channel");
                }
                return mDestinationChannel;
            }
            set
            {
                if (value == null)
                    throw new exception.MethodParameterIsNullException("The destination Channel can not be null");
                mDestinationChannel = value;
            }
        }
        
        private Uri mDestinationDirectory;
        public Uri DestinationDirectory
        {
            get
            {
                if (mDestinationChannel == null)
                {
                    throw new exception.IsNotInitializedException(
                        "The AbstractPublishManagedAudioVisitor has not ben initialized with a destination directory Uri");
                }
                return mDestinationDirectory;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The Uri of the destination directory can not be null");
                }
                mDestinationDirectory = value;
            }
        }
    }
}
