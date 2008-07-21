package org.daisy.urakawa.events.project;

import org.daisy.urakawa.IProject;
import org.daisy.urakawa.events.DataModelChangedEvent;

/**
 *
 */
public class ProjectEvent extends DataModelChangedEvent
{
    /**
     * @param source
     */
    public ProjectEvent(IProject source)
    {
        super(source);
        mSourceProject = source;
    }

    private IProject mSourceProject;

    /**
     * @return project
     */
    public IProject getSourceProject()
    {
        return mSourceProject;
    }
}
