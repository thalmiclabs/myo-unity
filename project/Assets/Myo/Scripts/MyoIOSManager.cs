using UnityEngine;
using System.Collections;

using Thalmic;
using Pose = Thalmic.Myo.Pose;
using Thalmic.MiniJSON;

/**
 * 
 * This class is automatically generated on the iOS platform by the Thalmic Hub instance. 
 * The manager will use the Myo iOS bindings to access the native Myo iOS SDK
 * 
 * */
using System.Collections.Generic;


public class MyoIOSManager : MonoBehaviour {

	#if UNITY_IPHONE

	void Start()
	{
		Debug.Log("Starting iOS Manager");
		MyoBindings.myo_SetApplicationID(ThalmicHub.instance.applicationIdentifier);
		MyoBindings.myo_SetLockingPolicy((int)ThalmicHub.instance.lockingPolicy);
	}

	#region Myo iOS Methods
	//IOS Specific methods
	public static void ShowSettings()
	{
		Debug.Log("Showing iOS Connection Settings");
	
		MyoBindings.myo_ShowSettings();
	}

	private static int _connectionAllowance = -1;
	public static int ConnectionAllowance
	{
		get 
		{ 	
			return _connectionAllowance < 0 ? MyoBindings.myo_MyoConnectionAllowance() : _connectionAllowance;
		}
		set
		{
			if (value >= 0)
			{
				_connectionAllowance = value;
				Debug.Log("Setting connection string allowance to be " + _connectionAllowance);
				MyoBindings.myo_SetMyoConnectionAllowance(_connectionAllowance);
			}
		}
	}
	
	#endregion

	#region Myo iOS Notifications
	void didConnectDevice()
	{
		//todo...determine which myo id this is...

		//if (ThalmicHub.DidConnectDevice != null)
		//	ThalmicHub.DidConnectDevice (PrimaryMyo);
	}
	
	void didDisconnectDevice()
	{
		//if (ThalmicHub.DidDisconnectDevice != null)
		//	ThalmicHub.DidDisconnectDevice (PrimaryMyo);
	}

	void didReceiveEmgEvent(string eventData)
	{
		if (eventData == null || eventData.Length == 0)
			return;

		var dict = Json.Deserialize (eventData) as Dictionary<string, object>;

		string myoIdentifier = (string)dict["myoIdentifier"];

		List<object> rawDataGeneric = (List<object>)dict ["rawData"];

		ThalmicMyo targetMyo = GetMyo(myoIdentifier);
		if (targetMyo != null)
		{
			for(int i = 0; i < rawDataGeneric.Count;i++){
				targetMyo.emg[i] = (sbyte)((long)( rawDataGeneric[i]));
			}
		}
	}
	
	void didUnlockDevice(string eventData)
	{
		if (eventData == null || eventData.Length == 0)
			return;

		var dict = Json.Deserialize(eventData) as Dictionary<string,object>;
	
		string myoIdentifier = (string) dict["myoIdentifier"]; 
		ThalmicMyo targetMyo = GetMyo(myoIdentifier);
		if (targetMyo != null)
		{
			if (ThalmicHub.DidUnlockDevice != null)
				ThalmicHub.DidUnlockDevice (targetMyo);
			targetMyo._myoUnlocked = true;
		}
	}
	
	void didLockDevice(string eventData)
	{	
		if (eventData == null || eventData.Length == 0)
			return;

		var dict = Json.Deserialize(eventData) as Dictionary<string,object>;

		string myoIdentifier = (string)dict["myoIdentifier"]; 
		ThalmicMyo targetMyo = GetMyo(myoIdentifier);
		
		if (targetMyo != null)
		{
			if (ThalmicHub.DidUnlockDevice != null)
				ThalmicHub.DidUnlockDevice (targetMyo);
			targetMyo._myoUnlocked = false;
		}
	}
	
	void didSyncArmEvent(string eventData)
	{
		if (eventData == null || eventData.Length == 0)
			return;

		var dict = Json.Deserialize(eventData) as Dictionary<string,object>;

		string myoIdentifier = (string) dict["myoIdentifier"]; 
		ThalmicMyo targetMyo = GetMyo(myoIdentifier);
		int armXDirection = (int)((long) dict["armXDirection"]);
		
		if (targetMyo != null)
		{
			if (ThalmicHub.DidSyncArm != null)
				ThalmicHub.DidSyncArm (targetMyo);

			targetMyo._myoXDirection = (Thalmic.Myo.XDirection) armXDirection;
			targetMyo._myoArmSynced = true;
		}
	}
	
