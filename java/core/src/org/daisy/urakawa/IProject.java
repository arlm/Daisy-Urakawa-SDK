package org.daisy.urakawa;

import java.net.URI;

import org.daisy.urakawa.events.DataModelChangedEvent;
import org.daisy.urakawa.events.IEventHandler;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * <p>
 * This is essentially a container for one or more
 * {@link org.daisy.urakawa.IPresentation}. It also provides methods for opening
 * and saving the XUK persistent XML format.
 * </p>
 * 
 */
public interface IProject extends IWithPresentations, IXukAble,
        IValueEquatable<IProject>, IEventHandler<DataModelChangedEvent>
{
    /**
     * <p>
     * Reads a XUK-formatted XML file, and generates the equivalent object data
     * that makes the IProject.
     * </p>
     * 
     * @param uri cannot be null.
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws XukDeserializationFailedException if the operation fails
     * 
     */
    public void openXUK(URI uri) throws MethodParameterIsNullException,
            XukDeserializationFailedException;

    /**
     * <p>
     * Writes the object data of the IProject into a XUK-formatted XML file.
     * </p>
     * 
     * @param uri cannot be null
     * @throws MethodParameterIsNullException NULL method parameters are
     *         forbidden
     * @throws XukSerializationFailedException if the operation fails
     * 
     */
    public void saveXUK(URI uri) throws MethodParameterIsNullException,
            XukSerializationFailedException;

    /**
     * This method calls {@link org.daisy.urakawa.IPresentation#cleanup()} for
     * each owned IPresentation.
     */
    public void cleanup();

    /**
     * @return
     */
    public PresentationFactory getPresentationFactory();
}
