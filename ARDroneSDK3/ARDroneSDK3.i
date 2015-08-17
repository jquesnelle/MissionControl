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

ENUM_PTR_TYPEMAP(eARDISCOVERY_ERROR)
ENUM_PTR_TYPEMAP(eARCONTROLLER_ERROR)

%cs_callback(ARCONTROLLER_Device_StateChangedCallback_t, System.IntPtr)
%cs_callback(ARCONTROLLER_DICTIONARY_CALLBACK_t, System.IntPtr)
%cs_callback(ARNETWORKAL_Stream_DidReceiveFrameCallback_t, System.IntPtr)
%cs_callback(ARNETWORKAL_Stream_TimeoutFrameCallback_t, System.IntPtr)

%typemap(cstype) void* "System.IntPtr" 
%typemap(csin) void* "new global::System.Runtime.InteropServices.HandleRef(null, $csinput)" 
%typemap(csout, excode=SWIGEXCODE) void* "{System.IntPtr res = $imcall; $excode; return res;}" 
%typemap(csvarout, excode=SWIGEXCODE2) void* "get{System.IntPtr res = $imcall; $excode; return res;}" 
%typemap(csdirectorin) void* "$iminput" 
%typemap(csdirectorout) void* "$cscall" 

%include <libARDiscovery/ARDISCOVERY_Connection.h>
%include <libARDiscovery/ARDISCOVERY_Discovery.h>
%include <libARDiscovery/ARDISCOVERY_NetworkConfiguration.h>
%include <libARDiscovery/ARDISCOVERY_Device.h>
%include <libARDiscovery/ARDISCOVERY_Error.h>
%include <libARController/ARCONTROLLER_Error.h>
%include <libARController/ARCONTROLLER_Network.h>
%include <libARController/ARCONTROLLER_DICTIONARY_Key.h>
%include <libARController/ARCONTROLLER_Dictionary.h>
%include <libARController/ARCONTROLLER_Feature.h>
%include <libARController/ARCONTROLLER_Device.h>

