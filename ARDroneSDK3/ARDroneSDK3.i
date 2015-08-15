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

ENUM_PTR_TYPEMAP(eARDISCOVERY_ERROR)

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

