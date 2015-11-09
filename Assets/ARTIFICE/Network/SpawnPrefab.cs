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
/// Class that can instantiate a Prefab
/// </summary>
public class SpawnPrefab : MonoBehaviour {
	public string PathInHierarchy="TrackingCamera"; //Object, which the spawned object is parented with

    /// <summary>
    /// Prefab that is to be instantiated
    /// </summary>
	public Transform playerPrefab;
	
	public bool spawnOnServer=false;
	public bool spawnOnClient=true;
	
	GameObject newObj=null;
	
    /// <summary>
    /// Client callback that instantiates the Prefab
    /// </summary>
    public void OnConnectedToServer()
	{
		if(spawnOnClient)
		{
			SpawnNetworkObject();
		}
	}
	
	/// <summary>
    /// Server callback that instantiates the Prefab
    /// </summary>
	void OnServerInitialized() 
	{
		if(spawnOnServer)
		{
			SpawnNetworkObject();
		}
	}
	
	/// <summary>
	/// Uses Network.Instantiate to create an Object
	/// </summary>
    private void SpawnNetworkObject()
    {
        //create prefab
        Network.Instantiate(playerPrefab, transform.position, transform.rotation, 0);
		this.networkView.RPC("relocateObjectRPC", RPCMode.AllBuffered);
		
		// enable IT selection gui
		GameObject itSelection=GameObject.Find("ITSelection");
		if(itSelection)
		{
			itSelection.GetComponent<ITSelectionGUI>().enabled = true;
		}
		
    }
	
	/// <summary>
	///  Parents the Object with the given parent-object
	/// </summary>
	private void relocateObject()
	{
		string objName="/"+playerPrefab.name+"(Clone)";
		Debug.Log(objName);
		newObj=GameObject.Find(objName);
		
		if((GameObject.Find(PathInHierarchy)!=null)&&(newObj!=null))
		{
			Debug.Log("attached to parent network");
			Vector3 locScale=newObj.transform.localScale;
			Vector3 locPos=newObj.transform.localPosition;
			newObj.transform.parent=GameObject.Find(PathInHierarchy).transform;
			newObj.transform.localScale=locScale;
			newObj.transform.localPosition=locPos;
		}
	}
	
	[RPC]
    public virtual void relocateObjectRPC()
    {
        relocateObject();
    }
}
