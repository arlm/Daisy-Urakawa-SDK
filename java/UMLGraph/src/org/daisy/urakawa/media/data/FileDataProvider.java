package org.daisy.urakawa.media.data;

/**
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public interface FileDataProvider extends DataProvider {
	public String getDataFileRelativePath();

	public String getDataFileFullPath();

	public FileDataProviderManager getFileDataProviderManager();
}
