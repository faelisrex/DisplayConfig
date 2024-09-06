// See https://aka.ms/new-console-template for more information
using System;
using System.Runtime.InteropServices;

class Program
{
    // 👇 Simple struct DEVMODE in the Win API
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DEVMODE
    {
        // 👇 Constants for the size of the device name and form name
        private const int CCHDEVICENAME = 32;
        private const int CCHFORMNAME = 32;

        // 👇 Device name (string with a fixed size of 32 characters)
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string dmDeviceName;

        // 👇 Version numbers and size information
        public ushort dmSpecVersion;
        public ushort dmDriverVersion;
        public ushort dmSize;
        public ushort dmDriverExtra;

        // 👇 Fields specifying which members of the DEVMODE structure have been initialized
        public uint dmFields;

        // 👇 Display settings
        public uint dmBitsPerPel;
        public uint dmPelsWidth;
        public uint dmPelsHeight;
        public uint dmDisplayFrequency;

        public int dmPositionX;
        public int dmPositionY;
        public uint dmDisplayOrientation;
        public uint dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        public ushort dmLogPixels;
        public uint dmDisplayFlags;
        public uint dmICMMethod;
        public uint dmICMIntent;
        public uint dmMediaType;
        public uint dmDitherType;
        public uint dmReserved1;
        public uint dmReserved2;
        public uint dmPanningWidth;
        public uint dmPanningHeight;
    }

    // 👇 Import the ChangeDisplaySettingsEx function from user32.dll to change display settings
    [DllImport("user32.dll", CharSet = CharSet.Ansi)]
    public static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, uint dwflags, IntPtr lParam);

    // 👇 Constant to specify the current display settings
    const int ENUM_CURRENT_SETTINGS = -1;
    // 👇 Constant to update the registry with the new settings
    const int CDS_UPDATEREGISTRY = 0x01;
    // 👇 Constant to indicate a successful change
    const int DISP_CHANGE_SUCCESSFUL = 0;

    // 👇 Import the EnumDisplaySettings function from user32.dll to get current display settings
    [DllImport("user32.dll", CharSet = CharSet.Ansi)]
    public static extern int EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

    static void Main(string[] args)
    {
        int refreshRate = 0, width = 0, height = 0, bitsPerPixel = 0;

        // 👇 Parse command-line arguments
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-r":
                    if (int.TryParse(args[++i], out refreshRate)) break;
                    Console.WriteLine("Invalid refresh rate.");
                    return;
                case "-w":
                    if (int.TryParse(args[++i], out width)) break;
                    Console.WriteLine("Invalid width.");
                    return;
                case "-h":
                    if (int.TryParse(args[++i], out height)) break;
                    Console.WriteLine("Invalid height.");
                    return;
                case "-b":
                    if (int.TryParse(args[++i], out bitsPerPixel)) break;
                    Console.WriteLine("Invalid bits per pixel.");
                    return;
                default:
                    Console.WriteLine("Usage: Program -r <refreshRate> -w <width> -h <height> -b <bitsPerPixel>");
                    return;
            }
        }

        try
        {
            // 👇 Attempt to change display settings
            ChangeDisplaySettings(refreshRate, width, height, bitsPerPixel);
            Console.WriteLine("Display settings changed successfully.");
        }
        catch (Exception ex)
        {
            // 👇 Handle any errors that occur during the change
            Console.WriteLine($"Failed to change display settings: {ex.Message}");
        }
    }

    static void ChangeDisplaySettings(int refreshRate, int width, int height, int bitsPerPixel)
    {
        DEVMODE dm = new DEVMODE();
        dm.dmSize = (ushort)Marshal.SizeOf(typeof(DEVMODE));

        // 👇 Get the current display settings
        if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm) != 0)
        {
            // 👇 Update the DEVMODE structure with new settings if provided
            if (refreshRate > 0) dm.dmDisplayFrequency = (uint)refreshRate;
            if (width > 0) dm.dmPelsWidth = (uint)width;
            if (height > 0) dm.dmPelsHeight = (uint)height;
            if (bitsPerPixel > 0) dm.dmBitsPerPel = (uint)bitsPerPixel;

            // 👇 Apply the new display settings
            int result = ChangeDisplaySettingsEx(null, ref dm, IntPtr.Zero, CDS_UPDATEREGISTRY, IntPtr.Zero);
            if (result != DISP_CHANGE_SUCCESSFUL)
            {
                throw new Exception("Unable to change display settings.");
            }
        }
        else
        {
            throw new Exception("Unable to enumerate display settings.");
        }
    }
}