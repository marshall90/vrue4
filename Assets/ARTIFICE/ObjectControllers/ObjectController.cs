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
/// ObjectController class is a component that makes an object selectable and transformable
/// </summary>
public class ObjectController : MonoBehaviour
{
    /// <summary>
    /// true if not selected by an interaction-device
    /// </summary>
    private bool isSelectable = true;
    private Color oldColor = Color.white;
	protected Quaternion selectTimeRot=Quaternion.identity;
	protected Vector3 selectTimePos=Vector3.zero;

    /// <summary>
    /// Updates the transformation of the controlled object
    /// This method rotates the controlled object relative to the specified 
    /// point by the given Quaternion. Then the object is moved by the specified
    /// translational offset.
    /// </summary>
    /// <param name="translation">Offset the object is moved</param>
    /// <param name="rotate">Additional rotation</param>
    /// <param name="poseOfInteraction">pivotpoint the object is rotated about</param>
    public void updateTransform(Vector3 translation, Quaternion rotate,Vector3 grabPoint)
    {
		transform.rotation= rotate * selectTimeRot;
		transform.position = translation - (rotate * (grabPoint - selectTimePos) - grabPoint);
    }
    /// <summary>
    /// Turns on/off selection of the controlled object. 
    /// Also sets the parameters required for certain components.
    /// e.g. Physics
    /// </summary>
    /// <param name="select">true for select, false for unselect</param>
    /// <returns>true if object not already selected, false otherwise</returns>
    public bool controlSelectedObjects(bool select)
    {
        if ((select && isSelectable) || (!select && !isSelectable))
        {
            gameObject.rigidbody.isKinematic = select;
            gameObject.rigidbody.useGravity = !select;
            
			if (select)
            {
                Debug.Log("Object" + this.GetInstanceID() + " selected.");
                
				isSelectable = false;
                if (this.GetComponent<Renderer>() != null)
				{
					oldColor=this.renderer.material.color;
                	this.renderer.material.color = Color.red;
				}
				
				selectTimePos = transform.position;
				selectTimeRot = transform.rotation;
            }
            else
            {
                Debug.Log("Object " + this.GetInstanceID() + " unselected");
                isSelectable = true;
                if (this.GetComponent<Renderer>() != null)
				{
					this.renderer.material.color=oldColor;
				}
            }
            return true;
        }
        return false;
    }
}