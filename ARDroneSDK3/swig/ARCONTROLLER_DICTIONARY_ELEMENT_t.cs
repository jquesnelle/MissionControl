//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.7
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class ARCONTROLLER_DICTIONARY_ELEMENT_t : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal ARCONTROLLER_DICTIONARY_ELEMENT_t(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(ARCONTROLLER_DICTIONARY_ELEMENT_t obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~ARCONTROLLER_DICTIONARY_ELEMENT_t() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          ARDroneSDK3PINVOKE.delete_ARCONTROLLER_DICTIONARY_ELEMENT_t(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public string key {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_DICTIONARY_ELEMENT_t_key_set(swigCPtr, value);
    } 
    get {
      string ret = ARDroneSDK3PINVOKE.ARCONTROLLER_DICTIONARY_ELEMENT_t_key_get(swigCPtr);
      return ret;
    } 
  }

  public ARCONTROLLER_DICTIONARY_ARG_t arguments {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_DICTIONARY_ELEMENT_t_arguments_set(swigCPtr, ARCONTROLLER_DICTIONARY_ARG_t.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = ARDroneSDK3PINVOKE.ARCONTROLLER_DICTIONARY_ELEMENT_t_arguments_get(swigCPtr);
      ARCONTROLLER_DICTIONARY_ARG_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new ARCONTROLLER_DICTIONARY_ARG_t(cPtr, false);
      return ret;
    } 
  }

  public SWIGTYPE_p_UT_hash_handle hh {
    set {
      ARDroneSDK3PINVOKE.ARCONTROLLER_DICTIONARY_ELEMENT_t_hh_set(swigCPtr, SWIGTYPE_p_UT_hash_handle.getCPtr(value));
      if (ARDroneSDK3PINVOKE.SWIGPendingException.Pending) throw ARDroneSDK3PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      SWIGTYPE_p_UT_hash_handle ret = new SWIGTYPE_p_UT_hash_handle(ARDroneSDK3PINVOKE.ARCONTROLLER_DICTIONARY_ELEMENT_t_hh_get(swigCPtr), true);
      if (ARDroneSDK3PINVOKE.SWIGPendingException.Pending) throw ARDroneSDK3PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public ARCONTROLLER_DICTIONARY_ELEMENT_t() : this(ARDroneSDK3PINVOKE.new_ARCONTROLLER_DICTIONARY_ELEMENT_t(), true) {
  }

}
