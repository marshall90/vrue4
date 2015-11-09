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
/// NetworkObjectController class is a component that makes an object 
/// selectable and transformable, especially for scenes distributed over
/// the network. This is achieved by providing RPC-calls to the clients
/// for the required methods.
/// </summary>
public class NetworkObjectController : ObjectController 
{
    /// <summary>
    /// RPC-method that updates the transformation of the controlled object
    /// This method rotates the controlled object relative to the specified 
    /// point by the given Quaternion. Then the object is moved by the specified
    /// translational offset.
    /// </summary>
    /// <param name="translation">Offset the object is moved</param>
    /// <param name="rotate">Additional rotation</param>
    /// <param name="poseOfInteraction">pivotpoint the object is rotated about</param>
    /// <param name="info">Info about the caller-client</param>
    [RPC]
    public virtual void updateTransform (NetworkViewID viewID, Vector3 translation, Quaternion rotation, Vector3 poseOfInteraction,NetworkMessageInfo info) 
    {
		
        if (isObjectAccessGranted(info.sender))
        {   
            base.updateTransform(translation, rotation, poseOfInteraction);
        }
    }

    /// <summary>
    /// RPC-method that turns on/off selection of the controlled object. 
    /// Also sets the parameters required for certain components.
    /// e.g. Physics
    /// </summary>
    /// <param name="select">true for select, false for unselect</param>
    /// <param name="info">Info about the caller-client</param>
    /// <returns>true if object not already selected, false otherwise</returns>
    [RPC]
    public virtual bool controlSelectedObjects(bool select, NetworkViewID viewID, NetworkMessageInfo info)
    {
        if (isObjectAccessGranted(info.sender))
        {
            if (base.controlSelectedObjects(select))
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Checks whether the NetworkPlayer is granted access to the object.
    /// </summary>
    /// <param name="player">NetworkPlayer</param>
    /// <returns>true if it is the access granted, false otherwise</returns>
    protected virtual bool isObjectAccessGranted(NetworkPlayer player)
    {
        return true;
    }


    /// <summary>
    /// Callback to stream data on the server-side to the client and
    /// decode it at the client sided.
    /// </summary>
    /// <param name="stream">Bitstream used</param>
    /// <param name="info">Info of the sender</param>
    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {	
        if (stream.isWriting)
        {
            //Executed on the owner of the networkview; in this case the Server
            //The server sends it's position over the network
            Vector3 pos = transform.position;
            Quaternion rot = transform.localRotation;
            bool isKinematic = this.rigidbody.isKinematic;
			float r = 0.0f;
			float g = 0.0f;
			float b = 0.0f;
			float a = 0.0f;
			
			
			//Debug.Log("gameObject:" + gameObject.name);
			if (this.GetComponent<Renderer>() != null)
			{
				r = renderer.material.color.r;
	            g = renderer.material.color.g;
	            b = renderer.material.color.b;
	            a = renderer.material.color.a;
			}

            stream.Serialize(ref pos);//"Encode" it, and send it
            stream.Serialize(ref rot);//"Encode" it, and send it
            stream.Serialize(ref isKinematic);//"Encode" it, and send it
           
			stream.Serialize(ref r);//"Encode" it, and send it
            stream.Serialize(ref g);//"Encode" it, and send it
            stream.Serialize(ref b);//"Encode" it, and send it
            stream.Serialize(ref a);//"Encode" it, and send it
		
        }
        else
        {
            //Executed on the others; in this case the Clients
            //The clients receive a position and set the object to it

            Vector3 posReceive = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            float r = 0.0f;
            float g = 0.0f;
            float b = 0.0f;
            float a = 0.0f;

            bool isKinematic = false;
            stream.Serialize(ref posReceive); //"Decode" it and receive it
            stream.Serialize(ref rot); //"Decode" it and receive it
            stream.Serialize(ref isKinematic); //"Decode" it and receive it
      
			stream.Serialize(ref r);//"Encode" it, and send it
	        stream.Serialize(ref g);//"Encode" it, and send it
	        stream.Serialize(ref b);//"Encode" it, and send it
	        stream.Serialize(ref a);//"Encode" it, and send it
		
				
            transform.position = posReceive;
            transform.localRotation = rot;
            this.rigidbody.isKinematic = isKinematic;
       		
			if (this.GetComponent<Renderer>() != null)
			{
				Color newColor=new Color(r, g, b, a);
            	this.renderer.material.SetColor("_Color", newColor);
			}
			
        }
    }
}
