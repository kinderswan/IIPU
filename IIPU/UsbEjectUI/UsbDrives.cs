using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MTPDevices;
using USBEject;

namespace UsbEjectUI
{
    class UsbDrives
    {
        private List<string> UsbDrivesList { get; set; }
        private VolumeDeviceClass volumeDeviceClass;
        private PortableDeviceCollection devices;

        public UsbDrives()
        {
        }

        public List<string> GetUsbDrivesList()
        {
            this.volumeDeviceClass = new VolumeDeviceClass();
            this.FillUsbDrivesList();
            return this.UsbDrivesList;
        }

        public List<string> MTPDrives()
        {
            this.devices = new PortableDeviceCollection();
            this.devices.Refresh();
            var results = new List<string>();
            foreach (var device in this.devices)
            {
                device.Connect();
                results.Add(this.CreateMTPData(device));
            }
            return results;
        } 

        public void EjectDrive(string name)
        {
            var volume = this.volumeDeviceClass.Devices.FirstOrDefault(vol => ((Volume) vol).LogicalDrive == name);
            if (volume != null)
            {
                volume.Eject(false);
            }
            else
            {
                var device = this.devices.FirstOrDefault(x => x.FriendlyName == name);
                if (device != null)
                {
                    device.Eject();
                    device.Disconnect();
                    
                }
            }
        }

        private void FillUsbDrivesList()
        {
            this.UsbDrivesList = this.volumeDeviceClass.Devices.Where(volume => volume.IsUsb).Select(volume => ((Volume)volume).LogicalDrive).ToList();
        }

        public string CreateUsbFieldData(string usb)
        {
            var info = this.GetUsbInfo(usb);
            return string.Format("Name: | {0} | VolumeLabel: {1} TotalSpace: {2} FreeSpace: {3} TakenSpace: {4}",
                usb,
                info.VolumeLabel,
                info.TotalSize,
                info.AvailableFreeSpace,
                info.TotalSize - info.AvailableFreeSpace);

        }

        private string CreateMTPData(PortableDevice device)
        {
            var contents = device.GetContents();
            long total = 0;
            long free = 0;
            foreach (var file in contents.Files)
            {
                total += file.TotalSize;
                free += file.FreeSpace;
            }
            long taken = total - free;

            return string.Format("Name: | {0} | TotalSpace: {1} FreeSpace: {2} TakenSpace: {3}",
                device.FriendlyName,
                total,
                free,
                total - free);
        }

        private System.IO.DriveInfo GetUsbInfo(string name)
        {
            var drives = System.IO.DriveInfo.GetDrives();
            return drives.FirstOrDefault(x => x.Name == name + @"\");
        }
    }
}
