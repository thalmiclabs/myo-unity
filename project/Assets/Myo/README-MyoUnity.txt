Thalmic Labs Myo Bindings for Unity
===================================

GETTING STARTED
===============

The easiest way to get started is to open a sample scene from the "Myo Samples"
folder.

Alternatively, you can drag one of the prefabs from "Myo/Prefabs" into your
scene. Prefabs are provided for the case of a single Myo armband (most common)
and more than one Myo armband (less common).

MYO'S REPRESENTATION IN UNITY
=============================

The Myo armband is represented in Unity by a GameObject with the ThalmicMyo.cs script
attached. Such a GameObject must be nested under another GameObject with the
ThalmicHub script attached, which is responsible for discovering Myo armbands.

The ThalmicHub script is a singleton; if you create more than a single
GameObject with a ThalmicHub attached, all but one will be destroyed. The
single instance of ThalmicHub can be obtained using the ThalmicHub.instance
property. The ThalmicHub GameObject and its children will not be destroyed when
a scene change occurs.

The ThalmicMyo script provides several properties relating to the Myo armband's
current state, including:
 - armSynced - whether or not the Myo armband has detected that it is on an arm.
 - arm - the arm that the Myo armband is on, e.g. Arm.Left if being worn on the left arm.
 - xDirection - the current direction of the Myo armband's +x axis relative to the arm.
 - pose - the current pose detected by the Myo armband, e.g. Pose.Fist if the wearer
   is making a fist.
 - transform - the Myo game object's local transformation corresponds to its
   world space orientation following Unity's coordinate system conventions.
 - accelerometer - a vector representing the acceleration due to the force being applied
   to the Myo armband, in units of g (about 9.8 m/s^2), relative to the game object's
   local transformation. A Myo armband at rest with its logo facing upward will thus
   yield an accelerometer value of (0, 1, 0), corresponding to up in Unity's space.
 - gyroscope - a vector representing the angular velocity of Myo about its axes
   in degrees per second.

Typically you will want to turn the Myo armband's orientation into an orientation
corresponding to the user's arm. Doing so requires a reference orientation
to be established corresponding to the user pointing their arm forward. Look
at the "Box On A Stick" sample, and in particular the "JointOrientation.cs"
script for a well-commented example of how to do this.

The "Box On A Stick" sample scene also illustrates how to vibrate the Myo armband
from Unity.

RESETTING THE HUB
=================

ThalmicHub exposes the function ResetHub() which allows you to reset the
hub in case it did not initialize properly. The hub will fail to initialize if
Myo Connect is not running.

THE THALMIC.MYO NAMESPACE
=========================

The scripts in Myo/Scripts/Myo.NET/ provide non-Unity-specific .NET bindings
for the Myo armband. Their contents are all in the Thalmic.Myo namespace.

The two types from Thalmic.Myo you are most likely to need are the Pose and
VibrationType enums. To access these conveniently in C# code, add using
declarations like so:

    using Pose = Thalmic.Myo.Pose;
    using VibrationType = Thalmic.Myo.VibrationType

We do not recommend using "import Thalmic.Myo" in your code, as Thalmic.Myo
contains some types (like Vector3) that have the same names as Unity built
in types.

You typically should not have to use anything else from these scripts directly,
as ThalmicHub and ThalmicMyo will take care of the low level details for you.
