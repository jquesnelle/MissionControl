/*
    Copyright (C) 2014 Parrot SA

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions
    are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in
      the documentation and/or other materials provided with the 
      distribution.
    * Neither the name of Parrot nor the names
      of its contributors may be used to endorse or promote products
      derived from this software without specific prior written
      permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
    FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
    COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
    INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
    BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS
    OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED 
    AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
    OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    SUCH DAMAGE.
*/
//
//  ARDiscovery.m
//  ARSDK 3
//
//  Created by Nicolas BRULEZ on 08/03/13.
//  Copyright (c) 2013 Parrot SA. All rights reserved.
//
#include <arpa/inet.h>
#include <libARSAL/ARSAL_Print.h>
#import <libARDiscovery/ARDISCOVERY_BonjourDiscovery.h>
#import <libARDiscovery/ARDISCOVERY_Discovery.h>
#import <netdb.h>

#define ARDISCOVERY_BONJOURDISCOVERY_TAG            "ARDISCOVERY_BonjourDiscovery"

#define kServiceNetControllerType                   @"_arsdk-ff3._udp"
#define kServiceNetDomain                           @ARDISCOVERY_SERVICE_NET_DEVICE_DOMAIN
#define kServiceNetDeviceFormat                     @ARDISCOVERY_SERVICE_NET_DEVICE_FORMAT"."

#define ARBLESERVICE_BLE_MANUFACTURER_DATA_LENGTH   8
#define ARBLESERVICE_PARROT_BT_VENDOR_ID            0X0043  // Parrot Company ID registered by Bluetooth SIG (Bluetooth Specification v4.0 Requirement)
#define ARBLESERVICE_PARROT_USB_VENDOR_ID           0x19cf  // Official Parrot USB Vendor ID

#define kServiceResolutionTimeout                   5.f    // Time in seconds
#define kServiceBLERefreshTime                      10.f    // Time in seconds

#define CHECK_VALID(DEFAULT_RETURN_VALUE)       \
    do                                          \
    {                                           \
        if (! self.valid)                       \
        {                                       \
            return DEFAULT_RETURN_VALUE;        \
        }                                       \
    } while (0)

#pragma mark - ARBLEService implementation

@implementation ARBLEService
@end

@implementation ARService
@synthesize signal;

- (BOOL)isEqual:(id)object
{
    BOOL result = YES;
    ARService *otherService = (ARService *)object;
    
    if((otherService != nil) && ([[self.service class] isEqual: [otherService.service class]]))
    {
        if ([self.service isKindOfClass:[NSNetService class]])
        {
            NSNetService *netService = (NSNetService *) self.service;
            NSNetService *otherNETService = (NSNetService *) otherService.service;
            
            result = ([netService.name isEqual:otherNETService.name]);
        }
        else if ([self.service isKindOfClass:[ARBLEService class]])
        {
            ARBLEService *bleService = (ARBLEService *) self.service;
            ARBLEService *otherBLEService = (ARBLEService *) otherService.service;
            
            result = ([[bleService.peripheral.identifier UUIDString] isEqual: [otherBLEService.peripheral.identifier UUIDString]]);
        }
        else
        {
            ARSAL_PRINT(ARSAL_PRINT_ERROR, ARDISCOVERY_BONJOURDISCOVERY_TAG, "Unknown network media type.");
            result = NO;
        }
    }
    else
    {
        result = NO;
    }
    
    return result;
}

@end

#pragma mark Private part
@interface ARDiscovery () <NSNetServiceBrowserDelegate, NSNetServiceDelegate, CBCentralManagerDelegate>

#pragma mark - Controller/Devices Services list
@property (strong, nonatomic) NSMutableDictionary *controllersServicesList;
@property (strong, nonatomic) NSMutableDictionary *devicesServicesList;

#pragma mark - Current published service
@property (strong, nonatomic) NSNetService *currentPublishedService;
@property (strong, nonatomic) NSNetService *tryPublishService;

#pragma mark - Services browser / resolution
@property (strong, nonatomic) ARService *currentResolutionService;
@property (strong, nonatomic) NSNetServiceBrowser *controllersServiceBrowser;
@property (strong, nonatomic) NSMutableArray *devicesServiceBrowsers;

