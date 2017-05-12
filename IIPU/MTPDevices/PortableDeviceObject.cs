namespace MTPDevices
{
    public abstract class PortableDeviceObject
    {
        protected PortableDeviceObject(string id, string name, long size, long freeSpace)
        {
            this.Id = id;
            this.Name = name;
            this.TotalSize = size;
            this.FreeSpace = freeSpace;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public long TotalSize { get; private set; }

        public long FreeSpace { get; private set; }
    }
}