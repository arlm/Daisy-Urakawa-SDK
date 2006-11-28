using System;
using System.Xml;

namespace urakawa.media
{
	/// <summary>
	/// AudioMedia is the audio object.
	/// It is time-based and comes from an external source.
	/// </summary>
	public class AudioMedia : IAudioMedia
	{
		private IMediaLocation mLocation;
		private ITime mClipBegin = new Time();
		private ITime mClipEnd = new Time();
		private IMediaFactory mFactory;

		private void resetClipTimes()
		{
			mClipBegin = new Time();
			mClipEnd = new Time();
		}

		/// <summary>
		/// Constructor setting the associated <see cref="IMediaFactory"/>
		/// </summary>
		protected internal AudioMedia(IMediaFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("Factory is null");
			}
			mFactory = fact;
			mLocation = fact.createMediaLocation();
		}
		
		#region IMedia members
		/// <summary>
		/// This always returns true, because
		/// audio media is always considered continuous
		/// </summary>
		/// <returns></returns>
		public bool isContinuous()
		{
			return true;
		}

		/// <summary>
		/// This always returns false, because
		/// audio media is never considered discrete
		/// </summary>
		/// <returns></returns>
		public bool isDiscrete()
		{
			return false;
		}

		/// <summary>
		/// This always returns false, because
		/// a single media object is never considered to be a sequence
		/// </summary>
		/// <returns></returns>
		public bool isSequence()
		{
			return false;
		}

		/// <summary>
		/// Return the urakawa media type
		/// </summary>
		/// <returns>always returns <see cref="MediaType.AUDIO"/></returns>
		public MediaType getMediaType()
		{
			return MediaType.AUDIO;
		}

		
		IMedia IMedia.copy()
		{
			return copy();
		}

		/// <summary>
		/// Copy function which returns an <see cref="IAudioMedia"/> object
		/// </summary>
		/// <returns>A copy of this</returns>
		/// <exception cref="exception.FactoryCanNotCreateTypeException">
		/// Thrown when the <see cref="IMediaFactory"/> associated with this 
		/// can not create an <see cref="IAudioMedia"/> matching the QName of <see cref="AudioMedia"/>
		/// </exception>
		public IAudioMedia copy()
		{
			IMedia copyM = getMediaFactory().createMedia(getXukLocalName(), getXukNamespaceUri());
			if (copyM == null || !(copyM is IAudioMedia))
			{
				throw new exception.FactoryCanNotCreateTypeException(
					"The media factory could not create an IAudioMedia");
			}
			IAudioMedia copyAM = (IAudioMedia)copyM;
			if (getClipBegin().isNegativeTimeOffset())
			{
				copyAM.setClipBegin(getClipBegin().copy());
				copyAM.setClipEnd(getClipEnd().copy());
			}
			else
			{
				copyAM.setClipEnd(getClipEnd().copy());
				copyAM.setClipBegin(getClipBegin().copy());
			}
			copyAM.setLocation(getLocation().copy());
			return copyAM;
		}

		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IMediaFactory"/></returns>
		public IMediaFactory getMediaFactory()
		{
			return mFactory;
		}

		#endregion

		#region IXUKAble members 

		/// <summary>
		/// Fill in audio data from an XML source.
		/// Assume that the XmlReader cursor is at the opening audio tag.
		/// </summary>
		/// <param localName="source">the input XML source</param>
		/// <returns>true or false, depending on whether the data could be processed</returns>
		public bool XukIn(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}
			if (source.NodeType != System.Xml.XmlNodeType.Element) return false;

			string cb = source.GetAttribute("clipBegin");
			string ce = source.GetAttribute("clipEnd");

			resetClipTimes();

