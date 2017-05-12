using System.Collections.Generic;

namespace MTPDevices
{
    public class PortableDeviceFolder : PortableDeviceObject
    {
        public PortableDeviceFolder(string id, string name, long size, long freeSpace)
            : base(id, name, size, freeSpace)
        {
            this.Files = new List<PortableDeviceObject>();
        }

        public IList<PortableDeviceObject> Files { get; set; }
    }
}