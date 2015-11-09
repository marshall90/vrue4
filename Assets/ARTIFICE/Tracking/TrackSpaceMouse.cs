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
using System;

/// <summary>
/// Provides tracking data received from SpaceMouse (Human Interface Device) to Unity3D game objects. 
/// Tracking data (translation, rotation) is converted to Unity data format. 
/// 
/// Inherits from abstract class TrackProvider. 
/// </summary>
public class TrackSpaceMouse : TrackProvider
{
	private byte _button;
	private byte _buttonPrev;
	 
	private bool _is1stPersonMirrowActive; // use this if you want to run SpaceMouse in MarkerSetup from 1stPersonView
	private bool _is1stPersonNonMirrowActive; // use this if you want to run SpaceMouse in NonMarkerSetup (Only SpaceMouse/kinect) from 1stPersonView
	private Quaternion _compensateOTRotation;
	private Vector3 _zeroValues;
	
	private Quaternion _savedRotation;

	private bool _waitToSaveOrientation;
	private bool _translateIsActive;
	
	private byte _button1State;
	private byte _button2State;
	private byte _button3State;
	
	/// <summary>
	/// Get number of current pressed button:
		 /// 0: no button is pressed
		 /// 1: left button is pressed
		 /// 2: right button is pressed
		 /// 3: left and right button are pressed simultaneously
	/// </summary>
	/// <returns>Button number (1, 2, 3, 0 = none button pressed)</returns>
	public byte button
    {
        get
        {
            return _button;
        }
    }
	
	/// <summary>
	/// Get state of button 1 (left button):
		 /// 0: not pressed
		 /// 1: pressed
		 /// 2: released
	/// </summary>	
	/// <returns>State of button 1</returns>
	public byte button1State
    {
        get
        {
            return _button1State;
        }
    }
	
	/// <summary>
	/// Get state of button 2 (right button):
		 /// 0: not pressed
		 /// 1: pressed
		 /// 2: released
	/// </summary>	
	/// <returns>State of button 2</returns>
	public byte button2State
    {
        get
        {
            return _button2State;
        }
    }
	
	/// <summary>
	/// Get state of button 3 (left+right button):
		 /// 0: not pressed
		 /// 1: pressed
		 /// 2: released
	/// </summary>
	/// <returns>State of button 3</returns>
	public byte button3State
    {
        get
        {
            return _button3State;
        }
    }
	
	/// <summary>
	/// </summary>
	void Start()
	{
		_waitToSaveOrientation = false;
		_translateIsActive = true;
		_savedRotation = Quaternion.identity;
		
		_is1stPersonMirrowActive = false;
		_is1stPersonNonMirrowActive = true;
		
		// open tracker data is rotated -> compensate for correct calculation of complete rotation
		if (_is1stPersonMirrowActive)
		{
			_compensateOTRotation = Quaternion.Euler(270.0f, 180.0f, 0.0f);
			_zeroValues = new Vector3(0.0f, 0.0f, 0.0f);
			
		}
		else if (_is1stPersonNonMirrowActive)
		{
			_compensateOTRotation = Quaternion.Euler(270.0f, 180.0f, 180.0f); // TODO: find angles which compensate OT rotation to 0/0/0 -> with this rotated to 360/0/0
			_zeroValues = new Vector3(360.0f, 0.0f, 0.0f);
		}
	}

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
		
