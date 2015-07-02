﻿using UnityEngine;
using System.Collections;

using Arm = Thalmic.Myo.Arm;
using XDirection = Thalmic.Myo.XDirection;
using VibrationType = Thalmic.Myo.VibrationType;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using EmgState = Thalmic.Myo.EmgState;
// Represents a Myo armband. Myo's orientation is made available through transform.localRotation, and other properties
// like the current pose are provided explicitly below. All spatial data about Myo is provided following Unity
// coordinate system conventions (the y axis is up, the z axis is forward, and the coordinate system is left-handed).
using System.Collections.Generic;


public class ThalmicMyo : MonoBehaviour {

	public string identifier;

    // True if and only if Myo has detected that it is on an arm.
    public bool armSynced;

    // Returns true if and only if Myo is unlocked.
    public bool unlocked;

    // The current arm that Myo is being worn on. An arm of Unknown means that Myo is unable to detect the arm
    // (e.g. because it's not currently being worn).
    public Arm arm;

    // The current direction of Myo's +x axis relative to the user's arm. A xDirection of Unknown means that Myo is
    // unable to detect the direction (e.g. because it's not currently being worn).
    public XDirection xDirection;

    // The current pose detected by Myo. A pose of Unknown means that Myo is unable to detect the pose (e.g. because
    // it's not currently being worn).
    public Pose pose = Pose.Unknown;

    // Myo's current accelerometer reading, representing the acceleration due to force on the Myo armband in units of
    // g (roughly 9.8 m/s^2) and following Unity coordinate system conventions.
    public Vector3 accelerometer;

    // Myo's current gyroscope reading, representing the angular velocity about each of Myo's axes in degrees/second
    // following Unity coordinate system conventions.
    public Vector3 gyroscope;

	// Myo's current emg reading.  Key represents the sensor index
	public Dictionary<int, sbyte> emg;

    // True if and only if this Myo armband has paired successfully, at which point it will provide data and a
    // connection with it will be maintained when possible.
    public bool isPaired {
        get { return _myo != null || (identifier !=null && identifier.Length > 0); }
    }

    // Vibrate the Myo with the provided type of vibration, e.g. VibrationType.Short or VibrationType.Medium.
    public void Vibrate (VibrationType type) {

#if UNITY_EDITOR || !UNITY_IOS

        _myo.Vibrate (type);
#else
		Thalmic.MyoBindings.myo_VibrateWithLength(identifier, (int) type);
#endif
    }

	public void SetEmgState(EmgState emgState)
	{

#if UNITY_EDITOR
		_myo.SetEmgState (emgState);
#elif UNITY_IOS
		Thalmic.MyoBindings.myo_SetStreamEmg(identifier, (int)emgState);
#endif
	}

    // Cause the Myo to unlock with the provided type of unlock. e.g. UnlockType.Timed or UnlockType.Hold.
    public void Unlock (UnlockType type) {

		#if UNITY_EDITOR || !UNITY_IOS
		_myo.Unlock (type);
		#else
		Thalmic.MyoBindings.myo_UnlockWithType(identifier, (int) type);
		#endif
    }

    // Cause the Myo to re-lock immediately.
    public void Lock () {

		#if UNITY_EDITOR || !UNITY_IOS
		_myo.Lock ();
		#else
		Thalmic.MyoBindings.myo_Lock(identifier);
		#endif
    }

    /// Notify the Myo that a user action was recognized.
    public void NotifyUserAction () {

		#if UNITY_EDITOR || !UNITY_IOS
		
		_myo.NotifyUserAction ();
		
		#else
		Thalmic.MyoBindings.myo_IndicateUserAction(identifier);
		#endif
    }

    void Start() {

		identifier = "";

    }

