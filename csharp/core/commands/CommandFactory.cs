﻿using urakawa.command;
using urakawa.core;
using urakawa.media.data.audio;
using urakawa.media.timing;
using urakawa.xuk;
using urakawa.metadata;

namespace urakawa.commands
{
    public class CommandFactory : CommandFactoryBase
    {
        public override string GetTypeNameFormatted()
        {
            return XukStrings.CommandFactory;
        }
        public CommandFactory(Presentation pres) : base(pres)
        {
        }

        public TreeNodeSetManagedAudioMediaCommand CreateTreeNodeSetManagedAudioMediaCommand(
                                    TreeNode treeNode, ManagedAudioMedia managedMedia)
        {
            TreeNodeSetManagedAudioMediaCommand command = Create<TreeNodeSetManagedAudioMediaCommand>();
            command.Init(treeNode, managedMedia);
            return command;
        }

        public ManagedAudioMediaInsertDataCommand CreateManagedAudioMediaInsertDataCommand(TreeNode treeNode, ManagedAudioMedia managedAudioMediaTarget, ManagedAudioMedia managedAudioMediaSource, Time timeInsert, TreeNode currentTreeNode)
        {
            ManagedAudioMediaInsertDataCommand command = Create<ManagedAudioMediaInsertDataCommand>();
            command.Init(treeNode, managedAudioMediaTarget, managedAudioMediaSource, timeInsert, currentTreeNode);
            return command;
        }

        public TreeNodeAudioStreamDeleteCommand CreateTreeNodeAudioStreamDeleteCommand(TreeNodeAndStreamSelection selection)
        {
            TreeNodeAudioStreamDeleteCommand command = Create<TreeNodeAudioStreamDeleteCommand>();
            command.Init(selection);
            return command;
        }

        public MetadataAddCommand CreateMetadataAddCommand(Metadata metadata)
        {
            MetadataAddCommand command = Create<MetadataAddCommand>();
            command.Init(metadata);
            return command;
        }

        public MetadataRemoveCommand CreateMetadataRemoveCommand(Metadata metadata)
        {
            MetadataRemoveCommand command = Create<MetadataRemoveCommand>();
            command.Init(metadata);
            return command;
        }

        public MetadataSetContentCommand CreateMetadataSetContentCommand(Metadata metadata, string content)
        {
            MetadataSetContentCommand command = Create<MetadataSetContentCommand>();
            command.Init(metadata, content);
            return command;
        }

        public MetadataSetNameCommand CreateMetadataSetNameCommand(Metadata metadata, string content)
        {
            MetadataSetNameCommand command = Create<MetadataSetNameCommand>();
            command.Init(metadata, content);
            return command;
        }

        public MetadataSetIdCommand CreateMetadataSetIdCommand(Metadata metadata, string content)
        {
            MetadataSetIdCommand command = Create<MetadataSetIdCommand>();
            command.Init(metadata, content);
            return command;
        }
    }
}