#pragma mark - Services CoreBluetooth
@property (strong, nonatomic) ARSAL_CentralManager *centralManager;
@property (nonatomic, assign) BOOL centralManagerInitialized;
@property (strong, nonatomic) NSMutableDictionary *devicesBLEServicesTimerList;

#pragma mark - Object properly created
@property (nonatomic) BOOL valid;

#pragma mark - Object properly created
@property (nonatomic) BOOL isNSNetDiscovering;
@property (nonatomic) BOOL isCBDiscovering;
@property (nonatomic) BOOL askForCBDiscovering;
@end

#pragma mark Implementation
@implementation ARDiscovery

@synthesize controllersServicesList;
@synthesize devicesServicesList;
@synthesize devicesBLEServicesTimerList;
@synthesize currentPublishedService;
@synthesize tryPublishService;
@synthesize currentResolutionService;
@synthesize controllersServiceBrowser;
@synthesize devicesServiceBrowsers;
@synthesize valid;
@synthesize centralManager;
@synthesize centralManagerInitialized;
@synthesize askForCBDiscovering;
@synthesize isNSNetDiscovering;
@synthesize isCBDiscovering;

#pragma mark - Init
+ (ARDiscovery *)sharedInstance
{
    static ARDiscovery *_sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
            _sharedInstance = [[ARDiscovery alloc] init];

            /**
             * Services list init
             */
            _sharedInstance.controllersServicesList = [[NSMutableDictionary alloc] init];
            _sharedInstance.devicesServicesList = [[NSMutableDictionary alloc] init];
            _sharedInstance.devicesBLEServicesTimerList = [[NSMutableDictionary alloc] init];

            /**
             * Current published service init
             */
            _sharedInstance.currentPublishedService = nil;
            _sharedInstance.tryPublishService = nil;

            /**
             * Services browser / resolution init
             */
            _sharedInstance.controllersServiceBrowser = [[NSNetServiceBrowser alloc] init];
            [_sharedInstance.controllersServiceBrowser setDelegate:_sharedInstance];
            _sharedInstance.devicesServiceBrowsers = [[NSMutableArray alloc] init];
            for (int i = ARDISCOVERY_PRODUCT_NSNETSERVICE; i < ARDISCOVERY_PRODUCT_BLESERVICE; ++i)
            {
                NSNetServiceBrowser *browser = [[NSNetServiceBrowser alloc] init];
                [browser setDelegate:_sharedInstance];
                [_sharedInstance.devicesServiceBrowsers addObject:browser];
            }

            _sharedInstance.currentResolutionService = nil;

            /**
             * Creation was done as a shared instance
             */
            _sharedInstance.valid = YES;

            /**
             * Discover is not in progress
             */
            _sharedInstance.centralManagerInitialized = NO;
            _sharedInstance.centralManager = [[ARSAL_CentralManager alloc] initWithQueue:nil];
            [_sharedInstance.centralManager addDelegate: _sharedInstance];
            _sharedInstance.isNSNetDiscovering = NO;
            _sharedInstance.isCBDiscovering = NO;
            _sharedInstance.askForCBDiscovering = NO;
        });

    return _sharedInstance;
}

#pragma mark - Getters
- (NSArray *)getCurrentListOfDevicesServices
{
    NSArray *array = nil;
    CHECK_VALID(array);
    @synchronized (self)
    {
        array = [[self.devicesServicesList allValues] copy];
    }
    return array;
}

- (NSArray *)getCurrentListOfControllersServices
{
    NSArray *array = nil;
    CHECK_VALID(array);
    @synchronized (self)
    {
        array = [[self.controllersServicesList allValues] copy];
    }
    return array;
}

- (NSString *)getCurrentPublishedServiceName
{
    NSString *name = nil;
    CHECK_VALID(name);
    @synchronized (self)
    {
        name = [[self.currentPublishedService name] copy];
    }
    return name;
}

#pragma mark - Discovery
- (BOOL)isNetServiceValid:(NSNetService *)aNetService
{
    for (int i = ARDISCOVERY_PRODUCT_NSNETSERVICE; i < ARDISCOVERY_PRODUCT_BLESERVICE; ++i)
    {
        NSString *deviceType = [NSString stringWithFormat:kServiceNetDeviceFormat, ARDISCOVERY_getProductID(i)];
        if ([aNetService.type isEqualToString:deviceType])
            return YES;
    }
    return NO;
}

