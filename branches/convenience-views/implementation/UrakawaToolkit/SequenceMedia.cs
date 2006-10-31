using System;
using System.Collections.Generic;
using System.Xml;

namespace urakawa.media
{

	/// <summary>
	/// SequenceMedia is a collection of same-type media objects
	/// The first object in the collection determines the collection's type.
	/// </summary>
	public class SequenceMedia : ISequenceMedia
	{
		private IList<IMedia> mSequence;
		private IMediaFactory mMediaFactory;

		/// <summary>
		/// Constructor setting the associated <see cref="IMediaFactory"/>
		/// </summary>
		/// <param name="fact">
		/// The <see cref="IMediaFactory"/> to associate the <see cref="SequenceMedia"/> with
		/// </param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="fact"/> is <c>null</c>
		/// </exception>
		protected internal SequenceMedia(IMediaFactory fact)
		{
			mSequence = new List<IMedia>();
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("Factory is null");
			}
			mMediaFactory = fact;
		}

		#region ISequenceMedia Members

		/// <summary>
		/// Get the item at the given index
		/// </summary>
		/// <param name="index">Index of the item to return</param>
		/// <returns></returns>
		public IMedia getItem(int index)
		{
			if (0<=index && index<getCount())
			{
				return (IMedia)mSequence[index];
			}
			else
			{
				throw new exception.MethodParameterIsOutOfBoundsException("SequenceMedia.getItem(" +
					index.ToString() + ") caused MethodParameterIsOutOfBoundsException");
			}
		}


