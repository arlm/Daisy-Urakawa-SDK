using System;
using urakawa.core.property;
using urakawa.media;

namespace urakawa.properties.channel
{
	/// <summary>
  /// This property maintains a mapping from Channel object to Media object.
  /// Channels referenced here are pointers to existing channels in the presentation.
	/// </summary>
	public interface IChannelsProperty : IProperty
	{
    /// <summary>
    /// Retrieves the <see cref="IMedia"/> of a given <see cref="IChannel"/>
    /// </summary>
    /// <param name="channel">The given <see cref="IChannel"/></param>
    /// <returns>The <see cref="IMedia"/> associated with the given channel, 
    /// <c>null</c> if no <see cref="IMedia"/> is associated</returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="channel"/> is null
    /// </exception>
    /// <exception cref="exception.ChannelDoesNotExistException">
    /// Thrown when <paramref name="channel"/> is not managed by the associated <see cref="IChannelsManager"/>
    /// </exception>
    IMedia getMedia(IChannel channel);

    /// <summary>
    /// Associates a given <see cref="IMedia"/> with a given <see cref="IChannel"/>
    /// </summary>
    /// <param name="channel">The given <see cref="IChannel"/></param>
    /// <param name="media">The given <see cref="IMedia"/></param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when parameters <paramref name="channel"/> or <paramref name="media"/>
    /// </exception>
    /// <exception cref="exception.ChannelDoesNotExistException">
    /// Thrown when <paramref name="channel"/> is not managed by the associated <see cref="IChannelsManager"/>
    /// </exception>
    /// <exception cref="exception.MediaTypeIsIllegalException">
    /// Thrown when <paramref name="channel"/> does not support the <see cref="MediaType"/> 
    /// of <paramref name="media"/>
    /// </exception>
    void setMedia(IChannel channel, IMedia media);

    /// <summary>
    /// Gets the list of <see cref="IChannel"/>s used by this instance of <see cref="IChannelsProperty"/>
    /// </summary>
    /// <returns>The list of used <see cref="IChannel"/>s</returns>
    System.Collections.IList getListOfUsedChannels();
	}
}