- (void)resolveService:(ARService *)aService
{
    CHECK_VALID();
    @synchronized (self)
    {
        if(self.currentResolutionService != nil)
        {
            [[self.currentResolutionService service] stop];
        }
        
        self.currentResolutionService = aService;
        [((NSNetService*)[self.currentResolutionService service]) setDelegate:self];
        [[self.currentResolutionService service] resolveWithTimeout:kServiceResolutionTimeout];
    }
}

- (void)start
{
    NSLog(@"%s:%d", __FUNCTION__, __LINE__);
    
    if (!isNSNetDiscovering)
    {
        /**
         * Start NSNetServiceBrowser
         */
        [controllersServiceBrowser searchForServicesOfType:kServiceNetControllerType inDomain:kServiceNetDomain];
        for (int i = 0; i < [devicesServiceBrowsers count]; ++i)
        {
            NSNetServiceBrowser *browser = [devicesServiceBrowsers objectAtIndex:i];
            [browser searchForServicesOfType:[NSString stringWithFormat:kServiceNetDeviceFormat, ARDISCOVERY_getProductID(ARDISCOVERY_PRODUCT_NSNETSERVICE + i)] inDomain:kServiceNetDomain];
        }
        
        isNSNetDiscovering = YES;
    }
    
    if(!isCBDiscovering)
    {
        if (centralManagerInitialized)
        {
            /**
             * Start CoreBluetooth discovery
             */
            [centralManager scanForPeripheralsWithServices:nil options:[NSDictionary dictionaryWithObjectsAndKeys:[NSNumber numberWithBool:YES], CBCentralManagerScanOptionAllowDuplicatesKey, nil]];
            isCBDiscovering = YES;
        }
        else
        {
            askForCBDiscovering = YES;
        }
    }
}

- (void)pauseBLE
{
    NSLog(@"%s:%d", __FUNCTION__, __LINE__);
    
    if (centralManagerInitialized && isCBDiscovering)
    {
        /**
         * Stop CBCentralManager
         */
        isCBDiscovering = NO;
        [centralManager stopScan];
    }
}

- (void)stop
{
    NSLog(@"%s:%d", __FUNCTION__, __LINE__);
    
    [self removeAllServices];
    
    if (isNSNetDiscovering)
    {
        /**
         * Stop NSNetServiceBrowser
         */
        [controllersServiceBrowser stop];
        for (NSNetServiceBrowser *browser in devicesServiceBrowsers)
        {
            [browser stop];
        }
        isNSNetDiscovering = NO;
    }
    
    if (centralManagerInitialized && isCBDiscovering)
    {
        /**
         * Stop CBCentralManager
         */
        isCBDiscovering = NO;
        [centralManager stopScan];
    }
}

- (void)removeAllServices
{
    @synchronized (self)
    {
        [self.devicesServicesList removeAllObjects];
        [self sendDevicesListUpdateNotification];
        
        [self.controllersServicesList removeAllObjects];
        [self sendControllersListUpdateNotification];
    }
}

- (void)removeAllBLEServices
{
    @synchronized (self)
    {
        NSMutableArray *bleDevices = [[NSMutableArray alloc] init];
        for (NSString *key in devicesServicesList)
        {
            ARService *aService = [devicesServicesList objectForKey:key];
            if([aService.service isKindOfClass:[ARBLEService class]])
            {
                [bleDevices addObject:key];
            }
        }

        [devicesServicesList removeObjectsForKeys:bleDevices];
        [self sendDevicesListUpdateNotification];
    }
}

- (void)removeAllWifiServices
{
    @synchronized (self)
    {
        NSMutableArray *wifiDevices = [[NSMutableArray alloc] init];
        for (NSString *key in devicesServicesList)
        {
            ARService *aService = [devicesServicesList objectForKey:key];
            if([aService.service isKindOfClass:[NSNetService class]])
            {
                [wifiDevices addObject:key];
            }
        }
        
        [devicesServicesList removeObjectsForKeys:wifiDevices];
        [self sendDevicesListUpdateNotification];
    }
}

