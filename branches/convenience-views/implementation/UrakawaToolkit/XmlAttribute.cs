using System;
using System.Xml;

namespace urakawa.properties.xml
{
	/// <summary>
	/// Default implementation of <see cref="IXmlAttribute"/>
	/// </summary>
	public class XmlAttribute : IXmlAttribute
	{
		IXmlProperty mParent;
		string mName;
		string mNamespace;
		string mValue;

		internal XmlAttribute(IXmlProperty parent, string newName, string newNamespace, string newValue)
		{
			if(parent != null)
				mParent = parent;
			else
				throw(new urakawa.exception.MethodParameterIsNullException(
          "Parent IXmlProperty needs to be specified when creating an XMLAtribute."));

			if(newNamespace != null)
				mNamespace = newNamespace;
			else
				throw(new urakawa.exception.MethodParameterIsNullException(
          "Namespace of an XmlAtrribute cannot be null. Empty string is allowed."));

			if(newName != null && newName != "")
				mName = newName;
			else
				throw(new urakawa.exception.MethodParameterIsNullException(
          "Name of an XmlAtrribute cannot be null or empty."));

			if (newValue != null)
				mValue = newValue;
			else
				throw (new urakawa.exception.MethodParameterIsNullException(
					"Value of an XmlAtrribute cannot be null."));
			//@todo
			//throw an exception here or not?  attribute values can probably be empty.
		}
		#region IXmlAttribute Members

    /// <summary>
    /// Creates a copy of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns>The copy</returns>
    public IXmlAttribute copy()
		{
			XmlAttribute tmpAttr = new XmlAttribute(this.mParent,this.mName,this.mNamespace,this.mValue);
			return tmpAttr;
		}

    /// <summary>
    /// Gets the value of gthe <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns>The value</returns>
    public string getValue()
		{
			return mValue;
		}

    /// <summary>
    /// Sets the value of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <param name="newValue">The new value</param>
    public void setValue(string newValue)
		{
			mValue = newValue;
		}

    /// <summary>
    /// Gets the namespace of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns>The namespace</returns>
    public string getNamespace()
		{
				return mNamespace;
		}

    /// <summary>
    /// Gets the local name of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns>The local name</returns>
    public string getName()
		{
			return mName;
		}

    /// <summary>
    /// Sets the QName of the <see cref="XmlAttribute"/> 
    /// </summary>
    /// <param name="newNamespace">The namespace part of the new QName</param>
    /// <param name="newName">The name part of the new QName</param>
    public void setQName(string newName, string newNamespace)
		{
      mName = newName;
      mNamespace = newNamespace;
		}

    /// <summary>
    /// Gets the parent <see cref="IXmlProperty"/> of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns></returns>
    public IXmlProperty getParent()
		{
			return mParent;
		}

		#endregion
		#region IXUKAble members 

		//marisa's comment: i don't think this one is required
    /// <summary>
    /// Reads the <see cref="XmlAttribute"/> instance from an XmlAttribute element in a XUK file
    /// </summary>
    /// <param name="source">The source <see cref="XmlReader"/></param>
    /// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		public bool XUKIn(System.Xml.XmlReader source)
		{
      if (source == null)
      {
        throw new exception.MethodParameterIsNullException("Xml Reader is null");
      }
			if (source.Name != "XmlAttribute") return false;
			if (source.NamespaceURI != urakawa.ToolkitSettings.XUK_NS) return false;
			if (source.NodeType != System.Xml.XmlNodeType.Element) return false;

      string name = source.GetAttribute("name");
      if (name==null || name=="") return false;
      string ns = source.GetAttribute("namespace");
      if (ns==null) ns = "";
      setQName(name, ns);
			setValue(source.Value);
      if (!source.IsEmptyElement)
      {
				source.ReadSubtree().Close();
      }
			return true;
    }

    /// <summary>
    /// Writes a XmlAttribute element representing the <see cref="XmlAttribute"/> instance
    /// to a XUK file
    /// </summary>
    /// <param name="destination">The destination <see cref="XmlWriter"/></param>
    /// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		public bool XUKOut(System.Xml.XmlWriter destination)
		{
			destination.WriteStartElement("XmlAttribute", urakawa.ToolkitSettings.XUK_NS);

			//name is required
			if (mName == "") return false;

			destination.WriteAttributeString("name", mName);
			
			if (mNamespace != "") destination.WriteAttributeString("namespace", mNamespace);

			destination.WriteString(this.mValue);

			destination.WriteEndElement();

			return true;
		}
		#endregion
	}
}
