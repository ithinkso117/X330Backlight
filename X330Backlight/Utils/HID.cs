using System.Runtime.InteropServices;

namespace X330Backlight.Utils
{
    public class HID
    {
        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetNumHidDevices", SetLastError = true)]
        public static extern uint GetNumHidDevices(ushort vid, ushort pid);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetHidString", SetLastError = true)]
        public static extern byte GetHidString(uint deviceIndex, ushort vid, ushort pid, byte hidStringType, ref sbyte deviceString, uint deviceStringLength);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetHidIndexedString", SetLastError = true)]
        public static extern byte GetHidIndexedString(uint deviceIndex, ushort vid, ushort pid, uint stringIndex, ref sbyte deviceString, uint deviceStringLength);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetHidAttributes", SetLastError = true)]
        public static extern byte GetHidAttributes(uint deviceIndex, ushort vid, ushort pid, ref ushort deviceVid, ref ushort devicePid, ref ushort deviceReleaseNumber);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetHidGuid", SetLastError = true)]
        public static extern void GetHidGuid(ref sbyte hidGuid);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetHidLibraryVersion", SetLastError = true)]
        public static extern byte GetHidLibraryVersion(ref byte major, ref byte minor, ref int release);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_Open", SetLastError = true)]
        public static extern byte Open(ref uint device, uint deviceIndex, ushort vid, ushort pid, uint numInputBuffers);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_IsOpened", SetLastError = true)]
        public static extern int IsOpened(uint device);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetHandle", SetLastError = true)]
        public static extern uint GetHandle(uint device);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetString", SetLastError = true)]
        public static extern byte GetString(uint device, byte hidStringType, ref sbyte deviceString, uint deviceStringLength);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetIndexedString", SetLastError = true)]
        public static extern byte GetIndexedString(uint device, uint stringIndex, ref sbyte deviceString, uint deviceStringLength);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetAttributes", SetLastError = true)]
        public static extern byte GetAttributes(uint device, ref ushort deviceVid, ref ushort devicePid, ref ushort deviceReleaseNumber);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_SetFeatureReport_Control", SetLastError = true)]
        public static extern byte SetFeatureReport_Control(uint device, ref byte buffer, uint bufferSize);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetFeatureReport_Control", SetLastError = true)]
        public static extern byte GetFeatureReport_Control(uint device, ref byte buffer, uint bufferSize);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_SetOutputReport_Interrupt", SetLastError = true)]
        public static extern byte SetOutputReport_Interrupt(uint device, ref byte buffer, uint bufferSize);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetInputReport_Interrupt", SetLastError = true)]
        public static extern byte GetInputReport_Interrupt(uint device, ref byte buffer, uint bufferSize, uint numReports, ref uint BytesReturned);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_SetOutputReport_Control", SetLastError = true)]
        public static extern byte SetOutputReport_Control(uint device, ref byte buffer, uint bufferSize);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetInputReport_Control", SetLastError = true)]
        public static extern byte GetInputReport_Control(uint device, ref byte buffer, uint bufferSize);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetInputReportBufferLength", SetLastError = true)]
        public static extern ushort GetInputReportBufferLength(uint device);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetOutputReportBufferLength", SetLastError = true)]
        public static extern ushort GetOutputReportBufferLength(uint device);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetFeatureReportBufferLength", SetLastError = true)]
        public static extern ushort GetFeatureReportBufferLength(uint device);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetMaxReportRequest", SetLastError = true)]
        public static extern uint GetMaxReportRequest(uint device);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_FlushBuffers", SetLastError = true)]
        public static extern int FlushBuffers(uint device);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_CancelIo", SetLastError = true)]
        public static extern int CancelIo(uint device);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_GetTimeouts", SetLastError = true)]
        public static extern void GetTimeouts(uint device, ref uint getReportTimeout, ref uint setReportTimeout);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_SetTimeouts", SetLastError = true)]
        public static extern void SetTimeouts(uint device, uint getReportTimeout, uint setReportTimeout);

        [DllImport("SLABHIDDevice.dll", EntryPoint = "HidDevice_Close", SetLastError = true)]
        public static extern byte Close(uint device);
    }
}
