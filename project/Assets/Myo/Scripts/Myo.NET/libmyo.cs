using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Thalmic.Myo
{
    internal static class libmyo
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        private const string MYO_DLL = "myo";
#elif UNITY_ANDROID
        private const string MYO_DLL = "myo-android";
#elif WIN64
        private const string MYO_DLL = "myo64.dll";
#elif WIN32
        private const string MYO_DLL = "myo32.dll";
#endif

		public enum EventType
		{
			Paired,
			Unpaired,
			Connected,
			Disconnected,
			ArmSynced,
			ArmUnsynced,
			Orientation,
			Pose,
			Rssi,
			Unlocked,
			Locked,
			Emg,
			BatteryLevel,
			WarmupCompleted
		}

		public enum Result
		{
			Success,
			Error,
			ErrorInvalidArgument,
			ErrorRuntime
		}

		public enum VibrationType
		{
			Short,
			Medium,
			Long
		}

		public enum PoseType
		{
			Rest = 0,
			Fist = 1,
			WaveIn = 2,
			WaveOut = 3,
			FingersSpread = 4,
			DoubleTap = 5,
			Unknown = 0xffff
		}
		
		public enum UnlockType
		{
			Timed = 0,
			Hold = 1
		}

		public enum UserActionType
		{
			Single = 0
		}

		public enum LockingPolicy
		{
			None,
			Standard
		}

		public enum EmgState
		{
			Disabled,
			Enabled
		}

#if !UNITY_IPHONE || UNITY_EDITOR

		[DllImport(MYO_DLL,
		           EntryPoint = "libmyo_event_get_emg",
		           CallingConvention = CallingConvention.Cdecl)]
		public static extern sbyte event_get_emg(IntPtr evt, uint sensor);


		[DllImport(MYO_DLL,
		           EntryPoint = "libmyo_set_stream_emg",
		           CallingConvention = CallingConvention.Cdecl)]
		public static extern Result set_stream_eng(IntPtr myo, EmgState emg, IntPtr out_error);

	
        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_error_cstring",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern string error_cstring(IntPtr error);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_error_kind",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern Result error_kind(IntPtr error);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_free_error_details",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void free_error_details(IntPtr error);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_init_hub",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern Result init_hub(out IntPtr hub, string applicationIdentifier, IntPtr error);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_shutdown_hub",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern Result shutdown_hub(IntPtr hub, IntPtr error);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_set_locking_policy",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern Result set_locking_policy(IntPtr hub, LockingPolicy lockingPolicy, IntPtr error);

      

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_vibrate",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void vibrate(IntPtr myo, VibrationType type, IntPtr error);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_request_rssi",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void request_rssi(IntPtr myo, IntPtr error);

       

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_myo_unlock",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void myo_unlock(IntPtr myo, UnlockType unlockType, IntPtr error);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_myo_lock",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void myo_lock(IntPtr myo, IntPtr error);

        

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_myo_notify_user_action",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern void myo_notify_user_action(IntPtr myo, UserActionType type, IntPtr error);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_event_get_type",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern EventType event_get_type(IntPtr evt);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_event_get_timestamp",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt64 event_get_timestamp(IntPtr evt);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_event_get_myo",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr event_get_myo(IntPtr evt);

        public enum VersionComponent
        {
            Major,
            Minor,
            Patch
        }

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_event_get_firmware_version",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern uint event_get_firmware_version(IntPtr evt, VersionComponent component);

        public enum Arm {
            Right,
            Left,
            Unknown
        }

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_event_get_arm",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern Arm event_get_arm(IntPtr evt);

        public enum XDirection {
            TowardWrist,
            TowardElbow,
            Unknown
        }

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_event_get_x_direction",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern XDirection event_get_x_direction(IntPtr evt);

        public enum OrientationIndex
        {
            X = 0,
            Y = 1,
            Z = 2,
            W = 3
        }

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_event_get_orientation",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern float event_get_orientation(IntPtr evt, OrientationIndex index);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_event_get_accelerometer",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern float event_get_accelerometer(IntPtr evt, uint index);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_event_get_gyroscope",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern float event_get_gyroscope(IntPtr evt, uint index);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_event_get_pose",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern PoseType event_get_pose(IntPtr evt);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_event_get_rssi",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern sbyte event_get_rssi(IntPtr evt);

        public enum HandlerResult
        {
            Continue,
            Stop
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate HandlerResult Handler(IntPtr userData, IntPtr evt);

        [DllImport(MYO_DLL,
                   EntryPoint = "libmyo_run",
                   CallingConvention = CallingConvention.Cdecl)]
        public static extern Result run(IntPtr hub, uint durationMs, Handler handler, IntPtr userData, IntPtr error);
		#endif
    }

}
