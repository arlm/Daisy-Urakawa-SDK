package org.daisy.urakawa.media;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;

/**
 * Reference implementation of the interface.
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class VideoMediaImpl implements VideoMedia {
	public Media copy() {
		
		return null;
	}

	public MediaFactory getMediaFactory() {
		
		return null;
	}

	public MediaType getMediaType() {
		
		return null;
	}

	public boolean isContinuous() {
		
		return false;
	}

	public boolean isDiscrete() {
		
		return false;
	}

	public boolean isSequence() {
		
		return false;
	}

	public void setMediaFactory(MediaFactory factory)
			throws MethodParameterIsNullException {
		
	}

	public boolean XukIn(XmlDataReader source)
			throws MethodParameterIsNullException {
		
		return false;
	}

	public boolean XukOut(XmlDataWriter destination)
			throws MethodParameterIsNullException {
		
		return false;
	}

	public String getXukLocalName() {
		
		return null;
	}

	public String getXukNamespaceURI() {
		
		return null;
	}

	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		
		return false;
	}

	public String getSrc() {
		
		return null;
	}

	public void setSrc(String newSrc) {
		
	}

	public Time getClipBegin() {
		
		return null;
	}

	public Time getClipEnd() {
		
		return null;
	}

	public void setClipBegin(Time newClipBegin)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		
	}

	public void setClipEnd(Time newClipEnd)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		
	}

	public TimeDelta getDuration() {
		
		return null;
	}

	public Continuous split(Time splitPoint) {
		
		return null;
	}

	public int getHeight() {
		
		return 0;
	}

	public int getWidth() {
		
		return 0;
	}

	public void setHeight(int h) throws MethodParameterIsOutOfBoundsException {
		
	}

	public void setWidth(int w) {
		
	}
}
