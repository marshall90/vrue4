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
/// Class that provides methods for the main network functionality
/// </summary>
public class NetworkBase : MonoBehaviour {

    public int serverPort = 12346;
	public string serverAddress="localhost";
    public int maxPlayers = 2;
	public int netwSendRate = 25;

    /// <summary>
    /// Starts the server with the configured parameters(port,maxPlayers,sendrate etc.)
    /// </summary>
	public void StartServer()
	{
		
		/* ------------------ VRUE Tasks START --------------------------
		 * 	- Start the server at port "serverPort" with players "maxPlayers"
		 * 	and set the network-send-rate to "netwSendRate". Print error message to
		 * 	console/log-file, in case it should fail
		 * 	(You should NOT use NAT punchthrough. It usually creates more problems than it solves)
		----------------------------------------------------------------- */


		NetworkConnectionError error =  Network.InitializeServer (maxPlayers, serverPort, false);
		if (error != NetworkConnectionError.NoError) {

			Debug.Log (error.ToString());
		}
		Network.sendRate = netwSendRate;


		// ------------------ VRUE Tasks END ----------------------------
		
	}
	
    /// <summary>
    /// Connects to server with the specified parameters.
    /// </summary>
    /// <param name="host">Hostname of the server</param>
    /// <param name="port">Port the server is running at</param>
	public void ConnectToServer(string host, int port)
	{
		/* ------------------ VRUE Tasks START --------------------------
		 * Try to connect to server
		 * 	- Connect to the server at adress "host" at port "port". 
		 * 	- Print error message to console/log-file, in case it should fail
		 * -------------------------------------------------------------- */

		Network.Connect (host, port);


		// ------------------ VRUE Tasks END ----------------------------
	}
	 
	/// <summary>
	/// Server callback
    /// See Unity-Script-Reference
	/// </summary>
    void OnServerInitialized()
    {
        Debug.Log("OnServerInitialized: "+Network.player.ipAddress+":"+Network.player.port);
    }

    /// <summary>
    /// Server callback
    /// See Unity-Script-Reference
    /// </summary>
    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("New player connected from " + player.ipAddress + ":" + player.port);
        //UserManager.instance.OnPlayerConnected(player);
	}

    /// <summary>
    /// Server callback
    /// See Unity-Script-Reference
    /// </summary>
    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Player has disconnected " + player.ipAddress + ":" + player.port);
        Debug.Log("Server destroying player");
        Network.RemoveRPCs(player, 0);
        Network.DestroyPlayerObjects(player);
        //UserManager.instance.OnPlayerDisconnected(player);
    }

    /// <summary>
    /// Client callback
    /// See Unity-Script-Reference
    /// </summary>
	void OnConnectedToServer()
    {
        Debug.Log("OnConnectedToServer()");
    }

    /// <summary>
    /// Client callback
    /// See Unity-Script-Reference
    /// </summary>
    void OnFailedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Could not connect to server: "+ error);
        Debug.Log("Retry to connect to: " + serverAddress + " - " + serverPort);
        // Try to connect to server
		ConnectToServer(serverAddress,serverPort);
    }
}
