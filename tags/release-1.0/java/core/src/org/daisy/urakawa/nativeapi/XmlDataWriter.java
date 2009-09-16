package org.daisy.urakawa.nativeapi;

/**
 * <p>
 * This interface does not have to be implemented "as-is". It is basically a
 * place-holder for a XML pull-parser, such as "System.Xml.XmlReader" in C#, or
 * StAX Java implementations. For more information, see
 * http://www.xmlpull.org/impls.shtml and http://stax.codehaus.org/
 * </p>
 * <p>
 * Note: the methods in this interface are directly inspired from the the C#
 * "System.Xml.XmlReader" implementation, but any XML pull-parser API should
 * provide a similar, if not an identical interface.
 * </p>
 * 
 * @stereotype Language-Dependent
 */
public interface XmlDataWriter {
	/**
	 * @param localName
	 * @param namespace
	 */
	public void writeStartElement(String localName, String namespace);

	/**
	 * 
	 */
	public void close();

	/**
	 * 
	 */
	public void writeStartDocument();

	/**
	 * 
	 */
	public void writeEndDocument();

	/**
	 * 
	 */
	public void writeEndElement();

	/**
	 * @param str1
	 *            a string
	 * @param str2
	 *            a string
	 */
	public void writeAttributeString(String str1, String str2);

	/**
	 * @param str
	 *            text
	 */
	public void writeString(String str);
}