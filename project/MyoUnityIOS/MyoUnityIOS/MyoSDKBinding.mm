//
//  MyoSDKBindings.m
//  HelloMyo
//
//  Created by Nelson Andre on 2015-04-24.
//  Copyright (c) 2015 Thalmic Labs. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "MyoSDKManager.h"

#define CharArrayToStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL
#define CharArrayToString( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

extern "C"
{
    /**
     This method is called upon starting the application by myo manager unity game object
     **/
    void myo_SetApplicationID(char* app_id){
        
        if (app_id == NULL){
            return;
        }
        
        //Convert the unity character array to a NSString
        NSString* appId = CharArrayToString(app_id);
        [[MyoSDKManager sharedManager] setApplicationID:appId];
    }
    
    void myo_SetShouldNotifyInBackground(bool value){
        [[TLMHub sharedHub] setShouldNotifyInBackground:value];
    }
    
    void myo_SetShouldSendUsageData(bool value){
        [[TLMHub sharedHub] setShouldSendUsageData:value];
    }
    
    
    bool myo_VibrateWithLength(char* myoId, int length){
        
        NSString* myoIdString = CharArrayToString(myoId);
        
        for (TLMMyo* myo in [[TLMHub sharedHub] myoDevices])
        {
       
            if ([[[myo identifier] UUIDString] isEqualToString:myoIdString ])
            {
                [myo vibrateWithLength:(TLMVibrationLength) length];
                return true;
            }
        }
        return false;
    }
    
    
    
    bool myo_SetStreamEmg(char *myoId, int type){
        
        NSString* myoIdString = CharArrayToString(myoId);
        
        for (TLMMyo* myo in [[TLMHub sharedHub] myoDevices])
        {
            
            if ([[[myo identifier] UUIDString] isEqualToString:myoIdString ])
            {
                [myo setStreamEmg:(TLMStreamEmgType)type];
                return true;
            }
        }
        
        return false;
    }
    
    bool myo_IndicateUserAction(char *myoId){
        
        NSString* myoIdString = CharArrayToString(myoId);
        
        for (TLMMyo* myo in [[TLMHub sharedHub] myoDevices])
        {
            
            if ([[[myo identifier] UUIDString] isEqualToString:myoIdString ])
            {
                [myo indicateUserAction];
                return true;
            }
        }
        
        return false;
    }
    
    
    int myo_MyoConnectionAllowance(){
        
        return [[TLMHub sharedHub] myoConnectionAllowance];
    }
    
    void myo_SetMyoConnectionAllowance(int value){
        
        NSLog(@"Set myo connection allowance to be %d", value);
        
        [[TLMHub sharedHub] setMyoConnectionAllowance:value];
    }
    
    bool myo_Lock(char *myoId){
        NSString* myoIdString = CharArrayToString(myoId);
        
        for (TLMMyo* myo in [[TLMHub sharedHub] myoDevices])
        {
            
            if ([[[myo identifier] UUIDString] isEqualToString:myoIdString ])
            {
                [myo lock];
                return true;
            }
        }
        
        return false;
    }

    bool myo_UnlockWithType(char* myoId, int type){
        
        NSString* myoIdString = CharArrayToString(myoId);
        
        for (TLMMyo* myo in [[TLMHub sharedHub] myoDevices])
        {
            
            if ([[[myo identifier] UUIDString] isEqualToString:myoIdString ])
            {
                [myo unlockWithType:(TLMUnlockType)type];
                return true;
            }
        }
        return false;
    }
    
    
    void myo_ShowSettings(){
        [[MyoSDKManager sharedManager] showSettings];
    }
    
    bool myo_IsArmLocked(){
        
        return [[MyoSDKManager sharedManager] locked];
    }
    
    void myo_SetLockingPolicy(int policy){
        
        [[MyoSDKManager sharedManager] setLockingPolicy:policy];
    }
    
    const char* myo_GetMyos(){
        NSString* result = [[MyoSDKManager sharedManager] getMyoDevices];
        
        return [result UTF8String];
    }
    
}