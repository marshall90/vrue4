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
/// Provides tracking data received from ARToolkit marker as part of a marker cube to Unity3D game object. 
/// Inherits from class TrackMarker. 
/// </summary>
public class TrackMMPart : TrackMarker 
{
	protected double _timeLastMessage=double.NegativeInfinity;
	public Vector3 translationToMM=Vector3.zero;
	public Quaternion rotationToMM=Quaternion.identity;
	
	private OneMarkerUpdate upd=new OneMarkerUpdate();
	
	/// <summary>
	/// Get updated Marker Update. 
	/// </summary>
	/// <returns>OneMarkerUpdate upd</returns>
	public OneMarkerUpdate getUpdate()
	{
		return upd;
	}
	
	/// <summary>
    /// Use Start() for initialization. Overrides the inherited base method. 
    /// </summary>
	public override void Start()
	{
		base.Start();
		if(upd!=null)
		{
			upd=new OneMarkerUpdate();
		}	
	}
	
	/// <summary>
    /// Update is called once per frame. Overrides the inherited base method. 
    /// </summary>
	public override void Update()
	{
		base.Update();
		
		if(_timeLastMessage<trackingTimestamp)
		{
			_timeLastMessage=trackingTimestamp;
			Vector3 vec1=new Vector3(1.0f,1.0f,1.0f);
			Matrix4x4 m_rotatationToMM=Matrix4x4.TRS(Vector3.zero,rotationToMM,vec1);
			Matrix4x4 m_translationToMM=Matrix4x4.TRS(translationToMM,Quaternion.identity,vec1);
			Matrix4x4 m_localRot=Matrix4x4.TRS(Vector3.zero,gameObject.transform.localRotation,vec1);
			Matrix4x4 m_localPos=Matrix4x4.TRS(gameObject.transform.localPosition,Quaternion.identity,vec1);
			
			upd.id=deviceName;
			upd.timestamp=trackingTimestamp;
			Matrix4x4 trans=m_localPos*m_localRot*m_translationToMM*m_rotatationToMM;
			upd.position=Helper.TranslationFromMatrix(trans);
			upd.rotation=Helper.QuaternionFromMatrix(trans);
		}
	}
}

/// <summary>
/// Encapsulates tracking data of one marker including the timestamp
/// </summary>
public class OneMarkerUpdate
{
	public string id="empty";
	public double timestamp=double.NegativeInfinity;
	public Vector3 position=Vector3.zero;
	public Quaternion rotation=Quaternion.identity;
	
	public OneMarkerUpdate()
	{}
}

/// <summary>
/// Providing methods useful for conversion between different transformation representation
/// i.e. Matrix to Quaternion and Matrix to Vector3 translation
/// </summary>
public class Helper
{
	/// <summary>
	/// Returns the rotation contained in the 4x4 Matrix as a Quaternion
	/// </summary>
	/// <param name="m">
	/// 4x4 rotation matrix <see cref="Matrix4x4"/>
	/// </param>
	/// <returns>
	/// Quaternion containing the rotation <see cref="Quaternion"/>
	/// </returns>
	public static Quaternion QuaternionFromMatrix(Matrix4x4 m) 
	{
	        Quaternion q = new Quaternion();
	        q.w = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] + m[1,1] + m[2,2] ) ) / 2; 
	        q.x = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] - m[1,1] - m[2,2] ) ) / 2; 
	        q.y = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] + m[1,1] - m[2,2] ) ) / 2; 
	        q.z = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] - m[1,1] + m[2,2] ) ) / 2; 
	        q.x *= Mathf.Sign( q.x * ( m[2,1] - m[1,2] ) );
	        q.y *= Mathf.Sign( q.y * ( m[0,2] - m[2,0] ) );
	        q.z *= Mathf.Sign( q.z * ( m[1,0] - m[0,1] ) );
	        return q;
	}
	
	/// <summary>
	/// Vector3 translation from the 4x4 matrix
	/// </summary>
	/// <param name="m">
	/// A 4x4 transformation matrix <see cref="Matrix4x4"/>
	/// </param>
	/// <returns>
	/// A translation vector <see cref="Vector3"/>
	/// </returns>
	public static Vector3 TranslationFromMatrix(Matrix4x4 m)
	{
		Vector4 pos = m.GetColumn(3);
		Vector3 returnPos=new Vector3(pos.x,pos.y,pos.z);
		return returnPos;
	}
}