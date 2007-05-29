package org.daisy.urakawa.media;

import org.daisy.urakawa.exceptions.IsAlreadyInitializedException;
import org.daisy.urakawa.exceptions.IsNotInitializedException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;

/**
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class MediaFactoryImpl implements MediaFactory {

	public Media createMedia(MediaType type) throws IsNotInitializedException {
		// TODO Auto-generated method stub
		return null;
	}

	public Media createMedia(String xukLocalName, String xukNamespaceUri)
			throws IsNotInitializedException {
		// TODO Auto-generated method stub
		return null;
	}

	public MediaPresentation getPresentation() throws IsNotInitializedException {
		// TODO Auto-generated method stub
		return null;
	}

	public void setPresentation(MediaPresentation presentation)
			throws IsAlreadyInitializedException,
			MethodParameterIsNullException {
		// TODO Auto-generated method stub
		
	}
	
}
