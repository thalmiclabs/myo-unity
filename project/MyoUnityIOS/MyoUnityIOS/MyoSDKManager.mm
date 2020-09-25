//
//  MyoUnitySDK
//
//  Created by Nelson Andre on 2015-04-24.
//  Copyright (c) 2015 Thalmic Labs. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MyoSDKManager.h"

#import <MyoKit/MyoKit.h>

#import <UIKit/UIKit.h>

@implementation MyoSDKManager

void UnityPause( bool pause );

extern "C" void UnitySendMessage( const char * className, const char * methodName, const char * param );

extern "C" UIViewController *UnityGetGLViewController();

#define MYO_UNITY_GAMEOBJECT "MyoIOSManager"

+ (MyoSDKManager*)sharedManager
{
    static MyoSDKManager *sharedManager = nil;
    
    if( !sharedManager )
        sharedManager = [[MyoSDKManager alloc] init];
        
        return sharedManager;
}

- (id)init
{
    if( ( self = [super init] ) )
    {
        [self subscribeToNotifications];
    }
    return self;
}

- (void) subscribeToNotifications {
    
    NSLog(@"Subscribing to Myo Notifications");
    
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(didConnectDevice:)
                                                 name:TLMHubDidConnectDeviceNotification
                                               object:nil];
    // Posted whenever a TLMMyo disconnects.
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(didDisconnectDevice:)
                                                 name:TLMHubDidDisconnectDeviceNotification
                                               object:nil];
    // Posted whenever the user does a successful Sync Gesture.
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(didSyncArm:)
                                                 name:TLMMyoDidReceiveArmSyncEventNotification
                                               object:nil];
    // Posted whenever Myo loses sync with an arm (when Myo is taken off, or moved enough on the user's arm).
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(didUnsyncArm:)
                                                 name:TLMMyoDidReceiveArmUnsyncEventNotification
                                               object:nil];
    // Posted whenever Myo is unlocked and the application uses TLMLockingPolicyStandard.
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(didUnlockDevice:)
                                                 name:TLMMyoDidReceiveUnlockEventNotification
                                               object:nil];
    // Posted whenever Myo is locked and the application uses TLMLockingPolicyStandard.
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(didLockDevice:)
                                                 name:TLMMyoDidReceiveLockEventNotification
                                               object:nil];
    // Posted when a new orientation event is available from a TLMMyo. Notifications are posted at a rate of 50 Hz.
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(didReceiveOrientationEvent:)
                                                 name:TLMMyoDidReceiveOrientationEventNotification
                                               object:nil];
    // Posted when a new accelerometer event is available from a TLMMyo. Notifications are posted at a rate of 50 Hz.
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(didReceiveAccelerometerEvent:)
                                                 name:TLMMyoDidReceiveAccelerometerEventNotification
                                               object:nil];
    
    // Posted when a new accelerometer event is available from a TLMMyo. Notifications are posted at a rate of 50 Hz.
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(didReceiveGyroscopeEvent:)
                                                 name:TLMMyoDidReceiveGyroscopeEventNotification
                                               object:nil];
    
    // Posted when a new pose is available from a TLMMyo.
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(didReceivePoseChange:)
                                                 name:TLMMyoDidReceivePoseChangedNotification
                                               object:nil];
    
    [[NSNotificationCenter defaultCenter] addObserver:  self
                                             selector:
                                                        @selector(didReceiveEmgEvent:)
                                                        name:TLMMyoDidReceiveEmgEventNotification
                                                        object:nil];
}

#pragma mark - NSNotificationCenter Methods
-(void) didReceiveEmgEvent:(NSNotification*) notification{
    
    TLMEmgEvent* event = notification.userInfo[kTLMKeyEMGEvent];
  
    NSMutableDictionary* eventDictionary = [NSMutableDictionary new];

    eventDictionary[@"myoIdentifier"] = [event.myo.identifier UUIDString];
    eventDictionary[@"rawData"] = event.rawData;
    
    UnitySendMessage( MYO_UNITY_GAMEOBJECT, "didReceiveEmgEvent", [[self dictToJSON:eventDictionary] UTF8String]  );
}



- (void)didConnectDevice:(NSNotification *)notification {
    
    TLMMyo* myo = notification.userInfo[kTLMKeyMyo];
    
    NSMutableDictionary* eventDictionary = [NSMutableDictionary new];
    eventDictionary[@"myoIdentifier"] = [myo.identifier UUIDString];
    
    self.status = CONNECTED;
    UnitySendMessage( MYO_UNITY_GAMEOBJECT, "didConnectDevice", [[self dictToJSON:eventDictionary] UTF8String]);
}

