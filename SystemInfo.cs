namespace PcPartPrices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management;
    using ByteSizeLib;

    public class SystemInfo
    {
        public Dictionary<string, string> myComputer = new Dictionary<string, string>();
        public List<RAM> memory = new List<RAM>();
        public List<StorageDevice> storageDevices = new List<StorageDevice>();
        public List<GPU> gpus = new List<GPU>();
        public List<SoundDevice> soundDevices = new List<SoundDevice>();
        public List<string> displays = new List<string>();
        public List<int> ramSpeeds = new List<int>();

        public SystemInfo()
        {
            this.GetOperatingSystemInfo();
            this.GetProcessorInfo();
            this.GetMemoryInfo();
            this.GetGPUInfo();
            this.GetMotherboard();
            this.GetStorage();
            this.GetSound();
            this.GetDisplay();

            this.ramSpeeds.Add(333);
            this.ramSpeeds.Add(400);
            this.ramSpeeds.Add(533);
            this.ramSpeeds.Add(667);
            this.ramSpeeds.Add(800);
            this.ramSpeeds.Add(1066);
            this.ramSpeeds.Add(1375);
            this.ramSpeeds.Add(1600);
            this.ramSpeeds.Add(1866);
            this.ramSpeeds.Add(2000);
            this.ramSpeeds.Add(2133);
            this.ramSpeeds.Add(2200);
            this.ramSpeeds.Add(2400);
            this.ramSpeeds.Add(2600);
            this.ramSpeeds.Add(2666);
            this.ramSpeeds.Add(2800);
            this.ramSpeeds.Add(2933);
            this.ramSpeeds.Add(3000);
            this.ramSpeeds.Add(3100);
            this.ramSpeeds.Add(3200);
            this.ramSpeeds.Add(3300);
            this.ramSpeeds.Add(3333);
            this.ramSpeeds.Add(3400);
            this.ramSpeeds.Add(3466);
            this.ramSpeeds.Add(3600);
            this.ramSpeeds.Add(3733);
            this.ramSpeeds.Add(3866);
            this.ramSpeeds.Add(4000);
            this.ramSpeeds.Add(4133);
            this.ramSpeeds.Add(4200);
            this.ramSpeeds.Add(4266);
            this.ramSpeeds.Add(4600);
            this.ramSpeeds.Add(4800);
        }

        public void GetOperatingSystemInfo()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            foreach (ManagementObject obj in mos.Get())
            {
                this.myComputer.Add("os_name", obj["Caption"].ToString());
                this.myComputer.Add("os_arch", obj["OSArchitecture"].ToString());
                this.myComputer.Add("os_version", obj["Version"].ToString());
            }
        }

        public void GetMemoryInfo()
        {
            ManagementObjectSearcher myMemoryObject2 = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
            var total_ram_capacity = 0.0;
            var speed = string.Empty;

            foreach (ManagementObject obj in myMemoryObject2.Get())
            {
                total_ram_capacity += ByteSize.FromBytes(Convert.ToDouble(obj["Capacity"].ToString())).GibiBytes;
                speed = obj["ConfiguredClockSpeed"].ToString();
                RAM stick = new RAM(
                    obj["Manufacturer"].ToString(),
                    ByteSize.FromBytes(Convert.ToDouble(obj["Capacity"].ToString())).GibiBytes.ToString() + "GB",
                    obj["ConfiguredClockSpeed"].ToString(),
                    obj["PartNumber"].ToString());
                this.memory.Add(stick);
            }

            this.myComputer.Add("total_ram_capacity", total_ram_capacity.ToString() + "GB");
            this.myComputer.Add("some_ram_speed", speed);
        }

        public void GetProcessorInfo()
        {
            ManagementObjectSearcher myVideoObject = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (ManagementObject obj in myVideoObject.Get())
            {
                this.myComputer.Add("cpu_name", obj["Name"].ToString());
                this.myComputer.Add("cpu_cores", obj["NumberOfCores"].ToString());
                this.myComputer.Add("cpu_threads", obj["ThreadCount"].ToString());
            }
        }

        public void GetGPUInfo()
        {
            ManagementObjectSearcher myVideoObject = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (ManagementObject obj in myVideoObject.Get())
            {
                GPU gpu = new GPU(obj["Name"].ToString(), obj["DriverVersion"].ToString());
                this.gpus.Add(gpu);
            }
        }

        public void GetMotherboard()
        {
            ManagementObjectSearcher myMotherboardObject = new ManagementObjectSearcher("select * from Win32_BaseBoard");
            foreach (ManagementObject obj in myMotherboardObject.Get())
            {
                this.myComputer.Add("mb_manufacturer", obj["Manufacturer"].ToString());
                this.myComputer.Add("mb_name", obj["Product"].ToString());
            }
        }

        public void GetStorage()
        {
            ManagementObjectSearcher myStorageObject = new ManagementObjectSearcher("select * from Win32_DiskDrive ");
            foreach (ManagementObject obj in myStorageObject.Get())
            {
                if (Convert.ToDouble(obj["Size"]) < 1000000000000)
                {
                    StorageDevice disk = new StorageDevice(
                        obj["Model"].ToString(),
                        Math.Truncate(ByteSize.FromBytes(Convert.ToDouble(obj["Size"])).GigaBytes).ToString() + "GB");
                    this.storageDevices.Add(disk);
                }
                else
                {
                    StorageDevice disk = new StorageDevice(
                        obj["Model"].ToString(),
                        Math.Truncate(ByteSize.FromBytes(Convert.ToDouble(obj["Size"])).TeraBytes).ToString() + "TB");
                    this.storageDevices.Add(disk);
                }
            }
        }

        public void GetSound()
        {
            ManagementObjectSearcher mySoundObject = new ManagementObjectSearcher("select * from Win32_SoundDevice ");
            string[] basic_corp = { "Microsoft", "Realtek", "Intel(R) Corporation", "(Generic USB Audio)", "NVIDIA", "AMD" };
            foreach (ManagementObject obj in mySoundObject.Get())
            {
                if (basic_corp.Contains(obj["Manufacturer"].ToString()))
                {
                }
                else
                {
                    SoundDevice device = new SoundDevice(obj["Manufacturer"].ToString(), obj["ProductName"].ToString());
                    this.soundDevices.Add(device);
                }
            }

            List<SoundDevice> temp = new List<SoundDevice>();
            foreach (SoundDevice device in this.soundDevices)
            {
                temp.Add(device);
            }

            foreach (string sound_device in Win32.GetSoundDevices())
            {
                foreach (SoundDevice t in temp)
                {
                    if (t.name.Equals(sound_device.Split('(', ')')[1], StringComparison.CurrentCultureIgnoreCase))
                    {
                    }
                    else
                    {
                        SoundDevice newdevice = new SoundDevice("unknown", sound_device.Split('(', ')')[1]);
                        this.soundDevices.Add(newdevice);
                    }
                }
            }
        }

        public void GetDisplay()
        {
            foreach (var target in WindowsDisplayAPI.DisplayConfig.PathDisplayTarget.GetDisplayTargets())
            {
                if (target.FriendlyName != null)
                {
                    try
                    {
                        this.displays.Add(target.FriendlyName);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
