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
using System.Collections;

/// <summary>
/// TrackProvider is a abstract component, that provides the tracking data, directly 
/// received from OpenTracker, to Unity3D game objects.
/// 
/// Currently there are two implemented components to control Unity3D game 
/// objects by open tracker data:
/// - TrackMarker       -> attach a ARToolkit Marker to a game object
/// - TrackSpaceMouse   -> attach the SpaceMouse to a game object
/// 
/// </summary>
public abstract class TrackProvider : TrackBase {

    /// <summary>
    /// Name of the tracking device. Must be the same according to value of 
    /// corresponding unity sink name attribute in opentracker.xml.
    /// 
    /// Set this value in the derived class or directly in the script's inspector view in Unity3D
    /// </summary>
    public string deviceName = "<EDIT: UNITY SINK NAME>";
	
	/// <summary>
	/// Scale translation vector depending on scale factor. Needed to compensate the difference between the small tracking volume (in [cm] range) in front of the webcam 
	/// compared to the the bigger unity scene (in [m]) -> adapt movement of interaction device to scene movement. 
	/// </summary>
	public float scalePosition = 1.0f;
	
	/// <summary>
    /// Tracker Object of open tracker tracking device indicated by device name.
    /// Provides tracking event data. 
    /// </summary>
    protected Tracker tracker = null;
	
	/// <summary>
    /// Processes raw open tracker data from current tracking event object, extract the data and transform in to 
    /// Unity3D vector data and passes data to Unity3D game object's transform node. 
    /// </summary>
    public static void transformOpenTrackerToUnity3D(TrackingEvent ev,out Vector3 position,out Quaternion orientation)
    {
			// get current position of tracking event
            floatvector pos = ev.getPosition();
            floatvector orient = ev.getOrientation();
		
			//convert it to unity vectors and rotation matrix
			position = new Vector3(pos[0], pos[1], pos[2]);

            orientation = new Quaternion(orient[0], orient[1], orient[2], orient[3]);
	}
	
    /// <summary>
    /// Instantiate the Tracker object for given tracking device indicated by deviceName (-> UnitySink name attributes in opentracker.xml)
    /// </summary>
	protected void generateTracker(string deviceName) 
    {
        tracker = Manager.instance.tracking.getTracker(deviceName);
    }

    /// <summary>
    /// Create a new TrackingEvent and passes this event to tracker object to receive tracking data from device.
    /// Update private member variables.
    /// </summary>
    /// <returns>TrackingEvent</returns>
    protected TrackingEvent updateTracking()
    {
        // create new tracking event and get current tracker event
        TrackingEvent trackingEvent = new TrackingEvent();
        //update tracking timestamp 
		if(tracker!=null)
		{
        	if (tracker.getEvent(trackingEvent))
				_trackingTimestamp = trackingEvent.getTime();
		}

        return trackingEvent;
    }
}
