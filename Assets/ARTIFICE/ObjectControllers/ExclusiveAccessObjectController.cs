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
/// This class makes sure only one client at a time can access the object
/// </summary>
public class ExclusiveAccessObjectController : NetworkObjectController
{
    /// <summary>
    /// The default client used if the object is not accessed.
    /// </summary>
    /// 
    protected static NetworkPlayer defaultAccessPlayer = new NetworkPlayer();

    /// <summary>
    /// The client currently "accessing" the object.
    /// </summary>
    /// 
    
	/* ------------------ VRUE Tasks START   -------------------
	 *	- Add a NetworkPlayer member to store the player that has the object currently selected
	 *	- The static member above should be used for initialization for the new member. 
	 *	- If the defaultAccessPlayer is assigned, no player has currently selected the object(You 
	 *	have to implement these semantics yourself in the code below).
	 ----------------------------------------------------------------- */



	// ------------------ VRUE Tasks END ----------------------------

    /// <summary>
    /// RPC-method that turns on/off selection of the controlled object. 
    /// Also sets the parameters required for certain components.
    /// e.g. Physics. Overrides the one from NetworkObjec Controller
    /// </summary>
    /// <param name="select">true for select, false for unselect</param>
    /// <param name="info">Info about the caller-client</param>
    /// <returns>true if object not already selected, false otherwise</returns>
    [RPC]
    public override bool controlSelectedObjects(bool select, NetworkViewID viewID, NetworkMessageInfo info)
    {
        if (isObjectAccessGranted(info.sender))
        {
            if (base.controlSelectedObjects(select,viewID,info))
            {
				/* ------------------ VRUE Tasks START   -------------------
				 *	- set the current NetworkPlayer to the player calling the RPC-method or 
                 *	the default NetworkPlayer depending on "select"
                ----------------------------------------------------------------- */

return true;//replace me











                // ------------------ VRUE Tasks END ----------------------------
				
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the player is currently the exclusive access user of the object.
    /// </summary>
    /// <param name="player">Player</param>
    /// <returns>True if the player accesses the object, false otherwise.</returns>
    protected bool isAccessingObject(NetworkPlayer player)
    {
        /* ------------------ VRUE Tasks START   -------------------
         * 	- return true if the player is the one that has currently selected the object
        ----------------------------------------------------------------- */

return false; //replace me









		// ------------------ VRUE Tasks END ----------------------------
    }

    /// <summary>
    /// Checks whether the NetworkPlayer is granted access to the object.
    /// </summary>
    /// <param name="player">NetworkPlayer</param>
    /// <returns>true if it is the access granted, false otherwise</returns>
    protected override bool isObjectAccessGranted(NetworkPlayer player)
    {
        if(base.isObjectAccessGranted(player))
        {
			/* ------------------ VRUE Tasks START   -------------------
			 * 	- return true if the player is the one that has currently selected
			 * the object or the object is not selected.
			----------------------------------------------------------------- */

return true; //replace me





            // ------------------ VRUE Tasks END ----------------------------
        }
        else
        {
            return false;
        }
        return false;
        
    }
    /// <summary>
    /// If object is selected by Player disconnecting then we have to unselect.
    /// </summary>
    /// <param name="player">Player that disconnected</param>
    protected virtual void OnPlayerDisconnected(NetworkPlayer player)
    {
        if (isAccessingObject(player))
        {
			/* ------------------ VRUE Tasks START   -------------------
			 * 	- Once the client disconnects you have to bring the
			 * 	object to an unselected state.
			 ----------------------------------------------------------------- */



			// ------------------ VRUE Tasks END ----------------------------
            
            //unselect
            NetworkMessageInfo info= new NetworkMessageInfo();
            base.controlSelectedObjects(false, this.networkView.viewID, info);
        }
    }


}
