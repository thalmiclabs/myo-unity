using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

namespace Thalmic
{
	using MiniJSON;
	
	public class MyoIOSDevice
	{
		public bool isConnected;
		public string name;
		public string identifier;
		public string state;
		public bool isLocked;
		public string poseType;
		public string poseTimestamp;
		public string pose;
		public Quaternion quaternion;
		public int arm;
		public int xdirection;
    }
    
    public class MyoBindings {

        #if UNITY_IOS
		[DllImport ("__Internal")]
		public static extern void myo_SetApplicationID(string appID);
		
		[DllImport ("__Internal")]
		public static extern bool myo_IsArmLocked();

		[DllImport ("__Internal")]
		public static extern void myo_ShowSettings();

		[DllImport ("__Internal")]
		public static extern string myo_GetMyos();

		[DllImport ("__Internal")]
		public static extern void myo_SetLockingPolicy(int policy);

		[DllImport ("__Internal")]
		public static extern void myo_SetShouldSendUsageData(bool value);

		[DllImport ("__Internal")]
		public static extern void myo_SetShouldNotifyInBackground(bool value);

		[DllImport ("__Internal")]
		public static extern bool myo_VibrateWithLength(string myoId, int length);

		[DllImport ("__Internal")]
		public static extern bool myo_SetStreamEmg(string myoId, int type);

		[DllImport ("__Internal")]
		public static extern bool myo_IndicateUserAction(string myoId);

		[DllImport ("__Internal")]
		public static extern bool myo_Lock(string myoId);

		[DllImport ("__Internal")]
		public static extern bool myo_UnlockWithType(string myoId, int type);

		[DllImport ("__Internal")]
		public static extern int myo_MyoConnectionAllowance();

		[DllImport ("__Internal")]
		public static extern void myo_SetMyoConnectionAllowance(int value);

		//This method deserializes the json retreived from the objective c bindings which will be used to update the thalmic myo objects in the unity scene
		public static List<MyoIOSDevice> GetMyos()
		{
			string myosJSON = myo_GetMyos ();
			Debug.Log("got myos" + myosJSON);
			if (myosJSON == null)
				return null;

			//JSONNode result = JSON.Parse(myosJSON);

			List<MyoIOSDevice> myos = new List<MyoIOSDevice>();
			return myos;
		}
	#endif
	}
}