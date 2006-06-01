using System;
using System.Xml;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of interface <see cref="IPresentation"/>
	/// </summary>
	public class Presentation : IPresentation
	{
    /// <summary>
    /// Default constructor
    /// </summary>
		public Presentation()
		{
			mCoreNodeFactory = new CoreNodeFactory(this);
			mChannelFactory = new ChannelFactory();
			mChannelsManager = new ChannelsManager();
			mPropertyFactory = new PropertyFactory(this);
			mMediaFactory = new urakawa.media.MediaFactory();
			mRootNode = mCoreNodeFactory.createNode();
			
		}

		private CoreNode mRootNode;
		private CoreNodeFactory mCoreNodeFactory;
		private ChannelFactory mChannelFactory;
		private ChannelsManager mChannelsManager;
		private PropertyFactory mPropertyFactory;
		private urakawa.media.MediaFactory mMediaFactory;

    
		/// <summary>
		/// Gets the <see cref="ChannelsManager"/> managing the list of <see cref="IChannel"/>s
		/// in the <see cref="Presentation"/>
		/// </summary>
		/// <returns>The <see cref="ChannelsManager"/></returns>
		public ChannelsManager getChannelsManager()
		{
		return mChannelsManager;
		}

    /// <summary>
    /// Gets the <see cref="ICoreNodeFactory"/>
    /// creating <see cref="ICoreNode"/>s for the <see cref="Presentation"/>
    /// </summary>
    /// <returns></returns>
    public CoreNodeFactory getCoreNodeFactory()
    {
      return mCoreNodeFactory;
    }

    /// <summary>
    /// Gets the root <see cref="ICoreNode"/> of the <see cref="Presentation"/>
    /// </summary>
    /// <returns>The root <see cref="ICoreNode"/></returns>
    public CoreNode getRootNode()
    {
      return mRootNode;
    }

    /// <summary>
    /// Gets the <see cref="PropertyFactory"/> associated with the <see cref="Presentation"/>
    /// </summary>
    /// <returns>The <see cref="PropertyFactory"/></returns>
    public PropertyFactory getPropertyFactory()
    {
      return mPropertyFactory;
    }


    /// <summary>
    /// Gets the <see cref="IChannelFactory"/> that creates <see cref="IChannel"/>s 
    /// for the <see cref="Presentation"/>
    /// </summary>
    /// <returns>The <see cref="IChannelFactory"/></returns>
    public ChannelFactory getChannelFactory()
    {
      return mChannelFactory;
    }

	public urakawa.media.MediaFactory getMediaFactory()
	{
		return mMediaFactory;
	}

    #region IPresentation Members

    ICoreNode IPresentation.getRootNode()
    {
      return getRootNode();
    }

    IChannelFactory IPresentation.getChannelFactory()
    {
      return getChannelFactory();
    }

    IChannelsManager IPresentation.getChannelsManager()
    {
      return getChannelsManager();
    }

    ICoreNodeFactory IPresentation.getCoreNodeFactory()
    {
      return getCoreNodeFactory();
    }

    IPropertyFactory IPresentation.getPropertyFactory()
    {
      return getPropertyFactory();
    }
    #endregion

	#region IXUKable members 

	public bool XUKin(System.Xml.XmlReader source)
	{
		if (source == null)
		{
			throw new exception.MethodParameterIsNullException("XML Reader is null");
		}

		//if we are not at the opening tag for the Presentation element, return false
		if (!(source.Name == "Presentation" && 
			source.NodeType == System.Xml.XmlNodeType.Element))
		{
			return false;
		}
		
		System.Diagnostics.Debug.WriteLine("XUKin: Presentation");

		bool bProcessedChannelsManager = false;
		bool bProcessedRootNode = false;

    bool bFoundError = false;

    while (source.Read())
    {
      if (source.NodeType==XmlNodeType.Element)
      {
        switch (source.LocalName)
        {
          case "ChannelsManager":
            if (bProcessedChannelsManager) 
            {
              bFoundError = true;
            }
            else
            {
              bProcessedChannelsManager = true;
              if (!getChannelsManager().XUKin(source)) bFoundError = true;
            }
            break;
          case "CoreNode":
            if (bProcessedRootNode) 
            {
              bFoundError = true;
            }
            else
            {
              bProcessedRootNode = true;
              if (!getRootNode().XUKin(source)) bFoundError = true;
            }
            break;
          default:
            bFoundError = true;
            break;
        }
      }
      if (source.EOF) break;
      if (bFoundError) break;
    }
    return bProcessedChannelsManager && bProcessedRootNode && (!bFoundError);

//		//read until the end of the presentation element
//		while (!(source.NodeType == System.Xml.XmlNodeType.EndElement && 
//			source.Name == "Presentation")
//			&&
//			source.EOF == false)
//		{
//			source.Read();
//
//			if (source.Name == "ChannelsManager" && 
//				source.NodeType == System.Xml.XmlNodeType.Element)
//			{
//				bProcessedChannelsManager = this.mChannelsManager.XUKin(source);
//			}
//			else if (source.Name == "CoreNode" && 
//				source.NodeType == System.Xml.XmlNodeType.Element)
//			{
//				bProcessedRootNode = mRootNode.XUKin(source);
//			}
//		}
//
//		return (bProcessedChannelsManager && bProcessedRootNode);
		
	}

	public bool XUKout(System.Xml.XmlWriter destination)
	{
		if (destination == null)
		{
			throw new exception.MethodParameterIsNullException("Xml Writer is null");
		}

		destination.WriteStartElement("Presentation");

		bool bWroteChMgr = false;
		bool bWroteRoot = false;
		
		if (mChannelsManager != null)
		{
			bWroteChMgr = mChannelsManager.XUKout(destination);
		}

		if (mRootNode != null)
		{
			bWroteRoot = mRootNode.XUKout(destination);
		}
		
		destination.WriteEndElement();

		return (bWroteChMgr && bWroteRoot);
	}
	#endregion
  }
}