- (NSString *)convertNSNetServiceToIp:(ARService *)aService
{
    NSString *name = nil;
    NSData *address = nil;
    struct sockaddr_in *socketAddress = nil;
    NSString *ipString = nil;
    int port;
    int i;
    
    name = [[aService service] name];
    NSArray *adresses = ((NSNetService *)[aService service]).addresses;
    for (i = 0 ; i < [adresses count] ; i++)
    {
        address = [adresses objectAtIndex:i];
        socketAddress = (struct sockaddr_in *) [address bytes];
        if (socketAddress->sin_family == AF_INET)//AF_INET -> IPv4, AF_INET6 -> IPv6
        {
            char ip[INET_ADDRSTRLEN];
            inet_ntop(AF_INET, &socketAddress->sin_addr, ip, INET_ADDRSTRLEN);
            ipString = [NSString stringWithFormat: @"%s", ip];
            port = ntohs(socketAddress->sin_port);
        }
    }
    
    // This will print the IP and port for you to connect to.
    NSLog(@"%@", [NSString stringWithFormat:@"Resolved:%@-->%@:%u\n", [[aService service] hostName], ipString, port]);

    return ipString;
}

#pragma mark - Publication
- (NSString *)uniqueNameFromServiceName:(NSString *)sname isController:(BOOL)isController
{
    NSString *rname = [sname copy];

    int addCount = 1;

    NSArray *servicesCopy;
    if (isController)
    {
        servicesCopy = [self getCurrentListOfControllersServices];
    }
    else
    {
        servicesCopy = [self getCurrentListOfDevicesServices];
    }
    BOOL rnameIsUnique = YES;
    do {
        rnameIsUnique = YES;
        for (NSNetService *ns in servicesCopy) {
            if ([rname isEqualToString:[ns name]])
            {
                rnameIsUnique = NO;
                break;
            }
        }
        if (! rnameIsUnique)
        {
            rname = [sname stringByAppendingFormat:@"%d", addCount++];
        }
    } while (! rnameIsUnique);
    return rname;
}

- (void)publishControllerServiceWithName:(NSString *)serviceName
{
    CHECK_VALID();
    @synchronized (self)
    {
        NSString *uniqueName = [self uniqueNameFromServiceName:serviceName isController:YES];
        [self.tryPublishService stop];
        self.tryPublishService = [[NSNetService alloc] initWithDomain:kServiceNetDomain type:kServiceNetControllerType name:uniqueName port:9];
        [self.tryPublishService setDelegate:self];
        [self.tryPublishService publish];
    }
}

- (void)unpublishService
{
    CHECK_VALID();
    @synchronized (self)
    {
        [self.tryPublishService stop];
        self.tryPublishService = nil;
        self.currentPublishedService = nil;
        [self sendPublishNotification];
    }
}

