// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
class Program
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DEVMODE
    {
        private const int CCHDEVICENAME = 32;
        private const int CCHFORMNAME = 32;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string dmDeviceName;
        public ushort dmSpecVersion;
        public ushort dmDriverVersion;
        public ushort dmSize;
        public ushort dmDriverExtra;
        public uint dmFields;
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
        public uint dmBitsPerPel;
        public uint dmPelsWidth;
        public uint dmPelsHeight;
        public uint dmDisplayFlags;
        public uint dmDisplayFrequency;
        public uint dmICMMethod;
        public uint dmICMIntent;
        public uint dmMediaType;
        public uint dmDitherType;
        public uint dmReserved1;
        public uint dmReserved2;
        public uint dmPanningWidth;
        public uint dmPanningHeight;
    }

    [DllImport("user32.dll", CharSet = CharSet.Ansi)]
    public static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, uint dwflags, IntPtr lParam);

    const int ENUM_CURRENT_SETTINGS = -1;
    const int CDS_UPDATEREGISTRY = 0x01;
    const int DISP_CHANGE_SUCCESSFUL = 0;

    static void Main(string[] args)
    {
        int refreshRate = 0, width = 0, height = 0, bitsPerPixel = 0;

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
            ChangeDisplaySettings(refreshRate, width, height, bitsPerPixel);
            Console.WriteLine("Display settings changed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to change display settings: {ex.Message}");
        }
    }

    static void ChangeDisplaySettings(int refreshRate, int width, int height, int bitsPerPixel)
    {
        DEVMODE dm = new DEVMODE();
        dm.dmSize = (ushort)Marshal.SizeOf(typeof(DEVMODE));

        if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm) != 0)
        {
            if (refreshRate > 0) dm.dmDisplayFrequency = (uint)refreshRate;
            if (width > 0) dm.dmPelsWidth = (uint)width;
            if (height > 0) dm.dmPelsHeight = (uint)height;
            if (bitsPerPixel > 0) dm.dmBitsPerPel = (uint)bitsPerPixel;

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

    [DllImport("user32.dll", CharSet = CharSet.Ansi)]
    public static extern int EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);
}
