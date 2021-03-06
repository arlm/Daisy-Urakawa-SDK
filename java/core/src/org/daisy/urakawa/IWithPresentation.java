package org.daisy.urakawa;

import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * <p>
 * Getting and Setting the presentation. Object classes that realize this
 * interface implement an object type that is "owned" by a Presentation
 * instance. In other words, the Presentation reference accessed by this
 * getter/setter corresponds to a UML aggregation relationship. This is merely a
 * "track-back" feature to allow sub-level objects to be aware of the
 * Presentation instance they belong to.
 * </p>
 */
public interface IWithPresentation extends IXukAble
{
    /**
     * @return the presentation object. Cannot be null.
     * @throws IsNotInitializedException
     *         when {@link IWithPresentation#setPresentation(Presentation)} has
     *         not been used yet to initialize the data.
     */
    public Presentation getPresentation() throws IsNotInitializedException;

    /**
     * @param iPresentation
     *        cannot be null
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws IsAlreadyInitializedException
     *         when the data has already been initialized using this method.
     * @stereotype Initialize
     */
    public void setPresentation(Presentation iPresentation)
            throws MethodParameterIsNullException,
            IsAlreadyInitializedException;
}