    void Update() {
        lock (_lock) {
            armSynced = _myoArmSynced;
            arm = _myoArm;
            xDirection = _myoXDirection;
            if (_myoQuaternion != null) {
                transform.localRotation = new Quaternion(_myoQuaternion.Y, _myoQuaternion.Z, -_myoQuaternion.X, -_myoQuaternion.W);
            }
            if (_myoAccelerometer != null) {
                accelerometer = new Vector3(_myoAccelerometer.Y, _myoAccelerometer.Z, -_myoAccelerometer.X);
            }
            if (_myoGyroscope != null) {
                gyroscope = new Vector3(_myoGyroscope.Y, _myoGyroscope.Z, -_myoGyroscope.X);
            }
            pose = _myoPose;
            unlocked = _myoUnlocked;
        }
    }

    void myo_OnArmSync(object sender, Thalmic.Myo.ArmSyncedEventArgs e) {
        lock (_lock) {
            _myoArmSynced = true;
            _myoArm = e.Arm;
            _myoXDirection = e.XDirection;
        }
    }

    void myo_OnArmUnsync(object sender, Thalmic.Myo.MyoEventArgs e) {
        lock (_lock) {
            _myoArmSynced = false;
            _myoArm = Arm.Unknown;
            _myoXDirection = XDirection.Unknown;
        }
    }

    void myo_OnOrientationData(object sender, Thalmic.Myo.OrientationDataEventArgs e) {
        lock (_lock) {
            _myoQuaternion = e.Orientation;
        }
    }

    void myo_OnAccelerometerData(object sender, Thalmic.Myo.AccelerometerDataEventArgs e) {
        lock (_lock) {
            _myoAccelerometer = e.Accelerometer;
        }
    }

    void myo_OnGyroscopeData(object sender, Thalmic.Myo.GyroscopeDataEventArgs e) {
        lock (_lock) {
            _myoGyroscope = e.Gyroscope;
        }
    }

    void myo_OnPoseChange(object sender, Thalmic.Myo.PoseEventArgs e) {
        lock (_lock) {
            _myoPose = e.Pose;
        }
    }

    void myo_OnUnlock(object sender, Thalmic.Myo.MyoEventArgs e) {
        lock (_lock) {
            _myoUnlocked = true;
        }
    }

    void myo_OnLock(object sender, Thalmic.Myo.MyoEventArgs e) {
		lock (_lock) {
			_myoUnlocked = false;
		}
	}

	void myo_OnEmg(object sender, Thalmic.Myo.EmgEventArgs e){
		lock (_lock) {
			emg = e.Emg;
		}
	}

    public Thalmic.Myo.Myo internalMyo {
        get { return _myo; }
        set {
            if (_myo != null) {
                _myo.ArmSynced -= myo_OnArmSync;
                _myo.ArmUnsynced -= myo_OnArmUnsync;
                _myo.OrientationData -= myo_OnOrientationData;
                _myo.AccelerometerData -= myo_OnAccelerometerData;
                _myo.GyroscopeData -= myo_OnGyroscopeData;
                _myo.PoseChange -= myo_OnPoseChange;
                _myo.Unlocked -= myo_OnUnlock;
                _myo.Locked -= myo_OnLock;
				_myo.Emg -= myo_OnEmg;
            }
            _myo = value;
            if (value != null) {
                value.ArmSynced += myo_OnArmSync;
                value.ArmUnsynced += myo_OnArmUnsync;
                value.OrientationData += myo_OnOrientationData;
                value.AccelerometerData += myo_OnAccelerometerData;
                value.GyroscopeData += myo_OnGyroscopeData;
                value.PoseChange += myo_OnPoseChange;
                value.Unlocked += myo_OnUnlock;
                value.Locked += myo_OnLock;
				value.Emg += myo_OnEmg;
            }
        }
    }

    private Object _lock = new Object();

    public bool _myoArmSynced = false;
	public Arm _myoArm = Arm.Unknown;
	public XDirection _myoXDirection = XDirection.Unknown;
	public Thalmic.Myo.Quaternion _myoQuaternion = null;
	public Thalmic.Myo.Vector3 _myoAccelerometer = null;
	public Thalmic.Myo.Vector3 _myoGyroscope = null;
	public Pose _myoPose = Pose.Unknown;
	public bool _myoUnlocked = false;
    private Thalmic.Myo.Myo _myo;
}
