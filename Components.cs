using System.Runtime.InteropServices;

namespace PcPartPrices
{
    public class Win32
    {
        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutGetNumDevs();

        [DllImport("winmm.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint waveOutGetDevCaps(uint hwo, ref WAVEOUTCAPS pwoc, uint cbwoc);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WAVEOUTCAPS
        {
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public uint dwFormats;
            public ushort wChannels;
            public ushort wReserved1;
            public uint dwSupport;
        }

        public static string[] GetSoundDevices()
        {
            uint devices = waveOutGetNumDevs();
            string[] result = new string[devices];
            WAVEOUTCAPS caps = new WAVEOUTCAPS();

            for (uint i = 0; i < devices; i++)
            {
                waveOutGetDevCaps(i, ref caps, (uint)Marshal.SizeOf(caps));
                result[i] = caps.szPname;
            }

            return result;
        }
    }

    public class RAM
    {
        public string Manufacturer;
        public string Capacity;
        public string ConfiguredClockSpeed;
        public string PartNumber;

        public RAM(string m, string c, string cs, string p)
        {
            this.Manufacturer = m;
            this.Capacity = c;
            this.ConfiguredClockSpeed = cs;
            this.PartNumber = p;
        }

        public string ToURLString()
        {
            return this.Capacity + "%20" + this.ConfiguredClockSpeed;
        }
    }

    public class GPU
    {
        public string name;
        public string driver;

        public GPU(string n, string d)
        {
            name = n;
            driver = d;

        }

        public string ToURLString()
        {
            return name;
        }
    }

    public class StorageDevice
    {
        public string model;
        public string size;


        public StorageDevice(string m, string s)
        {
            this.model = m;
            this.size = s;

        }

        public string ToURLString()
        {
            return this.model + "%20" + this.size;

        }

    }

    public class SoundDevice
    {
        public string manufacturer;
        public string name;

        public SoundDevice(string m, string n)
        {
            this.manufacturer = m;
            this.name = n;
        }

        public string ToURLString()
        {
            return this.name;

        }
    }

}
