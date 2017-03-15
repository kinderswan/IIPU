namespace LAB1.DrivesInfo
{
	partial class Lab1UI
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.driveModelsListBox = new System.Windows.Forms.ListBox();
			this.logicalDriveModelsListBox = new System.Windows.Forms.ListBox();
			this.driveModelsInfoListBox = new System.Windows.Forms.ListBox();
			this.logicalDrivesInfoListBox = new System.Windows.Forms.ListBox();
			this.getInfoButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// driveModelsListBox
			// 
			this.driveModelsListBox.FormattingEnabled = true;
			this.driveModelsListBox.Location = new System.Drawing.Point(4, 30);
			this.driveModelsListBox.Name = "driveModelsListBox";
			this.driveModelsListBox.Size = new System.Drawing.Size(140, 134);
			this.driveModelsListBox.TabIndex = 0;
			this.driveModelsListBox.SelectedIndexChanged += new System.EventHandler(this.driveModelsListBox_SelectedIndexChanged);
			// 
			// logicalDriveModelsListBox
			// 
			this.logicalDriveModelsListBox.FormattingEnabled = true;
			this.logicalDriveModelsListBox.Location = new System.Drawing.Point(388, 30);
			this.logicalDriveModelsListBox.Name = "logicalDriveModelsListBox";
			this.logicalDriveModelsListBox.Size = new System.Drawing.Size(100, 134);
			this.logicalDriveModelsListBox.TabIndex = 1;
			this.logicalDriveModelsListBox.SelectedIndexChanged += new System.EventHandler(this.logicalDriveModelsListBox_SelectedIndexChanged);
			// 
			// driveModelsInfoListBox
			// 
			this.driveModelsInfoListBox.FormattingEnabled = true;
			this.driveModelsInfoListBox.Location = new System.Drawing.Point(150, 30);
			this.driveModelsInfoListBox.Name = "driveModelsInfoListBox";
			this.driveModelsInfoListBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.driveModelsInfoListBox.Size = new System.Drawing.Size(230, 134);
			this.driveModelsInfoListBox.TabIndex = 2;
			// 
			// logicalDrivesInfoListBox
			// 
			this.logicalDrivesInfoListBox.FormattingEnabled = true;
			this.logicalDrivesInfoListBox.Location = new System.Drawing.Point(496, 30);
			this.logicalDrivesInfoListBox.Name = "logicalDrivesInfoListBox";
			this.logicalDrivesInfoListBox.Size = new System.Drawing.Size(262, 134);
			this.logicalDrivesInfoListBox.TabIndex = 3;
			// 
			// getInfoButton
			// 
			this.getInfoButton.Location = new System.Drawing.Point(496, 199);
			this.getInfoButton.Name = "getInfoButton";
			this.getInfoButton.Size = new System.Drawing.Size(262, 38);
			this.getInfoButton.TabIndex = 4;
			this.getInfoButton.Text = "Get Info";
			this.getInfoButton.UseVisualStyleBackColor = true;
			this.getInfoButton.Click += new System.EventHandler(this.getInfoButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Drives";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(150, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Drive Info";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(388, 10);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(77, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Drive partitions";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(496, 10);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(66, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Partition Info";
			// 
			// Lab1UI
			// 
			this.ClientSize = new System.Drawing.Size(770, 249);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.getInfoButton);
			this.Controls.Add(this.logicalDrivesInfoListBox);
			this.Controls.Add(this.driveModelsInfoListBox);
			this.Controls.Add(this.logicalDriveModelsListBox);
			this.Controls.Add(this.driveModelsListBox);
			this.Name = "Lab1UI";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox driveModelsListBox;
		private System.Windows.Forms.ListBox logicalDriveModelsListBox;
		private System.Windows.Forms.ListBox driveModelsInfoListBox;
		private System.Windows.Forms.ListBox logicalDrivesInfoListBox;
		private System.Windows.Forms.Button getInfoButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;

	}
}