- (void)didDisconnectDevice:(NSNotification *)notification {
    self.status = WAITING;
    UnitySendMessage( MYO_UNITY_GAMEOBJECT, "didDisconnectDevice", "" );
}

- (void)didUnlockDevice:(NSNotification *)notification {
    
    TLMUnlockEvent *event = notification.userInfo[kTLMKeyUnlockEvent];
    
    NSDictionary* eventDictionary = @{@"myoIdentifier": [event.myo.identifier UUIDString],
                                      };
    
    UnitySendMessage( MYO_UNITY_GAMEOBJECT, "didUnlockDevice", [[self dictToJSON:eventDictionary] UTF8String] );
}

- (void)didLockDevice:(NSNotification *)notification {
    TLMLockEvent *event = notification.userInfo[kTLMKeyLockEvent];
    
    NSDictionary* eventDictionary = @{@"myoIdentifier": [event.myo.identifier UUIDString],
          
                                      };

    UnitySendMessage( MYO_UNITY_GAMEOBJECT, "didLockDevice", [[self dictToJSON:eventDictionary] UTF8String] );
}

- (void)didSyncArm:(NSNotification *)notification {

    
    TLMArmSyncEvent *event = notification.userInfo[kTLMKeyArmSyncEvent];
    
    NSDictionary* eventDictionary = @{@"myoIdentifier": [event.myo.identifier UUIDString],
                    
                                      @"armXDirection": [NSNumber numberWithInt:(int)event.xDirection]
                                      };

    UnitySendMessage( MYO_UNITY_GAMEOBJECT, "didSyncArm", [[self dictToJSON:eventDictionary] UTF8String]);
}

- (void)didUnsyncArm:(NSNotification *)notification {
   
    TLMArmUnsyncEvent* event = notification.userInfo[kTLMKeyArmUnsyncEvent];
    
    NSDictionary* eventDictionary = @{@"myoIdentifier": [event.myo.identifier UUIDString],
                                    
                                             };
    
    UnitySendMessage( MYO_UNITY_GAMEOBJECT, "didUnsyncArmEvent", [[self dictToJSON:eventDictionary] UTF8String] );
}

-(NSString*) dictToJSON: (NSDictionary*) dictionary
{
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dictionary
                                                       options:0
                                                         error:&error];
    
    if (!jsonData) return NULL;
    
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    
}


- (void) didReceiveGyroscopeEvent: (NSNotification*) notification {
    
    
    TLMGyroscopeEvent* event = notification.userInfo[kTLMKeyGyroscopeEvent];
    
    // Get the acceleration vector from the accelerometer event.
    TLMVector3 accelerationVector = event.vector;
    
    NSDictionary* eventDictionary = @{
                                      @"vector": @{@"x": [NSNumber numberWithFloat: accelerationVector.x],
                                                         @"y": [NSNumber numberWithFloat: accelerationVector.y],
                                                         @"z": [NSNumber numberWithFloat: accelerationVector.z]
                                                         },
                                      @"myoIdentifier":[event.myo.identifier UUIDString],
                                      
                                      };
    
    UnitySendMessage( MYO_UNITY_GAMEOBJECT, "didReceiveGyroscopeEvent", [[self dictToJSON:eventDictionary] UTF8String] );
}

- (void)didReceiveOrientationEvent:(NSNotification *)notification {
    // Retrieve the orientation from the NSNotification's userInfo with the kTLMKeyOrientationEvent key.
    TLMOrientationEvent *orientationEvent = notification.userInfo[kTLMKeyOrientationEvent];
    TLMQuaternion quaternion = orientationEvent.quaternion;
    
    NSDictionary* eventDictionary = @{
                                      @"quaternion": @{@"x": [NSNumber numberWithFloat: quaternion.x],
                                                      @"y": [NSNumber numberWithFloat: quaternion.y],
                                                      @"z": [NSNumber numberWithFloat: quaternion.z],
                                                      @"w": [NSNumber numberWithFloat: quaternion.w]
                                                      },
                                      @"myoIdentifier":[orientationEvent.myo.identifier UUIDString],
                                      
                                      };

    
    UnitySendMessage( MYO_UNITY_GAMEOBJECT, "didReceiveOrientationEvent", [[self dictToJSON:eventDictionary] UTF8String] );
}

