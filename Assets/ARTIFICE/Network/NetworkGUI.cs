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
/// Class NetworkGUi is responsible for setting up the GUI containing elements
/// for configuring client and server.
/// </summary>
[RequireComponent (typeof(NetworkBase))]

public class NetworkGUI : MonoBehaviour {

	public string serverAddress = "";
	public string serverPort = "";
	/* ------------------ VRUE Tasks START --------------------------
	 * store player name
    ----------------------------------------------------------------- */



    // ------------------ VRUE Tasks END ----------------------------

	/// <summary>
	/// Unity Callback
    /// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
    {
        Application.targetFrameRate = 60;
		serverAddress = GetComponent<NetworkBase>().serverAddress;
		serverPort = GetComponent<NetworkBase>().serverPort.ToString();
    }
	
    /// <summary>
    /// Unity Callback
    /// OnGUI is called for rendering and handling GUI events.
    /// </summary>
	void OnGUI () {
		Rect windowRect = new Rect (0,0,240,140);
		if (!(Network.peerType == NetworkPeerType.Disconnected)) {
			windowRect = new Rect (0,0,240,140);
		}
		windowRect = GUI.Window(0, windowRect, MakeWindow, "Network Controls");
	}
	
    /// <summary>
    /// Sets up the GUI-elements
    /// </summary>
    /// <param name="id"></param>
	void MakeWindow(int id) {
		
		if (Network.peerType == NetworkPeerType.Disconnected) {
			
			GUILayout.Label("Connection status: disconnected");
			
			GUILayout.Space (5);

			
			if (GUILayout.Button ("Start Server")) {
				GetComponent<NetworkBase>().StartServer();
			}
			GUILayout.Space (10);
			
			if (GUILayout.Button ("Connect to Server"))
				GetComponent<NetworkBase>().ConnectToServer(serverAddress, Convert.ToInt32(serverPort));
			
			GUILayout.BeginHorizontal();
			serverAddress = GUILayout.TextField(serverAddress);
			serverPort = GUILayout.TextField(serverPort.ToString(),6);
			GUILayout.EndHorizontal();
		} else {
			
			if (Network.peerType == NetworkPeerType.Connecting){
				GUILayout.Label("Connection status: connecting ...");
			} else if (Network.peerType == NetworkPeerType.Client){
				GUILayout.Label("Connection status: connected as client");
				GUILayout.Label("Ping to server: "+Network.GetAveragePing(  Network.connections[0] ) );
                
				/* ------------------ VRUE Tasks START --------------------------
	 			*	- add label for player name
    			----------------------------------------------------------------- */




				// ------------------ VRUE Tasks END ----------------------------
				
			} else if (Network.peerType == NetworkPeerType.Server){
				GUILayout.Label("Connection status: running as server");
			}
			
			if (GUILayout.Button ("Disconnect")){
				Network.Disconnect(500);
			}
		}
	}
	
	/* ------------------ VRUE Tasks START --------------------------
	 *	- Add an RPC method, which has the Client's name-string
	 *	as a parameter
	 ----------------------------------------------------------------- */








    // ------------------ VRUE Tasks END ----------------------------
    
    
    /// <summary>
    /// Client Callback
    /// A Client that disconnects from the server reloads the scene 
    /// </summary>
    void OnDisconnectedFromServer()
    {
        Application.LoadLevel(0);
    }


}