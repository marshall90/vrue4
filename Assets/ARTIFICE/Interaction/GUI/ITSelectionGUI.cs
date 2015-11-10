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
using System.Collections.Generic;
using UnityEngine.UI;


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
	
	private List<ObjectSelectionBase> itComponents;
	private GameObject virtualHand;
	
	//Get the MultiMarkerSwitch so we can use it in our script
	private MultiMarkerSwitch mmSwitch=null;
	
	public bool guiEnabled;
	
	private GameObject guiPanel;
	private Text guiText;
	private string virtualHandText = "VirtualHandInteraction";
	private string homerText = "HomerInteraction";
	private string gogoText = "GogoInteraction";
	
	private enum InteractionMethods {VirtualHand, Gogo, Homer};
	private int guiCurrentInteractionMethod;
	
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
		
		//get the multiMarkerSwitch Script
		mmSwitch = gameObject.GetComponent<MultiMarkerSwitch>();
		
		//disable GUI
		guiEnabled = false;
		guiPanel = GameObject.Find ("GUIPanel");
		guiPanel.GetComponent<Image> ().enabled = true;
		guiText = GameObject.Find ("GUIText").GetComponent<Text>();
		guiText.enabled = true;
		guiPanel.SetActive (false);
		
		virtualHand = GameObject.Find (_vh);
		
		itComponents = new List<ObjectSelectionBase> ();
		
		ObjectSelectionBase[] allItComponents = GameObject.FindObjectsOfType<ObjectSelectionBase> ();
		
		//go through all ObjectSelectionBase and find the one that are attached to the virtual hand
		foreach (ObjectSelectionBase osb in allItComponents)
		{
			if(osb.gameObject == virtualHand)
			{
				itComponents.Add(osb);
			}
		}
		
		// if no ObjectSelectionBase components are attached to the virtualHand then add three of them
		if(itComponents.Count == 0)
		{
			//add them to the component and to the itComponents list
			itComponents.Add(virtualHand.AddComponent<VirtualHandInteraction>() as VirtualHandInteraction);
			itComponents.Add(virtualHand.AddComponent<GoGoInteraction>() as GoGoInteraction);
			itComponents.Add(virtualHand.AddComponent<HomerInteraction>() as HomerInteraction);
		}
		
		//disable all except the VirtualHandInteraction
		foreach (ObjectSelectionBase osb in itComponents)
		{
			if(osb.GetType().ToString().Equals("VirtualHandInteraction"))
			{
				//activate the VirtualHandInteraction
				osb.enabled = true;
			}
			else
			{
				osb.enabled = false;
			}
		}
		
		
		
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
		
		// get the current active IT
		ObjectSelectionBase currentIT = null;
		foreach (ObjectSelectionBase osb in itComponents)
		{
			if(osb.enabled)
			{
				currentIT = osb;
			}
		}
		
		if(currentIT == null)
		{
			Debug.LogError("No IT activated");
			return;
		}
		
		//check, that nothing is selected!
		if(currentIT.getSelectionState() == false)
		{
			if(mmSwitch.IsFaceFront("Marker2") && !guiEnabled)
			{
				//enable the GUI!
				guiEnabled = true;
				
				//set VirtualHandInteraction as default
				guiCurrentInteractionMethod = (int)InteractionMethods.VirtualHand;
			}
			
			//check the other markers for GUI interaction
			else if(mmSwitch.IsFaceFront("Marker4") && guiEnabled)
			{
				//GoGoInteraction
				guiCurrentInteractionMethod = (int)InteractionMethods.Gogo;
			}
			else if(mmSwitch.IsFaceFront("Marker0") && guiEnabled)
			{
				//HomerInteraction
				guiCurrentInteractionMethod = (int)InteractionMethods.Homer;
			}
			else if(mmSwitch.IsFaceFront("Marker5") && guiEnabled)
			{
				//VirtualHandInteraction
				guiCurrentInteractionMethod = (int)InteractionMethods.VirtualHand;
			}
			
			if(Input.GetButtonDown("Fire2") && guiEnabled)
			{
				guiEnabled = false;
				
				//when the same IT is selected as the currently running one, then do nothing
				bool exit = false;
				
				switch (guiCurrentInteractionMethod)
				{
				case (int) InteractionMethods.VirtualHand:
					if(currentIT.GetType().ToString().Equals("VirtualHandInteraction"))
					{
						exit = true;
					}
					break;
				case (int) InteractionMethods.Gogo:
					if(currentIT.GetType().ToString().Equals("GoGoInteraction"))
					{
						exit = true;
					}
					break;
				case (int) InteractionMethods.Homer:
					if(currentIT.GetType().ToString().Equals("HomerInteraction"))
					{
						exit = true;
					}
					break;
				}
				
				if(!exit)
				{
					
					//deactivate the current activated IT
					currentIT.enabled = false;
					
					//start the selected IT and close the GUI
					foreach (ObjectSelectionBase osb in itComponents)
					{
						exit = false;
						
						switch (guiCurrentInteractionMethod)
						{
						case (int) InteractionMethods.VirtualHand:
							if(osb.GetType().ToString().Equals("VirtualHandInteraction"))
							{
								osb.enabled = true;
								exit = true;
							}
							break;
						case (int) InteractionMethods.Gogo:
							if(osb.GetType().ToString().Equals("GoGoInteraction"))
							{
								osb.enabled = true;
								exit = true;
							}
							break;
						case (int) InteractionMethods.Homer:
							if(osb.GetType().ToString().Equals("HomerInteraction"))
							{
								osb.enabled = true;
								exit = true;
							}
							break;
						}
						
						if(exit)
						{
							// no need to look for the rest of the ITs in the list
							break;
						}
					}
					
				}
			}
		}
		else
		{
			guiEnabled = false;
		}
		
		if(guiEnabled)
		{
			guiPanel.SetActive(true);
			
			switch (guiCurrentInteractionMethod)
			{
			case (int) InteractionMethods.VirtualHand:
				guiText.text = virtualHandText;
				break;
			case (int) InteractionMethods.Gogo:
				guiText.text = gogoText;
				break;
			case (int) InteractionMethods.Homer:
				guiText.text = homerText;
				break;
			}
		}
		else
		{
			guiPanel.SetActive(false);
		}
		
		// ------------------ VRUE Tasks END ----------------------------
	}
	
	
	/* ------------------ VRUE Tasks START -------------------
	----------------------------------------------------------------- */
	
	
	
	
	
	// ------------------ VRUE Tasks END ----------------------------
	
}
