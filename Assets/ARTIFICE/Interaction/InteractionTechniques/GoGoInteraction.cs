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
/// Class to select and manipulate scene objects with gogo interaction technique (IT). 
/// 
/// GoGo is a 1st person view IT
/// </summary>
public class GoGoInteraction : ObjectSelectionBase
{
	/* ------------------ VRUE Tasks START -------------------
	* 	Implement GoGo interaction technique
	----------------------------------------------------------------- */
	
	private GameObject tracker = null;
	private GameObject interactionOrigin = null;
	private float r;
	private float factor;
	private float k;
	private float D;
	
	public void Start()
	{
		tracker = GameObject.Find("TrackerObject");
		//die virtual Camera simuliert meinen Torso
		interactionOrigin = GameObject.Find ("InteractionOrigin");
		
		//k: koeffizient zwischen 0 und 1
		k = 1f / 6f;
		
		//D: ca 2/3 der maximalen Armlänge
		//Ich nehme an die Armlänge ist ca 50cm, scalieren um den Faktor 20 nicht vergessen!
		D = (0.5f * 20f) * (2f/3f);
		
		
	}
	
	void OnEnable()
	{
		Debug.Log("Start GoGo Interaction");
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
				// Die Virtual-Hand wird bei der GoGo Methode auf 2 verschiedene arten gerendert, je nach position der echten Hand
				
				//länge vom echten Arm zum Torso (VirtualCamera)
				r = Vector3.Distance(interactionOrigin.transform.position, tracker.transform.position);
				Vector3 direction = (tracker.transform.position - interactionOrigin.transform.position).normalized;
				
				if(r < D)
				{
					this.transform.position = interactionOrigin.transform.position + direction * r;
				}
				else
				{
					factor = (r + k * Mathf.Pow(r-D, 2f));
					this.transform.position = interactionOrigin.transform.position + direction * factor;
				}
				
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
	
	
	
	
	// ------------------ VRUE Tasks END ----------------------------
	
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