			try
			{
				Time ceTime = new Time(ce);
				Time cbTime = new Time(cb);
				if (cbTime.isNegativeTimeOffset())
				{
					setClipBegin(cbTime);
					setClipEnd(ceTime);
				}
				else
				{
					setClipEnd(ceTime);
					setClipBegin(cbTime);
				}
			}
			catch (exception.TimeStringRepresentationIsInvalidException)
			{
				return false;
			}
			catch (exception.MethodParameterIsOutOfBoundsException)
			{
				return false;
			}
			IMediaLocation loc = null;

			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						loc = getMediaFactory().createMediaLocation(source.LocalName, source.NamespaceURI);
						if (loc == null)
						{
							if (!source.IsEmptyElement)
							{
								//Read past unrecognized element
								source.ReadSubtree().Close();
							}
						}
						else
						{
							if (!loc.XukIn(source)) return false;
						}
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) break;
				}
			}
			if (loc == null) return false;
			setLocation(loc);
			return true;
		}

		/// <summary>
		/// The opposite of <see cref="XukIn"/>, this function writes the object's data
		/// to an XML file
		/// </summary>
		/// <param localName="destination">the XML source for outputting data</param>
		/// <returns>so far, this function always returns true</returns>
		public bool XukOut(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}
			destination.WriteStartElement("AudioMedia", urakawa.ToolkitSettings.XUK_NS);
			destination.WriteAttributeString("clipBegin", this.getClipBegin().ToString());
			destination.WriteAttributeString("clipEnd", this.getClipEnd().ToString());
			if (!getLocation().XukOut(destination)) return false;
			destination.WriteEndElement();
			return true;
		}


		
		/// <summary>
		/// Gets the local localName part of the QName representing a <see cref="AudioMedia"/> in Xuk
		/// </summary>
		/// <returns>The local localName part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="AudioMedia"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}
		#endregion

		#region ILocated Members

		/// <summary>
		/// Gets the <see cref="IMediaLocation"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IMediaLocation"/></returns>
		public IMediaLocation getLocation()
		{
			return mLocation;
		}

		/// <summary>
		/// Sets the <see cref="IMediaLocation"/> of <c>this</c>
		/// </summary>
		/// <param localName="location">The new <see cref="IMediaLocation"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="location"/> is <c>null</c>
		/// </exception>
		public void setLocation(IMediaLocation location)
		{
			if (location == null)
			{
				throw new exception.MethodParameterIsNullException("The media location can not be null");
			}
			mLocation = location;
		}

		#endregion

		#region IClipped Members

		/// <summary>
		/// Gets the duration of <c>this</c>
		/// </summary>
		/// <returns>A <see cref="ITimeDelta"/> representing the duration</returns>
		public ITimeDelta getDuration()
		{
			return getClipEnd().getTimeDelta(getClipBegin());
		}

		/// <summary>
		/// Gets the clip begin <see cref="ITime"/> of <c>this</c>
		/// </summary>
		/// <returns>Clip begin</returns>
		public ITime getClipBegin()
		{
			return mClipBegin;
		}

		/// <summary>
		/// Gets the clip end <see cref="ITime"/> of <c>this</c>
		/// </summary>
		/// <returns>Clip end</returns>
		public ITime getClipEnd()
		{
			return mClipEnd;
		}

		/// <summary>
		/// Sets the clip begin <see cref="ITime"/>
		/// </summary>
		/// <param localName="beginPoint">The new clip begin <see cref="ITime"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="beginPoint"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="beginPoint"/> is beyond clip end of <c>this</c>
		/// </exception>
		public void setClipBegin(ITime beginPoint)
		{
			if (beginPoint==null)
			{
				throw new exception.MethodParameterIsNullException("ClipBegin can not be null");
			}
			if (beginPoint.isGreaterThan(getClipEnd()))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"ClipBegin can not be after ClipEnd"); 
			}
			mClipBegin = beginPoint;
		}

		/// <summary>
		/// Sets the clip end <see cref="ITime"/>
		/// </summary>
		/// <param localName="endPoint">The new clip end <see cref="ITime"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="endPoint"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref localName="endPoint"/> is before clip begin of <c>this</c>
		/// </exception>
		public void setClipEnd(ITime endPoint)
		{
			if (endPoint == null)
			{
				throw new exception.MethodParameterIsNullException("ClipEnd can not be null");
			}
			if (endPoint.isLessThan(getClipBegin()))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"ClipEnd can not be before ClipBegin");
			}
			mClipEnd = endPoint;
		}

		IMedia IClipped.split(ITime splitPoint)
		{
			return split(splitPoint);
		}

		/// <summary>
		/// Splits <c>this</c> at a given <see cref="ITime"/>
		/// </summary>
		/// <param localName="splitPoint">The <see cref="ITime"/> at which to split - 
		/// must be between clip begin and clip end <see cref="ITime"/>s</param>
		/// <returns>
		/// A newly created <see cref="IAudioMedia"/> containing the audio after,
		/// <c>this</c> retains the audio before <paramref localName="splitPoint"/>.
		/// </returns>
		public IAudioMedia split(ITime splitPoint)
		{
			if (splitPoint==null)
			{
				throw new exception.MethodParameterIsNullException(
					"The time at which to split can not be null");
			}
			if (splitPoint.isLessThan(getClipBegin()))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The split time can not be before ClipBegin");
			}
			if (splitPoint.isGreaterThan(getClipEnd()))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The split time can not be after ClipEnd");
			}
			IAudioMedia splitAM = copy();
			setClipEnd(splitPoint);
			splitAM.setClipBegin(splitPoint);
			return splitAM;

		}

		#endregion

		#region IValueEquatable<IMedia> Members

		/// <summary>
		/// Conpares <c>this</c> with a given other <see cref="IMedia"/> for equality
		/// </summary>
		/// <param name="other">The other <see cref="IMedia"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public bool ValueEquals(IMedia other)
		{
			if (!(other is IAudioMedia)) return false;
			IAudioMedia otherAudio = (IAudioMedia)other;
			if (!getLocation().Equals(otherAudio.getLocation())) return false;
			if (!getClipBegin().isEqualTo(otherAudio.getClipBegin())) return false;
			if (!getClipEnd().isEqualTo(otherAudio.getClipEnd())) return false;
			return true;
		}

		#endregion
	}
}
