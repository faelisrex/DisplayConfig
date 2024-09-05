// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
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

static void ChangeDisplaySettings(int refreshRate, int width, int height, int bitsPerPixel)
{
    var scope = new ManagementScope(@"\\.\root\cimv2");
    var query = new SelectQuery("SELECT * FROM CIM_VideoControllerResolution");

    using (var searcher = new ManagementObjectSearcher(scope, query))
    {
        foreach (ManagementObject obj in searcher.Get())
        {
            if (refreshRate > 0) obj["RefreshRate"] = refreshRate;
            if (width > 0) obj["HorizontalResolution"] = width;
            if (height > 0) obj["VerticalResolution"] = height;
            if (bitsPerPixel > 0) obj["BitsPerPixel"] = bitsPerPixel;
            obj.Put();
        }
    }
}