	void didUnsyncArmEvent(string eventData)
	{
		if (eventData == null || eventData.Length == 0)
			return;
		
		var dict = Json.Deserialize(eventData) as Dictionary<string,object>;
		string myoIdentifier = (string)dict["myoIdentifier"]; 
		ThalmicMyo targetMyo = GetMyo(myoIdentifier);
		int armXDirection = (int)((long)(dict["armXDirection"]));
		
		if (targetMyo != null)
		{
			if (ThalmicHub.DidUnsyncArm != null)
				ThalmicHub.DidUnsyncArm (targetMyo);
			
			targetMyo._myoArmSynced = false;
		}
	}


	Thalmic.Myo.Vector3 jsonDictToVector3(Dictionary<string, object> dict)
	{
		if (dict != null && dict["x"] is double && dict["y"] is double && dict["z"] is double)
		{
			return new Thalmic.Myo.Vector3((float)((double)dict["x"]), (float)((double) dict["y"]),(float)( (double)dict["z"]));
		}
		return new Thalmic.Myo.Vector3 ();
	}

	Thalmic.Myo.Quaternion jsonDictToQuaternion(Dictionary<string, object> dict)
	{
		return new Thalmic.Myo.Quaternion((float)((double)dict["x"]), (float)((double) dict["y"]),(float)( (double)dict["z"]),(float)( (double)dict["w"]));
	}
	
	public void didReceiveGyroscopeEvent(string eventData)
	{
		if (eventData == null || eventData.Length == 0)
			return;

		var dict = Json.Deserialize(eventData) as Dictionary<string,object>;

		var vectorNode = dict["vector"] as Dictionary<string, object>;
		string myoIdentifier = (string)dict["myoIdentifier"]; 
		ThalmicMyo targetMyo = GetMyo(myoIdentifier);
		
		if (targetMyo != null)
		{
			targetMyo._myoGyroscope = jsonDictToVector3(vectorNode);
			if (ThalmicHub.DidUpdateAccelerometerData != null) {
				//ThalmicHub.DidUpdateAccelerometerData(targetMyo, acceleration);
			}
		}
	}




	void didReceiveOrientationEvent(string eventData)
	{
		
		if (eventData == null || eventData.Length == 0)
			return;
		//deserialize the jsonified orientation event
		var dict = Json.Deserialize(eventData) as Dictionary<string,object>;
		
		string myoIdentifier = (string)dict["myoIdentifier"];
		ThalmicMyo targetMyo = GetMyo(myoIdentifier);
		
		var quaternionDict = dict["quaternion"] as Dictionary<string, object>;

		if (targetMyo != null)
		{
			targetMyo._myoQuaternion = jsonDictToQuaternion(quaternionDict);
		}
	}

	void didReceiveAccelerometerEvent(string eventData)
	{
		if (eventData == null || eventData.Length == 0)
			return;
		
		var dict = Json.Deserialize(eventData) as Dictionary<string,object>;
		var vectorNode = dict["acceleration"] as Dictionary<string, object>;
		string myoIdentifier = (string)dict["myoIdentifier"]; 
		ThalmicMyo targetMyo = GetMyo(myoIdentifier);

		if (targetMyo != null)
		{
			targetMyo._myoAccelerometer = jsonDictToVector3(vectorNode);
			if (ThalmicHub.DidUpdateAccelerometerData != null) {

			}
		}
	}
	
	void didReceivePoseChange(string param)
	{
		Pose receivedPose = Pose.Unknown;

		for (int i = (int) Pose.Rest; i <= (int) Pose.Unknown; i++) {
			Pose currPose = (Pose) i;
			if (currPose.ToString().Equals(param))
			{
				receivedPose = currPose;
				break;
			}
		}
		//Update all myos for now because we don't have the myo id
		foreach(ThalmicMyo myo in ThalmicHub.instance._myos)
		{
			if (ThalmicHub.DidReceivePoseChange != null) {
				ThalmicHub.DidReceivePoseChange(null, receivedPose);
			}
			myo._myoPose = receivedPose;
		}
	}
	#endregion
	
	#region Helper Methods
	ThalmicMyo GetMyo(string myoId)
	{
		if ( ThalmicHub.instance == null || ThalmicHub.instance._myos == null) return null;

		if (myoId == null || myoId.Length <= 0) return null;

		foreach(ThalmicMyo myo in ThalmicHub.instance._myos)
		{
			if (myo.identifier.Equals(myoId))
			{
				return myo;
			}
		}

		foreach(ThalmicMyo myo in  ThalmicHub.instance._myos)
		{
			if (myo.identifier == null || myo.identifier.Length == 0)
			{
				myo.identifier = myoId;
				return myo;
			}
		}

		return null;;
	}
	#endregion

	#endif

}