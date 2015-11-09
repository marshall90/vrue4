/* =====================================================================================
 * ARTiFICe - Augmented Reality Framework for Distributed Collaboration
 * ====================================================================================
 * Copyright (c) 2010-2012 
 * 
 * Annette Mossel, Christian Schönauer, Georg Gerstweiler, Hannes Kaufmann
 * mossel | schoenauer | gerstweiler | kaufmann @ims.tuwien.ac.at
 * Interactive Media Systems Group, Vienna University of Technology, Austria
 * www.ims.tuwien.ac.at
 * 
 * ====================================================================================
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * =====================================================================================
 */
 
using UnityEngine;
using System;
using System.Collections;

public class Manager : ScriptableObject {

    private static Manager manager;
    private Video m_video=null;
    private Tracking m_tracking=null;

    public static string videoConfigFile="openvideo.xml";
    public static string trackingConfigFile="opentracker.xml";
    public static bool useDedicatedServer = false;

    public string test
    {
        get
        {
            return videoConfigFile;
        }
    }

    static readonly object padlock = new object();

    public static Manager instance
    {
        get
        {
            lock (padlock)
            {
                return (manager ? manager : manager = CreateInstance<Manager>());
            }
        }
    }

    public Video video
    {
        get
        {
            return m_video;
        }
    }

    public Tracking tracking
    {
        get
        {
            return m_tracking;
        }
    }
    
    private Manager()
    {
    }

	void OnEnable () 
    {
		if (Debug.isDebugBuild)
        	UnityClient.showConsole();
		
		try
		{
		    m_video = new Video(videoConfigFile);
			
			if(m_video!=null)
			{
				if(!m_video.start())
				{
					m_video.Dispose();
					m_video=null;
				}
			}
	
	        m_tracking = new Tracking(trackingConfigFile);
	        m_tracking.start();
		}
		catch(Exception ex)
		{
			Debug.LogError("Error Manager.OnEnable - "+ex.Message);
		}
	}
	
 
    
	void OnDisable () 
    {
        Shutdown();
	}

    /// <summary>
    /// Shutdown Tracking
    /// //TODO check which of the two stops is hanging from time to time
    /// </summary>
    public void Shutdown()
    {
        try
        {
            if (m_video != null)
            {
				try
				{
	                video.stop();
	                m_video.Dispose();
	                m_video = null;
				}
				catch(Exception ex)
				{
					Debug.LogError("Error stopping video - have you forgotten openvideo.xml in your project?("+ex.Message+")");
				}
            }

            if (m_tracking != null)
            {
				try
				{
                	m_tracking.stop();
                	m_tracking.Dispose();
                	m_tracking = null;
				}
				catch(Exception ex)
				{
					Debug.LogError("Error stopping tracking.("+ex.Message+")");
				}
            }
        }
        catch(System.Exception ex)
        {
			Debug.LogError("Exception: "+ ex.ToString());
        }
		if (Debug.isDebugBuild)
	        UnityClient.hideConsole();
    }
}
