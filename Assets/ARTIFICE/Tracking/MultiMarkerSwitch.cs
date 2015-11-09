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
/// MultiMarkerSwitch attached to a GameObject which has as well a TrackMutiMarker script attached uses this game object to 
/// detect which marker of the marker cube is currently active (= facing towards the camera). 
/// 
/// It does not modify any components or transform nodes, can be rather used as a switch or to control a menu. 
/// </summary>
public class MultiMarkerSwitch : MonoBehaviour 
{
	public string faceFront="empty";
	private GameObject tMM;
	private Vector3 faceFrontVec=Vector3.forward;
	private Vector3 face0Normal=Vector3.forward;
	private Vector3 face1Normal=Vector3.down;
	private Vector3 face2Normal=Vector3.back;
	private Vector3 face3Normal=Vector3.up;
	private Vector3 face4Normal=Vector3.left;
	private Vector3 face5Normal=Vector3.right;

	
    /// <summary>
    /// Use Start() for initialization
    /// </summary>
	void Start () 
	{
		tMM=GameObject.Find("MultiMarkerObject");
		if(!tMM)
		{
			tMM=new GameObject("TrackingEssentials/TrackingCamera/MultiMarkerObject");
			tMM.AddComponent<TrackMultiMarker>();
			tMM.hideFlags |= HideFlags.NotEditable;
			tMM.hideFlags |= HideFlags.HideInInspector;
			tMM.hideFlags |= HideFlags.HideInHierarchy;
			tMM.transform.parent=this.transform.parent;
			tMM.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f); //pitfall!
			tMM.transform.localRotation = Quaternion.identity;
			tMM.transform.localScale = Vector3.one;
		}
	}
	
	/// <summary>
    /// Update is called once per frame
    /// </summary>
	void Update () 
	{
		tMM.GetComponent<TrackMultiMarker>().Update();
		UpdateFaceFront();
	}
	
	/// <summary>
	/// MMSwitch code, updates the faceFront attribute, which is used when using the Multimarker as switch
	/// </summary>
	void UpdateFaceFront()
	{
		if (tMM.GetComponent<TrackMultiMarker>().isTracked())
		{
			Vector3 bufferVec;
			float cos45=0.707107f;
			float dotProduct=0.0f;
			Quaternion correctY=new Quaternion(0.0f,1.0f,0.0f,0.0f);
			
			bufferVec=tMM.transform.localRotation*correctY*face1Normal;
			
			//face 0
			bufferVec=correctY*tMM.transform.localRotation*face0Normal;
			dotProduct=Vector3.Dot(bufferVec,faceFrontVec);
			if(dotProduct>cos45)
			{
				faceFront="Marker0";
				return;
			}
	
			//face1
			bufferVec=tMM.transform.localRotation*correctY*face1Normal;;
			dotProduct=Vector3.Dot(bufferVec,faceFrontVec);
			if(dotProduct>cos45)
			{
				faceFront="Marker1";
				return;
			}
	
			//face2
			bufferVec=tMM.transform.localRotation*correctY*face2Normal;
			dotProduct=Vector3.Dot(bufferVec,faceFrontVec);
			if(dotProduct>cos45)
			{
				faceFront="Marker2";
				return;
			}
	
			//face3
			bufferVec=tMM.transform.localRotation*correctY*face3Normal;
			dotProduct=Vector3.Dot(bufferVec,faceFrontVec);
			if(dotProduct>cos45)
			{
				faceFront="Marker3";
				return;
			}
	
			//face4
			bufferVec=tMM.transform.localRotation*correctY*face4Normal;
			dotProduct=Vector3.Dot(bufferVec,faceFrontVec);
			if(dotProduct>cos45)
			{
				faceFront="Marker4";
				return;
			}
	
			//face5
			bufferVec=tMM.transform.localRotation*correctY*face5Normal;
			dotProduct=Vector3.Dot(bufferVec,faceFrontVec);
			if(dotProduct>cos45)
			{
				faceFront="Marker5";
				return;
			}
	
			faceFront="empty";
		}
		else
		{
			faceFront="empty";
			return;
		}

	}

	/// <summary>
	/// Gets name of the marker which currently faces towards the camera.
	/// </summary>
	/// <returns>Name of marker which is currently active.</returns>
	public string GetFaceFront()
	{
		return faceFront;
	}

	/// <summary>
	/// Checks if given marker is currently active. 
	/// </summary>
	/// <param name="faceName">Name of marker</param>
	/// <returns>Returns true/false depending on check</returns>
	public bool IsFaceFront(string faceName)
	{
		if(faceName==faceFront)
		{
			return true;
		}
		return false;
	}
}
