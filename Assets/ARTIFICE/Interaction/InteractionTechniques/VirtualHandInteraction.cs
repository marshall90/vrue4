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
/// Class to select and manipulate scene objects with Virtual hand interaction technique (IT). 
/// 
/// Virtual hand is a 1st person view IT. The VirtualHand GameObject, this class is attached to, is a direct child of the interaction
/// device gameObject. 
/// </summary>
public class VirtualHandInteraction : ObjectSelectionBase
{
    GameObject tracker = null;

    /// <summary>
    /// </summary>
    public void Start()
    {
        tracker = GameObject.Find("TrackerObject");
		Debug.Log("Start VirtualHand");
    }
    /// <summary>
    /// Implementation of concrete IT selection behaviour. 
    /// </summary>
	protected override void UpdateSelect()
	{
		if(tracker)
		{
	        // INTERACTION TECHNIQUE THINGS ------------------------------------------------
	        if (tracker.transform.parent.GetComponent<TrackBase>().isTracked())
	        {
	            // show virtual hand -> physical hand is autmatically rendert due to tracking state
				tracker.transform.parent.GetComponent<TrackBase>().setVisability(gameObject, true);
	
	            //Update transform of the selector object (virtual hand)
	            this.transform.position = tracker.transform.position;
	            this.transform.rotation = tracker.transform.rotation;
	    	
	            // Transform (translate and rotate) selected object depending on of virtual hand's transformation
	            if (selected)
	            {
	                this.transformInter(this.transform.position, this.transform.rotation);
	            }
	        }else 
	        {
	            // make virtual hand invisible -> physical hand is autmatically rendert due to tracking state
				tracker.transform.parent.GetComponent<TrackBase>().setVisability(gameObject, false);
	        }
		}
		else
		{
			Debug.Log("No GameObject with name - TrackerObject - found in scene");
		}
  	}

	/// <summary>
    /// Callback
    /// If our selector-Object collides with anotherObject we store the other object 
    /// 
    /// For usability purpose change color of collided object
    /// </summary>
    /// <param name="other">GameObject giben by the callback</param>
    public void OnTriggerEnter(Collider other)
    {
        if (isOwnerCallback())
        {
            GameObject collidee = other.gameObject;

            if (hasObjectController(collidee))
            {
                collidees.Add(collidee.GetInstanceID(), collidee);
                //Debug.Log(collidee.GetInstanceID());

                // change color so user knows of intersection
                collidee.renderer.material.SetColor("_Color", Color.blue);
            }
        }
    }

    /// <summary>
    /// Callback
    /// If our selector-Object moves out of anotherObject we remove the other object from our list
    /// 
    /// For usability purpose change color of collided object
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other)
    {
        if (isOwnerCallback())
        {
            GameObject collidee = other.gameObject;

            if (hasObjectController(collidee))
            {
                collidees.Remove(collidee.GetInstanceID());

                // change color so user knows of intersection end
                collidee.renderer.material.SetColor("_Color", Color.white);
            }
        }
    }
}