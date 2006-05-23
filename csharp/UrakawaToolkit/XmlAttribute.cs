using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for XmlAttribute.
	/// </summary>
	public class XmlAttribute:IXmlAttribute
	{
		IXmlProperty mParent;
		string mName;
		string mNamespace;
		string mValue;

		public XmlAttribute(IXmlProperty parent, string newNamespace, string newName, string newValue)
		{
			if(parent != null)
				mParent = parent;
			else
				throw(new urakawa.exception.MethodParameterIsNullException("Parent IXmlProperty needs to be specified when creating an XMLAtribute."));

			if(newNamespace != null)
				mNamespace = newNamespace;
			else
				throw(new urakawa.exception.MethodParameterIsNullException("Namespace of an XmlAtrribute cannot be null. Empty string is allowed."));

			if(newName != null && newName != "")
				mName = newName;
			else
				throw(new urakawa.exception.MethodParameterIsNullException("Name of an XmlAtrribute cannot be null or empty."));

			if (newValue != null && newValue != "")
				mValue = newValue;
			//@todo
			//throw an exception here or not?  attribute values can probably be empty.
		}
		#region IXmlAttribute Members

		public IXmlAttribute copy()
		{
			XmlAttribute tmpAttr = new XmlAttribute(this.mParent,this.mNamespace,this.mName,this.mValue);
			return tmpAttr;
		}

		public string getValue()
		{
			return mValue;
		}
		public void setValue(string newValue)
		{
			mValue = newValue;
		}

		public string getNamespace()
		{
				return mNamespace;
		}

		public string getName()
		{
			return mName;
		}

		public void setQName(string newNamespace, string newName)
		{

		}

		public IXmlProperty getParent()
		{
			// TODO:  Add XmlAttribute.getParent implementation
			return mParent;
		}

		#endregion
	}
}
