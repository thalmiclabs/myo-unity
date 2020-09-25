using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thalmic.Myo
{
    public enum Pose
    {
        Rest = libmyo.PoseType.Rest,
        Fist = libmyo.PoseType.Fist,
        WaveIn = libmyo.PoseType.WaveIn,
        WaveOut = libmyo.PoseType.WaveOut,
        FingersSpread = libmyo.PoseType.FingersSpread,
        DoubleTap = libmyo.PoseType.DoubleTap,
        Unknown = libmyo.PoseType.Unknown
    }
}