#pragma mark - NSNetServiceBrowser Delegate
- (void)netServiceBrowser:(NSNetServiceBrowser *)aNetServiceBrowser didFindService:(NSNetService *)aNetService moreComing:(BOOL)moreComing
{
    NSLog(@"Service found : %@, %@", aNetService.name, aNetService.type);
    @synchronized (self)
    {
        if ([aNetService.type isEqualToString:kServiceNetControllerType])
        {
            ARService *aService = [self.controllersServicesList objectForKey:aNetService.name];
            if (aService == nil)
            {
                aService = [[ARService alloc] init];
                aService.service = aNetService;
            }
            aService.name = [aNetService name];
            aService.signal = [NSNumber numberWithInt:0];
            NSDictionary *dict = [NSNetService dictionaryFromTXTRecordData:aNetService.TXTRecordData];
            if(dict != nil && [dict objectForKey:[NSString stringWithUTF8String:ARDISCOVERY_SERVICE_NET_RSSI_SIGNAL_KEY]] != nil)
            {
                aService.signal = [dict objectForKey:[NSString stringWithUTF8String:ARDISCOVERY_SERVICE_NET_RSSI_SIGNAL_KEY]];
            }
            aService.product = ARDISCOVERY_PRODUCT_MAX;
            
            [self.controllersServicesList setObject:aService forKey:aService.name];
            if (!moreComing)
            {
                [self sendControllersListUpdateNotification];
            }
        }
        else
        {
            ARService *aService   = [self.devicesServicesList objectForKey:aNetService.name];
            
            if (aService == nil)
            {
                aService = [[ARService alloc] init];
                aService.service = aNetService;
            }
            else
            {
                NSString *serviceType = [NSString stringWithFormat:kServiceNetDeviceFormat, ARDISCOVERY_getProductID(aService.product)];
                if(![aNetService.type isEqualToString:serviceType])
                {
                    aService = [[ARService alloc] init];
                    aService.service = aNetService;
                }
            }
            
            aService.name = [aNetService name];
            aService.signal = [NSNumber numberWithInt:0];
            NSDictionary *dict = [NSNetService dictionaryFromTXTRecordData:aNetService.TXTRecordData];
            if(dict != nil && [dict objectForKey:[NSString stringWithUTF8String:ARDISCOVERY_SERVICE_NET_RSSI_SIGNAL_KEY]] != nil)
            {
                aService.signal = [dict objectForKey:[NSString stringWithUTF8String:ARDISCOVERY_SERVICE_NET_RSSI_SIGNAL_KEY]];
            }
            aService.product = ARDISCOVERY_PRODUCT_MAX;
            
            for (int i = ARDISCOVERY_PRODUCT_NSNETSERVICE; (aService.product == ARDISCOVERY_PRODUCT_MAX) && (i < ARDISCOVERY_PRODUCT_BLESERVICE); ++i)
            {
                NSString *deviceType = [NSString stringWithFormat:kServiceNetDeviceFormat, ARDISCOVERY_getProductID(i)];
                if ([aNetService.type isEqualToString:deviceType])
                {
                    aService.product = i;
                }
            }
            
            if (aService.product != ARDISCOVERY_PRODUCT_MAX)
            {
                [self.devicesServicesList setObject:aService forKey:aService.name];
                if (!moreComing)
                {
                    [self sendDevicesListUpdateNotification];
                }
            }
            else
            {
#ifdef DEBUG
                NSLog (@"Found an unknown service : %@", aNetService);
#endif
            }
        }
    }
}

- (void)netServiceBrowser:(NSNetServiceBrowser *)aNetServiceBrowser didRemoveService:(NSNetService *)aNetService moreComing:(BOOL)moreComing
{
    @synchronized (self)
    {
        if ([self isNetServiceValid:aNetService])
        {
            ARService *aService = (ARService *)[self.devicesServicesList objectForKey:aNetService.name];
            if (aService != nil)
            {
                NSLog(@"Removed service %@ : %@", aService.name, NSStringFromClass([[aService service] class]));
                [self.devicesServicesList removeObjectForKey:aService.name];
                if (!moreComing)
                {
                    [self sendDevicesListUpdateNotification];
                }
            }
        }
        else if ([[aNetService type] isEqual:kServiceNetControllerType])
        {
            ARService *aService = (ARService *)[self.controllersServicesList objectForKey:aNetService.name];
            if (aService != nil)
            {
                NSLog(@"Removed service %@ : %@", aService.name, NSStringFromClass([[aService service] class]));
                [self.controllersServicesList removeObjectForKey:aService.name];
                if (!moreComing)
                {
                    [self sendControllersListUpdateNotification];
                }
            }
        }
        else
        {
#ifdef DEBUG
            NSLog (@"Removed an unknown service : %@", aNetService);
#endif
        }
    }
}

#pragma mark - NSNetService Delegate
- (void)netService:(NSNetService *)service didNotPublish:(NSDictionary *)errorDict
{
    @synchronized (self)
    {
        self.currentPublishedService = nil;
        [self sendPublishNotification];
    }
}

- (void)netServiceDidPublish:(NSNetService *)service
{
    @synchronized (self)
    {
        self.currentPublishedService = service;
        [self sendPublishNotification];
    }
}

- (void)netService:(NSNetService *)service didNotResolve:(NSDictionary *)errorDict
{
    @synchronized (self)
    {
        self.currentResolutionService = nil;
        [self sendNotResolveNotification];
        [service stop];
    }
}

- (void)netServiceDidResolveAddress:(NSNetService *)service
{
    @synchronized (self)
    {
        [self sendResolveNotification];
        [service stop];
    }
}

