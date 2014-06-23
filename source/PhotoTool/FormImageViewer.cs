using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SAFish.PhotoTool
{
	/// <summary>
	/// Summary description for FormImageViewer.
	/// </summary>
	public class FormImageViewer : System.Windows.Forms.Form
	{

		#region Private Attributes

		private System.Windows.Forms.PictureBox picMain;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Button btnPrev;
		private System.Windows.Forms.Button btnNext;
		private string[] files = null;
		private int index = 0;
		private System.Windows.Forms.Button btnClose;

		#endregion

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormImageViewer(string[] files, int index)
		{
			this.files = files;
            this.index = index;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			DisplayPhoto();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Increment the photo number in the list.
		/// </summary>
		/// <param name="change">Skip to next photo - can be positive or negative.</param>
		private void Increment(int change) 
		{
			index = index + change;
			if (index >= this.files.Length) 
			{
				index = 0;
			}
			else if (index < 0) 
			{
				index = this.files.Length - 1;
			}
		}

		/// <summary>
		/// Method that does the work of showing the image in the picture box.
		/// </summary>
		private void DisplayPhoto() 
		{
            if (picMain.Width <= 0) return;
 
            string file = this.files[this.index];
            byte[] imgData = System.IO.File.ReadAllBytes(file);

            try 
			{
				int len = picMain.Width;
				imgData = ImageUtils.ResizeImage(imgData, len, 90);
				picMain.Image = ImageUtils.ConvertBytesToImage(imgData);
			}
			catch (Exception ex) 
			{
				MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImageViewer));
            this.picMain = new System.Windows.Forms.PictureBox();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // picMain
            // 
            this.picMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picMain.Location = new System.Drawing.Point(0, 0);
            this.picMain.Name = "picMain";
            this.picMain.Size = new System.Drawing.Size(504, 381);
            this.picMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picMain.TabIndex = 0;
            this.picMain.TabStop = false;
            this.picMain.SizeChanged += new System.EventHandler(this.picMain_SizeChanged);
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnClose);
            this.pnlBottom.Controls.Add(this.btnNext);
            this.pnlBottom.Controls.Add(this.btnPrev);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 381);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(504, 40);
            this.pnlBottom.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(440, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(56, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(368, 8);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(64, 23);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Next";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrev.Location = new System.Drawing.Point(296, 8);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(64, 23);
            this.btnPrev.TabIndex = 0;
            this.btnPrev.Text = "Previous";
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // FormImageViewer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(504, 421);
            this.Controls.Add(this.picMain);
            this.Controls.Add(this.pnlBottom);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormImageViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " PhotoTool - Image Viewer";
            this.ResizeEnd += new System.EventHandler(this.FormImageViewer_ResizeEnd);
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void btnNext_Click(object sender, System.EventArgs e)
		{
			Increment(1);
			DisplayPhoto();
		}

		private void btnPrev_Click(object sender, System.EventArgs e)
		{
			Increment(-1);
			DisplayPhoto();
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

        private void FormImageViewer_ResizeEnd(object sender, EventArgs e)
        {
        }

        private void picMain_SizeChanged(object sender, EventArgs e)
        {
            this.DisplayPhoto();
        }

	}
}
