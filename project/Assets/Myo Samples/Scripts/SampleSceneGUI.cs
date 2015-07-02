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
	
		ThalmicHub hub = ThalmicHub.instance;

		if (!hub.hubInitialized) {
			GUILayout.Label (
            "Cannot contact Myo Connect. Is Myo Connect running?\n" +
				"Press Q to try again."
			);
		} else if (!thalmicMyo.isPaired) {
			GUILayout.Label (
            "No Myo currently paired."
			);
		} else if (!thalmicMyo.armSynced) {
			GUILayout.Label (
            "Perform the Sync Gesture."
			);
		} else {
			GUILayout.Label (
            "Fist: Vibrate Myo armband\n" +
				"Wave in: Set box material to blue\n" +
				"Wave out: Set box material to green\n" +
				"Double tap: Reset box material\n" +
				"Fingers spread: Set forward direction"
			);
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
