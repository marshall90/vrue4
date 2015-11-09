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


/// <summary>
/// Class to show a GUI to select a IT by ARToolkitMarker during runtime. 
/// </summary>
public class ITSelectionGUI : MonoBehaviour
{
	public GUIStyle vrue11Style; 
	protected string _vh; 
	protected string _triggerMarker;
	
	/* ------------------ VRUE Tasks START --------------------------
	* Place required member variables here
	----------------------------------------------------------------- */








	// ------------------ VRUE Tasks END ----------------------------
	
	/// <summary>
	/// Set StartUp Data. Method is called by OnEnable Unity Callback
	/// Must be overwritten in deriving class
    /// </summary>
	protected virtual void StartUpData()
	{	
		// name of interaction object in Unity Hierarchy
		_vh = "VirtualHand";
		
		// name of trigger marker
		_triggerMarker = "Marker2";
	}
	
	/// <summary>
    /// </summary>
	void OnEnable()
	{	
		// set init data
		StartUpData();
		Debug.Log("IT Selection GUI enabled");
		
		/* ------------------ VRUE Tasks START --------------------------
		* find ITs (components) attached to interaction game object
		* if none is attached, manually attach 3 ITs to interaction game object
		* initially det default IT
		----------------------------------------------------------------- */




























		// ------------------ VRUE Tasks END ----------------------------
	}


	/// <summary>
    /// Unity Callback
    /// OnGUI is called every frame for rendering and handling GUI events.
    /// </summary>
	void OnGUI () {
		
		/* ------------------ VRUE Tasks START --------------------------
		* check if ITs are available
		* if trigger marker is visible and no objects are currently selected by interaction game object show GUI
		* depending on visible marker switch through availabe ITs
		* implement user confirmation and set selected IT only if user has confirmed it
		* disable the GUI if virtual hand has selected objects and if user has confirmend an IT
		----------------------------------------------------------------- */



































		// ------------------ VRUE Tasks END ----------------------------
	}
	
	
	/* ------------------ VRUE Tasks START -------------------
	----------------------------------------------------------------- */































































































	// ------------------ VRUE Tasks END ----------------------------

}
