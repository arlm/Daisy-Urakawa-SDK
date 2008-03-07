package org.daisy.urakawa.metadata;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * The data for the content of the Metadata
 * </p>
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface WithContent {
	/**
	 * @return cannot be null, but can be empty string
	 */
	public String getContent();

	/**
	 * Sets the data for the content of the metadata
	 * 
	 * @param data
	 *            cannot be null but can be empty string
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @tagvalue Events "Metadata"
	 */
	public void setContent(String data) throws MethodParameterIsNullException;
}