- (void)didReceiveAccelerometerEvent:(NSNotification *)notification {

    //maybe want to have this be tied to the references to the accelerometer values...
    // Retrieve the accelerometer event from the NSNotification's userInfo with the kTLMKeyAccelerometerEvent.
     TLMAccelerometerEvent *accelerometerEvent = notification.userInfo[kTLMKeyAccelerometerEvent];
    
    // Get the acceleration vector from the accelerometer event.
    TLMVector3 accelerationVector = accelerometerEvent.vector;

    NSDictionary* eventDictionary = @{
                                      @"acceleration": @{@"x": [NSNumber numberWithFloat: accelerationVector.x],
                                                       @"y": [NSNumber numberWithFloat: accelerationVector.y],
                                                       @"z": [NSNumber numberWithFloat: accelerationVector.z]
                                                       },
                                      @"myoIdentifier":[accelerometerEvent.myo.identifier UUIDString],
                                      
                                      };
    
    UnitySendMessage( MYO_UNITY_GAMEOBJECT, "didReceiveAccelerometerEvent", [[self dictToJSON:eventDictionary] UTF8String] );
}

- (void)didReceivePoseChange:(NSNotification *)notification {
    
    TLMPose *pose = notification.userInfo[kTLMKeyPose];
    
    NSString* poseString = @"";
    
    switch (pose.type) {
        case TLMPoseTypeUnknown:
             poseString = @"Unknown";
            break;
            
        case TLMPoseTypeRest:
            poseString = @"Rest";
            break;
            
        case TLMPoseTypeDoubleTap:
            poseString = @"DoubleTap";
            break;
            
        case TLMPoseTypeFist:
            poseString = @"Fist";
            break;
            
        case TLMPoseTypeWaveIn:
            poseString = @"WaveOut";
            break;
            
        case TLMPoseTypeWaveOut:
            poseString = @"WaveOut";
            break;
        case TLMPoseTypeFingersSpread:
            poseString = @"FingersSpread";
            break;
    }

    const char* params = [poseString UTF8String];
    
    UnitySendMessage( MYO_UNITY_GAMEOBJECT, "didReceivePoseChange", params );
}

#pragma mark Public methods



//This method returns a json representation of the myo devices to unity as a string
-(NSString*) getMyoDevices
{
    NSArray* myoDevices = [[TLMHub sharedHub] myoDevices];

    NSMutableDictionary* resultDictionary = [NSMutableDictionary dictionary];
    
    //Serialize the myo object into a dictionary to be converted to json to be deserialized by unity
    for (TLMMyo* myo : myoDevices)
    {
        NSMutableDictionary* myoDictionary = [NSMutableDictionary dictionary];
        resultDictionary[@"name"] = myo.name;
        resultDictionary[@"identifier"] = myo.identifier;
        resultDictionary[@"state"] = [NSNumber numberWithInt:myo.state];
        resultDictionary[@"isLocked"] = [NSNumber numberWithBool:myo.isLocked];
        
        NSMutableDictionary* poseDictionary = [NSMutableDictionary dictionary];
        poseDictionary[@"type"] = [NSNumber numberWithInt:myo.pose.type];

        resultDictionary[@"pose"] = poseDictionary;
        
        
        NSMutableDictionary* orientationDictionary = [NSMutableDictionary dictionary];
        
        NSMutableDictionary* quaternionDict = [NSMutableDictionary dictionary];
        
        quaternionDict[@"x"] = [NSNumber numberWithFloat:myo.orientation.quaternion.x];
        quaternionDict[@"y"] = [NSNumber numberWithFloat:myo.orientation.quaternion.y];
        quaternionDict[@"z"] = [NSNumber numberWithFloat:myo.orientation.quaternion.z];
        quaternionDict[@"w"] = [NSNumber numberWithFloat:myo.orientation.quaternion.w];
        orientationDictionary[@"data"] = quaternionDict;
        resultDictionary[@"orientation"] = orientationDictionary;
        
        resultDictionary[@"arm"] = [NSNumber numberWithInt:myo.arm];
        resultDictionary[@"xDirection"] = [NSNumber numberWithInt:myo.xDirection];
        resultDictionary[myo.identifier] = myoDictionary;
    }
    NSError* error = nil;
    
    NSData* data = [NSJSONSerialization dataWithJSONObject:resultDictionary options:0 error:&error];
    
    return [[NSString alloc] initWithBytes:[data bytes] length:[data length] encoding:NSUTF8StringEncoding];
}

-(void) setLockingPolicy: (int) policy {

    [[TLMHub sharedHub] setLockingPolicy:(TLMLockingPolicy)policy];
}

- (void) showSettings{
    
    // Note that when the settings view controller is presented to the user, it must be in a UINavigationController.
    UINavigationController *controller = [TLMSettingsViewController settingsInNavigationController];
    // Present the settings view controller modally.
    
    UIViewController* unityController = UnityGetGLViewController();
    
    [unityController presentViewController:controller animated:YES completion:nil];
}

- (void) setApplicationID: (NSString*) appID{
     [[TLMHub sharedHub] setApplicationIdentifier:appID];
}

@end

