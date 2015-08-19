//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.7
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


class ARDroneSDK3PINVOKE {

  protected class SWIGExceptionHelper {

    public delegate void ExceptionDelegate(string message);
    public delegate void ExceptionArgumentDelegate(string message, string paramName);

    static ExceptionDelegate applicationDelegate = new ExceptionDelegate(SetPendingApplicationException);
    static ExceptionDelegate arithmeticDelegate = new ExceptionDelegate(SetPendingArithmeticException);
    static ExceptionDelegate divideByZeroDelegate = new ExceptionDelegate(SetPendingDivideByZeroException);
    static ExceptionDelegate indexOutOfRangeDelegate = new ExceptionDelegate(SetPendingIndexOutOfRangeException);
    static ExceptionDelegate invalidCastDelegate = new ExceptionDelegate(SetPendingInvalidCastException);
    static ExceptionDelegate invalidOperationDelegate = new ExceptionDelegate(SetPendingInvalidOperationException);
    static ExceptionDelegate ioDelegate = new ExceptionDelegate(SetPendingIOException);
    static ExceptionDelegate nullReferenceDelegate = new ExceptionDelegate(SetPendingNullReferenceException);
    static ExceptionDelegate outOfMemoryDelegate = new ExceptionDelegate(SetPendingOutOfMemoryException);
    static ExceptionDelegate overflowDelegate = new ExceptionDelegate(SetPendingOverflowException);
    static ExceptionDelegate systemDelegate = new ExceptionDelegate(SetPendingSystemException);

    static ExceptionArgumentDelegate argumentDelegate = new ExceptionArgumentDelegate(SetPendingArgumentException);
    static ExceptionArgumentDelegate argumentNullDelegate = new ExceptionArgumentDelegate(SetPendingArgumentNullException);
    static ExceptionArgumentDelegate argumentOutOfRangeDelegate = new ExceptionArgumentDelegate(SetPendingArgumentOutOfRangeException);

    [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="SWIGRegisterExceptionCallbacks_ARDroneSDK3")]
    public static extern void SWIGRegisterExceptionCallbacks_ARDroneSDK3(
                                ExceptionDelegate applicationDelegate,
                                ExceptionDelegate arithmeticDelegate,
                                ExceptionDelegate divideByZeroDelegate, 
                                ExceptionDelegate indexOutOfRangeDelegate, 
                                ExceptionDelegate invalidCastDelegate,
                                ExceptionDelegate invalidOperationDelegate,
                                ExceptionDelegate ioDelegate,
                                ExceptionDelegate nullReferenceDelegate,
                                ExceptionDelegate outOfMemoryDelegate, 
                                ExceptionDelegate overflowDelegate, 
                                ExceptionDelegate systemExceptionDelegate);

