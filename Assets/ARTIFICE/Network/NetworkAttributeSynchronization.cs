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
/// 
/// 
/// 
/// 
/// </summary>
public class NetworkAttributeSynchronization :  MonoBehaviour
{
	public bool distributePos = true;
	public bool distributeRot = true;
	public bool distributeVisibility = true;

    /// <summary>
    /// Callback to stream data to all other clients and the server.
    /// For the distribution of the position, localRotation, and renderer visibility
    /// </summary>
    /// <param name="stream">Bitstream used</param>
    /// <param name="info">Info of the sender</param>
    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {	
        if (stream.isWriting)
        {
            //Executed on the owner of the networkview;
            //position and orientation is distributed over the network
           	if (distributePos)
			{
				Vector3 pos = transform.position;
	            stream.Serialize(ref pos);//"Encode" it, and send it
			}
			
			if (distributeRot)
			{
				Quaternion rot = transform.localRotation;
	            stream.Serialize(ref rot);//"Encode" it, and send it
			}
			
			if (distributeVisibility)
			{
				bool rendererState = true;
				// also search for child objects, which should be enables / disabled if marker is not tracked
				Renderer[] rendererComponents = this.GetComponentsInChildren<Renderer>();
				if(rendererComponents != null)
				{
		        	foreach (Renderer component in rendererComponents) 
					{
						// todo: save and send every single renderer state
						rendererState = component.enabled;
					}
				}
				stream.Serialize(ref rendererState);
			}
            
                        
			
		
        }
        else
        {
            //Executed on the others; in this case the Clients and Server
            //The clients receive a pos & orient and set the object to it
			if (distributePos)
			{
				Vector3 posReceive = Vector3.zero;
	            stream.Serialize(ref posReceive); //"Decode" it and receive it
				transform.position = posReceive;
			}
			
			if (distributeRot)
			{
				Quaternion rot = Quaternion.identity;
				stream.Serialize(ref rot); //"Decode" it and receive it
				transform.localRotation = rot;
			}
			
			if (distributeVisibility)
			{
				bool rendererState = true;
				stream.Serialize(ref rendererState);//"Decode" it and receive it
	      		
				
				Renderer[] rendererComponents = this.GetComponentsInChildren<Renderer>();
				
				// enable/disable all child components, due to marker visibility
				if(rendererComponents != null)
				{
		        	foreach (Renderer component in rendererComponents) 
					{
		            	component.enabled = rendererState;
					}
				}
			}
			
        }
    }
}
