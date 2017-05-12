using System;
using PortableDeviceApiLib;
using PortableDeviceTypesLib;
using _tagpropertykey = PortableDeviceApiLib._tagpropertykey;
using IPortableDeviceKeyCollection = PortableDeviceApiLib.IPortableDeviceKeyCollection;
using IPortableDeviceValues = PortableDeviceApiLib.IPortableDeviceValues;

namespace MTPDevices
{
    public class PortableDevice
    {
        #region Fields

        private bool _isConnected;
        private readonly PortableDeviceClass _device;

        #endregion

        #region ctor(s)

        public PortableDevice(string deviceId)
        {
            this._device = new PortableDeviceClass();
            this.DeviceId = deviceId;
        }

        #endregion

        #region Properties

        public string DeviceId { get; set; }

        public string FriendlyName
        {
            get
            {
                if (!this._isConnected)
                {
                    throw new InvalidOperationException("Not connected to device.");
                }

                // Retrieve the properties of the device
                IPortableDeviceContent content;
                IPortableDeviceProperties properties;
                this._device.Content(out content);
                content.Properties(out properties);

                // Retrieve the values for the properties
                IPortableDeviceValues propertyValues;
                properties.GetValues("DEVICE", null, out propertyValues);

                // Identify the property to retrieve
                var property = new _tagpropertykey();
                property.fmtid = new Guid(0x26D4979A, 0xE643, 0x4626, 0x9E, 0x2B, 0x73, 0x6D, 0xC0, 0xC9, 0x2F, 0xDC);
                property.pid = 8;

                // Retrieve the friendly name
                string propertyValue;
                propertyValues.GetStringValue(ref property, out propertyValue);

                return propertyValue;
            }
        }

        internal PortableDeviceClass PortableDeviceClass
        {
            get
            {
                return this._device;
            }
        }

        #endregion

        #region Methods

        public void Connect()
        {
            if (this._isConnected) { return; }

            var clientInfo = (IPortableDeviceValues) new PortableDeviceValuesClass();
            this._device.Open(this.DeviceId, clientInfo);
            this._isConnected = true;
        }

        public void Disconnect()
        {
            if (!this._isConnected) { return; }
            this._device.Close();
            this._isConnected = false;
        }

         public void Eject()
        {
            IPortableDeviceValues commandValues = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();
            IPortableDeviceValues results;

            var commonCommandCategory = new _tagpropertykey()
            {
                fmtid = new Guid(0xF0422A9C, 0x5DC8, 0x4440, 0xB5, 0xBD, 0x5D, 0xF2, 0x88, 0x35, 0x65, 0x8A),
                pid = 1001
            };

            var commonCommandID = new _tagpropertykey()
            {
                fmtid = new Guid(0xF0422A9C, 0x5DC8, 0x4440, 0xB5, 0xBD, 0x5D, 0xF2, 0x88, 0x35, 0x65, 0x8A),
                pid = 1002
            };

            var ejectCommand = new _tagpropertykey()
            {
                fmtid = new Guid(0xD8F907A6, 0x34CC, 0x45FA, 0x97, 0xFB, 0xD0, 0x07, 0xFA, 0x47, 0xEC, 0x94),
                pid = 4
            };

            commandValues.SetGuidValue(ref commonCommandCategory, ejectCommand.fmtid);
            commandValues.SetUnsignedIntegerValue(ref commonCommandID, ejectCommand.pid);
            ((IPortableDevice)this._device).SendCommand(0, commandValues, out results);
        }

        public PortableDeviceFolder GetContents()
        {
            var root = new PortableDeviceFolder("DEVICE", "DEVICE", 0, 0);

            IPortableDeviceContent content;
            this._device.Content(out content);

            PortableDevice.EnumerateContents(ref content, root);

            return root;
        }

        private static void EnumerateContents(ref IPortableDeviceContent content,
            PortableDeviceFolder parent)
        {
            // Get the properties of the object
            IPortableDeviceProperties properties;
            content.Properties(out properties);

            // Enumerate the items contained by the current object
            IEnumPortableDeviceObjectIDs objectIds;
            content.EnumObjects(0, parent.Id, null, out objectIds);

            uint fetched = 0;
            do
            {
                string objectId;

                objectIds.Next(1, out objectId, ref fetched);
                if (fetched > 0)
                {
                    var currentObject = PortableDevice.WrapObject(properties, objectId);

                    parent.Files.Add(currentObject);
                }
            } while (fetched > 0);
        }


        private static PortableDeviceObject WrapObject(IPortableDeviceProperties properties,
            string objectId)
        {
            IPortableDeviceKeyCollection keys;
            properties.GetSupportedProperties(objectId, out keys);

            IPortableDeviceValues values;
            properties.GetValues(objectId, keys, out values);

            // Get the name of the object
            string name;
            var property = new _tagpropertykey();
            property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                                      0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            property.pid = 4;
            values.GetStringValue(property, out name);

            long size;
            property = new _tagpropertykey();
            property.fmtid = new Guid(0x01A3057A, 0x74D6, 0x4E80, 0xBE, 0xA7, 0xDC, 0x4C, 0x21, 0x2C, 0xE5, 0x0A);
            property.pid = 4;
            values.GetSignedLargeIntegerValue(property, out size);

            long free;
            property = new _tagpropertykey();
            property.fmtid = new Guid(0x01A3057A, 0x74D6, 0x4E80, 0xBE, 0xA7, 0xDC, 0x4C, 0x21, 0x2C, 0xE5, 0x0A);
            property.pid = 5;
            values.GetSignedLargeIntegerValue(property, out free);


            // Get the type of the object
            Guid contentType;
            property = new _tagpropertykey();
            property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                                      0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            property.pid = 7;
            values.GetGuidValue(property, out contentType);

            var folderType = new Guid(0x27E2E392, 0xA111, 0x48E0, 0xAB, 0x0C,
                                      0xE1, 0x77, 0x05, 0xA0, 0x5F, 0x85);
            var functionalType = new Guid(0x99ED0160, 0x17FF, 0x4C44, 0x9D, 0x98,
                                          0x1D, 0x7A, 0x6F, 0x94, 0x19, 0x21);

            if (contentType == folderType || contentType == functionalType)
            {
                return new PortableDeviceFolder(objectId, name, size, free);
            }
            property.pid = 12;//WPD_OBJECT_ORIGINAL_FILE_NAME
            values.GetStringValue(property, out name);
            return new PortableDeviceFile(objectId, name, size, free);
        }

        #endregion
    }
}