namespace MTPDevices
{
    public class PortableDeviceFile : PortableDeviceObject
    {
        public PortableDeviceFile(string id, string name, long size, long freeSpace)
            : base(id, name, size, freeSpace)
        {        
        }
    }
}