//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.7
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class ARCONTROLLER_FEATURE_ARDrone3Debug_t : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal ARCONTROLLER_FEATURE_ARDrone3Debug_t(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(ARCONTROLLER_FEATURE_ARDrone3Debug_t obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~ARCONTROLLER_FEATURE_ARDrone3Debug_t() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          ARDroneSDK3PINVOKE.delete_ARCONTROLLER_FEATURE_ARDrone3Debug_t(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t_uint8_t__eARCONTROLLER_ERROR sendVideoEnableWobbleCancellation {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_ARDrone3Debug_t_sendVideoEnableWobbleCancellation_set(swigCPtr, SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t_uint8_t__eARCONTROLLER_ERROR.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_ARDrone3Debug_t_sendVideoEnableWobbleCancellation_get(swigCPtr);
      SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t_uint8_t__eARCONTROLLER_ERROR ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t_uint8_t__eARCONTROLLER_ERROR(cPtr, false);
      return ret;
    } 
  }

  public SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t_float_float__eARCONTROLLER_ERROR sendVideoSyncAnglesGyros {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_ARDrone3Debug_t_sendVideoSyncAnglesGyros_set(swigCPtr, SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t_float_float__eARCONTROLLER_ERROR.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_ARDrone3Debug_t_sendVideoSyncAnglesGyros_get(swigCPtr);
      SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t_float_float__eARCONTROLLER_ERROR ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t_float_float__eARCONTROLLER_ERROR(cPtr, false);
      return ret;
    } 
  }

  public SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t__eARCONTROLLER_ERROR sendVideoManualWhiteBalance {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_ARDrone3Debug_t_sendVideoManualWhiteBalance_set(swigCPtr, SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t__eARCONTROLLER_ERROR.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_ARDrone3Debug_t_sendVideoManualWhiteBalance_get(swigCPtr);
      SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t__eARCONTROLLER_ERROR ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t__eARCONTROLLER_ERROR(cPtr, false);
      return ret;
    } 
  }

  public SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t_uint8_t__eARCONTROLLER_ERROR sendBatteryDebugSettingsUseDrone2Battery {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_ARDrone3Debug_t_sendBatteryDebugSettingsUseDrone2Battery_set(swigCPtr, SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t_uint8_t__eARCONTROLLER_ERROR.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_ARDrone3Debug_t_sendBatteryDebugSettingsUseDrone2Battery_get(swigCPtr);
      SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t_uint8_t__eARCONTROLLER_ERROR ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_ARDrone3Debug_t_uint8_t__eARCONTROLLER_ERROR(cPtr, false);
      return ret;
    } 
  }

  public SWIGTYPE_p_ARCONTROLLER_FEATURE_ARDrone3Debug_Private_t privatePart {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_ARDrone3Debug_t_privatePart_set(swigCPtr, SWIGTYPE_p_ARCONTROLLER_FEATURE_ARDrone3Debug_Private_t.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_ARDrone3Debug_t_privatePart_get(swigCPtr);
      SWIGTYPE_p_ARCONTROLLER_FEATURE_ARDrone3Debug_Private_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_ARCONTROLLER_FEATURE_ARDrone3Debug_Private_t(cPtr, false);
      return ret;
    } 
  }

  public ARCONTROLLER_FEATURE_ARDrone3Debug_t() : this(ARDroneSDK3PINVOKE.new_ARCONTROLLER_FEATURE_ARDrone3Debug_t(), true) {
  }

}