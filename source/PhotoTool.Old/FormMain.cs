using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Linq;
using Microsoft.Win32;
using PhotoTool.Properties;
using System.Collections.Generic;
using System.ComponentModel;
using PhotoTool.Models;
using ImageMagick;
using PhotoTool.Logging;

namespace SAFish.PhotoTool
{
	/// <summary>
	/// The main form for the application.
	/// </summary>
	public class FormMain : System.Windows.Forms.Form
	{

		#region Private Attributes

		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.Button btnAddDir;
		private System.Windows.Forms.Button btnAddFile;
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.Button btnRem;
		private System.Windows.Forms.PictureBox picBox;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ListView listView;
		private ArrayList alFiles = null;
		private Bitmap bBlank = null;
        private Bitmap bLoadingPreview = null;
        private Settings settings = null;
		private System.Windows.Forms.GroupBox gbImageOptions;
		private System.Windows.Forms.Label lblImgLen;
		private System.Windows.Forms.Label lblThumbLen;
		private System.Windows.Forms.TextBox txtImgLen;
		private System.Windows.Forms.TextBox txtThumbLen;
		private System.Windows.Forms.CheckBox cbGenThumbs;
		private System.Windows.Forms.Label lblQuality;
		private System.Windows.Forms.TextBox txtImgQuality;
		private System.Windows.Forms.CheckBox cbReplaceSpaces;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem miFile;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem miFileExit;
		private System.Windows.Forms.MenuItem miProjNew;
		private System.Windows.Forms.MenuItem miProjOpen;
		private System.Windows.Forms.MenuItem miProjSave;
		private System.Windows.Forms.MenuItem miProjSaveAs;
		private System.Windows.Forms.MenuItem miFileAddFile;
		private System.Windows.Forms.MenuItem miProj;
		private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem miFileAddDir;
        private System.ComponentModel.IContainer components;

		#endregion

        private BackgroundWorker bgwFileLoader = null;
        private BackgroundWorker bgwImageProcessor = null;
        private ProgressDialog dlgProgress = null;
        private Label lblWarningMessage;
        private ImageService _imageService;
        private LogService _logService;

		#region Constructors

		/// <summary>
		/// Constructor for class FormMain.
		/// </summary>
		public FormMain(LogService logService, ImageService imageService)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			alFiles = new ArrayList();
            _logService = logService;
            _imageService = imageService;

            // create empty bitmap
            bBlank = _imageService.CreateBlankImage("No image selected.", picBox.Width, picBox.Height);
            bLoadingPreview = _imageService.CreateBlankImage("Loading preview...", picBox.Width, picBox.Height);

            picBox.Image = bBlank;

			// set form settings
            this.Text = " PhotoTool " + FormMain.Version;

			// load settings here - if any errors occur loading errors the app will never start
			settings = Settings.Access();
			this.Left = settings.FormMainLeftPos;
			this.Height = settings.FormMainHeight;
			this.Top = settings.FormMainTopPos;
			this.Width = settings.FormMainWidth;
			// image options
			txtImgLen.Text = settings.ImageLength.ToString();
			txtThumbLen.Text = settings.ThumbnailLength.ToString();
			cbGenThumbs.Checked = settings.GenerateThumbnails;
			cbReplaceSpaces.Checked = settings.ReplaceSpaces;
			txtImgQuality.Text = settings.ImageQuality.ToString();

            // initialise the background workers
            this.bgwFileLoader = new BackgroundWorker();
            this.bgwFileLoader.WorkerReportsProgress = true;
            this.bgwFileLoader.WorkerSupportsCancellation = true;
            this.bgwFileLoader.ProgressChanged += bgwFileLoader_ProgressChanged;
            this.bgwFileLoader.RunWorkerCompleted += bgwFileLoader_RunWorkerCompleted;
            this.bgwFileLoader.DoWork += bgwFileLoader_DoWork;

