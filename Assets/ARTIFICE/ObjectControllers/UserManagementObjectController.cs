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

public class UserManagementObjectController : ExclusiveAccessObjectController
{
    /// <summary>
    /// Name of the player that is assigned in Unity-editor, only player1, player2,.... allowed
    /// </summary>
    public string accessGrantedName = "player1";

    /// <summary>
    /// NetworkPlayer that is associated with the name above in UserManager
    /// </summary>
    private NetworkPlayer accessGrantedPlayer;

    /// <summary>
    /// Checks whether the NetworkPlayer is granted access to the object.
    /// </summary>
    /// <param name="player">NetworkPlayer</param>
    /// <returns>true if it is the access granted, false otherwise</returns>
    protected override bool isObjectAccessGranted(NetworkPlayer player)
    {
        if(base.isObjectAccessGranted(player))
        {
			/* ------------------ VRUE Tasks START  -------------------
			 * 	- find out if the NetworkPlayer is the owner of the object
			----------------------------------------------------------------- */
return false; //replace me








            // ------------------ VRUE Tasks END ----------------------------
        }
        else
        {
            return false;
        }
        
    }
    
    /// <summary>
    /// Server-Callback
    /// If Player connects we update the UserManager and look up if the access
    /// </summary>
    /// <param name="player">Player that disconnected</param>
    void OnPlayerConnected(NetworkPlayer player)
    {
        UserManager.instance.AddNewPlayer(player,true);
        accessGrantedPlayer = UserManager.instance.getNetworkPlayer(accessGrantedName);
    }
	/*
	void OnServerInitialized(NetworkPlayer player) 
	{
		UserManager.instance.AddNewPlayer(player,false);
        accessGrantedPlayer = UserManager.instance.getNetworkPlayer(accessGrantedName);
	}*/

    /// <summary>
    /// Server-Callback
    /// If a player, that is allowed to access the object exits, we have to reset.
    /// </summary>
    /// <param name="player">Player that disconnected</param>
    protected new virtual void OnPlayerDisconnected(NetworkPlayer player)
    {
        UserManager.instance.OnPlayerDisconnected(player);
        if (isAccessGrantedPlayer(player))
        {
            base.OnPlayerDisconnected(player);
            accessGrantedPlayer = UserManager.nonExistingPlayer;
        }
    }

    /// <summary>
    /// Check if current player is the one the object is assigned to
    /// </summary>
    /// <param name="player">Networkplayer in question</param>
    /// <returns>True if the object is assigned to the player, false otherwise</returns>
    bool isAccessGrantedPlayer(NetworkPlayer player)
    {
        if (player == accessGrantedPlayer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
}