		if(tracker == null)
		{
			if(!Manager.useDedicatedServer || (Manager.useDedicatedServer && Network.isClient)) 
			{
		        // generate tracker object for space mouse
		        generateTracker(deviceName);
		        gameObject.transform.hideFlags |= HideFlags.NotEditable;
			}
		}
		else
		{
			try
	        {   
				// get current tracking event
	            TrackingEvent ev = updateTracking();
				
				// get button value
				_button = (byte)(ev.getButton() & 0xFF);
				setButtonStates();

				Vector3 position;
				Quaternion orientation;
				
				transformOpenTrackerToUnity3D(ev, out position, out orientation);
				
				// perform transform 
				handlePosition(position);
				handleOrientation(orientation);	
				
	        } catch (System.Exception ex) 
	        {
				Console.WriteLine("{0} Detailed Exception:", ex);
	        }
		}
    }
	
	/// <summary>
	/// Handles translation data (Continously sum up translation) from space mouse and sends 
	/// it to unity transform node if transation is set to active. 
	/// When performing a rotation with the space mouse translation can be deactivated by clicking button 2 
	/// which offers better usability for rotating and storing current rotation. 
	/// </summary>
	/// <param name="position">Position of current translation</param>
	private void handlePosition(Vector3 position)
	{
		
		// if button 2 has been released -> (de)active translation
		if (button2State == 2) 
		{
			
			if(_translateIsActive)
			{
				_translateIsActive = false;
				Debug.Log("Translate deactivated");
			}
			else
			{
				_translateIsActive = true;
				Debug.Log("Translate activated");
			}
		}
		
		if (_translateIsActive)
		{
			// sum up position values
			position = (position * base.scalePosition) + gameObject.transform.localPosition;
			transform.localPosition = position;
		}
	}
	
	/// <summary>
	/// Handles rotation data from space mouse and sends it to unity transform node. 
	/// 
	/// User can save current rotation (so next rotation will be summed up) by clicking button 1. 
	/// User can set rotation to default (= no rotation) by clicking button 3. 
	/// </summary>
	/// <param name="orientation">Quaternion of current rotation of the spacemouse</param>
	private void handleOrientation(Quaternion orientation)
	{
		// compensate rotated SpaceMouse CS by OpenTracker -> see config.xml
		orientation = orientation* Quaternion.Inverse(_compensateOTRotation);
		// if button 1 has been released -> store curr rotation and wait until mouse has been moved back to init position
		if (button1State == 2 && !_waitToSaveOrientation)
		{
			// store & set current orientation
			_savedRotation = orientation* _savedRotation; 
			transform.localRotation = _savedRotation; 
			
			_waitToSaveOrientation = true;
		}
		
		// if button 3 has been released -> set back to init rotation 
		if (button3State == 2)
		{
			_savedRotation = Quaternion.identity;
			_waitToSaveOrientation = false;
			
			Debug.Log("Set orientation to default");
		}
		

		
		
		if (!_waitToSaveOrientation)
		{
			transform.localRotation =  orientation* _savedRotation; 
		} 
		else  // hold current rot position
		{
			Debug.Log("wait to save");
			// compensate jitter of SpaceMouse in default position
			//if ( Math.Floor(orientation.eulerAngles.x) == _zeroValues.x && Math.Ceiling(orientation.eulerAngles.y) == _zeroValues.y && Math.Ceiling(orientation.eulerAngles.z) == _zeroValues.z)
			if ( ((Math.Floor(orientation.eulerAngles.x) == _zeroValues.x) || (Math.Ceiling(orientation.eulerAngles.x) == _zeroValues.x )) 
			    && ((Math.Floor(orientation.eulerAngles.y) == _zeroValues.y) || (Math.Ceiling(orientation.eulerAngles.y) == _zeroValues.y )) 
			    && ((Math.Floor(orientation.eulerAngles.z) == _zeroValues.z) || (Math.Ceiling(orientation.eulerAngles.z) == _zeroValues.z )) )
			{
				_waitToSaveOrientation = false;
			}
		}
	}
	
	/// <summary>
	/// Set current button states depending on user interaction.
	/// </summary>
	private void setButtonStates()
	{
		// check if any button is pressed and set button state
		if (_button != 0 && _buttonPrev == 0)
		{
			_buttonPrev = _button;
			
			if (_button == 1)
			{
				_button1State = 1;
			}
			else if (_button == 2)
			{
				_button2State = 1;
			}
			else if (_button == 3)
			{
				_button3State = 1;
			}
		} 
		// check if any button is released and set button state
		else if (_button == 0 && _buttonPrev != 0)
		{
			if( _buttonPrev == 1)
			{
				_button1State = 2;
			}
			else if ( _buttonPrev == 2)
			{
				_button2State = 2;
			}
			else if ( _buttonPrev == 3)
			{
				_button3State = 2;
			}
		
			_buttonPrev = 0;
		} 
		else 
		{
			_button1State = 0;
			_button2State = 0;
			_button3State = 0;
		}
	}
	
}