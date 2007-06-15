package org.daisy.urakawa.media;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * This is the factory that creates generic
 * {@link org.daisy.urakawa.media.Media} instances.
 * </p>
 * 
 * @depend - Create - org.daisy.urakawa.media.Media
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public interface MediaFactory extends WithPresentation {
	/**
	 * <p>
	 * Creates a new media.
	 * </p>
	 * <p>
	 * This factory method take a single argument to specify the exact type of built-in media object
	 * to create.
	 * </p>
	 * @param type cannot be null.
	 * @return can return null (in case the given argument does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	Media createMedia(MediaType type)throws MethodParameterIsNullException;

	/**
	 * <p>
	 * Creates a new media.
	 * </p>
	 * <p>
	 * This factory method takes arguments to specify the exact type of object
	 * to create, given by the unique QName (XML Qualified Name) used in the XUK
	 * serialization format. This method can be used to generate instances of
	 * subclasses of the base object type.
	 * </p>
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the QName specification does not match
	 *         any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 */
	Media createMedia(String xukLocalName, String xukNamespaceUri)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}
