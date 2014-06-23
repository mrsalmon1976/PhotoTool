using System;
using System.IO;
using System.Xml;

namespace SAFish.PhotoTool
{
	/// <summary>
	/// Singleton class for retrieving application settings.
	/// </summary>
	public class Settings 
	{

		#region Private Attributes
		private static Settings settings = null;
		private static Object syncLock = new Object();
		private static XmlDocument xmlDoc = null;
		private static string fileName = String.Empty;
        private const string FileName = "PhotoTool.config";
		#endregion

		#region Constructors and Access methods

		private Settings() 
		{
			fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), FileName);
            xmlDoc = new XmlDocument();
            try 
			{
                if (File.Exists(fileName))
                {
                    xmlDoc.Load(fileName);
                }
                else
                {
                    // file hasn't been written yet, just load the template
                    xmlDoc.Load(Path.Combine(FormMain.AppPath, FileName));
                }
			}
			catch (Exception e) 
			{
				throw new Exception("There is an error in the configuration file:\n\n" + e.Message);
			}
		}

		/// <summary>
		/// Access method for singleton implementation.
		/// </summary>
		/// <returns>An instance of the singleton.</returns>
		public static Settings Access() 
		{
			lock (syncLock) 
			{
				if (settings == null) 
				{
					settings = new Settings();
				}
			}
			return settings;
		}	

		#endregion

		#region Settings to do with the main form layout

		/// <summary>
		/// Height of the main form at startup.
		/// </summary>
		public int FormMainHeight
		{
			get 
			{
				return GetSetting("phototool/settings/main_form/height", 480);
			}
			set 
			{
				SetSetting("phototool/settings/main_form/height", value.ToString());
			}
		}

		/// <summary>
		/// Left position of the main form at startup.
		/// </summary>
		public int FormMainLeftPos 
		{
			get 
			{
				return GetSetting("phototool/settings/main_form/left", 100);
			}
			set 
			{
				SetSetting("phototool/settings/main_form/left", value.ToString());
			}
		}

		/// <summary>
		/// Top position of the main form at startup.
		/// </summary>
		public int FormMainTopPos 
		{
			get 
			{
				return GetSetting("phototool/settings/main_form/top", 100);
			}
			set 
			{
				SetSetting("phototool/settings/main_form/top", value.ToString());
			}
		}

		/// <summary>
		/// Width of the main form at startup.
		/// </summary>
		public int FormMainWidth
		{
			get 
			{
				return GetSetting("phototool/settings/main_form/width", 640);
			}
			set 
			{
				SetSetting("phototool/settings/main_form/width", value.ToString());
			}
		}

		#endregion

		#region Settings to do with the output images

		/// <summary>
		/// Whether or not to generate thumbnails when resizing.
		/// </summary>
		public bool GenerateThumbnails
		{
			get 
			{
				return GetSetting("phototool/settings/image_options/generate_thumbs", true);
			}
			set 
			{
				SetSetting("phototool/settings/image_options/generate_thumbs", (value ? "1" : "0"));
			}
		}


		/// <summary>
		/// Longest side of the images when they are resized.
		/// </summary>
		public int ImageLength
		{
			get 
			{
				return GetSetting("phototool/settings/image_options/image_length", 800);
			}
			set 
			{
				SetSetting("phototool/settings/image_options/image_length", value.ToString());
			}
		}

		/// <summary>
		/// Quality of the images when they are resized.
		/// </summary>
		public int ImageQuality
		{
			get 
			{
				return GetSetting("phototool/settings/image_options/image_quality", 80);
			}
			set 
			{
				SetSetting("phototool/settings/image_options/image_quality", value.ToString());
			}
		}

		/// <summary>
		/// Whether or not to replace spaces in the names of the images when they are 
		/// resized.
		/// </summary>
		public bool ReplaceSpaces
		{
			get 
			{
				return GetSetting("phototool/settings/image_options/replace_spaces", true);
			}
			set 
			{
				SetSetting("phototool/settings/image_options/replace_spaces", (value ? "1" : "0"));
			}
		}

		/// <summary>
		/// Longest side of the thumbnails.
		/// </summary>
		public int ThumbnailLength
		{
			get 
			{
				return GetSetting("phototool/settings/image_options/thumbnail_length", 100);
			}
			set 
			{
				SetSetting("phototool/settings/image_options/thumbnail_length", value.ToString());
			}
		}

		#endregion

		#region Settings to do with the dialog folders

		/// <summary>
		/// Folder to initialise the "Add Directory" dialog.
		/// </summary>
		public string FolderAddDirectory
		{
			get 
			{
				return GetSetting("phototool/settings/folders/add_dir_folder", String.Empty);
			}
			set 
			{
				SetSetting("phototool/settings/folders/add_dir_folder", value);
			}
		}

		/// <summary>
		/// Folder to initialise the "Add File" dialog.
		/// </summary>
		public string FolderAddFile
		{
			get 
			{
				return GetSetting("phototool/settings/folders/add_file_folder", String.Empty);
			}
			set 
			{
				SetSetting("phototool/settings/folders/add_file_folder", value);
			}
		}

		/// <summary>
		/// Folder to initialise the "Project" dialogs.
		/// </summary>
		public string FolderProject
		{
			get 
			{
				return GetSetting("phototool/settings/folders/project_folder", String.Empty);
			}
			set 
			{
				SetSetting("phototool/settings/folders/project_folder", value);
			}
		}

		/// <summary>
		/// Folder to initialise the "Output" dialog.
		/// </summary>
		public string FolderOutput
		{
			get 
			{
				return GetSetting("phototool/settings/folders/output_folder", String.Empty);
			}
			set 
			{
				SetSetting("phototool/settings/folders/output_folder", value);
			}
		}


		#endregion

		#region Private Methods

		/// <summary>
		/// Helper method to retrieve the value of a setting in the xml document 
		/// that is a boolean value.
		/// </summary>
		/// <param name="xPath">XPATH to the node storing the setting.</param>
		/// <param name="defVal">Default value of the setting.</param>
		/// <returns>The stored value if it exists as a boolean, otherwise the default value.</returns>
		private bool GetSetting(string xPath, bool defVal) 
		{
			int i = GetSetting(xPath, (defVal ? 1 : 0));
			return (i == 1);
		}

		/// <summary>
		/// Helper method to retrieve the value of a setting in the xml document 
		/// that is a number.
		/// </summary>
		/// <param name="xPath">XPATH to the node storing the setting.</param>
		/// <param name="defVal">Default value of the setting.</param>
		/// <returns>The stored value if it exists in a valid numeric format, 
		/// otherwise the default value.</returns>
		private int GetSetting(string xPath, int defVal) 
		{
			int val = defVal;
			XmlNode node = xmlDoc.SelectSingleNode(xPath);
			try 
			{
				val = Convert.ToInt32(node.InnerText);
			}
			catch (Exception) { }
			return val;
		}

		/// <summary>
		/// Helper method to retrieve the value of a setting in the xml document 
		/// that is a string.
		/// </summary>
		/// <param name="xPath">XPATH to the node storing the setting.</param>
		/// <param name="defVal">Default value of the setting.</param>
		/// <returns>The stored value if it exists, otherwise the default value.</returns>
		private string GetSetting(string xPath, string defVal) 
		{
			XmlNode node = xmlDoc.SelectSingleNode(xPath);
			return node.InnerText;
		}


		/// <summary>
		/// Adjusts the value of a setting in the xml document.
		/// </summary>
		/// <param name="xPath">XPATH to the node that stored the value.</param>
		/// <param name="val">New setting value.</param>
		private void SetSetting(string xPath, string val) 
		{
			XmlNode node = xmlDoc.SelectSingleNode(xPath);
			node.InnerText = val;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Saves the settings to disk.
		/// </summary>
		public void Save() 
		{
			xmlDoc.Save(fileName);
		}
		#endregion


	}
}