		/// <summary>
		/// Inserts a given <see cref="IMedia"/> item at a given index
		/// </summary>
		/// <param name="index">The given index</param>
		/// <param name="newItem">The given <see cref="IMedia"/> item</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the given <see cref="IMedia"/> to insert is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterOutOfBoundsException">
		/// Thrown when the given index is out of bounds
		/// </exception>
		/// <exception cref="exception.MethodParameterIsWrongTypeException">
		/// The <see cref="IMedia"/> item to insert has a <see cref="MediaType"/> that 
		/// is incompatible with the <see cref="MediaSequence"/>
		/// </exception>
		/// <remarks>
		/// The first <see cref="IMedia"/> inserted into an <see cref="MediaSequence"/> 
		/// determines it's <see cref="MediaType"/>. 
		/// Prior to the first insertion an <see cref="IMediaSequence"/> has <see cref="MediaType"/>
		/// <see cref="MediaType.EMPTY_SEQUENCE"/>
		/// </remarks>
		public void insertItem(int index, IMedia newItem)
		{
			if (index < 0 || getCount() <= index)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The index at which to insert media is out of bounds");
			}
			if (!isAllowed(newItem))
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"The new media to insert is of a type that is incompatible with the sequence media");
			}
			mSequence.Insert(index, newItem);
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Remove an item from the sequence.
		/// </summary>
		/// <param name="index">The index of the item to remove.</param>
		/// <returns>The removed <see cref="IMedia"/> item</returns>
		/// <exception cref="exception.MethodParameterOutOfBoundsException">
		/// Thrown when the given index is out of bounds
		/// </exception>
		public IMedia removeItem(int index)
		{
			IMedia removedMedia = getItem(index);
			mSequence.RemoveAt(index);
			return removedMedia;
		}

		/// <summary>
		/// Return the number of items in the sequence.
		/// </summary>
		/// <returns></returns>
		public int getCount()
		{
			return mSequence.Count;
		}

		#endregion

		#region IMedia Members


		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> associated with the <see cref="ISequenceMedia"/>
		/// </summary>
		/// <returns>The <see cref="IMediaFactory"/></returns>
		public IMediaFactory getMediaFactory()
		{
			return mMediaFactory;
		}


		/// <summary>
		/// Use the first item in the collection to determine if this sequence is continuous or not.
		/// </summary>
		/// <returns></returns>
		public bool isContinuous()
		{
			if (getCount() > 0)
			{
				return getItem(0).isContinuous();
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Use the first item in the collection to determine if this 
		/// sequence is discrete or not.
		/// </summary>
		/// <returns></returns>
		public bool isDiscrete()
		{
			//use the first item in the collection to determine the value
			if (getCount() > 0)
			{
				return getItem(0).isDiscrete();
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// This function always returns true, because this 
		/// object is always considered to be a sequence (even if it contains only one item).
		/// </summary>
		/// <returns><c>true</c></returns>
		public bool isSequence()
		{
			return true;
		}

		/// <summary>
		/// If the sequence is non-empty, then this function will return the <see cref="MediaType"/> of
		/// <see cref="IMedia"/> items it contains (it will only contain one type at a time)
		/// If the sequence is empty, this function will return <see cref="MediaType.EMPTY_SEQUENCE"/>.
		/// </summary>
		/// <returns>The <see cref="MediaType"/></returns>
		public MediaType getMediaType()
		{
			//use the first item in the collection to determine the value
			if (getCount() > 0)
			{
				return getItem(0).getMediaType();
			}
			else
			{
				return MediaType.EMPTY_SEQUENCE;
			}
		}

		IMedia IMedia.copy()
		{
			return copy();
		}

		/// <summary>
		/// Make a copy of this media sequence
		/// </summary>
		/// <returns>The copy</returns>
		public ISequenceMedia copy()
		{
			IMedia newMedia = getMediaFactory().createMedia(
				getXukLocalName(), getXukNamespaceUri());
			if (!(newMedia is ISequenceMedia))
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The media factory can not create an ISequenceMedia matching QName {0}:{1}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			ISequenceMedia newSeqMedia = (ISequenceMedia)newMedia;
			foreach (IMedia item in mSequence)
			{
				newSeqMedia.insertItem(newSeqMedia.getCount(), item.copy());
			}
			return newSeqMedia;
		}

		#endregion


		/// <summary>
		/// test a new media object to see if it can belong to this collection 
		/// (only objects of the same type are allowed)
		/// </summary>
		/// <param name="proposedAddition"></param>
		/// <returns></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the proposed addition is null
		/// </exception>
		private bool isAllowed(IMedia proposedAddition)
		{
			if (proposedAddition == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The proposed addition is null");
			}
			if (getMediaType() == MediaType.EMPTY_SEQUENCE) return true;
			return (getMediaType() == proposedAddition.getMediaType());
		}

		private string getTypeAsString()
		{
			MediaType type = this.getMediaType();
			switch (type)
			{
				case MediaType.EMPTY_SEQUENCE:
					return String.Empty;
				default:
					return type.ToString("g");
			}
		}


		#region IXukAble Members

		/// <summary>
		/// Reads the <see cref="SequenceMedia"/> from an xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the read was succesful</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the <paramref name="source"/> <see cref="XmlReader"/> is null
		/// </exception>
		public bool XukIn(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Source Xml Reader is null");
			}
			if (source.NodeType != System.Xml.XmlNodeType.Element) return false;
			mSequence.Clear();
			if (source.IsEmptyElement) return true;
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					IMedia newMedia = mMediaFactory.createMedia(source.LocalName, source.NamespaceURI);
					if (newMedia != null)
					{
						if (!newMedia.XukIn(source)) return false;
						try
						{
							insertItem(getCount(), newMedia);
						}
						catch (exception.MethodParameterIsWrongTypeException)
						{
							//The new media item is not compatible with previously inserted items
							return false;
						}
					}
					else if (!source.IsEmptyElement)
					{
						//If the QName of the IMedia item xuk element is not recognised read past it
						source.ReadSubtree().Close();
					}
				}
				else if (source.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (source.EOF) break;
			}
			return true;
		}


		/// <summary>
		/// Writes the <see cref="SequenceMedia"/> to an xuk element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the swrite was succesful</returns>
		public bool XukOut(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}
			//empty sequences are not allowed
			if (mSequence.Count == 0) return false;

			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			destination.WriteAttributeString("type", this.getTypeAsString());
			foreach (IMedia media in mSequence)
			{
				if (!media.XukOut(destination)) return false;
			}
			destination.WriteEndElement();
			return true;
		}

		
		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="SequenceMedia"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="SequenceMedia"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion
	}
}
