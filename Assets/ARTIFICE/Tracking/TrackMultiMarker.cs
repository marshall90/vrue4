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
using System;

/// <summary>
/// Provides tracking data for a marker cube received from ARToolkit marker to Unity3D game objects. 
/// Inherits from abstract class TrackBase. 
/// 
/// TrackMultiMarker gets its data indirectly from six different GameObjects which have a TrackMMPart component attached. 
/// This component provides transformation data to the transform node of the six GameObjects.
/// </summary>
public class TrackMultiMarker : TrackBase {

	public float scalePosition = 1.0f;
	public double deviceTimeout = 200.0;
	private GameObject[] markerArray = new GameObject[6];
	private double currentTimestamp = double.NegativeInfinity;
	private TransformationContainer transCont = new TransformationContainer();

    /// <summary>
    /// Use Start() for initialization
    /// For each marker of the cube create new game object and attach component trackMMPart to it. 
    /// Then set initial position and orientation of each marker game obejct
    /// </summary>
	void Start () {
		
		for(int i=0;i<=5;i++)
		{
			GameObject go;
			go=new GameObject("TrackMarkerPart"+i.ToString());
			go.transform.parent=this.transform.parent;
			go.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f); //pitfall!
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			
			go.hideFlags = HideFlags.HideInHierarchy;
        	go.hideFlags |= HideFlags.HideInInspector;
        	go.hideFlags |= HideFlags.NotEditable;
			
			go.AddComponent<TrackMMPart>();
			TrackMMPart tm = go.GetComponent<TrackMMPart>();
			tm.translationToMM = new Vector3(0.0f,0.0f,-0.06f*scalePosition);
			tm.scalePosition = this.scalePosition;
			tm.deviceName = "Marker"+i.ToString();
			switch( i ) 
			{
			    case 0:
					tm.rotationToMM=new Quaternion(0.0f,0.0f,0.0f,1.0f);
			        break;
			    case 1:
					tm.rotationToMM=new Quaternion(0.7071068f,0.0f,0.0f,0.7071068f);
			        break;
				case 2:
					tm.rotationToMM=new Quaternion(1.0f,0.0f,0.0f,0.0f);
			        break;
				case 3:
					tm.rotationToMM=new Quaternion(0.7071068f,0.0f,0.0f,-0.7071068f);
			        break;
				case 4:
					tm.rotationToMM=new Quaternion(0.7071068f,0.0f,-0.7071068f,0.0f);
			        break;
				case 5:
					tm.rotationToMM=new Quaternion(0.5f,0.5f,0.5f,-0.5f);
			        break;
			}
			

			markerArray[i]=go;
		}
	}
	
	/// <summary>
    /// Update is called once per frame
    /// </summary>
	public void Update () 
	{
		foreach(GameObject marker in markerArray)
	    {
			if(marker)
			{
				TrackMMPart markerInfo=marker.GetComponent<TrackMMPart>();
				//markerInfo.Update(); //TODO/b: fix performance issues with this update. fixed?
				if(markerInfo)
				{
					OneMarkerUpdate upd = markerInfo.getUpdate();
					if(upd.timestamp >= currentTimestamp)
					{
						currentTimestamp=upd.timestamp;
						_trackingTimestamp=upd.timestamp;
						UpdateMarker(upd);
					}
				}
			}
	    }
		
		transform.localPosition=transCont.GetPosition();
		transform.localRotation=transCont.GetRotation();
		
		// control rendering
		setVisability(gameObject, isTracked());
	}
	
	/// <summary>
    /// Update the given one marker update by calling the UpdateMarker method from the TransformationContainer class. 
    /// </summary
    /// <param name="upd"></param>
	void UpdateMarker(OneMarkerUpdate upd)
	{
		if(upd != null)
		{
			transCont.UpdateMarker(upd);
		}
	}
	
	/// <summary>
    /// Override inherited method to provide tracking state of tracking device. 
    /// </summary
	public override bool isTracked() 
    {
		double currTime = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;	
		return (currTime - trackingTimestamp) < deviceTimeout;
	}
}

/// <summary>
/// Class used to store and interpolate transformations received from different tracked markers
/// </summary>
public class TransformationContainer
{
	private double time=double.NegativeInfinity;
	private Vector3 currentPosition=Vector3.zero;
	private Quaternion currentRotation=Quaternion.identity;
	private float currentWeight=0.0f;
	
	/// <summary>
	/// Pushes the data of newly received marker data into the interpolation process
	/// </summary>
	/// <param name="upd">
	/// An update from a single marker <see cref="OneMarkerUpdate"/>
	/// </param>
	public void UpdateMarker(OneMarkerUpdate upd)
	{
		if(time<upd.timestamp)
		{
			time=upd.timestamp;
			currentPosition=upd.position;
			currentRotation=upd.rotation;
			currentWeight=1.0f;
		}
		else if(time==upd.timestamp)
		{
			currentPosition=Vector3.Lerp(currentPosition,upd.position,currentWeight/(currentWeight+1.0f));
			currentRotation=Quaternion.Slerp(currentRotation,upd.rotation,currentWeight/(currentWeight+1.0f));
			currentWeight+=1.0f;
		}
	}
	
	/// <summary>
	/// Returns the current position of the cube
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	public Vector3 GetPosition()
	{
		return currentPosition;
	}
	
	/// <summary>
	/// Returns the current rotation of the cube
	/// </summary>
	/// <returns>
	/// A <see cref="Quaternion"/>
	/// </returns>
	public Quaternion GetRotation()
	{
		return currentRotation;
	}
	
	/// <summary>
	/// Parameters are updated with the current transformation
	/// </summary>
	/// <param name="position">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <param name="rotation">
	/// A <see cref="Quaternion"/>
	/// </param>
	public void GetTransformation(ref Vector3 position,ref Quaternion rotation)
	{
		position=currentPosition;
		rotation=currentRotation;
	}
	

}


