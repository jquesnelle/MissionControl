//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.7
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class ARCONTROLLER_FEATURE_JumpingSumoDebug_t : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal ARCONTROLLER_FEATURE_JumpingSumoDebug_t(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(ARCONTROLLER_FEATURE_JumpingSumoDebug_t obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~ARCONTROLLER_FEATURE_JumpingSumoDebug_t() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          ARDroneSDK3PINVOKE.delete_ARCONTROLLER_FEATURE_JumpingSumoDebug_t(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_int8_t__eARCONTROLLER_ERROR sendJumpSetJumpMotor {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_sendJumpSetJumpMotor_set(swigCPtr, SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_int8_t__eARCONTROLLER_ERROR.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_sendJumpSetJumpMotor_get(swigCPtr);
      SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_int8_t__eARCONTROLLER_ERROR ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_int8_t__eARCONTROLLER_ERROR(cPtr, false);
      return ret;
    } 
  }

  public SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_int8_t__eARCONTROLLER_ERROR sendJumpSetCameraOrientation {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_sendJumpSetCameraOrientation_set(swigCPtr, SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_int8_t__eARCONTROLLER_ERROR.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_sendJumpSetCameraOrientation_get(swigCPtr);
      SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_int8_t__eARCONTROLLER_ERROR ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_int8_t__eARCONTROLLER_ERROR(cPtr, false);
      return ret;
    } 
  }

  public SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_p_char__eARCONTROLLER_ERROR sendAudioPlaySoundWithName {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_sendAudioPlaySoundWithName_set(swigCPtr, SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_p_char__eARCONTROLLER_ERROR.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_sendAudioPlaySoundWithName_get(swigCPtr);
      SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_p_char__eARCONTROLLER_ERROR ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_p_char__eARCONTROLLER_ERROR(cPtr, false);
      return ret;
    } 
  }

  public SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t__eARCONTROLLER_ERROR sendMiscDebugEvent {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_sendMiscDebugEvent_set(swigCPtr, SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t__eARCONTROLLER_ERROR.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_sendMiscDebugEvent_get(swigCPtr);
      SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t__eARCONTROLLER_ERROR ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t__eARCONTROLLER_ERROR(cPtr, false);
      return ret;
    } 
  }

  public System.IntPtr sendAnimationPlayAnimation {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_sendAnimationPlayAnimation_set(swigCPtr, new global::System.Runtime.InteropServices.HandleRef(null, value));
    } get{System.IntPtr res = ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_sendAnimationPlayAnimation_get(swigCPtr); ; return res;}
  }

  public SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_float__eARCONTROLLER_ERROR sendAnimationAddCapOffset {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_sendAnimationAddCapOffset_set(swigCPtr, SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_float__eARCONTROLLER_ERROR.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_sendAnimationAddCapOffset_get(swigCPtr);
      SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_float__eARCONTROLLER_ERROR ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t_float__eARCONTROLLER_ERROR(cPtr, false);
      return ret;
    } 
  }

  public SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t__eARCONTROLLER_ERROR sendUserScriptUserScriptUploaded {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_sendUserScriptUserScriptUploaded_set(swigCPtr, SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t__eARCONTROLLER_ERROR.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_sendUserScriptUserScriptUploaded_get(swigCPtr);
      SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t__eARCONTROLLER_ERROR ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_f_p_struct_ARCONTROLLER_FEATURE_JumpingSumoDebug_t__eARCONTROLLER_ERROR(cPtr, false);
      return ret;
    } 
  }

  public SWIGTYPE_p_ARCONTROLLER_FEATURE_JumpingSumoDebug_Private_t privatePart {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_privatePart_set(swigCPtr, SWIGTYPE_p_ARCONTROLLER_FEATURE_JumpingSumoDebug_Private_t.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = ARDroneSDK3PINVOKE.ARCONTROLLER_FEATURE_JumpingSumoDebug_t_privatePart_get(swigCPtr);
      SWIGTYPE_p_ARCONTROLLER_FEATURE_JumpingSumoDebug_Private_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_ARCONTROLLER_FEATURE_JumpingSumoDebug_Private_t(cPtr, false);
      return ret;
    } 
  }

  public ARCONTROLLER_FEATURE_JumpingSumoDebug_t() : this(ARDroneSDK3PINVOKE.new_ARCONTROLLER_FEATURE_JumpingSumoDebug_t(), true) {
  }

}