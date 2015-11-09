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
/// Class to show a GUI to select a IT by ARToolkitMarker during runtime. Implements the functionality for selecting
/// the IT if application is distributed over the network. 
///
/// Inherits from ITSelectionGUI
/// </summary>
public class NetworkITSelectionGUI : ITSelectionGUI
{

	/// <summary>
	/// Set StartUp Data for IT Selection in distributed environments
	/// Method is called by OnEnable Unity Callback
    /// </summary>
	protected override void StartUpData()
	{	
		/* ------------------ VRUE Tasks START --------------------------
		- set name for interaction game object
		- set name for trigger marker
		----------------------------------------------------------------- */


		// ------------------ VRUE Tasks END ----------------------------
	}
}
