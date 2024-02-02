using System.Runtime.InteropServices;
using System.IO.Compression;


namespace ExperimentalWrapper.Util;

internal enum OperatingSystem
{
    Unknown,
    Windows,
    Linux,
    OSX
}

internal class Env
{
    internal static OperatingSystem Platform
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return OperatingSystem.OSX;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return OperatingSystem.Linux;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return OperatingSystem.Windows;

            return OperatingSystem.Unknown;
        }
    }

    public static string Bin
    {
        get
        {
            var binaryDir = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? Path.Combine(BinDir, Os)
                : Path.Combine(BinDir, $"{Os}-{Arch}");
            if (Directory.Exists(binaryDir))
                Directory.Delete(binaryDir, true);
            Directory.CreateDirectory(binaryDir);
            var compressedBinary = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? Path.Combine(BinDir, $"{Os}.zip")
                : Path.Combine(BinDir, $"{Os}-{Arch}.zip");

            if (!File.Exists(compressedBinary))
                throw new Exception($"Missing dependency: {compressedBinary}");

            var binaryFilePath = Path.Combine(binaryDir, BinName);
            ZipFile.ExtractToDirectory(compressedBinary, binaryDir);
            return binaryFilePath;
        }
    }

    private static string BinDir
    {
        get
        {
            var assemblyDir = Path.GetDirectoryName(typeof(Env).Assembly.Location);
            return Path.Combine(assemblyDir!, "tools", "sd");
        }
    }

    private static string BinName
    {
        get => Platform switch
        {
            OperatingSystem.Windows => "sd.exe",
            _ => "sd"
        };
    }

    private static string Os
    {
        get
        {
            return Platform switch
            {
                OperatingSystem.Windows => "win",
                OperatingSystem.Linux => "linux",
                OperatingSystem.OSX => "osx",
                _ => throw new Exception("Unsupported OS")
            };
        }
    }

    private static string Arch
    {
        get
        {
            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 or Architecture.X86 => "x86",
                Architecture.Arm or Architecture.Arm64 => "arm",
                _ => throw new Exception("Unsupported OS architecture")
            };
        }
    }

    private static void EnsureDependencyExists(string path)
    {
        if (!File.Exists(path))
            throw new Exception($"Missing dependency: {path}");
    }
}