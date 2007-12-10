package org.daisy.urakawa.property.channel;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ChannelsManagerImpl implements ChannelsManager {
	public Channel getEquivalentChannel(Channel sourceChannel) {
		for (Channel channel : getListOfChannels()) {
			if (channel.isEquivalentTo(sourceChannel)) {
				return channel;
			}
		}
		return null;
	}

	public void addChannel(Channel channel)
			throws MethodParameterIsNullException,
			ChannelAlreadyExistsException {
	}

	public void clearChannels() {
	}

	public void detachChannel(Channel channel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException {
	}

	public Channel getChannel(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public List<Channel> getListOfChannels() {
		return null;
	}

	public List<Channel> getListOfChannelsByName(String channelName)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public List<String> getListOfUids() {
		return null;
	}

	public String getUidOfChannel(Channel channel)
			throws MethodParameterIsNullException {
		return null;
	}

	public Presentation getPresentation() {
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public boolean ValueEquals(ChannelsManager other)
			throws MethodParameterIsNullException {
		return false;
	}

	public List<Channel> getListOfChannels(String channelName)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public void removeChannel(Channel channel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException {
	}

	public void removeChannel(String uid)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, MethodParameterIsEmptyStringException {
	}

	public boolean isManagerOf(String uid) {
		return false;
	}

	public void xukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void xukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}
}
