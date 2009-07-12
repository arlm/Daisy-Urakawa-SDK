﻿using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.exception;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.progress;
using urakawa.property.channel;
using urakawa.xuk;

namespace urakawa.commands
{
    public class TreeNodeSetManagedAudioMediaCommand : Command
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.TreeNodeSetManagedAudioMediaCommand;
        }

        private TreeNode m_TreeNode;
        private ManagedAudioMedia m_ManagedAudioMedia;
        private Media m_PreviousMedia;

        public void Init(TreeNode treeNode, ManagedAudioMedia managedMedia)
        {
            if (treeNode == null)
            {
                throw new ArgumentNullException("TreeNode");
            }
            if (managedMedia == null)
            {
                throw new ArgumentNullException("ManagedAudioMedia");
            }
            if (treeNode.Presentation != managedMedia.Presentation)
            {
                throw new NodeInDifferentPresentationException("TreeNode vs ManagedAudioMedia");
            }
            if (treeNode.Presentation != Presentation)
            {
                throw new NodeInDifferentPresentationException("TreeNode vs ManagedAudioMedia");
            }
            m_TreeNode = treeNode;
            m_ManagedAudioMedia = managedMedia;

            m_ListOfUsedMediaData.Add(m_ManagedAudioMedia.AudioMediaData);

            AudioChannel audioChannel = Presentation.ChannelsManager.GetOrCreateAudioChannel();
            ChannelsProperty chProp = m_TreeNode.GetOrCreateChannelsProperty();
            m_PreviousMedia = chProp.GetMedia(audioChannel);
            if (m_PreviousMedia is ManagedAudioMedia)
            {
                m_ListOfUsedMediaData.Add(((ManagedAudioMedia)m_PreviousMedia).AudioMediaData);
            }
            if (m_PreviousMedia is SequenceMedia)
            {
                foreach (Media media in ((SequenceMedia)m_PreviousMedia).ListOfItems)
                {
                    if (media is ManagedAudioMedia)
                    {
                        m_ListOfUsedMediaData.Add(((ManagedAudioMedia)media).AudioMediaData);
                    }
                }
            }

            ShortDescription = "Add new audio";
            LongDescription = "Attach a ManagedAudioMedia to a TreeNode in the AudioChannel via the ChannelsProperty";
        }

        public override bool CanExecute
        {
            get { return true; }
        }

        public override bool CanUnExecute
        {
            get { return true; }
        }

        public override void Execute()
        {
            AudioChannel audioChannel = Presentation.ChannelsManager.GetOrCreateAudioChannel();
            ChannelsProperty chProp = m_TreeNode.GetOrCreateChannelsProperty();
            chProp.SetMedia(audioChannel, m_ManagedAudioMedia);
        }

        public override void UnExecute()
        {
            AudioChannel audioChannel = Presentation.ChannelsManager.GetOrCreateAudioChannel();
            ChannelsProperty chProp = m_TreeNode.GetOrCreateChannelsProperty();
            chProp.SetMedia(audioChannel, m_PreviousMedia);
        }

        private List<MediaData> m_ListOfUsedMediaData = new List<MediaData>();
        public override List<MediaData> ListOfUsedMediaData
        {
            get
            {
                return m_ListOfUsedMediaData;
            }
        }

        protected override void XukInAttributes(XmlReader source)
        {
            //nothing new here
            base.XukInAttributes(source);
        }

        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            //nothing new here
            base.XukInChild(source, handler);
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            //nothing new here
            base.XukOutAttributes(destination, baseUri);
        }

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            //nothing new here
            base.XukOutChildren(destination, baseUri, handler);
        }
    }
}
