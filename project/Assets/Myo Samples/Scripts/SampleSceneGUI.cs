using UnityEngine;
using System.Collections;

// Draw simple instructions for sample scene.
// Check to see if a Myo armband is paired.
using System.Collections.Generic;


public class SampleSceneGUI : MonoBehaviour
{
    // Myo game object to connect with.
    // This object must have a ThalmicMyo script attached.
    public ThalmicMyo thalmicMyo;

	int numConnectionsAllowed = 1;

    // Draw some basic instructions.
    void OnGUI ()
    {
		int fontSize = (int)(Screen.height * 0.03f);
		int smallFontSize = fontSize / 2;
		GUI.skin.button.fontSize = fontSize;
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.textField.fontSize = fontSize;

		GUILayout.BeginArea(new Rect(0,0,Screen.width, Screen.height));

		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			if (GUILayout.Button ("Show Myo Connection Screen")) {
				MyoIOSManager.ShowSettings ();
			}
		}
		GUILayout.BeginHorizontal();
		
		string numConnectionsString = GUILayout.TextField(""+numConnectionsAllowed);
		if (!int.TryParse(numConnectionsString, out numConnectionsAllowed))
		{
			numConnectionsAllowed = 1;
		}
		if (numConnectionsAllowed < 0)
		{
			numConnectionsAllowed = 1;
		}

		if (GUILayout.Button("Set # of Connections Allowed"))
		{
			MyoIOSManager.ConnectionAllowance = numConnectionsAllowed;
		}
		
		GUILayout.EndHorizontal();

		if (GUILayout.Button("Enable EMG Streaming"))
		{
			thalmicMyo.SetEmgState(Thalmic.Myo.EmgState.Enabled);
			//MyoIOSManager
		}
	
		bool initialized = false;

		if (Application.platform == RuntimePlatform.IPhonePlayer) {

			initialized = true;

		} else {
			initialized = ThalmicHub.instance.hubInitialized;

			if (!initialized)
			{
				GUILayout.Label (
					"Cannot contact Myo Connect. Is Myo Connect running?\n" +
					"Press Q to try again."
					);
			}
		}
		if (initialized)
		{
			 if (!thalmicMyo.isPaired) {
				GUILayout.Label (
	            "No Myo currently paired."
				);
			} else 
			{
				if (!thalmicMyo.armSynced) {
					GUILayout.Label (
		            "Perform the Sync Gesture."
					);
				} else {
					//Show the myo pose commands if the myo has been synced
					GUILayout.Label (
		            "Fist: Vibrate Myo armband\n" +
						"Wave in: Set box material to blue\n" +
						"Wave out: Set box material to green\n" +
						"Double tap: Reset box material\n" +
						"Fingers spread: Set forward direction"
					);
				}

				GUILayout.Label("Accelerometer");
				GUI.skin.label.fontSize = smallFontSize;
				GUILayout.Label(string.Format("x: {0}, y: {1}, z: {2}", thalmicMyo.accelerometer.x, thalmicMyo.accelerometer.y, thalmicMyo.accelerometer.z));
				GUI.skin.label.fontSize = fontSize;
				GUILayout.Label("Gyroscope");
				GUI.skin.label.fontSize = smallFontSize;
				GUILayout.Label(string.Format("x: {0}, y: {1}, z: {2}", thalmicMyo.gyroscope.x, thalmicMyo.gyroscope.y, thalmicMyo.gyroscope.z));
				GUI.skin.label.fontSize = fontSize;

				//GUILayout.Label("Emg");
				
				//GUILayout.Label(string.Format("x: {0}, y: {1}, z: {2}", thalmicMyo.gyroscope.x, thalmicMyo.gyroscope.y, thalmicMyo.gyroscope.z));


			}



		}
		GUILayout.EndArea();
    }

	MyoIOSManager iosManager = null;

    void Update ()
    {
        ThalmicHub hub = ThalmicHub.instance;

        if (Input.GetKeyDown ("q")) {
            hub.ResetHub();
        }


    }
}
