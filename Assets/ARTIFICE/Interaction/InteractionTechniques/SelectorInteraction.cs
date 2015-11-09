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
/// Class to control objects by keyboard-input. 
/// 
/// Class to select and manipulate scene objects with keyboard input.  Works from 1st and 3rd person view. 
/// </summary>
public class SelectorInteraction : ObjectSelectionBase
{   

    public float speed_trans = 0.1f;
    float speed_rot = 30.0f;


    /// <summary>
    /// Implementation of concrete IT selection behaviour. 
    /// </summary>
	protected override void UpdateSelect()
	{
        //Keyboard input
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * speed_trans;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * speed_trans;
        float y = Input.GetAxis("Updown") * Time.deltaTime * speed_trans;
        float q = Input.GetAxis("Rot") * Time.deltaTime * speed_rot;

        // get new translation
		Vector3 offset = new Vector3(x, y, z);

        // get rotation
        Vector3 axis = new Vector3(0.0f, 1.0f, 0.0f);
        Quaternion rotate = Quaternion.AngleAxis(q, axis);

        if (selected)
        {
            this.transformInter(offset + this.transform.position, this.transform.rotation * rotate);
        }

        //Update transform of the selector object (virtual hand)
        this.transform.position += offset;
        this.transform.rotation *= rotate;
	}

	/// <summary>
    /// Callback
    /// If our selector-Object collides with anotherObject we store the other object 
    /// 
    /// For usability purpose change color of collided object
    /// </summary>
    /// <param name="other">GameObject giben by the callback</param>
    public void OnTriggerEnter(Collider other)
    {
        if (isOwnerCallback())
        {
            GameObject collidee = other.gameObject;

            if (hasObjectController(collidee))
            {

                collidees.Add(collidee.GetInstanceID(), collidee);
                Debug.Log(collidee.GetInstanceID());

                // change color so user knows of intersection
                collidee.renderer.material.SetColor("_Color", Color.blue);
            }
        }
    }

    /// <summary>
    /// Callback
    /// If our selector-Object moves out of anotherObject we remove the other object from our list
    /// 
    /// For usability purpose change color of collided object
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other)
    {
        if (isOwnerCallback())
        {
            GameObject collidee = other.gameObject;

            if (hasObjectController(collidee))
            {
                collidees.Remove(collidee.GetInstanceID());

                // change color so user knows of intersection end
                collidee.renderer.material.SetColor("_Color", Color.white);
            }
        }
    }
}