using System;
using System.Collections;
using System.Xml;

namespace urakawa.core
{
	/// <summary>
	/// Implementation of <see cref="CoreNode"/> interface
	/// </summary>
	public class CoreNode : TreeNode, ICoreNode
  {
    /// <summary>
    /// Compares two <see cref="CoreNode"/>s to see if they are equal 
    /// (they can belong to different <see cref="IPresentation"/>s and still be equal)
    /// </summary>
    /// <param name="cn1">The first <see cref="CoreNode"/></param>
    /// <param name="cn2">The second <see cref="CoreNode"/></param>
    /// <param name="testDeep">A <see cref="bool"/> indicating if the test should be deep,
    /// ie. if child nodes should also be tested for equality</param>
    /// <returns></returns>
    public static bool areCoreNodesEqual(CoreNode cn1, CoreNode cn2, bool testDeep)
    {
      foreach (PropertyType pt in PROPERTY_TYPE_ARRAY)
      {
        if ((cn1.getProperty(pt)!=null)!=(cn2.getProperty(pt)!=null))
        {
          return false;
        }
      }
      IChannelsProperty chp1 = (IChannelsProperty)cn1.getProperty(PropertyType.CHANNEL);
      IChannelsProperty chp2 = (IChannelsProperty)cn2.getProperty(PropertyType.CHANNEL);
      if (chp1!=null && chp2!=null)
      {
        System.Collections.IList chs1 = chp1.getListOfUsedChannels();
        System.Collections.IList chs2 = chp2.getListOfUsedChannels();
        if (chs1.Count!=chs2.Count) return false;
        for (int chIndex=0; chIndex<chs1.Count; chIndex++)
        {
          IChannel ch1 = (IChannel)chs1[chIndex];
          IChannel ch2 = (IChannel)chs2[chIndex];
          if (ch1.getName()!=ch2.getName()) return false;
          urakawa.media.IMedia m1 = chp1.getMedia(ch1);
          urakawa.media.IMedia m2 = chp2.getMedia(ch2);
          if ((m1!=null)!=(m2!=null)) return false;
          if (m1!=null)
          {
            if (m1.getType()!=m2.getType()) return false;
            if (
              m1.GetType().IsSubclassOf(typeof(urakawa.media.IClippedMedia))
              && m1.GetType().IsSubclassOf(typeof(urakawa.media.IClippedMedia)))
            {
              urakawa.media.IClippedMedia cm1 = (urakawa.media.IClippedMedia)m1;
              urakawa.media.IClippedMedia cm2 = (urakawa.media.IClippedMedia)m2;
              if (
                typeof(urakawa.media.Time).IsAssignableFrom(cm1.getDuration().GetType())
                && typeof(urakawa.media.Time).IsAssignableFrom(cm2.getDuration().GetType()))
              {
                if (
                  ((urakawa.media.Time)cm1.getDuration()).getTime()
                  !=((urakawa.media.Time)cm2.getDuration()).getTime())
                {
                  return false;
                }
              }

            }
            if (
              m1.GetType().IsSubclassOf(typeof(urakawa.media.IImageSize))
              && m1.GetType().IsSubclassOf(typeof(urakawa.media.IImageSize)))
            {
              urakawa.media.IImageSize ism1 = (urakawa.media.IImageSize)m1;
              urakawa.media.IImageSize ism2 = (urakawa.media.IImageSize)m2;
              if (ism1.getHeight()!=ism2.getHeight()) return false;
              if (ism1.getWidth()!=ism2.getWidth()) return false;
            }
          }
        }
      }
      IXmlProperty xp1 = (IXmlProperty)cn1.getProperty(PropertyType.XML);
      IXmlProperty xp2 = (IXmlProperty)cn2.getProperty(PropertyType.XML);
      if (xp1!=null && xp2!=null)
      {
        if (xp1.getName()!=xp2.getName()) return false;
        if (xp1.getNamespace()!=xp2.getNamespace()) return false;
        IList xp1Attrs = xp1.getListOfAttributes();
        IList xp2Attrs = xp2.getListOfAttributes();
        if (xp1Attrs.Count!=xp2Attrs.Count) return false;
        foreach (IXmlAttribute attr1 in xp1.getListOfAttributes())
        {
          IXmlAttribute attr2 = xp2.getAttribute(attr1.getName(), attr1.getNamespace());
          if (attr2==null) return false;
          if (attr1.getValue()!=attr2.getValue()) return false;
        }
        if (cn1.getChildCount()!=cn2.getChildCount()) return false;
        if (testDeep)
        {
          for (int index=0; index<cn1.getChildCount(); index++)
          {
            IBasicTreeNode ch1 = cn1.getChild(index);
            IBasicTreeNode ch2 = cn1.getChild(index);
            if (!typeof(CoreNode).IsAssignableFrom(ch1.GetType())) return false;
            if (!typeof(CoreNode).IsAssignableFrom(ch2.GetType())) return false;
            if (!areCoreNodesEqual((CoreNode)ch1, (CoreNode)ch2, true)) return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// An array holding the possible <see cref="PropertyType"/> 
    /// of <see cref="IProperty"/>s associated with the class.
    /// This array defines the indexing within the private member array <see cref="mProperties"/>
    /// of <see cref="IProperty"/>s of a given <see cref="PropertyType"/>
    /// </summary>
    private static PropertyType[] PROPERTY_TYPE_ARRAY 
      = (PropertyType[])Enum.GetValues(typeof(PropertyType));
    
    /// <summary>
    /// Gets the index of a given <see cref="PropertyType"/> 
    /// within <see cref="PROPERTY_TYPE_ARRAY"/>/<see cref="mProperties"/>
    /// </summary>
    /// <param name="type">The <see cref="PropertyType"/> for which to find the index</param>
    /// <returns>The index</returns>
    private static int IndexOfPropertyType(PropertyType type)
    {
      for (int i=0; i<PROPERTY_TYPE_ARRAY.Length; i++)
      {
        if (PROPERTY_TYPE_ARRAY[i]==type) return i;
      }
      return -1;
    }

    /// <summary>
    /// The owner <see cref="Presentation"/>
    /// </summary>
    private IPresentation mPresentation;


    /// <summary>
    /// Contains the <see cref="IProperty"/>s of the node
    /// </summary>
    private IProperty[] mProperties;

    /// <summary>
    /// Constructor setting the owner <see cref="Presentation"/>
    /// </summary>
    /// <param name="presentation"></param>
    internal CoreNode(IPresentation presentation)
    {
      mPresentation = presentation;
      mProperties = new IProperty[PROPERTY_TYPE_ARRAY.Length];
    }

    #region ICoreNode Members

    /// <summary>
    /// Gets the <see cref="Presentation"/> owning the <see cref="ICoreNode"/>
    /// </summary>
    /// <returns>The owner</returns>
    public IPresentation getPresentation()
    {
      return mPresentation;
    }

    /// <summary>
    /// Gets the <see cref="IProperty"/> of the given <see cref="PropertyType"/>
    /// </summary>
    /// <param name="type">The given <see cref="PropertyType"/></param>
    /// <returns>The <see cref="IProperty"/> of the given <see cref="PropertyType"/>,
    /// <c>null</c> if no property of the given <see cref="PropertyType"/> has been set</returns>
    public IProperty getProperty(PropertyType type)
    {
      return mProperties[IndexOfPropertyType(type)];
    }

    /// <summary>
    /// Sets a <see cref="IProperty"/>, possible overwriting previously set <see cref="IProperty"/>
    /// of the same <see cref="PropertyType"/>
    /// </summary>
    /// <param name="prop">The <see cref="IProperty"/> to set. 
    /// If <c>null</c> is passed, an <see cref="exception.MethodParameterIsNullException"/> is thrown</param>
    /// <returns>A <see cref="bool"/> indicating if a previously set <see cref="IProperty"/>
    /// was overwritten
    /// </returns>
    public bool setProperty(IProperty prop)
    {
      if (prop==null) throw new exception.MethodParameterIsNullException("No PropertyType was given");
      int index = IndexOfPropertyType(prop.getPropertyType());
      bool retVal = (mProperties[index]!=null);
      mProperties[index] = prop;
      prop.setOwner(this);
      return retVal;
    }

	/// <summary>
	/// Remove a property from the node's properties array
	/// Leave the slot available in the properties array (its size is fixed), but 
	/// make sure the contents are gone
	/// </summary>
	/// <param name="type">Specify the type of property to remove</param>
	/// <returns>The property which was just removed, or null if it did not exist</returns>
	public IProperty removeProperty(PropertyType type)
	{
		IProperty removedProperty = null;

		for (int i = 0; i<mProperties.Length; i++)
		{
			if (mProperties[i] != null && 
				mProperties[i].getPropertyType() == type)
			{
				removedProperty = mProperties[i];
				//we need to leave the slot in the array, just leave it empty
				mProperties[i] = null;
			}
		}

		if (removedProperty != null)
		{
			//a property which was just removed no longer has an owner
			//so set it to null
			removedProperty.setOwner(null);
		}
		return removedProperty;
	}

	/// <summary>
	/// Make a copy of the node.  The copy has the same presentation and no parent.
	/// </summary>
	/// <param name="deep">If true, then include the node's entire subtree.  
	/// Otherwise, just copy the node itself.</param>
	/// <returns>A <see cref="CoreNode"/> containing the copied data.</returns>
	public CoreNode copy(bool deep)
	{
		CoreNode theCopy = (CoreNode)this.getPresentation().getCoreNodeFactory().createNode();
	
		//copy the properties
		for (int i=0; i<this.mProperties.Length; i++)
		{
			if (this.mProperties[i] != null)
			{
				theCopy.setProperty(this.mProperties[i].copy());
			}
		}
		
		//copy the children
		if (deep == true)
		{
			for (int i=0; i<this.getChildCount(); i++)
			{
				//@todo does getType work this way?
				if (this.getChild(i).GetType() == typeof(urakawa.core.CoreNode))
				{
					theCopy.appendChild(((CoreNode)this.getChild(i)).copy(true));
				}
				else
				{
					//@todo what would be the graceful thing to do if it's not a core node?
				}
			}
		}

		return theCopy;
	}

    #endregion

    #region IVisitableCoreNode Members

    /// <summary>
    /// Accept a <see cref="ICoreNodeVisitor"/> in depth first mode
    /// </summary>
    /// <param name="visitor">The <see cref="ICoreNodeVisitor"/></param>
    public void acceptDepthFirst(ICoreNodeVisitor visitor)
    {
      if (visitor.preVisit(this))
      {
        for (int i=0; i<getChildCount(); i++)
        {
          ((ICoreNode)getChild(i)).acceptDepthFirst(visitor);
        }
      }
      visitor.postVisit(this);
    }

    /// <summary>
    /// Accept a <see cref="ICoreNodeVisitor"/> in breadth first mode
    /// </summary>
    /// <param name="visitor">The <see cref="ICoreNodeVisitor"/></param>
    /// <remarks>HACK: Not yet implemented, does nothing!!!!</remarks>
    public void acceptBreadthFirst(ICoreNodeVisitor visitor)
    {
      // TODO:  Add CoreNode.AcceptBreadthFirst implementation
    }

    #endregion

	  #region IXUKable members 

    /// <summary>
    /// Reads the <see cref="CoreNode"/> instance from a CoreNode xml element
    /// <list type="table">
    /// <item>
    /// <term>Entry state</term>
    /// <description>
    /// The cursor of <paramref name="source"/> must be at the start of the CoreNode element
    /// </description>
    /// </item>
    /// <item>
    /// <term>Exit state</term>
    /// </item>
    /// <description>
    /// The cursor of  <paramref name="source"/> must be at the end of the CoreNode element
    /// </description>
    /// </list>
    /// </summary>
    /// <param name="source">The <see cref="XmlReader"/> from which to read the core node</param>
    /// <returns>A <see cref="bool"/> indicating if the properties were succesfully read</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="source"/> is null
    /// </exception>
    public bool XUKin(System.Xml.XmlReader source)
	  {
		  if (source == null)
		  {
			  throw new exception.MethodParameterIsNullException("Xml Reader is null");
		  }

		  //if we are not at the opening tag of a core node element, return false
		  if (!(source.Name == "CoreNode" && source.NodeType == System.Xml.XmlNodeType.Element))
		  {
			  return false;
		  }

		  System.Diagnostics.Debug.WriteLine("XUKin: CoreNode");

      bool bFoundError = false;

      while (source.Read())
      {
        if (source.NodeType==XmlNodeType.Element)
        {
          switch (source.LocalName)
          {
            case "mProperties":
              if (!XUKin_Properties(source)) bFoundError = true;
              break;
            case "CoreNode":
              ICoreNode newChild = getPresentation().getCoreNodeFactory().createNode();
              if (newChild.XUKin(source))
              {
                this.appendChild(newChild);
              }
              else
              {
                bFoundError = false;
              }
              break;
          }
        }
        else if (source.NodeType==XmlNodeType.EndElement)
        {
          break;
        }
        if (source.EOF) break;
        if (bFoundError) break;
      }

      return !bFoundError;

//		  bool bPropertiesParsed = false;
//		  bool bNewNodesParsed = false;
//		  bool bFoundNodes = false;
//
//
//		  bool bSkipOneRead = false;
//
//		  //this part is TRICKY because it deals with nested elements
//		  //read until this CoreNode element ends, or until the document ends
//		  while (!(source.Name == "CoreNode" && 
//			  source.NodeType == System.Xml.XmlNodeType.EndElement) &&
//			  source.EOF == false)
//		  {
//			  //we might want to skip this
//			  if (bSkipOneRead == false)
//			  {
//				  source.Read();
//			  }
//			  else
//			  {
//				  bSkipOneRead = false;
//			  }
//  			
//			  //add the properties for this CoreNode
//			  if (source.Name == "mProperties" && source.NodeType == System.Xml.XmlNodeType.Element)
//			  {
//				  bPropertiesParsed = XUKin_Properties(source);
//			  }
//
//			  //if you encounter a CoreNode child, process it recursively
//			  else if (source.Name == "CoreNode" && source.NodeType == System.Xml.XmlNodeType.Element)
//			  {
//				  bool bTmpResult = false;					
//				  CoreNode newNode = new CoreNode(this.mPresentation);
//
//				  //process the XUK file on this new node
//				  bTmpResult = newNode.XUKin(source);
//  				
//				  if (bTmpResult == true)
//				  {
//					  this.appendChild(newNode);
//				  }
//
//				  //this is just error handling
//				  //accumulate the result of processing for all nodes found so far
//				  if (bFoundNodes == false)
//				  {					
//					  bNewNodesParsed = bTmpResult;
//				  }
//				  else
//				  {
//					  bNewNodesParsed = bNewNodesParsed && bTmpResult;
//				  }
//
//				  bFoundNodes = true;
//
//				  //VERY IMPORTANT PART
//				  //if we are at the end of a child CoreNode, read to the next element
//				  //and flag the system to skip one read the next time around the loop
//				  //this part is very important
//				  //if we don't call source.read() here, then the loop will exit because
//				  //it will see a </CoreNode>.  
//				  //and if we don't skip the next source.read(), it will never see
//				  //any new elements that are starting.
//				  if(source.Name == "CoreNode" && 
//					  source.NodeType == System.Xml.XmlNodeType.EndElement)
//				  {
//					  source.Read();
//					  bSkipOneRead = true;
//				  }
//			  }
//		  }
//  		
//
//		  //chlid nodes are not required, so if we didn't find any, just
//		  //return the results of the properties as the result of our processing
//		  if (bFoundNodes == false)
//		  {
//			  return bPropertiesParsed;
//		  }
//		  //if we did find child nodes, then judge our success by taking both
//		  //node and property processing into account
//		  else
//		  {
//			  return (bNewNodesParsed && bPropertiesParsed);
//		  }
	  }

    /// <summary>
    /// Writes the CoreNode element to a XUK file representing the <see cref="CoreNode"/> instance
    /// </summary>
    /// <param name="destination">The destination <see cref="XmlWriter"/></param>
    /// <returns>A <see cref="bool"/> indicating the write was succesful</returns>
	  public bool XUKout(System.Xml.XmlWriter destination)
	  {
		  if (destination == null)
		  {
			  throw new exception.MethodParameterIsNullException("Xml Writer is null");
		  }

		  bool bWroteProperties = true;
		  bool bWroteChildNodes = true;

		  destination.WriteStartElement("CoreNode");

		  destination.WriteStartElement("mProperties");

		  for (int i = 0; i<mProperties.Length; i++)
		  {
			  if (mProperties[i] != null)
			  {
				  bool bTmp = mProperties[i].XUKout(destination);
				  bWroteProperties = bTmp && bWroteProperties;
			  }
		  }

		  destination.WriteEndElement();

  		
		  for (int i = 0; i<this.getChildCount(); i++)
		  {
			  if (this.getChild(i).GetType() == typeof(CoreNode))
			  {
				  bool bTmp = ((CoreNode)this.getChild(i)).XUKout(destination);
				  bWroteChildNodes = bTmp && bWroteChildNodes;
			  }
			  else
			  {
				  //@todo
				  //will this case ever arise?
			  }
		  }

		  destination.WriteEndElement();

		  return (bWroteProperties && bWroteChildNodes);
	  }
	  /// <summary>
	  /// Helper function to read in the properties and invoke their respective XUKin methods. 
	  /// Reads the <see cref="IProperty"/>s of the <see cref="CoreNode"/> instance from a mProperties xml element
	  /// <list type="table">
	  /// <item>
	  /// <term>Entry state</term>
	  /// <description>
	  /// The cursor of <paramref name="source"/> must be at the start of the mProperties element
	  /// </description>
	  /// </item>
	  /// <item>
	  /// <term>Exit state</term>
	  /// </item>
	  /// <description>
	  /// The cursor of  <paramref name="source"/> must be at the end of the mProperties element
	  /// </description>
	  /// </list>
	  /// </summary>
	  /// <remarks>If the mProperties element is empty, the start and the end of of it are the nsame positions</remarks>
	  /// <param name="source">The <see cref="XmlReader"/> from which to read the properties</param>
	  /// <returns>A <see cref="bool"/> indicating if the properties were succesfully read</returns>
	  /// <exception cref="exception.MethodParameterIsNullException">
	  /// Thrown when the <paramref name="source"/> <see cref="XmlReader"/> is null
	  /// </exception>
	  private bool XUKin_Properties(System.Xml.XmlReader source)
	  {
      if (source == null)
      {
        throw new exception.MethodParameterIsNullException("Xml Reader is null");
      }
      if (!(source.Name == "mProperties" && 
			  source.NodeType == System.Xml.XmlNodeType.Element))
		  {
			  return false;
		  }

		  System.Diagnostics.Debug.WriteLine("XUKin: CoreNode::Properties");

      if (source.IsEmptyElement) return true;

      bool bFoundError = false;

      while (source.Read())
      {
        if (source.NodeType==XmlNodeType.Element)
        {
          IProperty newProp = null;
          switch (source.LocalName)
          {
            case "XmlProperty":
              newProp = getPresentation().getPropertyFactory().createXmlProperty("dummy", "");
              break;
            case "ChannelsProperty":
              newProp = getPresentation().getPropertyFactory().createChannelsProperty();
              break;
            default:
              bFoundError = true;
              break;
          }
          if (!bFoundError)
          {
            if (newProp.XUKin(source))
            {
              setProperty(newProp);
            }
            else
            {
              bFoundError = true;
            }
          }
        }
        else if (source.NodeType==XmlNodeType.EndElement)
        {
          break;
        }
        if (source.EOF) break;
        if (bFoundError) break;
      }
      return !bFoundError;
//
//		  bool bXmlPropertyProcessed = false;
//		  bool bXmlPropertyFound = false;
//		  bool bChannelsPropertyFound = false;
//		  bool bChannelsPropertyProcessed = false;
//
//		  while (!(source.Name == "mProperties" &&
//			  source.NodeType == System.Xml.XmlNodeType.EndElement)
//			  &&
//			  source.EOF == false)
//		  {
//			  source.Read();
//
//			  //set the xml property for this node
//			  if (source.Name == "XmlProperty" && 
//				  source.NodeType == System.Xml.XmlNodeType.Element)
//			  {
//				  bXmlPropertyFound = true;
//
//				  XmlProperty newXmlProp = (XmlProperty)mPresentation.getPropertyFactory().createXmlProperty("dummy", "");
//
//				  bXmlPropertyProcessed = newXmlProp.XUKin(source);
//				  if (bXmlPropertyProcessed == true)
//				  {
//					  this.setProperty(newXmlProp);
//				  }
//			  }
//
//			  //set the channels property for this node
//			  else if (source.Name == "ChannelsProperty" &&
//				  source.NodeType == System.Xml.XmlNodeType.Element)
//			  {
//				  bChannelsPropertyFound = true;
//
//				  ChannelsProperty newChannelsProp = 
//					  (ChannelsProperty)mPresentation.getPropertyFactory().createChannelsProperty();
//
//				  bChannelsPropertyProcessed = newChannelsProp.XUKin(source);
//
//				  if (bChannelsPropertyProcessed == true)
//				  {
//					  this.setProperty(newChannelsProp);
//				  }
//			  }
//		  }
//
//		  //now, decide what to return
//
//		  bool bXmlPropertyOk = false;
//		  bool bChannelsPropertyOk = false;
//
//		  //if we found an xml property, make sure it was processed ok
//		  if (bXmlPropertyFound == true)
//		  {
//			  bXmlPropertyOk = bXmlPropertyProcessed;
//		  }
//		  //if we didn't find one, that's ok too
//		  else
//		  {
//			  bXmlPropertyOk = true;
//		  }
//		  //if we found a channels property, make sure it was processed ok
//		  if (bChannelsPropertyFound == true)
//		  {
//			  bChannelsPropertyOk = bChannelsPropertyProcessed;
//		  }
//		  //if we didn't find one, that's ok too
//		  else
//		  {
//			  bChannelsPropertyOk = true;
//		  }
//
//		  return (bChannelsPropertyOk && bXmlPropertyOk);
	  }
	  #endregion

  }
}
