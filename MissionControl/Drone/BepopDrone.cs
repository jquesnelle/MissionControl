using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;
using System.Runtime.InteropServices;
using SharpDX.MediaFoundation;
using SharpDX;

namespace MissionControl.Drone
{
    class BepopDrone : IDrone
    {

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void StateChangedCallbackDelegate(eARCONTROLLER_DEVICE_STATE newState, eARCONTROLLER_ERROR error, System.IntPtr customData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CommandReceivedCallbackDelegate(eARCONTROLLER_DICTIONARY_KEY commandKey, System.IntPtr elementDictionary, System.IntPtr customData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DidReceiveFrameCallbackDelegate(System.IntPtr frame, System.IntPtr customData);

        [DllImport("libyuv.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ConvertToARGB(IntPtr sample, int sample_size, IntPtr crop_argb, int argb_stride,
          int crop_x, int crop_y, int src_width, int src_height, int crop_width, int crop_height, int rotation, uint fourcc);

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern IntPtr memcpy(IntPtr dest, IntPtr src, uint count);

        private static readonly Guid CLSID_CMSH264DecoderMFT = Guid.Parse("62CE7E72-4C71-4D20-B15D-452831A87D9D");
        private static readonly Guid CODECAPI_AVDecVideoAcceleration_H264 = Guid.Parse("f7db8a2f-4f48-4ee8-ae31-8b6ebe558ae2");
        private static readonly Guid CODECAPI_AVLowLatencyMode = Guid.Parse("9c27891a-ed7a-40e1-88e8-b22727a024ee");

        private const string BD_IP_ADDRESS = "192.168.42.1";

        private const int BD_DISCOVERY_PORT = 44444;
        private const int BD_VIDEO_WIDTH = 640;
        private const int BD_VIDEO_HEIGHT = 368;
        private const int MF_E_TRANSFORM_NEED_MORE_INPUT = unchecked((int)0xC00D6D72);
        private const int MF_E_TRANSFORM_STREAM_CHANGE = unchecked((int)0xC00D6D61);
        private const uint FOURC_I420 = 0x30323449;

        private StateChangedCallbackDelegate OnStateChanged;
        private CommandReceivedCallbackDelegate OnCommandReceived;
        private DidReceiveFrameCallbackDelegate OnFrameReceived;

        private bool decoderCreated = false;
        private bool waitForIFrame = true;
        private bool isConnected = false;

        private byte[] latestYuvFrame;
        private byte[] rgbPixels;

        private int latestYuvFrameLen = 0;
        private int batteryPercent;

        private ARCONTROLLER_Device_t deviceController;
        private Transform h264;
        private DateTime start;
        private Bitmap videoBitmap;
        private Rectangle sourceRect = new Rectangle(0, 0, 640, 368);
        private Object yuvFrameLock = new Object();

        public event EventHandler VideoFrameReady;


        public bool IsConnected
        {
            get { return isConnected;  }
        }

        public int BatteryPercent
        {
            get { return batteryPercent; }
        }

        public ARCONTROLLER_Device_t DeviceController
        {
            get { return deviceController; }
        }

        public void Connect()
        {
            eARDISCOVERY_ERROR errorDiscovery = eARDISCOVERY_ERROR.ARDISCOVERY_OK;
            eARCONTROLLER_ERROR error = eARCONTROLLER_ERROR.ARCONTROLLER_OK;

            var device = ARDroneSDK3.ARDISCOVERY_Device_New(ref errorDiscovery);
            if (errorDiscovery != eARDISCOVERY_ERROR.ARDISCOVERY_OK)
                return;
            errorDiscovery = ARDroneSDK3.ARDISCOVERY_Device_InitWifi(device, eARDISCOVERY_PRODUCT.ARDISCOVERY_PRODUCT_ARDRONE, "BepopDrone", BD_IP_ADDRESS, BD_DISCOVERY_PORT);
            if (errorDiscovery != eARDISCOVERY_ERROR.ARDISCOVERY_OK)
                return;

            deviceController = ARDroneSDK3.ARCONTROLLER_Device_New(device, ref error);
            if (error != eARCONTROLLER_ERROR.ARCONTROLLER_OK)
                return;

            OnStateChanged = new StateChangedCallbackDelegate(StateChangedCallback);

            error = ARDroneSDK3.ARCONTROLLER_Device_AddStateChangedCallback(deviceController, Marshal.GetFunctionPointerForDelegate(OnStateChanged), ARCONTROLLER_Device_t.getCPtr(deviceController).Handle);
            if (error != eARCONTROLLER_ERROR.ARCONTROLLER_OK)
                return;

            OnCommandReceived = new CommandReceivedCallbackDelegate(CommandReceivedCallback);

            error = ARDroneSDK3.ARCONTROLLER_Device_AddCommandReceivedCallback(deviceController, Marshal.GetFunctionPointerForDelegate(OnCommandReceived), ARCONTROLLER_Device_t.getCPtr(deviceController).Handle);
            if (error != eARCONTROLLER_ERROR.ARCONTROLLER_OK)
                return;

            OnFrameReceived = new DidReceiveFrameCallbackDelegate(DidReceiveFrameCallback);

            error = ARDroneSDK3.ARCONTROLLER_Device_SetVideoReceiveCallback(deviceController, Marshal.GetFunctionPointerForDelegate(OnFrameReceived), System.IntPtr.Zero, IntPtr.Zero);
            if (error != eARCONTROLLER_ERROR.ARCONTROLLER_OK)
                return;

            error = ARDroneSDK3.ARCONTROLLER_Device_Start(deviceController);
            if (error != eARCONTROLLER_ERROR.ARCONTROLLER_OK)
                return;

            StartVideo();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void RenderVideo(RenderTarget target)
        {
            if (videoBitmap == null)
            {
                videoBitmap = new Bitmap(target, new Size2(BD_VIDEO_WIDTH, BD_VIDEO_HEIGHT), new BitmapProperties(new PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Ignore)));
                rgbPixels = new byte[BD_VIDEO_WIDTH * BD_VIDEO_HEIGHT * 4];
            }

            if (latestYuvFrameLen > 0)
            {
                lock (yuvFrameLock)
                {
                    unsafe
                    {
                        fixed (byte* sample = latestYuvFrame)
                        {
                            fixed (byte* crop_argb = rgbPixels)
                            {
                                ConvertToARGB((IntPtr)sample, latestYuvFrameLen, (IntPtr)crop_argb, BD_VIDEO_WIDTH * 4, 0, 0, BD_VIDEO_WIDTH, BD_VIDEO_HEIGHT, BD_VIDEO_WIDTH, BD_VIDEO_HEIGHT, 0, FOURC_I420);
                                videoBitmap.CopyFromMemory((IntPtr)crop_argb, BD_VIDEO_WIDTH * 4);
                            }

                        }
                    }

                    RectangleF destRect = new RectangleF(0, 0, target.Size.Width, target.Size.Height);

                    target.DrawBitmap(videoBitmap, destRect, 1.0f, BitmapInterpolationMode.Linear);
                }
            }

        }

        private void StateChangedCallback(eARCONTROLLER_DEVICE_STATE newState, eARCONTROLLER_ERROR error, System.IntPtr customData)
        {
            switch (newState)
            {
                case eARCONTROLLER_DEVICE_STATE.ARCONTROLLER_DEVICE_STATE_RUNNING:
                    isConnected = true;
                    break;
                case eARCONTROLLER_DEVICE_STATE.ARCONTROLLER_DEVICE_STATE_STOPPING:
                case eARCONTROLLER_DEVICE_STATE.ARCONTROLLER_DEVICE_STATE_STOPPED:
                    isConnected = false;
                    break;

            }
        }

        private bool TryGetSingleIntElement(System.IntPtr elementDictionary, string key, out int val)
        {
            var nativeDictionary = new ARCONTROLLER_DICTIONARY_ELEMENT_t(elementDictionary, false);
            var nativeElement = ARDroneSDK3.GetDictionaryElement(nativeDictionary, ARCONTROLLER_DICTIONARY_SINGLE_KEY);
            var arg = ARDroneSDK3.GetDictionaryArg(nativeElement, key); 

            switch(arg.valueType)
            {
                case eARCONTROLLER_DICTIONARY_VALUE_TYPE.ARCONTROLLER_DICTIONARY_VALUE_TYPE_U8:
                    val = arg.value.U8;
                    break;
                case eARCONTROLLER_DICTIONARY_VALUE_TYPE.ARCONTROLLER_DICTIONARY_VALUE_TYPE_I8:
                    val = arg.value.I8;
                    break;
                case eARCONTROLLER_DICTIONARY_VALUE_TYPE.ARCONTROLLER_DICTIONARY_VALUE_TYPE_U16:
                    val = arg.value.U16;
                    break;
                case eARCONTROLLER_DICTIONARY_VALUE_TYPE.ARCONTROLLER_DICTIONARY_VALUE_TYPE_I16:
                    val = arg.value.I16;
                    break;
                case eARCONTROLLER_DICTIONARY_VALUE_TYPE.ARCONTROLLER_DICTIONARY_VALUE_TYPE_U32:
                    val = (int)arg.value.U32;
                    break;
                case eARCONTROLLER_DICTIONARY_VALUE_TYPE.ARCONTROLLER_DICTIONARY_VALUE_TYPE_ENUM:
                case eARCONTROLLER_DICTIONARY_VALUE_TYPE.ARCONTROLLER_DICTIONARY_VALUE_TYPE_I32:
                    val = arg.value.I32;
                    break;
                default:
                    val = 0;
                    return false;
            }

            return true;
        }

        static string ARCONTROLLER_DICTIONARY_SINGLE_KEY = ARDroneSDK3.ARCONTROLLER_DICTIONARY_SINGLE_KEY;
        static string BATTERY_DICT_KEY = "arcontroller_dictionary_key_common_commonstate_batterystatechanged_percent";

        private void CommandReceivedCallback(eARCONTROLLER_DICTIONARY_KEY commandKey, System.IntPtr elementDictionary, System.IntPtr customData)
        {
            if (elementDictionary.Equals(IntPtr.Zero))
                return;

            switch (commandKey)
            {
                case eARCONTROLLER_DICTIONARY_KEY.ARCONTROLLER_DICTIONARY_KEY_COMMON_COMMONSTATE_BATTERYSTATECHANGED:
                    int batteryLevel;
                    if(TryGetSingleIntElement(elementDictionary, BATTERY_DICT_KEY, out batteryLevel))
                    {

                    }
                    break;
            }

        }

        private void DidReceiveFrameCallback(System.IntPtr frame, System.IntPtr customData)
        {
            if (!decoderCreated)
                return;

            ARCONTROLLER_Frame_t frameData = new ARCONTROLLER_Frame_t(frame, false);

            if (frameData.isIFrame == 1)
            {
                if (waitForIFrame)
                {
                    waitForIFrame = false;
                    start = DateTime.Now; ;
                }
            }
            else if (waitForIFrame)
                return;

            TInputStreamInformation inputInfo = new TInputStreamInformation();
            h264.GetInputStreamInfo(0, out inputInfo);

            int frameDataSize = (int)frameData.used;
            Sample videoSample = MediaFactory.CreateSample();
            MediaBuffer mediaBuffer;
            MediaFactory.CreateAlignedMemoryBuffer(frameDataSize, (inputInfo.CbAlignment > 1) ? inputInfo.CbAlignment - 1 : 0, out mediaBuffer);
            videoSample.AddBuffer(mediaBuffer);
            videoSample.SampleTime = DateTime.Now.Ticks - start.Ticks;
            videoSample.SampleDuration = 330000;

            int maxBufferLen, curBufferLen;
            System.IntPtr unsafeBuffer = mediaBuffer.Lock(out maxBufferLen, out curBufferLen);
            System.IntPtr frameBuffer = frameData.data;
            memcpy(unsafeBuffer, frameBuffer, (uint)frameDataSize);
            mediaBuffer.CurrentLength = frameDataSize;
            mediaBuffer.Unlock();

            h264.ProcessInput(0, videoSample, 0);

            TOutputStreamInformation streamInfo = new TOutputStreamInformation();
            h264.GetOutputStreamInfo(0, out streamInfo);

            Sample outputSample = MediaFactory.CreateSample();
            MediaBuffer outputMediaBuffer;
            MediaFactory.CreateAlignedMemoryBuffer(streamInfo.CbSize, (streamInfo.CbAlignment > 1) ? streamInfo.CbAlignment - 1 : 0, out outputMediaBuffer);
            outputSample.AddBuffer(outputMediaBuffer);

            TOutputDataBuffer[] outputBuffers = { new TOutputDataBuffer() };
            outputBuffers[0].DwStreamID = 0;
            outputBuffers[0].DwStatus = 0;
            outputBuffers[0].PEvents = IntPtr.Zero;
            outputBuffers[0].PSample = outputSample.NativePointer;

            TransformProcessOutputStatus outputStatus;
            try
            {
                h264.ProcessOutput(TransformProcessOutputFlags.None, outputBuffers, out outputStatus);
            }
            catch (SharpDXException ex)
            {
                if (ex.HResult == MF_E_TRANSFORM_STREAM_CHANGE)
                {
                    int i = 0;

                    MediaType availableOut = new MediaType();
                    while (true)
                    {
                        try
                        {
                            if (!h264.TryGetOutputAvailableType(0, i++, out availableOut))
                                break;
                            Guid subType = (Guid)availableOut.Get(MediaTypeAttributeKeys.Subtype.Guid);
                            int interlaceMode = availableOut.Get(MediaTypeAttributeKeys.InterlaceMode);
                            long size = availableOut.Get(MediaTypeAttributeKeys.FrameSize);
                            System.Diagnostics.Debug.Print(String.Format("MajorType: {0}, SubType: {1}, FrameSize: {2}x{3}, InterlaceMode: {4}",
                                availableOut.MajorType.ToString(), subType.ToString(), Convert.ToString(size >> 32), Convert.ToString(size & 0xffffffff), Convert.ToString(interlaceMode)
                                ));
                        }
                        catch (SharpDXException ex2)
                        {
                            break;
                        }
                    }

                }
            }

            Sample finalMediaSample = new Sample(outputBuffers[0].PSample);
            MediaBuffer finalOutputBuffer = finalMediaSample.ConvertToContiguousBuffer();
            int decodedLen = finalOutputBuffer.CurrentLength;
            if (decodedLen > 0)
            {
                System.IntPtr decodedData = finalOutputBuffer.Lock(out maxBufferLen, out curBufferLen);
                lock (yuvFrameLock)
                {
                    if (latestYuvFrame == null || latestYuvFrame.Length < decodedLen)
                        latestYuvFrame = new byte[decodedLen];
                    Marshal.Copy(decodedData, latestYuvFrame, 0, decodedLen);
                    latestYuvFrameLen = decodedLen;
                }
                finalOutputBuffer.Unlock();
            }
            finalOutputBuffer.Dispose();


            outputSample.Dispose();
            videoSample.Dispose();
            mediaBuffer.Dispose();
            outputMediaBuffer.Dispose();

            if (decodedLen > 0 && VideoFrameReady != null)
                VideoFrameReady(this, EventArgs.Empty);

        }

        private static ulong PackLong(uint a, uint b)
        {
            ulong ret = a;
            ret <<= 32;
            ret |= b;
            return ret;
        }


        private void StartVideo()
        {

            h264 = new SharpDX.MediaFoundation.Transform(CLSID_CMSH264DecoderMFT);

            MediaType inputType = new MediaType();
            inputType.Set(MediaTypeAttributeKeys.MajorType.Guid, MediaTypeGuids.Video);
            inputType.Set(MediaTypeAttributeKeys.Subtype.Guid, VideoFormatGuids.H264);
            inputType.Set(MediaTypeAttributeKeys.FrameSize.Guid, PackLong(640, 368));
            inputType.Set(MediaTypeAttributeKeys.FrameRate.Guid, PackLong(30, 1));
            inputType.Set(MediaTypeAttributeKeys.PixelAspectRatio.Guid, PackLong(1, 1));
            inputType.Set(MediaTypeAttributeKeys.InterlaceMode.Guid, VideoInterlaceMode.MixedInterlaceOrProgressive);
            h264.SetInputType(0, inputType, 0);

            MediaType outputType = new MediaType();
            outputType.Set(MediaTypeAttributeKeys.MajorType.Guid, MediaTypeGuids.Video);
            outputType.Set(MediaTypeAttributeKeys.Subtype.Guid, VideoFormatGuids.I420);
            outputType.Set(MediaTypeAttributeKeys.FrameSize.Guid, PackLong(640, 368));
            outputType.Set(MediaTypeAttributeKeys.FrameRate.Guid, PackLong(30, 1));
            outputType.Set(MediaTypeAttributeKeys.PixelAspectRatio.Guid, PackLong(1, 1));
            outputType.Set(MediaTypeAttributeKeys.InterlaceMode.Guid, VideoInterlaceMode.Progressive);
            h264.SetOutputType(0, outputType, 0);

            h264.Attributes.Set<uint>(CODECAPI_AVDecVideoAcceleration_H264, 1);
            h264.Attributes.Set<uint>(CODECAPI_AVLowLatencyMode, 1);

            int inputStatus;
            h264.GetInputStatus(0, out inputStatus);

            if (inputStatus == (int)MftInputStatusFlags.MftInputStatusAcceptData)
            {
                h264.ProcessMessage(TMessageType.NotifyBeginStreaming, IntPtr.Zero);
                h264.ProcessMessage(TMessageType.NotifyStartOfStream, IntPtr.Zero);
            }

            decoderCreated = true;
        }

    }
}
