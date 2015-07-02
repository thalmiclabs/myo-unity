//
//  MyoSDKManager.h
//  HelloMyo
//
//  Created by Nelson Andre on 2015-04-24.
//  Copyright (c) 2015 Thalmic Labs. All rights reserved.
//


#import <MyoKit/MyoKit.h>

enum Status
{
    WAITING,
    CONNECTED
};

@interface MyoSDKManager : NSObject
{
   
}

- (void) setApplicationID: (NSString*) appID;
- (void) showSettings;
- (void) setLockingPolicy: (int) policy;
- (NSString*) getMyoDevices;

@property (nonatomic) Status status;

@property (nonatomic) BOOL locked;

@property (nonatomic) TLMArmXDirection armXDirection;

+ (MyoSDKManager*)sharedManager;
@end