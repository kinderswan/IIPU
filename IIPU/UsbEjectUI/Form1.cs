using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UsbEjectUI
{
	public partial class Form1 : Form
	{
		private List<string> usbs;
        private List<string> devices;
		private UsbDrives usbDrivesService;
 
		public Form1()
		{
			InitializeComponent();
			this.usbs = null;
		    this.devices = null;
			this.usbDrivesService = new UsbDrives();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.RefreshItems();
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			var text = ((ListView) sender).FocusedItem.Text.Split('|')[1].Trim();
		    bool errorDisplayed = false;
		    try
		    {
		        this.usbDrivesService.EjectDrive(text);
		    }
		    catch (Win32Exception)
		    {
                MessageBox.Show(string.Format("{0} disk is being used. Please, try again later", text));
		        errorDisplayed = true;
		    }
		    finally
		    {
		        this.RefreshItems();
		        if (!errorDisplayed)
		        {
                    MessageBox.Show(string.Format("{0} disk has been ejected", text));
		        }
		    }
		}

		private void RefreshItems() {
			this.usbs = this.usbDrivesService.GetUsbDrivesList();
		    this.devices = this.usbDrivesService.MTPDrives();

			this.listView1.Items.Clear();
			foreach (var info in this.usbs.Select(usb => this.usbDrivesService.CreateUsbFieldData(usb)))
			{
				this.listView1.Items.Add(info);
			}

            foreach (var info in this.devices)
            {
                this.listView1.Items.Add(info);
            }

			this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		}
	}
}