            this.bgwImageProcessor = new BackgroundWorker();
            this.bgwImageProcessor.WorkerReportsProgress = true;
            this.bgwImageProcessor.WorkerSupportsCancellation = true;
            this.bgwImageProcessor.ProgressChanged += bgwImageProcessor_ProgressChanged;
            this.bgwImageProcessor.RunWorkerCompleted += bgwImageProcessor_RunWorkerCompleted;
            this.bgwImageProcessor.DoWork += bgwImageProcessor_DoWork;

		}


		#endregion

		#region Public Static Properties and Methods

		/// <summary>
		/// Gets the path that the application executable is sitting in.
		/// </summary>
		public static string AppPath 
		{
			get 
			{
				string path = Application.ExecutablePath;
				return path.Substring(0, path.LastIndexOf("\\") + 1);
			}
		}

		/// <summary>
		/// Returns the current version number of the application as a formatted 
		/// string.
		/// </summary>
		public static string Version 
		{
			get 
			{
				string version = Application.ProductVersion;
				return version.Substring(0, version.Length - 2);
			}
		}
		#endregion

		#region Private Methods

		/// <summary>
		/// Adds a directory.
		/// </summary>
		private void AddDirectory() 
		{
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.SelectedPath = settings.FolderAddDirectory;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    this.dlgProgress = new ProgressDialog();
                    this.dlgProgress.Text = "Scanning folder...";
                    BackgroundWorker bgw = new BackgroundWorker();
                    bgw.DoWork += delegate(object sender, DoWorkEventArgs e)
                    {
                        string folder = (string)e.Argument;
                        DirectoryInfo di = new DirectoryInfo(folder);
                        e.Result = di.GetFiles().Select(x => x.FullName).ToArray();
                    };
                    bgw.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
                    {
                        this.dlgProgress.Dispose();
                        string[] files = (string[])e.Result;
                        if (files.Length > 0)
                        {
                            LoadFiles(files);
                        }
                    };
                    bgw.RunWorkerAsync(dlg.SelectedPath);
                    settings.FolderAddDirectory = dlg.SelectedPath;
                    
                }
            }
        
		}

		/// <summary>
		/// Method used to add a file - displays a file explorer dialog box.
		/// </summary>
		private void AddFile() 
		{
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.CheckFileExists = true;
                dlg.CheckPathExists = true;
                dlg.Multiselect = true;
                dlg.InitialDirectory = settings.FolderAddFile;
                dlg.Filter = "Image Files(*.bmp;*.jpg;*.jpeg;*.gif;*.png;*.heic)|*.bmp;*.jpg;*.jpeg;*.gif;*.png;*.heic|All files (*.*)|*.*";
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    this.LoadFiles(dlg.FileNames);
                }
            }
		}

        private void LoadFiles(string[] files)
        {
            FileLoadInfo fli = new FileLoadInfo();
            fli.Files = files;
            fli.ReportErrors = true;

            // settings.FolderAddFile = files[0].Substring(0, files[0].LastIndexOf("\\"));

            this.dlgProgress = new ProgressDialog();
            bgwFileLoader.RunWorkerAsync(fli);

            this.dlgProgress.ShowDialog(this);
        }

		private void RemoveSelectedFiles() 
		{
			foreach (ListViewItem item in listView.SelectedItems) 
			{
				listView.Items.Remove(item);
				this.alFiles.Remove(item.SubItems[0].Text);
			}

            ReloadWarningMessage();
        }

		private string ValidateInput() 
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			string prefix = "\n* ";
			int imgLen = -1;
			int thumbLen = -1;
			int imgQuality = -1;
			// make sure some files have been selected
			if (this.listView.Items.Count == 0) 
			{
				sb.Append(prefix);
				sb.Append("You need to select at least one image to resize.");
			}
			// check that values have been entered for all fields
			if (txtImgLen.Text.Trim().Length == 0 ||
				txtThumbLen.Text.Trim().Length == 0 ||
				txtImgQuality.Text.Trim().Length == 0) 
			{
				sb.Append(prefix);
				sb.Append("You need to enter an image length, a thumbnail length and an image quality.");
			}
			else 
			{
				// make sure all numbers have been entered correctly
				try 
				{
					imgLen = Convert.ToInt32(txtImgLen.Text);
					thumbLen = Convert.ToInt32(txtThumbLen.Text);
					imgQuality = Convert.ToInt32(txtImgQuality.Text);
				}
				catch (Exception) 
				{
					sb.Append(prefix);
					sb.Append("You need to enter numeric values for image length, thumbnail length and image quality.");
				}
			}
			// check number ranges
			if (imgLen < 0 ||
				thumbLen < 1 || thumbLen > 999 ||
				imgQuality < 0 || imgQuality > 100) 
			{
				sb.Append(prefix);
				sb.Append("Image length must be greater than -1.");
				sb.Append(prefix);
				sb.Append("Thumbnail length must be in the range 1 - 999.");
				sb.Append(prefix);
				sb.Append("Image quality must be in the range 0 - 100.");
			}
			return sb.ToString();

		}

		#endregion

		#region Overrides

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAddDir = new System.Windows.Forms.Button();
            this.btnAddFile = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.picBox = new System.Windows.Forms.PictureBox();
            this.btnRem = new System.Windows.Forms.Button();
            this.gbImageOptions = new System.Windows.Forms.GroupBox();
            this.txtImgQuality = new System.Windows.Forms.TextBox();
            this.lblQuality = new System.Windows.Forms.Label();
            this.cbReplaceSpaces = new System.Windows.Forms.CheckBox();
            this.cbGenThumbs = new System.Windows.Forms.CheckBox();
            this.txtThumbLen = new System.Windows.Forms.TextBox();
            this.txtImgLen = new System.Windows.Forms.TextBox();
            this.lblThumbLen = new System.Windows.Forms.Label();
            this.lblImgLen = new System.Windows.Forms.Label();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.miFile = new System.Windows.Forms.MenuItem();
            this.miFileAddFile = new System.Windows.Forms.MenuItem();
            this.miFileAddDir = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.miFileExit = new System.Windows.Forms.MenuItem();
            this.miProj = new System.Windows.Forms.MenuItem();
            this.miProjNew = new System.Windows.Forms.MenuItem();
            this.miProjOpen = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.miProjSave = new System.Windows.Forms.MenuItem();
            this.miProjSaveAs = new System.Windows.Forms.MenuItem();
            this.lblWarningMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).BeginInit();
            this.gbImageOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.AllowDrop = true;
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 80);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(518, 408);
            this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            this.listView.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView_DragDrop);
            this.listView.DragEnter += new System.Windows.Forms.DragEventHandler(this.listView_DragEnter);
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            this.listView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_KeyUp);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Image Location";
            this.columnHeader1.Width = 280;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Width";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Height";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnAddDir
            // 
            this.btnAddDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddDir.Image = global::PhotoTool.Properties.Resources.add;
            this.btnAddDir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddDir.Location = new System.Drawing.Point(526, 80);
            this.btnAddDir.Name = "btnAddDir";
            this.btnAddDir.Size = new System.Drawing.Size(128, 23);
            this.btnAddDir.TabIndex = 2;
            this.btnAddDir.Text = "Add Directory";
            this.btnAddDir.Click += new System.EventHandler(this.btnAddDir_Click);
            // 
            // btnAddFile
            // 
            this.btnAddFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddFile.Image = global::PhotoTool.Properties.Resources.folder;
            this.btnAddFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddFile.Location = new System.Drawing.Point(526, 112);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(128, 23);
            this.btnAddFile.TabIndex = 3;
            this.btnAddFile.Text = "Add File";
            this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGo.Image = global::PhotoTool.Properties.Resources.go;
            this.btnGo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGo.Location = new System.Drawing.Point(526, 464);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(128, 23);
            this.btnGo.TabIndex = 4;
            this.btnGo.Text = "Go";
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // picBox
            // 
            this.picBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBox.Location = new System.Drawing.Point(526, 176);
            this.picBox.Name = "picBox";
            this.picBox.Size = new System.Drawing.Size(128, 128);
            this.picBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picBox.TabIndex = 5;
            this.picBox.TabStop = false;
            // 
            // btnRem
            // 
            this.btnRem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRem.Image = global::PhotoTool.Properties.Resources.remove;
            this.btnRem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRem.Location = new System.Drawing.Point(526, 144);
            this.btnRem.Name = "btnRem";
            this.btnRem.Size = new System.Drawing.Size(128, 23);
            this.btnRem.TabIndex = 6;
            this.btnRem.Text = "Remove";
            this.btnRem.Click += new System.EventHandler(this.btnRem_Click);
            // 
            // gbImageOptions
            // 
            this.gbImageOptions.Controls.Add(this.txtImgQuality);
            this.gbImageOptions.Controls.Add(this.lblQuality);
            this.gbImageOptions.Controls.Add(this.cbReplaceSpaces);
            this.gbImageOptions.Controls.Add(this.cbGenThumbs);
            this.gbImageOptions.Controls.Add(this.txtThumbLen);
            this.gbImageOptions.Controls.Add(this.txtImgLen);
            this.gbImageOptions.Controls.Add(this.lblThumbLen);
            this.gbImageOptions.Controls.Add(this.lblImgLen);
            this.gbImageOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbImageOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbImageOptions.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gbImageOptions.Location = new System.Drawing.Point(0, 0);
            this.gbImageOptions.Name = "gbImageOptions";
            this.gbImageOptions.Size = new System.Drawing.Size(662, 72);
            this.gbImageOptions.TabIndex = 7;
            this.gbImageOptions.TabStop = false;
            this.gbImageOptions.Text = "Image Options";
            // 
            // txtImgQuality
            // 
            this.txtImgQuality.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtImgQuality.Location = new System.Drawing.Point(576, 20);
            this.txtImgQuality.MaxLength = 3;
            this.txtImgQuality.Name = "txtImgQuality";
            this.txtImgQuality.Size = new System.Drawing.Size(32, 20);
            this.txtImgQuality.TabIndex = 7;
            this.txtImgQuality.Text = "85";
            // 
            // lblQuality
            // 
            this.lblQuality.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQuality.Location = new System.Drawing.Point(456, 24);
            this.lblQuality.Name = "lblQuality";
            this.lblQuality.Size = new System.Drawing.Size(120, 16);
            this.lblQuality.TabIndex = 6;
            this.lblQuality.Text = "Image quality (0 - 100):";
            // 
            // cbReplaceSpaces
            // 
            this.cbReplaceSpaces.Checked = true;
            this.cbReplaceSpaces.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbReplaceSpaces.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbReplaceSpaces.Location = new System.Drawing.Point(264, 48);
            this.cbReplaceSpaces.Name = "cbReplaceSpaces";
            this.cbReplaceSpaces.Size = new System.Drawing.Size(192, 16);
            this.cbReplaceSpaces.TabIndex = 5;
            this.cbReplaceSpaces.Text = "Replace spaces with underscores";
            // 
            // cbGenThumbs
            // 
            this.cbGenThumbs.Checked = true;
            this.cbGenThumbs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGenThumbs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbGenThumbs.Location = new System.Drawing.Point(264, 24);
            this.cbGenThumbs.Name = "cbGenThumbs";
            this.cbGenThumbs.Size = new System.Drawing.Size(128, 16);
            this.cbGenThumbs.TabIndex = 4;
            this.cbGenThumbs.Text = "Generate thumbnails";
            // 
            // txtThumbLen
            // 
            this.txtThumbLen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtThumbLen.Location = new System.Drawing.Point(216, 44);
            this.txtThumbLen.MaxLength = 3;
            this.txtThumbLen.Name = "txtThumbLen";
            this.txtThumbLen.Size = new System.Drawing.Size(32, 20);
            this.txtThumbLen.TabIndex = 3;
            this.txtThumbLen.Text = "120";
            // 
            // txtImgLen
            // 
            this.txtImgLen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtImgLen.Location = new System.Drawing.Point(216, 20);
            this.txtImgLen.MaxLength = 4;
            this.txtImgLen.Name = "txtImgLen";
            this.txtImgLen.Size = new System.Drawing.Size(32, 20);
            this.txtImgLen.TabIndex = 2;
            this.txtImgLen.Text = "800";
            // 
            // lblThumbLen
            // 
            this.lblThumbLen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThumbLen.Location = new System.Drawing.Point(8, 48);
            this.lblThumbLen.Name = "lblThumbLen";
            this.lblThumbLen.Size = new System.Drawing.Size(216, 16);
            this.lblThumbLen.TabIndex = 1;
            this.lblThumbLen.Text = "Thumbnail length (longest side) in pixels:";
            // 
            // lblImgLen
            // 
            this.lblImgLen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImgLen.Location = new System.Drawing.Point(8, 24);
            this.lblImgLen.Name = "lblImgLen";
            this.lblImgLen.Size = new System.Drawing.Size(268, 24);
            this.lblImgLen.TabIndex = 0;
            this.lblImgLen.Text = "Longest side in pixels (0 for original):";
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miFile,
            this.miProj});
            // 
            // miFile
            // 
            this.miFile.Index = 0;
            this.miFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miFileAddFile,
            this.miFileAddDir,
            this.menuItem2,
            this.miFileExit});
            this.miFile.Text = "&File";
            // 
            // miFileAddFile
            // 
            this.miFileAddFile.Index = 0;
            this.miFileAddFile.Text = "Add &File";
            this.miFileAddFile.Click += new System.EventHandler(this.miFileAddFile_Click);
            // 
            // miFileAddDir
            // 
            this.miFileAddDir.Index = 1;
            this.miFileAddDir.Text = "Add &Directory";
            this.miFileAddDir.Click += new System.EventHandler(this.miFileAddDir_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 2;
            this.menuItem2.Text = "-";
            // 
            // miFileExit
            // 
            this.miFileExit.Index = 3;
            this.miFileExit.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
            this.miFileExit.Text = "E&xit";
            this.miFileExit.Click += new System.EventHandler(this.miFileExit_Click);
            // 
            // miProj
            // 
            this.miProj.Index = 1;
            this.miProj.Text = "";
            // 
            // miProjNew
            // 
            this.miProjNew.Index = -1;
            this.miProjNew.Text = "";
            // 
            // miProjOpen
            // 
            this.miProjOpen.Index = -1;
            this.miProjOpen.Text = "";
            // 
            // menuItem1
            // 
            this.menuItem1.Index = -1;
            this.menuItem1.Text = "-";
            // 
            // miProjSave
            // 
            this.miProjSave.Index = -1;
            this.miProjSave.Text = "";
            // 
            // miProjSaveAs
            // 
            this.miProjSaveAs.Index = -1;
            this.miProjSaveAs.Text = "";
            // 
            // lblWarningMessage
            // 
            this.lblWarningMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWarningMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarningMessage.ForeColor = System.Drawing.Color.Red;
            this.lblWarningMessage.Location = new System.Drawing.Point(528, 408);
            this.lblWarningMessage.Name = "lblWarningMessage";
            this.lblWarningMessage.Size = new System.Drawing.Size(126, 53);
            this.lblWarningMessage.TabIndex = 8;
            this.lblWarningMessage.Text = "Writing HEIC/HEIF files is not supported - they will be saved as JPG";
            // 
            // FormMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(662, 493);
            this.Controls.Add(this.lblWarningMessage);
            this.Controls.Add(this.gbImageOptions);
            this.Controls.Add(this.btnRem);
            this.Controls.Add(this.picBox);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.btnAddFile);
            this.Controls.Add(this.btnAddDir);
            this.Controls.Add(this.listView);
            this.Icon = global::PhotoTool.Properties.Resources.phototool;
            this.Menu = this.mainMenu;
            this.MinimumSize = new System.Drawing.Size(640, 400);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = " PhotoTool";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormMain_Closing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).EndInit();
            this.gbImageOptions.ResumeLayout(false);
            this.gbImageOptions.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		// Adds all the files under a directory to the list view.  This is fairly slow 
		// as all files are loaded as bitmaps to ensure that they are valid images, 
		// although no errors are thrown for non image files.
		private void btnAddDir_Click(object sender, System.EventArgs e)
		{
			AddDirectory();
		}

		// event handler for when the Add File button is clicked.  This displays a file 
		// dialog box which allows the user to selected multiple files
		private void btnAddFile_Click(object sender, System.EventArgs e)
		{
			AddFile();
		}

		// event handler for when the go button is clicked
		private void btnGo_Click(object sender, System.EventArgs e)
		{
			// make sure all data is correct:
			string valid = ValidateInput();
			if (valid.Length > 0) 
			{
				string msg = "You need to correct the following: \n" + valid;
				MessageBox.Show(this, msg, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.SelectedPath = settings.FolderOutput;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string targetFolder = dlg.SelectedPath;
                    // check to see if any of the files already exists
                    foreach (ListViewItem lvi in listView.Items)
                    {
                        string img = GetOutputFileName(lvi.Text, targetFolder, this.cbReplaceSpaces.Checked, false);
                        string thumb = GetOutputFileName(lvi.Text, targetFolder, this.cbReplaceSpaces.Checked, true);
                        if ((File.Exists(img)) || (File.Exists(thumb)))
                        {
                            if (MessageBox.Show(this, "Files exist in the output folder that will be over-written - are you sure you want to do this?",
                                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                            {
                                // user clicked no - exit so no files are overwritten
                                return;
                            }
                            else
                            {
                                // user clicked yes - jump out of the loop and continue
                                break;
                            }
                        }
                    }
                    // update settings and create the progress dialog with the callback that actually does the 
                    // work of converting the images
                    settings.FolderOutput = targetFolder;
                    ProgressDialog pd = new ProgressDialog();

                    List<string> files = new List<string>();
                    foreach (ListViewItem li in listView.Items)
                    {
                        files.Add(li.Text);
                    }

                    ImageProcessInfo info = new ImageProcessInfo();
                    info.Files = files.ToArray();
                    info.CreateThumbnails = this.cbGenThumbs.Checked;
                    info.OutputFolder = dlg.SelectedPath;
                    info.ReplaceSpaces = this.cbReplaceSpaces.Checked;
                    info.MaxLength = Convert.ToInt32(txtImgLen.Text);
                    info.ThumbnailLength = Convert.ToInt32(txtThumbLen.Text);
                    info.Quality = Convert.ToInt32(txtImgQuality.Text);

                    this.dlgProgress = new ProgressDialog();
                    this.bgwImageProcessor.RunWorkerAsync(info);
                    this.dlgProgress.ShowDialog(this);

                }
            }
		}

		// event handler for when the Remove button is clicked
		private void btnRem_Click(object sender, System.EventArgs e)
		{
			RemoveSelectedFiles();
		}

		// event doing cleanup operations when the main form closes
		private void FormMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// save form position if form is not maximised/minimised
			if (this.WindowState == FormWindowState.Normal) 
			{
				settings.FormMainHeight = this.Height - 20;
				settings.FormMainLeftPos = this.Left;
				settings.FormMainTopPos = this.Top;
				settings.FormMainWidth = this.Width;
			}
			// save image settings
			settings.ImageLength = Convert.ToInt32(txtImgLen.Text);
			settings.ThumbnailLength = Convert.ToInt32(txtThumbLen.Text);
			settings.GenerateThumbnails = cbGenThumbs.Checked;
			settings.ReplaceSpaces = cbReplaceSpaces.Checked;
			settings.ImageQuality = Convert.ToInt32(txtImgQuality.Text);
			settings.Save();
		}

		private void FormMain_Resize(object sender, System.EventArgs e)
		{
			int w = listView.Width - 20;
			this.listView.Columns[0].Width = w / 2;
			this.listView.Columns[1].Width = w / 6;
			this.listView.Columns[2].Width = w / 6;
			this.listView.Columns[3].Width = w / 6;
        }

		// event handler for when an image is selected in the list view
		private void listView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (listView.SelectedItems.Count == 1) 
			{
				try 
				{
                    string file = listView.SelectedItems[0].Text;
                    Bitmap img = null;
                    picBox.Image = bLoadingPreview;
                    Application.DoEvents();

                    MagickImage image = new MagickImage(file);
                    image.Format = MagickFormat.Jpeg;
                    MagickGeometry geometry = new MagickGeometry(picBox.Width, picBox.Height);
                    image.Resize(geometry);

                    using (var memStream = new MemoryStream())
                    {
                        // Write the image to the memorystream
                        image.Write(memStream);

                        img = new System.Drawing.Bitmap(memStream);
                    }
                    picBox.Image = img;
                }
                catch (Exception ex) 
				{
                    _logService.Error(ex);
					picBox.Image = _imageService.CreateBlankImage("Unable to preview image.", picBox.Width, picBox.Height);
					return;
				}
			}
			else 
			{
				picBox.Image = bBlank;
			}
		}

		private void listView_DoubleClick(object sender, System.EventArgs e)
		{
			if (listView.SelectedItems.Count == 1) 
			{
                string[] files = this.alFiles.ToArray().Select(x => x.ToString()).ToArray();
                using (FormImageViewer fiv = new FormImageViewer(files, listView.SelectedIndices[0]))
                {
                    fiv.ShowDialog(this);
                }
			}
		}

		private void listView_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) 
			{
				e.Effect = DragDropEffects.Copy;
			}
			else 
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void listView_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) 
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                this.LoadFiles(files);
			}
		}

		private void miFileExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void miFileAddFile_Click(object sender, System.EventArgs e)
		{
			AddFile();
		}

		private void miFileAddDir_Click(object sender, System.EventArgs e)
		{
			AddDirectory();
		}

		#endregion

		#region Private Callback Methods

		private string GetOutputFileName(string sourceFile, string targetFolder, 
			bool replaceSpaces, bool thumb) 
		{
			string fileOut = sourceFile.Substring(sourceFile.LastIndexOf("\\"));
			if (replaceSpaces) 
			{
				fileOut = fileOut.Replace(" ", "_");
			}
			if (thumb) 
			{
				fileOut = fileOut.Substring(0, fileOut.LastIndexOf(".")) + "_tn" + fileOut.Substring(fileOut.LastIndexOf("."));
			}
			return targetFolder + fileOut;
		}

		#endregion

		private void listView_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete) 
			{
				this.RemoveSelectedFiles();
			}
        }

        private void ReloadWarningMessage()
        {
            bool isWarningVisible = false;
            foreach (ListViewItem item in listView.Items)
            {
                string filePath = item.Text;
                string extension = Path.GetExtension(filePath).ToLowerInvariant();
                if (extension == ".heic" || extension == ".heif") {
                    isWarningVisible = true;
                    break;
                }
            }

            lblWarningMessage.Visible = isWarningVisible;

        }


        #region Background Worker Methods for Loading Files

        void bgwFileLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgw = (BackgroundWorker)sender;
            FileLoadInfo fli = (FileLoadInfo)e.Argument;
            int errorCount = 0;
            int fileCount = fli.Files.Length;
            this.dlgProgress.SetRange(0, fli.Files.Length);

            for (int i = 0; i < fileCount; i++)
            {
                if (bgw.CancellationPending) break;

                string file = fli.Files[i];
                this.dlgProgress.SetText("Reading file " + file);

                try
                {
                    var image = new MagickImage(file);
                    bgw.ReportProgress(0, new ImageLoadInfo(file, image));
                }
                catch (Exception ex)
                {
                    _logService.Error(ex);
                    errorCount++;
                }
                this.dlgProgress.Increment(1);
            }

            fli.ErrorCount = errorCount;
            e.Result = fli;

        }

        void bgwFileLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FileLoadInfo fli = (FileLoadInfo)e.Result;
            this.dlgProgress.Close();
            this.dlgProgress.Dispose();

            if (fli.ReportErrors && fli.ErrorCount > 0)
            {
                string msg = String.Format("Unable to load {0} of {1} files.", fli.ErrorCount, fli.Files.Length);
                MessageBox.Show(this, msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ReloadWarningMessage();
        }

        void bgwFileLoader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ImageLoadInfo imgLoadInfo = (ImageLoadInfo)e.UserState;

            if (this.alFiles.Contains(imgLoadInfo.File)) return;

            // get all the details of the file and add to the list
            FileInfo fi = new FileInfo(imgLoadInfo.File);
            string[] listItem = new string[4];
            listItem[0] = imgLoadInfo.File;
            listItem[1] = fi.Length.ToString();
            listItem[2] = imgLoadInfo.Image.Width.ToString();
            listItem[3] = imgLoadInfo.Image.Height.ToString();
            ListViewItem item = new ListViewItem(listItem);

            // add to the listview and the array
            listView.Items.Add(item);
            alFiles.Add(imgLoadInfo.File);

            // add to the project
            // add to the current project
            // ((addToProject) && (project != null))
            //
              //project.AddFile(file);
            //
        }

        #endregion

        #region Background Worker Methods for Processing Images

        void bgwImageProcessor_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgw = (BackgroundWorker)sender;
            ImageProcessInfo ipi = (ImageProcessInfo)e.Argument;
            int errorCount = 0;
            int fileCount = ipi.Files.Length;
            this.dlgProgress.SetRange(0, ipi.CreateThumbnails ? fileCount * 2 : fileCount);

            for (int i = 0; i < fileCount; i++)
            {
                if (bgw.CancellationPending) break;

                string file = ipi.Files[i];
                this.dlgProgress.SetText("Resizing image " + file);

                try
                {
                    string fileOut = GetOutputFileName(file, ipi.OutputFolder, ipi.ReplaceSpaces, false);
                    _imageService.ResizeImage(file, ipi.MaxLength, ipi.Quality, fileOut);
                    this.dlgProgress.Increment(1);

                    // create the thumbnail
                    if (ipi.CreateThumbnails)
                    {
                        fileOut = GetOutputFileName(file, ipi.OutputFolder, ipi.ReplaceSpaces, true);
                        _imageService.ResizeImage(file, ipi.ThumbnailLength, ipi.Quality, fileOut);
                        this.dlgProgress.Increment(1);
                    }
                }
                catch (Exception ex)
                {
                    _logService.Error(ex);
                    errorCount++;
                }
            }

            ipi.ErrorCount = errorCount;
            e.Result = ipi;

        }

        void bgwImageProcessor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ImageProcessInfo ipi = (ImageProcessInfo)e.Result;
            this.dlgProgress.Close();
            this.dlgProgress.Dispose();

            if (ipi.ErrorCount > 0)
            {
                string msg = String.Format("Errors occurred while resizing {0} of {1} images.", ipi.ErrorCount, ipi.Files.Length);
                MessageBox.Show(this, msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void bgwImageProcessor_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        #endregion

        private void FormMain_Load(object sender, EventArgs e)
        {
            ReloadWarningMessage();
        }
    }
}
