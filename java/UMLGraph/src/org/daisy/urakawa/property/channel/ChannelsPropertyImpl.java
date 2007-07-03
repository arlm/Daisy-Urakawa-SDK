package org.daisy.urakawa.property.channel;

import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.DoesNotAcceptMediaException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.property.Property;
import org.daisy.urakawa.property.PropertyImpl;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ChannelsPropertyImpl extends PropertyImpl implements
		ChannelsProperty {
	public Property export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		ChannelsProperty destProp;
		try {
			destProp = (ChannelsProperty) super.export(destPres);
		} catch (MethodParameterIsNullException e1) {
			e1.printStackTrace();
			return null;
		}
		if (destProp == null) {
			return null;
		}
		ChannelsManager destManager = destPres.getChannelsManager();
		List<Channel> channels = getListOfUsedChannels();
		for (Channel channel : channels) {
			Channel destChannel = destManager.getEquivalentChannel(channel);
			if (destChannel == null) {
				try {
					destChannel = channel.export(destPres);
				} catch (MethodParameterIsNullException e) {
					e.printStackTrace();
					return null;
				}
				if (destChannel == null) {
					return null;
				}
				// destManager.add(destChannel); // NO NEED TO DO THIS: because
				// the above export() uses the factory create method, and
				// therefore handles the association of the channel with its
				// manager.
			}
			Media media;
			try {
				media = getMedia(channel);
			} catch (MethodParameterIsNullException e) {
				e.printStackTrace();
				return null;
			} catch (ChannelDoesNotExistException e) {
				e.printStackTrace();
				return null;
			}
			Media destMedia;
			try {
				destMedia = media.export(destPres);
			} catch (MethodParameterIsNullException e1) {
				e1.printStackTrace();
				return null;
			}
			if (destMedia == null) {
				return null;
			}
			try {
				destProp.setMedia(destChannel, destMedia);
			} catch (MethodParameterIsNullException e) {
				e.printStackTrace();
				return null;
			} catch (ChannelDoesNotExistException e) {
				e.printStackTrace();
				return null;
			} catch (DoesNotAcceptMediaException e) {
				e.printStackTrace();
				return null;
			}
		}
		return destProp;
	}

	public List<Channel> getListOfUsedChannels() {
		return null;
	}

	public Media getMedia(Channel channel)
			throws MethodParameterIsNullException, ChannelDoesNotExistException {
		return null;
	}

	public void setMedia(Channel channel, Media media)
			throws MethodParameterIsNullException,
			ChannelDoesNotExistException, DoesNotAcceptMediaException {
	}

	public ChannelsProperty copyChannelsProperty() {
		return null;
	}
}
