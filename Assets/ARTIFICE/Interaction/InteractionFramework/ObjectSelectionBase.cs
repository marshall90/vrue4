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
/// ObjectSelectionBase is a component, that derives from InteractionBase. All interaction techniques must derive from this class. 
/// It provides ths functionality to add/remove selectable game objects and to rotate a selected game objects realtive to marker or spacemouse transformation. 
/// </summary>
public class ObjectSelectionBase: InteractionBase
{
	// select object handling
    protected bool selected = false;
    protected Hashtable collidees = new Hashtable();
	
    protected Quaternion prevRot = Quaternion.identity; // store rotation of marker from previous frame for local movement of selected object
	protected Vector3 prevPos=Vector3.zero;
	protected Quaternion selectTimeRot=Quaternion.identity;
	protected Vector3 selectTimePos=Vector3.zero;
	protected bool doSelect;

	
    /// <summary>
    /// Get the current selection state of the IT. 
    /// </summary>
    /// <returns>If object is currently selected, returns true otherwise false.</returns>
	public bool getSelectionState()
	{
		return selected;
	}
	
    /// <summary>
    /// </summary>
    public void Update()
    {
        // selection of game objects
        if (isOwnerCallback())
        {
            doSelect = false;
            bool doUnselect = false;

            doSelect = false;
            doUnselect = false;

            if (Input.GetButtonDown("Fire1"))
            {
                if (!selected)
                {
                    doSelect = true;
                    //selected = true; - set when adding collidee to private hash interactionObjs to avoid that if is collidees is empty changing to selection mode is possible 
                }
                else
                {
                    doUnselect = true;
                    //selected = false;
                }
            }


            if (doSelect)
            {
                //for all the objects our Interaction-object is colliding with, select them by calling addInteractionObj
                foreach (DictionaryEntry coll in collidees)
                {
                    GameObject collidee = coll.Value as GameObject;
                    if (collidee != null)
                    {

                        collidee = collidee.gameObject;//Do we need that?
                        try
                        {
                            if (doSelect)
                            {
                                this.addInteractionObj(collidee);
                                Debug.Log("Selected" + collidee.gameObject.GetInstanceID());
								
								selected = true;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Debug.Log("Cannt modify" + collidee.name + ex.Message);
                        }
                    }
                }
            }

            if (doUnselect)
            {
                //Unselect all objects
                this.Clear();
				selected = false;
				
                Debug.Log("Unselected all");
            }

            // ------------- Interaction technique implementation -------
			UpdateSelect();
        }	
    }
	
	/// <summary>
    /// Sets the transform of all selected objects to the given translation and orientation
    /// 
    /// TransformInter MUST use the pose of the InteractionGameObject (i.e. this.position and this.rotate of virtual hand of gogo IT)
    /// </summary>
    /// <param name="_position">Absolute WC position of the object is moved</param>
    /// <param name="_orientation">Absolute WC rotation</param>
    protected void transformInter(Vector3 _position, Quaternion _orientation)
	{
		if (doSelect)
		{
			selectTimeRot=this.transform.rotation;
			selectTimePos=this.transform.position;
		}
		transformInterBase(_position - selectTimePos,_orientation* Quaternion.Inverse(selectTimeRot), selectTimePos);
	}

    /// <summary>
    /// Implement the particular selection and manipulation technique here -> must be overwritten in subclass.
    /// </summary>
	protected virtual void UpdateSelect()
	{
	}
	

}


