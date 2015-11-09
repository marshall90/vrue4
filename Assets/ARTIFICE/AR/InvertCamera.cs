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

[RequireComponent (typeof(Camera))]

/// <summary>
/// Inverts/flips the camera image along the x-axes. Functionality is used for first person view interaction techniques
/// so that the virtual cam image equals the web cam image. 
/// </summary>
public class InvertCamera : MonoBehaviour
{
	private readonly Matrix4x4 inversion = Matrix4x4.Scale(new Vector3(-1.0f,1.0f,1.0f));

	public void OnPreCull()
	{
		this.camera.ResetWorldToCameraMatrix();
		this.camera.ResetProjectionMatrix();
		this.camera.projectionMatrix=this.camera.projectionMatrix*inversion;
	}
	
	public void OnPreRender()
	{
		GL.SetRevertBackfacing(true);
	}
	
	public void OnPostRender()
	{
		GL.SetRevertBackfacing(false);
	}
}
