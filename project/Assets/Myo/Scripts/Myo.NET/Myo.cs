using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Thalmic.Myo
{
    public class Myo
    {
        private readonly Hub _hub;
        private IntPtr _handle;

        internal Myo(Hub hub, IntPtr handle)
        {
            Debug.Assert(handle != IntPtr.Zero, "Cannot construct Myo instance with null pointer.");

            _hub = hub;
            _handle = handle;
        }

        public event EventHandler<MyoEventArgs> Connected;

        public event EventHandler<MyoEventArgs> Disconnected;

        public event EventHandler<ArmSyncedEventArgs> ArmSynced;

        public event EventHandler<MyoEventArgs> ArmUnsynced;

        public event EventHandler<PoseEventArgs> PoseChange;

        public event EventHandler<OrientationDataEventArgs> OrientationData;

        public event EventHandler<AccelerometerDataEventArgs> AccelerometerData;

        public event EventHandler<GyroscopeDataEventArgs> GyroscopeData;

        public event EventHandler<RssiEventArgs> Rssi;

        public event EventHandler<MyoEventArgs> Unlocked;

        public event EventHandler<MyoEventArgs> Locked;

		public event EventHandler<EmgEventArgs> Emg;

        internal Hub Hub
        {
            get { return _hub; }
        }

        internal IntPtr Handle
        {
            get { return _handle; }
        }


#if UNITY_EDITOR || !UNITY_IOS


		public void SetEmgState(EmgState emgState)
		{
			libmyo.set_stream_eng (_handle, (libmyo.EmgState)emgState, IntPtr.Zero);
		}

        public void Vibrate(VibrationType type)
        {
            libmyo.vibrate(_handle, (libmyo.VibrationType)type, IntPtr.Zero);
        }

        public void RequestRssi()
        {
            libmyo.request_rssi(_handle, IntPtr.Zero);
        }

        public void Unlock(UnlockType type)
        {
            libmyo.myo_unlock(_handle, (libmyo.UnlockType)type, IntPtr.Zero);
        }

        public void Lock()
        {
            libmyo.myo_lock(_handle, IntPtr.Zero);
        }

        public void NotifyUserAction()
        {
            libmyo.myo_notify_user_action(_handle, libmyo.UserActionType.Single, IntPtr.Zero);
        }

        internal void HandleEvent(libmyo.EventType type, DateTime timestamp, IntPtr evt)
        {
            switch (type)
            {
                case libmyo.EventType.Connected:
                    if (Connected != null)
                    {
                        Connected(this, new MyoEventArgs(this, timestamp));
                    }
                    break;

                case libmyo.EventType.Disconnected:
                    if (Disconnected != null)
                    {
                        Disconnected(this, new MyoEventArgs(this, timestamp));
                    }
                    break;

                case libmyo.EventType.ArmSynced:
                    if (ArmSynced != null)
                    {
                        Arm arm = (Arm)libmyo.event_get_arm(evt);
                        XDirection xDirection = (XDirection)libmyo.event_get_x_direction(evt);

                        ArmSynced(this, new ArmSyncedEventArgs(this, timestamp, arm, xDirection));
                    }
                    break;

                case libmyo.EventType.ArmUnsynced:
                    if (ArmUnsynced != null)
                    {
                        ArmUnsynced(this, new MyoEventArgs(this, timestamp));
                    }
                    break;

                case libmyo.EventType.Orientation:
                    if (AccelerometerData != null)
                    {
                        float x = libmyo.event_get_accelerometer(evt, 0);
                        float y = libmyo.event_get_accelerometer(evt, 1);
                        float z = libmyo.event_get_accelerometer(evt, 2);

                        var accelerometer = new Vector3(x, y, z);
                        AccelerometerData(this, new AccelerometerDataEventArgs(this, timestamp, accelerometer));
                    }
                    if (GyroscopeData != null)
                    {
                        float x = libmyo.event_get_gyroscope(evt, 0);
                        float y = libmyo.event_get_gyroscope(evt, 1);
                        float z = libmyo.event_get_gyroscope(evt, 2);

                        var gyroscope = new Vector3(x, y, z);
                        GyroscopeData(this, new GyroscopeDataEventArgs(this, timestamp, gyroscope));
                    }
                    if (OrientationData != null)
                    {
                        float x = libmyo.event_get_orientation(evt, libmyo.OrientationIndex.X);
                        float y = libmyo.event_get_orientation(evt, libmyo.OrientationIndex.Y);
                        float z = libmyo.event_get_orientation(evt, libmyo.OrientationIndex.Z);
                        float w = libmyo.event_get_orientation(evt, libmyo.OrientationIndex.W);

                        var orientation = new Quaternion(x, y, z, w);
                        OrientationData(this, new OrientationDataEventArgs(this, timestamp, orientation));
                    }
                    break;

                case libmyo.EventType.Pose:
                    if (PoseChange != null)
                    {
                        var pose = (Pose)libmyo.event_get_pose(evt);
                        PoseChange(this, new PoseEventArgs(this, timestamp, pose));
                    }
                    break;

                case libmyo.EventType.Rssi:
                    if (Rssi != null)
                    {
                        var rssi = libmyo.event_get_rssi(evt);
                        Rssi(this, new RssiEventArgs(this, timestamp, rssi));
                    }
                    break;
                case libmyo.EventType.Unlocked:
                    if (Unlocked != null)
                    {
                        Unlocked(this, new MyoEventArgs(this, timestamp));
                    }
                    break;
                case libmyo.EventType.Locked:
                    if (Locked != null)
                    {
                        Locked(this, new MyoEventArgs(this, timestamp));
                    }
                    break;

				case libmyo.EventType.Emg:
					
					Dictionary<int, sbyte> emgData = new Dictionary<int, sbyte>();
					for(int i = 0; i < 8;i++)
					{
						emgData[i] = libmyo.event_get_emg(evt, (uint)i);
					}

					if (Emg != null)
					{
						Emg(this, new EmgEventArgs(this, timestamp, emgData));
					}

					
					break;
            }
        }
#endif
    }

    public enum Arm
    {
        Right,
        Left,
        Unknown
    }

    public enum XDirection
    {
        TowardWrist,
        TowardElbow,
        Unknown
    }

    public enum VibrationType
    {
        Short,
        Medium,
        Long
    }

	public enum EmgState
	{
		Disabled,
		Enabled
	}

    public enum UnlockType
    {
        Timed = 0,  ///< Unlock for a fixed period of time.
        Hold = 1    ///< Unlock until explicitly told to re-lock.
    }
}
