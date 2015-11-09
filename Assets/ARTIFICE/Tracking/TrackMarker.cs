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

/// <summary>
/// Provides tracking data received from ARToolkit marker to Unity3D game objects. 
/// Inherits from abstract class TrackProvider. 
/// </summary>
public class TrackMarker : TrackProvider 
{

    /// <summary>
    /// Time which must pass without receiving new tracking data for this device until attached game
    /// object will be invisible.
    /// [ms]
    /// 
    /// Set this value in the derived class or directly in the script's property view in Unity3D
    /// </summary>
    public double deviceTimeout = 200.0;

    /// <summary>
    /// Flag if tracking device is currently tracked
    /// </summary>
    //private bool _deviceTracked = false;

    /// <summary>
    /// Use Start() for initialization
    /// </summary>
	public virtual void Start() 
    {
		// disable rendering
		base.setVisability(gameObject, false);
		
        gameObject.transform.hideFlags |= HideFlags.NotEditable;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    public virtual void Update() 
    {
		if(tracker == null)
		{
			if(!Manager.useDedicatedServer || (Manager.useDedicatedServer&&Network.isClient)) 
			{
		        // generate tracker object for space mouse
		        generateTracker(deviceName);
		        gameObject.transform.hideFlags |= HideFlags.NotEditable;
			}
		}
		else
		{
			try 
	        {
				
	            // get current tracking event
	            TrackingEvent ev = updateTracking();
	
				Vector3 position=new Vector3(0.0f,0.0f,0.0f);
				Quaternion orientation=new Quaternion(0.0f,0.0f,0.0f,1.0f);
				transformOpenTrackerToUnity3D(ev,out position,out orientation);
				transform.localPosition = (position * base.scalePosition);
				transform.localRotation = orientation;
				
	        } catch (System.Exception ex) 
	        {
				Debug.Log("Exception:"+ ex.ToString());
	        }
		}
		
		// control rendering
		base.setVisability(gameObject, isTracked());
    }

    /// <summary>
    /// Check if new marker tracking data is received until device timout is reached.
    /// Tracking event data is only upated as long as marker is visible in video image.
    /// </summary>
	public override bool isTracked() 
    {
		return (Tracking.getTime() - trackingTimestamp) < deviceTimeout;
	}
}