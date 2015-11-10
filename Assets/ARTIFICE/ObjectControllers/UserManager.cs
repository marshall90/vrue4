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
using System.Collections.Generic;
using System.Linq;
public class UserManager : ScriptableObject {

    static readonly object padlock = new object();
    private static UserManager manager;

    /// <summary>
    /// The player that is returned if the Client is not connected
    /// </summary>
    /// 
    public static NetworkPlayer nonExistingPlayer = new NetworkPlayer();
    
	/* ------------------ VRUE Tasks START  -------------------
	 * 	- Add a data-structure, that can help you map the NetworkPlayers to name-strings
    ----------------------------------------------------------------- */

	public Dictionary<NetworkPlayer, string> networkPlayers;

	private NetworkGUI _networkGUI = null;
	public NetworkGUI networkGUI {
		get {
			if (_networkGUI == null) {

				_networkGUI = GameObject.Find ("GUIObj").GetComponent<NetworkGUI>() as NetworkGUI;
			}
			return _networkGUI;
		}
	}

	// ------------------ VRUE Tasks END ----------------------------

    /// <summary>
    /// Singleton method returning the instance of UserManager
    /// </summary>
    public static UserManager instance
    {
        get
        {
            lock (padlock)
            {
                return (manager ? manager : manager = new UserManager());
            }
        }
    }
    /// <summary>
    /// Called by UserManagementObjectController on the server whenever a new player has successfully connected.
    /// </summary>
    /// <param name="player">Newly connected player</param>
    /// <param name="isClient">True if Client connected</param>
    public void AddNewPlayer(NetworkPlayer player,bool isClient)
    {
		
		/* ------------------ VRUE Tasks START  -------------------
 		* 	- If a new player connects we want to assign him a name string.
 		* 	- If the player is already in the data-structure, nothing should happen.
 		* 	- These strings should be "player1" for the first client connected "player2"
 		* 	for the second and so forth. Make sure that the indices always stay as low 
 		* 	as possible(e.g. if "player1" disconnects, we want the next client that connects
 		* 	to be "player1"
 		* 	- Make an RPC-call that sends the player name to client and show it in the GUI
		----------------------------------------------------------------- */
		if ( networkPlayers.ContainsKey (player) ) return; // do nothing, since the player is already in the structure

		string userString = "player" + highestAvailableUserIndex();
		networkGUI.networkView.RPC ("receivePlayerName", player, userString);
		networkPlayers.Add (player, userString);




        // ------------------ VRUE Tasks END ----------------------------
    }

    /// <summary>
    /// Called by UserManagementObjectController on the server whenever a player disconnected from the server.
    /// </summary>
    /// <param name="player">Disconnected player</param>
	/// 
	private int highestAvailableUserIndex() {
		int highest = 1;
		//int[] indexes = new int[networkPlayers.Count];
		List<int> indexes = new List<int>(networkPlayers.Count);


		if (networkPlayers.Count ==0) {
			return 1;
		}  

		// build an array of indexes
		int c = 0;
		foreach (string value in networkPlayers.Values) {

			int id = int.Parse(value.Substring("player".Length));
			indexes[c] = id;

			c++;
		}

		indexes.Sort();
		// walk through the list until we find a gap
		for (int i = 1; i < indexes.Count; i++) {

			if (indexes[i] - indexes[i-1] > 1) return (indexes[i-1] + 1);
			

		}

		return indexes[indexes.Count] + 1;


	}
    public void OnPlayerDisconnected(NetworkPlayer player) 
    {
		/* ------------------ VRUE Tasks START  -------------------
		 * If a player disconnects remove it from our datastructure (if it is in there!)
        ----------------------------------------------------------------- */
		if (networkPlayers.ContainsKey (player)) {

			networkPlayers.Remove(player);
		}

        // ------------------ VRUE Tasks END ----------------------------
    }

    /// <summary>
    /// Looks up the NetworkPlayer associated with the name
    /// </summary>
    /// <param name="playerName">Name of the NetworkPlayer</param>
    /// <returns>NetworkPlayer reference</returns>
    public NetworkPlayer getNetworkPlayer(string playerName)
    {
        /* ------------------ VRUE Tasks START  -------------------
         * 	- Find the NetworkPlayer assigned to the playerName in your datastructure
         * 	and return it.
        ----------------------------------------------------------------- */
		var found = networkPlayers.Where (e => e.Equals (playerName)).FirstOrDefault ();
		return found.Key;








        // ------------------ VRUE Tasks END ----------------------------
		
        return nonExistingPlayer;
    }
}
