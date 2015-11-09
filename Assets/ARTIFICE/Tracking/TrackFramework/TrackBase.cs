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
/// TrackBase is a abstract component, that provides the tracking data, 
/// implements timeout, set visabilzty due to tracking state of game objects who have a tracking component attached
/// 
/// All tracking sources who change the transform node of a unity game object should derive from this class.
/// </summary>
public abstract class TrackBase : MonoBehaviour
{
	/// <summary>
    /// Timestamp in milliseconds since 1970.
    /// [ms]
    /// </summary>
    protected double _trackingTimestamp = double.NegativeInfinity; //in {ms]
	
	/// <summary>
    /// Return tracking device state. true = tracked. 
	///
	/// Deriving tracking device classes may override it.
    /// </summary>
	public virtual bool isTracked() 
    {
		this.setVisability(gameObject, true);
		return true;
	}
		
	/// <summary>
    /// Return current tracking timestamp
    /// </summary>
    public double trackingTimestamp
    {
        get
        {
            return _trackingTimestamp;
        }
    }
	
	/// <summary>
    /// Set the visability of GameObj obj (and all its children) true/false by enabling/disabling the 
    /// renderer component attached to this GameObject obj (and those attached to its children)
	/// Visability depends on given bool parameter trackingFound. 
	/// 
	/// This method is implicitly called in update() method of inherting tracking device classes whose 
	/// tracking state may vary. So the game objects (and their children) that have a tracking component 
	/// attached are automatically rendered due to their tracking state. 
	///
	/// If you want to control the rendering of a game object depending on the tracking state that does not 
	/// have a tracking component attached (like virtual hand in example3+4) call this method. 
    /// </summary>
	/// <param name="obj">GameObj (and its children) that shall be rendered/not rendered</param>
    /// <param name="trackingFound">true = rendered, false = not rendered</param>
	public void setVisability(GameObject obj, bool trackingFound)
	{
		Renderer[] rendererComponents = obj.GetComponentsInChildren<Renderer>();
		
		if(trackingFound)
		{
			// Enable rendering:
	        foreach (Renderer component in rendererComponents) 
			{
	            component.enabled = true;
	        }
		}
		else
		{
	        // Disable rendering:
	        foreach (Renderer component in rendererComponents) 
			{
	            component.enabled = false;
			}
		}
	}
}