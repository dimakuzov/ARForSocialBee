#import <Foundation/Foundation.h>
#import "NativeCallProxy.h"


@implementation FrameworkLibAPI

id<NativeCallsProtocol> api = NULL;
+(void) registerAPIforNativeCalls:(id<NativeCallsProtocol>) aApi
{
    api = aApi;
}

@end


extern "C" {

    void showNativePage(const char* page) {
        return [api showNativePage:[NSString stringWithUTF8String:page]];
    }

    void giveActivityData(const char* mapID, const char* activity){
        return [api giveActivityData:[NSString stringWithUTF8String:mapID] act:[NSString stringWithUTF8String:activity]];
    }
}
