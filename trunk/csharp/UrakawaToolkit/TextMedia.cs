using System;

namespace urakawa.media
{
	/// <summary>
	/// TextMedia represents a text string
	/// </summary>
	public class TextMedia : ITextMedia
	{
		private string mTextString;

		
		//internal constructor encourages use of MediaFactory to create TextMedia objects
		internal TextMedia()
		{
		}

		/// <summary>
		/// this override is useful while debugging
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "TextMedia";
		}
		#region ITextMedia Members

		public string getText()
		{
			return mTextString;
		}

		public void setText(string text)
		{
			if (text == null)
			{
				throw new exception.MethodParameterIsNullException("TextMedia.setText(null) caused MethodParameterIsNullException");
			}

			if (text.Length == 0)
			{
				throw new exception.MethodParameterIsEmptyStringException("TextMedia.setText(" + 
					text + ") caused MethodParameterIsEmptyStringException");

				//causing a return here might be too oppositional, what if you are using an empty string?
				//(assuming it even matters in c#)
			}
			
			mTextString = text;
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
			return MediaType.TEXT;
		}

		#endregion

		#region IXUKable members 

		public bool XUKin(System.Xml.XmlReader source)
		{
			//TODO: actual implementation, for now we return false as default, signifying that all was not done
			return false;
		}

		public bool XUKout(System.Xml.XmlWriter destination)
		{
			//TODO: actual implementation, for now we return false as default, signifying that all was not done
			return false;
		}
		#endregion


	}
}
