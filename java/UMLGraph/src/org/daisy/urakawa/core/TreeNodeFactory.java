package org.daisy.urakawa.core;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * <p>
 * This is the factory that creates {@link org.daisy.urakawa.core.TreeNode}
 * nodes for the document tree.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Create - org.daisy.urakawa.core.TreeNode
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public interface TreeNodeFactory extends WithPresentation {
	/**
	 * Creates a new node, which is not linked to the core data tree yet.
	 * 
	 * @return cannot return null.
	 */
	public TreeNode createNode();

	/**
	 * The namespace+name combination defines the key to a map that provides
	 * specific node implementation. This is used for allowing TreeNode to be
	 * deserialized in XUK format.
	 * 
	 * @param xukLocalName
	 * @param xukNamespaceURI
	 * @return can return null (in case the NS:name specification does not match
	 *         any supported node type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameter is forbidden:
	 *             <b>xukLocalName</b>
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public TreeNode createNode(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}