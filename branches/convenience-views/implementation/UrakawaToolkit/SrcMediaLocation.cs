using System;

namespace urakawa.media
{
	/// <summary>
	/// An implementation of <see cref="IMediaLocation"/> based on a simple Src string value
	/// representing the uri or path of the media location
	/// </summary>
	public class SrcMediaLocation : IMediaLocation
	{
		private string mSrc = "";
		private IMediaFactory mFactory;

		/// <summary>
		/// Constructor initializing the <see cref="SrcMediaLocation"/> with a 
		/// </summary>
		protected internal SrcMediaLocation(IMediaFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("The media factory can not be null");
			}
			mFactory = fact;
		}

		/// <summary>
		/// Gets the Src value of <c>this</c>
		/// </summary>
		/// <returns>The Src value</returns>
		public string getSrc()
		{
			return mSrc;
		}

		/// <summary>
		/// Sets the Src value of this
		/// </summary>
		/// <param name="newSrc">The new Src value - must not be <c>null</c></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the new Src value is <c>null</c>
		/// </exception>
		public void setSrc(string newSrc)
		{
			if (newSrc == null)
			{
				throw new exception.MethodParameterIsNullException("The Src can not be null");
			}
			mSrc = newSrc;
		}

		/// <summary>
		/// Returns Src as <see cref="string"/> representation of <c>this</c>
		/// </summary>
		/// <returns>The Src value prefixed with MediaLocation=</returns>
		public override string ToString()
		{
			return String.Format("{0}={1}", GetType().Name, getSrc());
		}

		# region IMediaLocation members
		IMediaLocation IMediaLocation.copy()
		{
			return copy();
		}

		/// <summary>
		/// Copy the media location object.
		/// </summary>
		/// <returns>The copy</returns>
		/// <exception cref="exception.FactoryCanNotCreateTypeException">
		/// Thrown when the associated <see cref="IMediaFactory"/> 
		/// can not create a <see cref="SrcMediaLocation"/> instance
		/// </exception>
		public SrcMediaLocation copy()
		{
			IMediaLocation iCopyLoc = getMediaFactory().createMediaLocation(
				getXukLocalName(), getXukNamespaceUri());
			if (iCopyLoc == null || !(GetType().IsAssignableFrom(iCopyLoc.GetType())))
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The media factory could not create a {0} (QName {1}:{2})",
					GetType().FullName, getXukLocalName(), getXukNamespaceUri()));
			}
			SrcMediaLocation copyLoc = (SrcMediaLocation)iCopyLoc;
			copyLoc.setSrc(getSrc());
			return copyLoc;
		}

		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> associated with the <see cref="MediaLocation"/>
		/// </summary>
		/// <returns>The <see cref="IMediaFactory"/></returns>
		public IMediaFactory getMediaFactory()
		{
			return mFactory;
		}

		# endregion

		#region IXukAble Members

		/// <summary>
		/// Loads the <see cref="MediaLocation"/>from an xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the load was succesful</returns>
		/// <exception cref="">
		/// Thrown when the source <see cref="XmlReader"/> is <c>null</c>
		/// </exception>
		public bool XukIn(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The source XmlReader is null");
			}
			if (source.NodeType != System.Xml.XmlNodeType.Element) return false;
			string src = source.GetAttribute("mSrc");
			if (src == null) return false;
			setSrc(src);
			if (!source.IsEmptyElement)
			{
				//Read past element subtree, leaving the curcor the the element end tag
				source.ReadSubtree().Close();
			}
			return true;
		}

		/// <summary>
		/// Writes the <see cref="MediaLication"/> to an xuk element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the load was succesful</returns>
		/// <exception cref="">
		/// Thrown when the destination <see cref="XmlWriter"/> is <c>null</c>
		/// </exception>
		public bool XukOut(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The destination XmlWriter is null");
			}
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			destination.WriteAttributeString("mSrc", getSrc());
			destination.WriteEndElement();
			return true;
		}

		
		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="MediaLocation"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="MediaLocation"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion
	}
}
