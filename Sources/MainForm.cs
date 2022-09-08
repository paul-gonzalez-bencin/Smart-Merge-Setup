
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

public class MainForm : Form
{
	#region Properties
	
	private Button find;
	
	public MainForm()
	{
		this.Size = new Size(300, 100);
		this.Text = "Setup Smart Merge";
		
		this.find = new Button();
		
		this.find.Text = "Find Unity Project";
		this.find.Location = new Point(20, 20);
		this.find.Size = new Size(225, 24);
		this.find.Click += OnFind;
		
		this.Controls.Add(this.find);
	}
	
	#endregion // Properties
	
	#region Private Methods
	
	private void OnFind(object sender, System.EventArgs args)
	{
		using(FolderBrowserDialog dialog = new FolderBrowserDialog())
		{
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				string path = this.FindUnityProject(dialog.SelectedPath);
				
				if(File.Exists(path))
				{
					string smartMerge = this.FindUnityEditorTools(path);
					
					if(smartMerge == "")
					{
						this.ShowError();
						return;
					}
					
					if(!File.Exists(path))
					{
						File.WriteAllText($"{dialog.SelectedPath}/.git/config", this.GetConfigData(smartMerge));
						MessageBox.Show(
							"Created git config to use Unity's Smart Merge tool whenever you merge!",
							"Task Successful!",
							MessageBoxButtons.OK,
							MessageBoxIcon.Information
						);
					}
					else
					{
						File.AppendAllText($"{dialog.SelectedPath}/.git/config", this.GetConfigData(smartMerge));
						MessageBox.Show(
							"Updated git config to use Unity's Smart Merge tool whenever you merge!",
							"Task Successful!",
							MessageBoxButtons.OK,
							MessageBoxIcon.Information
						);
					}
				}
				else
				{
					this.ShowError();
				}
			}
		}
	}
	
	private void ShowError()
	{
		MessageBox.Show(
			"Please select a valid Unity project",
			"Selected folder is not a Unity project",
			MessageBoxButtons.OK,
			MessageBoxIcon.Error
		);
	}
	
	private string GetConfigData(string smartMerge)
	{
		return (
			"\n"
			+ "[merge]\n"
			+ "tool = unityyamlmerge\n"
			+ "\n"
			+ "[mergetool \"unityyamlmerge\"]\n"
			+ "trustExitCode = false\n"
			+ $"cmd = '{smartMerge}' merge -p \"$BASE\" \"$REMOTE\" \"$LOCAL\" \"$MERGED\"\n\n"
		);
	}
	
	private string FindUnityProject(string path)
	{
		XmlDocument document = new XmlDocument();
		
		foreach(string filepath in Directory.GetFiles(path, "*.csproj"))
		{
			document.Load(filepath);
			
			foreach(XmlNode node in document.GetElementsByTagName("Reference"))
			{
				if(node.Attributes["Include"] != null && node.Attributes["Include"].Value == "UnityEngine")
				{
					return filepath;
				}
			}
		}
		
		return "";
	}
	
	private string FindUnityEditorTools(string path)
	{
		XmlDocument document = new XmlDocument();
		
		document.Load(path);
		
		foreach(XmlNode node in document.GetElementsByTagName("Reference"))
		{
			if(node.Attributes["Include"] != null && node.Attributes["Include"].Value == "UnityEngine")
			{
				string editorPath = node.FirstChild.InnerText;
				
				editorPath = editorPath.Replace("Managed\\UnityEngine\\UnityEngine.dll", "");
				editorPath += "Tools\\UnityYAMLMerge.exe";
				
				return editorPath.Replace("\\", "\\\\");
			}
		}
		
		return "";
	}
	
	#endregion // Private Methods
}
