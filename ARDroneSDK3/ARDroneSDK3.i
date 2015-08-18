%module ARDroneSDK3
%include "typemaps.i"
%include "enums.swg"


%{
#include <libARDiscovery/ARDISCOVERY_Discovery.h>
#include <libARDiscovery/ARDISCOVERY_Device.h>
#include <libARController/ARCONTROLLER_Error.h>
#include <libARController/ARCONTROLLER_Network.h>
#include <libARController/ARCONTROLLER_DICTIONARY_Key.h>
#include <libARController/ARCONTROLLER_Dictionary.h>
#include <libARController/ARCONTROLLER_Feature.h>
#include <libARController/ARCONTROLLER_Device.h>
#include <libARDiscovery/ARDISCOVERY_Connection.h>
#include <libARDiscovery/ARDISCOVERY_NetworkConfiguration.h>
#include <libARDiscovery/ARDISCOVERY_Error.h>
%}

%inline %{
typedef int         int32_t;
typedef unsigned char       uint8_t;
typedef unsigned int        uint32_t;
%}

%define ENUM_PTR_TYPEMAP(TYPE)
	%typemap(ctype, out="void *") TYPE*, TYPE& "TYPE *"
	%typemap(imtype, out="global::System.IntPtr") TYPE*, TYPE& "ref TYPE"
	%typemap(cstype, out="$csclassname") TYPE*, TYPE& "ref TYPE"
	%typemap(csin) TYPE*, TYPE& "ref $csinput"
	%typemap(in) TYPE*, TYPE&
	%{ $1 = ($1_ltype)$input; %}
%enddef

%define %cs_callback(TYPE, CSTYPE)
        %typemap(ctype) TYPE, TYPE& "void*"
        %typemap(in) TYPE  %{ $1 = ($1_type)$input; %}
        %typemap(in) TYPE& %{ $1 = ($1_type)&$input; %}
        %typemap(imtype, out="IntPtr") TYPE, TYPE& "CSTYPE"
        %typemap(cstype, out="IntPtr") TYPE, TYPE& "CSTYPE"
        %typemap(csin) TYPE, TYPE& "$csinput"
%enddef

%define OPAQUE_PTR(TYPE)
	%typemap(cstype) TYPE "System.IntPtr" 
	%typemap(csin) TYPE "new global::System.Runtime.InteropServices.HandleRef(null, $csinput)" 
	%typemap(csout, excode=SWIGEXCODE) TYPE "{System.IntPtr res = $imcall; $excode; return res;}" 
	%typemap(csvarout, excode=SWIGEXCODE2) TYPE "get{System.IntPtr res = $imcall; $excode; return res;}" 
	%typemap(csdirectorin) TYPE "$iminput" 
	%typemap(csdirectorout) TYPE "$cscall" 
%enddef

ENUM_PTR_TYPEMAP(eARDISCOVERY_ERROR)
ENUM_PTR_TYPEMAP(eARCONTROLLER_ERROR)

%cs_callback(ARCONTROLLER_Device_StateChangedCallback_t, System.IntPtr)
%cs_callback(ARCONTROLLER_DICTIONARY_CALLBACK_t, System.IntPtr)
%cs_callback(ARNETWORKAL_Stream_DidReceiveFrameCallback_t, System.IntPtr)
%cs_callback(ARNETWORKAL_Stream_TimeoutFrameCallback_t, System.IntPtr)

OPAQUE_PTR(unsigned char*)
OPAQUE_PTR(void*)

%inline %{
typedef signed char     int8_t;
typedef short int       int16_t;
typedef int         int32_t;
typedef long long int       int64_t;
typedef unsigned char       uint8_t;
typedef unsigned short int  uint16_t;
typedef unsigned int        uint32_t;
typedef unsigned long long int  uint64_t;
typedef long int _time_t;
%}

%include <libARController/ARCONTROLLER_Error.h>
%include <libARDiscovery/ARDISCOVERY_Error.h>
%include <libARController/ARCONTROLLER_DICTIONARY_Key.h>
%include <libARController/ARCONTROLLER_Frame.h>

typedef enum
{
    ARDISCOVERY_PRODUCT_NSNETSERVICE = 0,                               ///< WiFi products category
    ARDISCOVERY_PRODUCT_ARDRONE = ARDISCOVERY_PRODUCT_NSNETSERVICE,     ///< AR DRONE product
    ARDISCOVERY_PRODUCT_JS,                                             ///< JUMPING SUMO product
    ARDISCOVERY_PRODUCT_SKYCONTROLLER,                                  ///< Sky controller product
    
    ARDISCOVERY_PRODUCT_BLESERVICE,                                     ///< BlueTooth products category
    ARDISCOVERY_PRODUCT_MINIDRONE = ARDISCOVERY_PRODUCT_BLESERVICE,         ///< DELOS product
    
    ARDISCOVERY_PRODUCT_MAX                                             ///< Max of products
} eARDISCOVERY_PRODUCT;

typedef enum
{
    ARCONTROLLER_DEVICE_STATE_STOPPED = 0, /**< device controller is stopped */
    ARCONTROLLER_DEVICE_STATE_STARTING, /**< device controller is starting */
    ARCONTROLLER_DEVICE_STATE_RUNNING, /**< device controller is running */
    ARCONTROLLER_DEVICE_STATE_PAUSED, /**< device controller is paused */
    ARCONTROLLER_DEVICE_STATE_STOPPING, /**< device controller is stopping */
    
    ARCONTROLLER_DEVICE_STATE_MAX /**< Max of the enumeration */
}eARCONTROLLER_DEVICE_STATE;

ARDISCOVERY_Device_t *ARDISCOVERY_Device_New (eARDISCOVERY_ERROR *error);
eARDISCOVERY_ERROR ARDISCOVERY_Device_InitWifi (ARDISCOVERY_Device_t *device, eARDISCOVERY_PRODUCT product, const char *name, const char *address, int port);
ARCONTROLLER_Device_t *ARCONTROLLER_Device_New (ARDISCOVERY_Device_t *discoveryDevice, eARCONTROLLER_ERROR *error);
eARCONTROLLER_ERROR ARCONTROLLER_Device_AddStateChangedCallback (ARCONTROLLER_Device_t *deviceController, ARCONTROLLER_Device_StateChangedCallback_t stateChangedCallback, void *customData);
eARCONTROLLER_ERROR ARCONTROLLER_Device_AddCommandReceivedCallback (ARCONTROLLER_Device_t *deviceController, ARCONTROLLER_DICTIONARY_CALLBACK_t commandReceivedCallback, void *customData);
eARCONTROLLER_ERROR ARCONTROLLER_Device_SetVideoReceiveCallback (ARCONTROLLER_Device_t *deviceController, ARNETWORKAL_Stream_DidReceiveFrameCallback_t receiveFrameCallback, ARNETWORKAL_Stream_TimeoutFrameCallback_t timeoutFrameCallback, void *customData);
eARCONTROLLER_ERROR ARCONTROLLER_Device_Start (ARCONTROLLER_Device_t *deviceController);