#pragma mark - Refresh BLE services methods
- (void)deviceBLERemoveServices:(ARService *)aService
{
    @synchronized (self)
    {
        CBPeripheral *peripheral = ((ARBLEService *) aService.service).peripheral;
        NSLog(@"Removed service %@ : %@", aService.name, NSStringFromClass([[aService service] class]));
        [self.devicesBLEServicesTimerList removeObjectForKey:[peripheral.identifier UUIDString]];
        [self.devicesServicesList removeObjectForKey:[peripheral.identifier UUIDString]];
        [self sendDevicesListUpdateNotification];
    }
}

- (void)deviceBLETimeout:(NSTimer *)timer
{
    ARService *aService = [timer userInfo];
    [self deviceBLERemoveServices:aService];
}

#pragma mark - CBCentralManagerDelegate methods
- (void)centralManagerDidUpdateState:(CBCentralManager *)central
{
    NSString *sNewState = @"New CBCentralManager state :";
    switch(central.state)
    {
        case CBCentralManagerStatePoweredOn:
            NSLog(@"%@ CBCentralManagerStatePoweredOn", sNewState);
            centralManagerInitialized = YES;
            if(askForCBDiscovering)
            {
                askForCBDiscovering = NO;
                [self start];
            }
            break;
            
        case CBCentralManagerStateResetting:
            NSLog(@"%@ CBCentralManagerStateResetting", sNewState);
            centralManagerInitialized = NO;
            isCBDiscovering = NO;
            askForCBDiscovering = YES;
            
            break;
            
        case CBCentralManagerStateUnsupported:
            NSLog(@"%@ CBCentralManagerStateUnsupported", sNewState);
            centralManagerInitialized = NO;
            break;
            
        case CBCentralManagerStateUnauthorized:
            NSLog(@"%@ CBCentralManagerStateUnauthorized", sNewState);
            centralManagerInitialized = NO;
            break;
            
        case CBCentralManagerStatePoweredOff:
            NSLog(@"%@ CBCentralManagerStatePoweredOff", sNewState);
            centralManagerInitialized = NO;
            isCBDiscovering = NO;
            askForCBDiscovering = YES;
            
            [self removeAllBLEServices];
            
            break;
            
        default:
        case CBCentralManagerStateUnknown:
            NSLog(@"%@ CBCentralManagerStateUnknown", sNewState);
            centralManagerInitialized = NO;
            break;
    }
}

- (void)centralManager:(CBCentralManager *)central didDiscoverPeripheral:(CBPeripheral *)peripheral advertisementData:(NSDictionary *)advertisementData RSSI:(NSNumber *)RSSI
{
    @synchronized (self)
    {
        if([peripheral name] != nil)
        {
            if ( [self isParrotBLEDevice:advertisementData] )
            {
                ARService *aService = [self.devicesServicesList objectForKey:[peripheral.identifier UUIDString]];
                if(aService == nil)
                {
                    NSLog(@"New device %@", [advertisementData objectForKey:CBAdvertisementDataLocalNameKey]);
                    ARBLEService *bleService = [[ARBLEService alloc] init];
                    bleService.centralManager = self.centralManager;
                    bleService.peripheral = peripheral;
                    
                    aService = [[ARService alloc] init];
                    aService.service = bleService;
                    aService.name = [advertisementData objectForKey:CBAdvertisementDataLocalNameKey];
                    aService.signal = RSSI;
                    
                    NSData *manufacturerData = [advertisementData valueForKey:CBAdvertisementDataManufacturerDataKey];
                    uint16_t *ids = (uint16_t *) manufacturerData.bytes;
                    aService.product = ARDISCOVERY_PRODUCT_MAX;
                    for (int i = ARDISCOVERY_PRODUCT_BLESERVICE ; (aService.product == ARDISCOVERY_PRODUCT_MAX) && (i < ARDISCOVERY_PRODUCT_MAX) ; i++)
                    {
                        if (ids[2] == ARDISCOVERY_getProductID(i))
                            aService.product = i;
                    }

                    [self.devicesServicesList setObject:aService forKey:[peripheral.identifier UUIDString]];
                    [self sendDevicesListUpdateNotification];
                }
                else
                {
                    BOOL sendNotification = NO;
                    if(![aService.name isEqualToString:[advertisementData objectForKey:CBAdvertisementDataLocalNameKey]])
                    {
                        aService.name = [advertisementData objectForKey:CBAdvertisementDataLocalNameKey];
                        sendNotification = YES;
                    }
                    
                    if([aService.signal compare:RSSI] != NSOrderedSame)
                    {
                        aService.signal = RSSI;
                        sendNotification = YES;
                    }

                    if(sendNotification)
                    {
                        [self sendDevicesListUpdateNotification];
                    }
                }
                
                NSTimer *timer = (NSTimer *)[self.devicesBLEServicesTimerList objectForKey:[peripheral.identifier UUIDString]];
                if(timer != nil)
                {
                    [timer invalidate];
                    timer = nil;
                }
                timer = [NSTimer scheduledTimerWithTimeInterval:kServiceBLERefreshTime target:self selector:@selector(deviceBLETimeout:) userInfo:aService repeats:NO];
                [self.devicesBLEServicesTimerList setObject:timer forKey:[peripheral.identifier UUIDString]];
            }
        }
    }
}

