package org.daisy.urakawa.undo;

import org.daisy.urakawa.exception.CheckedException;


/**
 * <p>
 * This exception is raised when trying to un-execute a command and it fails.
 * </p>
 */
public class CommandCannotUnExecuteException extends CheckedException  {

	/**
	 * @hidden
	 */
	private static final long serialVersionUID = -2382291306023779725L;
}
