using System;

namespace urakawa.media
{
	/// <summary>
	/// ImageMedia is the image object. 
	/// It has width, height, and an external source.
	/// </summary>
	public class ImageMedia : IImageMedia
	{
		int mWidth;
		int mHeight;
		MediaLocation mMediaLocation = new MediaLocation();

		
		//internal constructor encourages use of MediaFactory to create ImageMedia objects
		internal ImageMedia()
		{
			mWidth = 0;
			mHeight = 0;
		}

		/// <summary>
		/// this override is useful while debugging
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "ImageMedia";
		}


		//are these functions necessary, or should they be renamed something like
		//"cropImage" or "resizeImage"?
		//image width and height could be calculated from the file itself, 
		//assuming all editing is destructive, otherwise some access to "virtual" height and width 
		//will be required
		public void setWidth(int width)
		{
			mWidth = width;
		}

		public void setHeight(int height)
		{
			mHeight = height;
		}

		#region IExternalMedia Members

		public IMediaLocation getLocation()
		{
			return mMediaLocation;
		}

		public void setLocation(IMediaLocation location)
		{
			if (location == null)
			{
				throw new exception.MethodParameterIsNullException("ImageMedia.setLocation(null) caused MethodParameterIsNullException");
			}
			mMediaLocation = (MediaLocation)location;
		}

		#endregion

		#region IMedia Members

		public bool isContinuous()
		{
			return false;
		}

		public bool isDiscrete()
		{
			return true;
		}

		public bool isSequence()
		{
			return false;
		}

		public urakawa.media.MediaType getType()
		{
			return MediaType.IMAGE;
		}

		IMedia IMedia.copy()
		{
			return copy();
		}

		public ImageMedia copy()
		{
			ImageMedia newMedia = new ImageMedia();
			newMedia.setHeight(this.getHeight());
			newMedia.setWidth(this.getWidth());
			newMedia.setLocation(this.getLocation().copy());
			return newMedia;
		}

		#endregion

		#region IImageSize Members

		public int getWidth()
		{
			return mWidth;
		}

		public int getHeight()
		{
			return mHeight;
		}

		#endregion

		#region IXUKable members 

		public bool XUKin(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}

			if (!(source.Name == "Media" && source.NodeType == System.Xml.XmlNodeType.Element &&
				source.GetAttribute("type") == "IMAGE"))
			{
				return false;
			}

			
			System.Diagnostics.Debug.WriteLine("XUKin: ImageMedia");


			string height = source.GetAttribute("height");
			string width = source.GetAttribute("width");
			string src = source.GetAttribute("src");

			this.setHeight(int.Parse(height));
			this.setWidth(int.Parse(width));
			this.mMediaLocation = new MediaLocation(src);

			//move the cursor to the closing tag
			if (source.IsEmptyElement == false)
			{
				while (!(source.Name == "Media" && 
					source.NodeType == System.Xml.XmlNodeType.EndElement)
					&&
					source.EOF == false)
				{
					source.Read();
				}
			}

			return true;
		}

		
		public bool XUKout(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}

			destination.WriteStartElement("Media");

			destination.WriteAttributeString("type", "IMAGE");

			destination.WriteAttributeString("src", this.mMediaLocation.mLocation);

			destination.WriteAttributeString("height", this.mHeight.ToString());

			destination.WriteAttributeString("width", this.mWidth.ToString());

			destination.WriteEndElement();

			return true;
		}
		#endregion
	}
}