- (void)centralManager:(CBCentralManager *)central didDisconnectPeripheral:(CBPeripheral *)peripheral error:(NSError *)error
{
    NSTimer *timer = (NSTimer *)[self.devicesBLEServicesTimerList objectForKey:[peripheral.identifier UUIDString]];
    
    if(timer != nil)
    {
        ARService *aService = [timer userInfo];
        
        [timer invalidate];
        timer = nil;
        
        [self deviceBLERemoveServices:aService];
        
    }
}

- (BOOL)isParrotBLEDevice:(NSDictionary *)advertisementData
{
    /* Read the advertisementData to check if it is a PARROT Delos device with the good version */

    BOOL res = NO;
    NSData *manufacturerData = [advertisementData valueForKey:CBAdvertisementDataManufacturerDataKey];

    if ((manufacturerData != nil) && (manufacturerData.length == ARBLESERVICE_BLE_MANUFACTURER_DATA_LENGTH))
    {
        uint16_t *ids = (uint16_t*) manufacturerData.bytes;
        
#ifdef DEBUG
        NSLog(@"manufacturer Data: BTVendorID:0x%.4x USBVendorID:0x%.4x USBProduitID=0x%.4x versionID=0x%.4x", ids[0], ids[1], ids[2], ids[3]);
#endif
        
        if ((ids[0] == ARBLESERVICE_PARROT_BT_VENDOR_ID) &&
            (ids[1] == ARBLESERVICE_PARROT_USB_VENDOR_ID) &&
            (ids[2] == ARDISCOVERY_getProductID(ARDISCOVERY_PRODUCT_MINIDRONE)))
        {
            res = YES;
        }
    }

    return res;
}

#pragma mark - Notification sender
- (void)sendPublishNotification
{
    NSDictionary *userInfos = @{kARDiscoveryServiceName: ([self getCurrentPublishedServiceName] != nil) ? [self getCurrentPublishedServiceName] : @""};
    [[NSNotificationCenter defaultCenter] postNotificationName:kARDiscoveryNotificationServicePublished object:self userInfo:userInfos];
}

- (void)sendDevicesListUpdateNotification
{
    NSDictionary *userInfos = @{kARDiscoveryServicesList: [self getCurrentListOfDevicesServices]};
    [[NSNotificationCenter defaultCenter] postNotificationName:kARDiscoveryNotificationServicesDevicesListUpdated object:self userInfo:userInfos];
}

- (void)sendControllersListUpdateNotification
{
    NSDictionary *userInfos = @{kARDiscoveryServicesList: [self getCurrentListOfControllersServices]};
    [[NSNotificationCenter defaultCenter] postNotificationName:kARDiscoveryNotificationServicesControllersListUpdated object:self userInfo:userInfos];
}

- (void)sendResolveNotification
{
    NSDictionary *userInfos = @{kARDiscoveryServiceResolved: self.currentResolutionService};
    [[NSNotificationCenter defaultCenter] postNotificationName:kARDiscoveryNotificationServiceResolved object:self userInfo:userInfos];
}

- (void)sendNotResolveNotification
{
    [[NSNotificationCenter defaultCenter] postNotificationName:kARDiscoveryNotificationServiceNotResolved object:self userInfo:nil];
}

@end
