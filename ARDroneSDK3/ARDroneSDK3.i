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

SWIG_CSBODY_PROXY(public, public, SWIGTYPE)
SWIG_CSBODY_TYPEWRAPPER(public, public, public, SWIGTYPE)

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

%include <libARDiscovery/ARDISCOVERY_Error.h>
%include <libARDiscovery/ARDISCOVERY_Device.h>

%include <libARController/ARCONTROLLER_Error.h>
%include <libARController/ARCONTROLLER_DICTIONARY_Key.h>
%include <libARController/ARCONTROLLER_Frame.h>
%include <libARController/ARController_Device.h>
%include <libARController/ARCONTROLLER_Dictionary.h>

eARCONTROLLER_ERROR ARCONTROLLER_FEATURE_ARDrone3_SendPilotingTakeOff (ARCONTROLLER_FEATURE_ARDrone3_t *feature);
eARCONTROLLER_ERROR ARCONTROLLER_FEATURE_ARDrone3_SendPilotingLanding (ARCONTROLLER_FEATURE_ARDrone3_t *feature);
eARCONTROLLER_ERROR ARCONTROLLER_FEATURE_ARDrone3_SetPilotingPCMD (ARCONTROLLER_FEATURE_ARDrone3_t *feature, uint8_t flag, int8_t roll, int8_t pitch, int8_t yaw, int8_t gaz, float psi);