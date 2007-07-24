using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using urakawa.media;
using urakawa.media.timing;
using urakawa.media.data.audio;

namespace urakawa.media.data.audio
{
	/// <summary>
	/// Managed implementation of <see cref="IAudioMedia"/>, that uses <see cref="AudioMediaData"/> to store audio data
	/// </summary>
	public class ManagedAudioMedia : IAudioMedia, IManagedMedia
	{
		internal ManagedAudioMedia(IMediaFactory fact, AudioMediaData amd)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("The MediaFactory of a AudioMedia can not be null");
			}
			mFactory = fact;
			setMediaData(amd);
		}

		private IMediaFactory mFactory;
		private AudioMediaData mAudioMediaData;

		#region IMedia Members
		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The media factory</returns>
		public IMediaFactory getMediaFactory()
		{
			return mFactory;
		}

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if <c>this</c> is a continuous <see cref="IMedia"/>
		/// </summary>
		/// <returns><c>true</c></returns>
		public bool isContinuous()
		{
			return true;
		}

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if <c>this</c> is a discrete <see cref="IMedia"/>
		/// </summary>
		/// <returns><c>false</c></returns>
		public bool isDiscrete()
		{
			return false;
		}

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if <c>this</c> is a sequence <see cref="IMedia"/>
		/// </summary>
		/// <returns><c>false</c></returns>
		public bool isSequence()
		{
			return false;
		}

		IMedia IMedia.copy()
		{
			return copy();
		}

		/// <summary>
		/// Gets a copy of <c>this</c>. 
		/// The copy is deep in the sense that the underlying <see cref="AudioMediaData"/> is also copied
		/// </summary>
		/// <returns>The copy</returns>
		public ManagedAudioMedia copy()
		{
			IMedia oCopy = getMediaFactory().createMedia(getXukLocalName(), getXukNamespaceUri());
			if (!(oCopy is ManagedAudioMedia))
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The MediaFactory can not a ManagedAudioMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			ManagedAudioMedia copyMAM = (ManagedAudioMedia)oCopy;
			copyMAM.setMediaData(getMediaData().copy());
			return copyMAM;
		}

		/// <summary>
		/// Gets a 'copy' of <c>this</c>, including only the audio after the given clip begin time
		/// </summary>
		/// <param name="clipBegin">The given clip begin time</param>
		/// <returns>The copy</returns>
		public ManagedAudioMedia copy(Time clipBegin)
		{
			return copy(clipBegin, Time.Zero.addTimeDelta(getDuration()));
		}


		/// <summary>
		/// Gets a 'copy' of <c>this</c>, including only the audio between the given clip begin and end times
		/// </summary>
		/// <param name="clipBegin">The given clip begin time</param>
		/// <param name="clipEnd">The given clip end time</param>
		/// <returns>The copy</returns>
		public ManagedAudioMedia copy(Time clipBegin, Time clipEnd)
		{
			IMedia oCopy = getMediaFactory().createMedia(getXukLocalName(), getXukNamespaceUri());
			if (!(oCopy is ManagedAudioMedia))
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The MediaFactory can not a ManagedAudioMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			ManagedAudioMedia copyMAM = (ManagedAudioMedia)oCopy;
			MediaData oDataCopy = getMediaDataFactory().createMediaData(
				getMediaData().getXukLocalName(), getMediaData().getXukNamespaceUri());
			if (!(oDataCopy is AudioMediaData))
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The MediaDataFactory can not an AudioMediaData matching QName {1}:{0}",
					getMediaData().getXukLocalName(), getMediaData().getXukNamespaceUri()));
			}
			AudioMediaData dataCopy = (AudioMediaData)oDataCopy;
			Stream audioDataStream = getMediaData().getAudioData(clipBegin, clipEnd);
			try
			{
				dataCopy.appendAudioData(audioDataStream, null);
			}
			finally
			{
				audioDataStream.Close();
			}
			copyMAM.setMediaData(dataCopy);
			return copyMAM;
		}


		#endregion
		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="ManagedAudioMedia"/> from a ManagedAudioMedia xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read ManagedAudioMedia from a non-element node");
			}
			try
			{
				XukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							XukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of ManagedAudioMedia: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a ManagedAudioMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
			string uid = source.GetAttribute("AudioMediaDataUid");
			if (uid == null || uid == "")
			{
				throw new exception.XukException("AudioMediaDataUid attribute is missing from AudioMediaData");
			}
			if (!getMediaDataFactory().getMediaDataManager().isManagerOf(uid))
			{
				throw new exception.XukException(String.Format(
					"The MediaDataManager does not mamage a AudioMediaData with uid {0}", uid));
			}
			MediaData md = getMediaDataFactory().getMediaDataManager().getMediaData(uid);
			if (!(md is AudioMediaData))
			{
				throw new exception.XukException(String.Format(
					"The MediaData with uid {0} is a {1} which is not a urakawa.media.data.audio.AudioMediaData",
					uid, md.GetType().FullName));
			}
			setMediaData(md);
		}

		/// <summary>
		/// Reads a child of a ManagedAudioMedia xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		private void XukInAudioMediaData(XmlReader source)
		{
			bool readData = false;
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						MediaData newMediaData = getMediaDataFactory().createMediaData(
							source.LocalName, source.NamespaceURI);
						if (newMediaData is AudioMediaData)
						{
							newMediaData.XukIn(source);
							setMediaData(newMediaData);
						}
						else
						{
							if (!source.IsEmptyElement) source.ReadSubtree().Close();
						}
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
			if (!readData)
			{
				throw new exception.XukException("mAudioMediaData element contained no valid AudioMediaData");
			}
		}

		/// <summary>
		/// Write a ManagedAudioMedia element to a XUK file representing the <see cref="ManagedAudioMedia"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		public void XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				XukOutAttributes(destination);
				XukOutChildren(destination);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of ManagedAudioMedia: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a ManagedAudioMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{
			destination.WriteAttributeString("AudioMediaDataUid", getMediaData().getUid());
		}

		/// <summary>
		/// Write the child elements of a ManagedAudioMedia element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{

		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="ManagedAudioMedia"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="ManagedAudioMedia"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<IMedia> Members

		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>		
		public bool ValueEquals(IMedia other)
		{
			if (!(other is ManagedAudioMedia)) return false;
			ManagedAudioMedia otherMAM = (ManagedAudioMedia)other;
			if (!getMediaData().ValueEquals(otherMAM.getMediaData())) return false;
			return true;
		}

		#endregion

		#region IContinuous Members

		/// <summary>
		/// Gets the duration of <c>this</c>, that is the duration of the underlying <see cref="AudioMediaData"/>
		/// </summary>
		/// <returns>The duration</returns>
		public TimeDelta getDuration()
		{
			return getMediaData().getAudioDuration();
		}

		IContinuous IContinuous.split(urakawa.media.timing.Time splitPoint)
		{
			return split(splitPoint);
		}

		/// <summary>
		/// Splits the managed audio media at a given split point in time,
		/// <c>this</c> retaining the audio before the split point,
		/// creating a new <see cref="ManagedAudioMedia"/> containing the audio after the split point
		/// </summary>
		/// <param name="splitPoint">The given split point</param>
		/// <returns>A managed audio media containing the audio after the split point</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the given split point is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when the given split point is negative or is beyond the duration of <c>this</c>
		/// </exception>
		public ManagedAudioMedia split(urakawa.media.timing.Time splitPoint)
		{
			if (splitPoint == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The split point can not be null");
			}
			if (splitPoint.isNegativeTimeOffset())
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The split point can not be negative");
			}
			if (splitPoint.isGreaterThan(Time.Zero.addTimeDelta(getDuration())))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The split point can not be beyond the end of the underlying AudioMediaData");
			}
			IMedia oSecondPart = getMediaFactory().createMedia(getXukLocalName(), getXukNamespaceUri());
			if (!(oSecondPart is ManagedAudioMedia))
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The MediaFactory can not create a ManagedAudioMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			ManagedAudioMedia secondPartMAM = (ManagedAudioMedia)oSecondPart;
			TimeDelta spDur = Time.Zero.addTimeDelta(getDuration()).getTimeDelta(splitPoint);
			Stream secondPartAudioStream = getMediaData().getAudioData(splitPoint);
			try
			{
				secondPartMAM.getMediaData().appendAudioData(secondPartAudioStream, spDur);
			}
			finally
			{
				secondPartAudioStream.Close();
			}
			getMediaData().removeAudio(splitPoint);
			return secondPartMAM;
		}

		#endregion

		MediaData IManagedMedia.getMediaData()
		{
			return getMediaData();
		}

		/// <summary>
		/// Gets the <see cref="AudioMediaData"/> storing the audio of <c>this</c>
		/// </summary>
		/// <returns>The audio media data</returns>
		public AudioMediaData getMediaData()
		{
			return mAudioMediaData;
		}

		/// <summary>
		/// Sets the <see cref="MediaData"/> of the managed audio media
		/// </summary>
		/// <param name="data">The new media data, must be a <see cref="AudioMediaData"/></param>
		/// <exception cref="exception.MethodParameterIsWrongTypeException">
		/// </exception>
		public void setMediaData(MediaData data)
		{
			if (!(data is AudioMediaData))
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"The MediaData of a ManagedAudioMedia must be a AudioMediaData");
			}
			mAudioMediaData = (AudioMediaData)data;
		}

		/// <summary>
		/// Gets the <see cref="MediaDataFactory"/> creating the <see cref="MediaData"/>
		/// used by <c>this</c>.
		/// Convenience for <c>getMediaData().getMediaDataManager().getMediaDataFactory()</c>
		/// </summary>
		/// <returns>The media data factory</returns>
		public MediaDataFactory getMediaDataFactory()
		{
			return getMediaData().getMediaDataManager().getMediaDataFactory();
		}

		/// <summary>
		/// Merges <c>this</c> with a given other <see cref="ManagedAudioMedia"/>,
		/// appending the audio data of the other <see cref="ManagedAudioMedia"/> to <c>this</c>,
		/// leaving the other <see cref="ManagedAudioMedia"/> without audio data
		/// </summary>
		/// <param name="other">The given other managed audio media</param>
		public void mergeWith(ManagedAudioMedia other)
		{
			if (!getMediaData().getPCMFormat().isCompatibleWith(other.getMediaData().getPCMFormat()))
			{
				throw new exception.InvalidDataFormatException(
					"Can not merge this with a ManagedAudioMedia with incompatible audio data");
			}
			System.IO.Stream otherData = other.getMediaData().getAudioData();
			try
			{
				getMediaData().appendAudioData(otherData, other.getMediaData().getAudioDuration());
			}
			finally
			{
				otherData.Close();
			}
			other.getMediaData().removeAudio(Time.Zero);
		}

	}
}
