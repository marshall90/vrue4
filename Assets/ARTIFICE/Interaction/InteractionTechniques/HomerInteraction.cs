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
/// Class to select and manipulate scene objects with HOMER interaction technique (IT). 
/// 
/// HOMER is a 1st person view IT
/// </summary>
public class HomerInteraction : ObjectSelectionBase
{
	/* ------------------ VRUE Tasks START -------------------
	* 	Implement Homer interaction technique
	----------------------------------------------------------------- */
	
	private LineRenderer lineRenderer;
	private GameObject tracker = null;
	private GameObject interactionOrigin = null;
	
	private bool multipleSelection;

	public Vector3 virtualHandPivot = Vector3.zero;
	
	private float distanceHand = 0f;
	private float distanceObject = 0f;
	
	public bool selected2;
	
	public void Start()
	{
		tracker = GameObject.Find("TrackerObject");
		
		//die virtual Camera simuliert meinen Torso
		interactionOrigin = GameObject.Find ("InteractionOrigin");
		
		multipleSelection = true;
	}
	
	/// <summary>
	/// Implementation of concrete IT selection behaviour. 
	/// </summary>
	protected override void UpdateSelect()
	{
		if(Input.GetButtonDown("SelectionMode"))
		{
			multipleSelection = !multipleSelection;
		}
		
		if(tracker)
		{
			// INTERACTION TECHNIQUE THINGS ------------------------------------------------
			if (tracker.transform.parent.GetComponent<TrackBase>().isTracked())
			{
				selected2 = selected;
				
				// show virtual hand -> physical hand is autmatically rendert due to tracking state
				tracker.transform.parent.GetComponent<TrackBase>().setVisability(gameObject, true);
				
				//Update transform of the selector object (virtual hand)
				this.transform.position = tracker.transform.position;
				this.transform.rotation = tracker.transform.rotation;
				
				//cast a ray and select objects depending on the selection Mode
				Vector3 direction = (tracker.transform.position - (interactionOrigin.transform.position)).normalized;
				
				//set lineRenderer start and end point
				lineRenderer.SetPosition(0, this.transform.position);
				lineRenderer.SetPosition(1, this.transform.position + direction * 1000);
				
				//only select objects, if we have the permission to do so
				if (isOwnerCallback() && !selected)
				{
					//make a raycast with the same position as the line Renderer
					if(multipleSelection)
					{
						//select all objects intersected with the raycast
						RaycastHit[] hits;
						hits = Physics.RaycastAll(transform.position, direction, 1000f);
						
						Hashtable collideesCopy = new Hashtable(collidees);
						
						foreach (DictionaryEntry pair in collideesCopy)
						{
							GameObject col = pair.Value as GameObject;
							bool remove = true;
							for (int i = 0; i < hits.Length; i++)
							{
								GameObject collidee = hits[i].collider.transform.gameObject;
								if(collidee.GetInstanceID() == col.GetInstanceID())
								{
									//dont remove the collidee if it exists in the collidees list and the raycast hit list
									remove = false;
									break;
								}
							}
							if(remove)
							{
								collidees.Remove(col.GetInstanceID());
								//change the color back to normal
								col.renderer.material.SetColor("_Color", Color.white);
							}
						}
						
						virtualHandPivot = Vector3.zero;
						
						for (int i = 0; i < hits.Length; i++) 
						{
							
							GameObject collidee = hits[i].collider.transform.gameObject;
							
							if (hasObjectController(collidee))
							{
								if(!collidees.Contains(collidee.GetInstanceID()))
								{
									collidees.Add(collidee.GetInstanceID(), collidee);
									
									// change color so user knows of intersection
									collidee.renderer.material.SetColor("_Color", Color.blue);
								}
								
								//add this object to the pivot point candidates
								virtualHandPivot += collidee.transform.position;
							}
						}
						
						virtualHandPivot = virtualHandPivot / (float) hits.Length;
						
						distanceHand = Vector3.Distance (tracker.transform.position, interactionOrigin.transform.position);
						distanceObject = Vector3.Distance (virtualHandPivot, interactionOrigin.transform.position);
					}
					else
					{
						//select only the first object intersected with the raycast
						RaycastHit[] hits;
						hits = Physics.RaycastAll(transform.position, direction, 1000f);
						
						//find the hit object with the shortest hit.distance and has ObjectController
						//the gameobject with the shortest distance to the virtual hand
						GameObject hitGm = null;
						float shortestDistance = 99999f;
						
						for (int i = 0; i < hits.Length; i++)
						{
							RaycastHit curHit = hits[i];
							GameObject curGm = curHit.transform.gameObject;
							if(hasObjectController(curGm))
							{
								if(curHit.distance < shortestDistance)
								{
									shortestDistance = curHit.distance;
									hitGm = curGm;
								}
							}
						}
						
						if (hitGm == null)
						{
							return;
						}
						
						Hashtable collideesCopy = new Hashtable(collidees);
						
						foreach (DictionaryEntry pair in collideesCopy)
						{
							GameObject col = pair.Value as GameObject;
							
							if(hitGm.GetInstanceID() != col.GetInstanceID())
							{
								collidees.Remove(col.GetInstanceID());
								//change the color back to normal
								col.renderer.material.SetColor("_Color", Color.white);
							}
						}
						
						if(!collidees.Contains(hitGm.GetInstanceID()))
						{
							collidees.Add(hitGm.GetInstanceID(), hitGm);
							
							// change color so user knows of intersection
							hitGm.renderer.material.SetColor("_Color", Color.blue);
						}
						
						virtualHandPivot = hitGm.transform.position;
						
						distanceHand = Vector3.Distance (tracker.transform.position, interactionOrigin.transform.position);
						distanceObject = Vector3.Distance (virtualHandPivot, interactionOrigin.transform.position);
					}
					
				}
				
				// Transform (translate and rotate) selected object depending on of virtual hand's transformation
				// also do this only if we have the permission to do so
				if (isOwnerCallback() && selected)
				{
					float currentHandDistance = Vector3.Distance (tracker.transform.position, interactionOrigin.transform.position);
					float virtualHandDistance = currentHandDistance * (distanceObject / distanceHand);
					
					this.transform.position = interactionOrigin.transform.position + direction * virtualHandDistance;
					
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
	
	void OnEnable()
	{
		// create the lineRenderer when the gameObject is enabled
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.SetWidth(0.02f * 20, 0.02f * 20);
		lineRenderer.SetColors (Color.magenta, Color.magenta);
		// just a start and end point is needed
		lineRenderer.SetVertexCount (2);
		
		Debug.Log("Start Homer Interaction");
	}
	
	void OnDisable ()
	{
		// remove the lineRenderer when the gameObject is disabled
		Destroy (lineRenderer);
	}
	
	// ------------------ VRUE Tasks END ----------------------------
}