    [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="SWIGRegisterExceptionArgumentCallbacks_ARDroneSDK3")]
    public static extern void SWIGRegisterExceptionCallbacksArgument_ARDroneSDK3(
                                ExceptionArgumentDelegate argumentDelegate,
                                ExceptionArgumentDelegate argumentNullDelegate,
                                ExceptionArgumentDelegate argumentOutOfRangeDelegate);

    static void SetPendingApplicationException(string message) {
      SWIGPendingException.Set(new global::System.ApplicationException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingArithmeticException(string message) {
      SWIGPendingException.Set(new global::System.ArithmeticException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingDivideByZeroException(string message) {
      SWIGPendingException.Set(new global::System.DivideByZeroException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingIndexOutOfRangeException(string message) {
      SWIGPendingException.Set(new global::System.IndexOutOfRangeException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingInvalidCastException(string message) {
      SWIGPendingException.Set(new global::System.InvalidCastException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingInvalidOperationException(string message) {
      SWIGPendingException.Set(new global::System.InvalidOperationException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingIOException(string message) {
      SWIGPendingException.Set(new global::System.IO.IOException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingNullReferenceException(string message) {
      SWIGPendingException.Set(new global::System.NullReferenceException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingOutOfMemoryException(string message) {
      SWIGPendingException.Set(new global::System.OutOfMemoryException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingOverflowException(string message) {
      SWIGPendingException.Set(new global::System.OverflowException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingSystemException(string message) {
      SWIGPendingException.Set(new global::System.SystemException(message, SWIGPendingException.Retrieve()));
    }

    static void SetPendingArgumentException(string message, string paramName) {
      SWIGPendingException.Set(new global::System.ArgumentException(message, paramName, SWIGPendingException.Retrieve()));
    }
    static void SetPendingArgumentNullException(string message, string paramName) {
      global::System.Exception e = SWIGPendingException.Retrieve();
      if (e != null) message = message + " Inner Exception: " + e.Message;
      SWIGPendingException.Set(new global::System.ArgumentNullException(paramName, message));
    }
    static void SetPendingArgumentOutOfRangeException(string message, string paramName) {
      global::System.Exception e = SWIGPendingException.Retrieve();
      if (e != null) message = message + " Inner Exception: " + e.Message;
      SWIGPendingException.Set(new global::System.ArgumentOutOfRangeException(paramName, message));
    }

    static SWIGExceptionHelper() {
      SWIGRegisterExceptionCallbacks_ARDroneSDK3(
                                applicationDelegate,
                                arithmeticDelegate,
                                divideByZeroDelegate,
                                indexOutOfRangeDelegate,
                                invalidCastDelegate,
                                invalidOperationDelegate,
                                ioDelegate,
                                nullReferenceDelegate,
                                outOfMemoryDelegate,
                                overflowDelegate,
                                systemDelegate);

      SWIGRegisterExceptionCallbacksArgument_ARDroneSDK3(
                                argumentDelegate,
                                argumentNullDelegate,
                                argumentOutOfRangeDelegate);
    }
  }

  protected static SWIGExceptionHelper swigExceptionHelper = new SWIGExceptionHelper();

  public class SWIGPendingException {
    [global::System.ThreadStatic]
    private static global::System.Exception pendingException = null;
    private static int numExceptionsPending = 0;

    public static bool Pending {
      get {
        bool pending = false;
        if (numExceptionsPending > 0)
          if (pendingException != null)
            pending = true;
        return pending;
      } 
    }

    public static void Set(global::System.Exception e) {
      if (pendingException != null)
        throw new global::System.ApplicationException("FATAL: An earlier pending exception from unmanaged code was missed and thus not thrown (" + pendingException.ToString() + ")", e);
      pendingException = e;
      lock(typeof(ARDroneSDK3PINVOKE)) {
        numExceptionsPending++;
      }
    }

    public static global::System.Exception Retrieve() {
      global::System.Exception e = null;
      if (numExceptionsPending > 0) {
        if (pendingException != null) {
          e = pendingException;
          pendingException = null;
          lock(typeof(ARDroneSDK3PINVOKE)) {
            numExceptionsPending--;
          }
        }
      }
      return e;
    }
  }


  protected class SWIGStringHelper {

    public delegate string SWIGStringDelegate(string message);
    static SWIGStringDelegate stringDelegate = new SWIGStringDelegate(CreateString);

    [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="SWIGRegisterStringCallback_ARDroneSDK3")]
    public static extern void SWIGRegisterStringCallback_ARDroneSDK3(SWIGStringDelegate stringDelegate);

    static string CreateString(string cString) {
      return cString;
    }

    static SWIGStringHelper() {
      SWIGRegisterStringCallback_ARDroneSDK3(stringDelegate);
    }
  }

  static protected SWIGStringHelper swigStringHelper = new SWIGStringHelper();


  static ARDroneSDK3PINVOKE() {
  }


  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Error_ToString")]
  public static extern string ARCONTROLLER_Error_ToString(int jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARDISCOVERY_Error_ToString")]
  public static extern string ARDISCOVERY_Error_ToString(int jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_DICTIONARY_Key_GetFeatureFromCommandKey")]
  public static extern int ARCONTROLLER_DICTIONARY_Key_GetFeatureFromCommandKey(int jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_FRAME_DEFAULT_CAPACITY_get")]
  public static extern int ARCONTROLLER_FRAME_DEFAULT_CAPACITY_get();

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_data_set")]
  public static extern void ARCONTROLLER_Frame_t_data_set(global::System.Runtime.InteropServices.HandleRef jarg1, global::System.Runtime.InteropServices.HandleRef jarg2);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_data_get")]
  public static extern global::System.IntPtr ARCONTROLLER_Frame_t_data_get(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_capacity_set")]
  public static extern void ARCONTROLLER_Frame_t_capacity_set(global::System.Runtime.InteropServices.HandleRef jarg1, uint jarg2);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_capacity_get")]
  public static extern uint ARCONTROLLER_Frame_t_capacity_get(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_used_set")]
  public static extern void ARCONTROLLER_Frame_t_used_set(global::System.Runtime.InteropServices.HandleRef jarg1, uint jarg2);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_used_get")]
  public static extern uint ARCONTROLLER_Frame_t_used_get(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_missed_set")]
  public static extern void ARCONTROLLER_Frame_t_missed_set(global::System.Runtime.InteropServices.HandleRef jarg1, uint jarg2);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_missed_get")]
  public static extern uint ARCONTROLLER_Frame_t_missed_get(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_width_set")]
  public static extern void ARCONTROLLER_Frame_t_width_set(global::System.Runtime.InteropServices.HandleRef jarg1, uint jarg2);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_width_get")]
  public static extern uint ARCONTROLLER_Frame_t_width_get(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_height_set")]
  public static extern void ARCONTROLLER_Frame_t_height_set(global::System.Runtime.InteropServices.HandleRef jarg1, uint jarg2);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_height_get")]
  public static extern uint ARCONTROLLER_Frame_t_height_get(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_isIFrame_set")]
  public static extern void ARCONTROLLER_Frame_t_isIFrame_set(global::System.Runtime.InteropServices.HandleRef jarg1, int jarg2);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_isIFrame_get")]
  public static extern int ARCONTROLLER_Frame_t_isIFrame_get(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_available_set")]
  public static extern void ARCONTROLLER_Frame_t_available_set(global::System.Runtime.InteropServices.HandleRef jarg1, int jarg2);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_t_available_get")]
  public static extern int ARCONTROLLER_Frame_t_available_get(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_new_ARCONTROLLER_Frame_t")]
  public static extern global::System.IntPtr new_ARCONTROLLER_Frame_t();

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_delete_ARCONTROLLER_Frame_t")]
  public static extern void delete_ARCONTROLLER_Frame_t(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_New")]
  public static extern global::System.IntPtr ARCONTROLLER_Frame_New(ref eARCONTROLLER_ERROR jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_NewWithCapacity")]
  public static extern global::System.IntPtr ARCONTROLLER_Frame_NewWithCapacity(uint jarg1, ref eARCONTROLLER_ERROR jarg2);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_Delete")]
  public static extern void ARCONTROLLER_Frame_Delete(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_ensureCapacityIsAtLeast")]
  public static extern int ARCONTROLLER_Frame_ensureCapacityIsAtLeast(global::System.Runtime.InteropServices.HandleRef jarg1, uint jarg2, ref eARCONTROLLER_ERROR jarg3);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Frame_SetFree")]
  public static extern int ARCONTROLLER_Frame_SetFree(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARDISCOVERY_Device_New")]
  public static extern global::System.IntPtr ARDISCOVERY_Device_New(ref eARDISCOVERY_ERROR jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARDISCOVERY_Device_Delete")]
  public static extern void ARDISCOVERY_Device_Delete(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARDISCOVERY_Device_InitWifi")]
  public static extern int ARDISCOVERY_Device_InitWifi(global::System.Runtime.InteropServices.HandleRef jarg1, int jarg2, string jarg3, string jarg4, int jarg5);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Device_New")]
  public static extern global::System.IntPtr ARCONTROLLER_Device_New(global::System.Runtime.InteropServices.HandleRef jarg1, ref eARCONTROLLER_ERROR jarg2);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Device_AddStateChangedCallback")]
  public static extern int ARCONTROLLER_Device_AddStateChangedCallback(global::System.Runtime.InteropServices.HandleRef jarg1, System.IntPtr jarg2, global::System.Runtime.InteropServices.HandleRef jarg3);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Device_AddCommandReceivedCallback")]
  public static extern int ARCONTROLLER_Device_AddCommandReceivedCallback(global::System.Runtime.InteropServices.HandleRef jarg1, System.IntPtr jarg2, global::System.Runtime.InteropServices.HandleRef jarg3);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Device_SetVideoReceiveCallback")]
  public static extern int ARCONTROLLER_Device_SetVideoReceiveCallback(global::System.Runtime.InteropServices.HandleRef jarg1, System.IntPtr jarg2, System.IntPtr jarg3, global::System.Runtime.InteropServices.HandleRef jarg4);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Device_Start")]
  public static extern int ARCONTROLLER_Device_Start(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Device_Stop")]
  public static extern int ARCONTROLLER_Device_Stop(global::System.Runtime.InteropServices.HandleRef jarg1);

  [global::System.Runtime.InteropServices.DllImport("ARDroneSDK3", EntryPoint="CSharp_ARCONTROLLER_Device_Delete")]
  public static extern void ARCONTROLLER_Device_Delete(global::System.Runtime.InteropServices.HandleRef jarg1);
}
