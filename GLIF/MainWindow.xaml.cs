using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using ColdenCullen.DataStructures;
using Microsoft.Win32;

namespace GLIF
{
    /// <summary>
    /// Interaction logic for Main Window
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region GUI
        private Solution sln;

        private void slnDirBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Visual Studio Solution Files (*.sln)|*.sln";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;

            if( ofd.ShowDialog() == true )
            {
                try
                {
                    solutionFileText.Text = ofd.FileName;

                    SolutionDir = ofd.FileName.Substring(0, ofd.FileName.LastIndexOf("\\"));

                    sln = new Solution(ofd.FileName);

                    foreach (SolutionProject proj in sln.Projects)
                        projectList.Items.Add(proj.ProjectName);
                }
                catch( Exception ex )
                {
#if DEBUG
                    MessageBox.Show( e.ToString() );
#endif
                }
            }
        }

        /// <summary>
        /// Handles event when list selection changes, because it is not bound to anything
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            OnPropertyChanged("ProjectList");
        }

        /// <summary>
        /// Copies files and setups projects to look for them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void installButton_Click(object sender, RoutedEventArgs e)
        {
            SetupProjects( sln.Projects, projectList.SelectedIndex );
            CopyFiles();

            MessageBox.Show("Done!");
        }
        #endregion

        #region Binding
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            if( PropertyChanged != null )
            {
                PropertyChanged( this, new PropertyChangedEventArgs( name ) );

                // Check for enabled-ness
                if (SolutionDir != null)
                {
                    ProjectPanel.IsEnabled = true;

                    if (projectList.SelectedItem != null)
                    {
                        LibraryPanel.IsEnabled = true;

                        if (Glut || Freeglut || Glew || Glfw || FreeImage)
                            SubmitPanel.IsEnabled = true;
                        else
                            SubmitPanel.IsEnabled = false;
                    }
                    else
                        LibraryPanel.IsEnabled = SubmitPanel.IsEnabled = false;
                }
                else
                    ProjectPanel.IsEnabled = LibraryPanel.IsEnabled = SubmitPanel.IsEnabled = false;
            }
        }
        #endregion

        #region Installer
        #region Constants
        // ----------------------------
        //          Folders
        // ----------------------------
        /// <summary>
        /// 32bit folder name
        /// </summary>
        private const string X86 = "x86";
        /// <summary>
        /// 64bit folder name
        /// </summary>
        private const string X64 = "x64";
        /// <summary>
        /// Root folder in output dir for files to be in
        /// </summary>
        private const string OUT = "GL";
        /// <summary>
        /// Relative location of input files to exe
        /// </summary>
        private const string INPUT = "GL";

        // ----------------------------
        //          Libraries
        // ----------------------------
        /// <summary>
        /// GLUT folder name
        /// </summary>
        private const string GLUT = "glut";
        /// <summary>
        /// freeglut folder name
        /// </summary>
        private const string FREE_GLUT = "freeglut";
        /// <summary>
        /// GLEW folder name
        /// </summary>
        private const string GLEW = "glew";
        /// <summary>
        /// GLFW folder name
        /// </summary>
        private const string GLFW = "glfw";
        /// <summary>
        /// GLEW folder name
        /// </summary>
        private const string FREEIMAGE = "freeimage";
        #endregion

        #region Fields
        /// <summary>
        /// Whether or not to install glut
        /// </summary>
        private bool glut;
        /// <summary>
        /// Whether or not to install freeglut
        /// </summary>
        private bool freeglut;
        /// <summary>
        /// Whether or not to install glfw
        /// </summary>
        private bool glfw;
        /// <summary>
        /// Whether or not to install glew
        /// </summary>
        private bool glew;
        /// <summary>
        /// Whether or not to install soil
        /// </summary>
        private bool freeimage;
        /// <summary>
        /// Folder path of project file
        /// </summary>
        private string slnDir;
        #endregion

        #region Properties
        /// <summary>
        /// Whether or not to install glut
        /// </summary>
        public bool Glut
        {
            get { return glut; }
            set
            {
                if (glut != value)
                {
                    glut = value;
                    OnPropertyChanged("Glut");
                }
            }
        }
        /// <summary>
        /// Whether or not to install freeglut
        /// </summary>
        public bool Freeglut
        {
            get { return freeglut; }
            set
            {
                if (freeglut != value)
                {
                    freeglut = value;
                    OnPropertyChanged("Freeglut");
                }
            }
        }
        /// <summary>
        /// Whether or not to install glfw
        /// </summary>
        public bool Glfw
        {
            get { return glfw; }
            set
            {
                if (glfw != value)
                {
                    glfw = value;
                    OnPropertyChanged("Glfw");
                }
            }
        }
        /// <summary>
        /// Whether or not to install glew
        /// </summary>
        public bool Glew
        {
            get { return glew; }
            set
            {
                if (glew != value)
                {
                    glew = value;
                    OnPropertyChanged("Glew");
                }
            }
        }
        /// <summary>
        /// Whether or not to install soil
        /// </summary>
        public bool FreeImage
        {
            get { return freeimage; }
            set
            {
                if (freeimage != value)
                {
                    freeimage = value;
                    OnPropertyChanged("FreeImage");
                }
            }
        }
        /// <summary>
        /// Folder path of project file
        /// </summary>
        public string SolutionDir
        {
            get { return slnDir; }
            set
            {
                if (slnDir != value)
                {
                    slnDir = value;
                    OnPropertyChanged("SolutionDir");
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Splice file paths into VSC++ Project settings file
        /// </summary>
        /// <param name="filePath">Path to file to modify</param>
        public bool SetupProjects( List<SolutionProject> projects, int selectedIndex )
        {
            try
            {
                XDocument project;

                // Load project file
                project = XDocument.Load(slnDir + "\\" + projects[ selectedIndex ].RelativePath);

                // Add properties
                project.Descendants().First<XElement>().Add(
                    new XElement(project.Root.GetDefaultNamespace() + "PropertyGroup", new XAttribute("Condition", "'$(Configuration)|$(Platform)'=='Debug|Win32'"),
                        new XElement(project.Root.GetDefaultNamespace() + "ExecutablePath", "$(SolutionDir)" + OUT + "\\" + X86 + "\\bin;$(ExecutablePath)"),
                        new XElement(project.Root.GetDefaultNamespace() + "LibraryPath", "$(SolutionDir)" + OUT + "\\" + X86 + "\\lib;$(LibraryPath)"),
                        new XElement(project.Root.GetDefaultNamespace() + "IncludePath", "$(SolutionDir)" + OUT + "\\" + X86 + "\\include;$(IncludePath)")
                    ),
                    new XElement(project.Root.GetDefaultNamespace() + "PropertyGroup", new XAttribute("Condition", @"'$(Configuration)|$(Platform)'=='Release|Win32'"),
                        new XElement(project.Root.GetDefaultNamespace() + "ExecutablePath", "$(SolutionDir)" + OUT + "\\" + X86 + "\\bin;$(ExecutablePath)"),
                        new XElement(project.Root.GetDefaultNamespace() + "LibraryPath", "$(SolutionDir)" + OUT + "\\" + X86 + "\\lib;$(LibraryPath)"),
                        new XElement(project.Root.GetDefaultNamespace() + "IncludePath", "$(SolutionDir)" + OUT + "\\" + X86 + "\\include;$(IncludePath)")
                    ),
                    new XElement(project.Root.GetDefaultNamespace() + "ItemDefinitionGroup", new XAttribute("Condition", "'$(Configuration)|$(Platform)'=='Debug|Win32'"),
                        new XElement(project.Root.GetDefaultNamespace() + "PostBuildEvent",
                            new XElement(project.Root.GetDefaultNamespace() + "Command", "xcopy \"$(SolutionDir)" + OUT + "\\" + X86 + "\\bin\" \"$(SolutionDir)$(Configuration)\" /y")
                        )
                    ),
                    new XElement(project.Root.GetDefaultNamespace() + "ItemDefinitionGroup", new XAttribute("Condition", "'$(Configuration)|$(Platform)'=='Release|Win32'"),
                        new XElement(project.Root.GetDefaultNamespace() + "PostBuildEvent",
                            new XElement(project.Root.GetDefaultNamespace() + "Command", "xcopy \"$(SolutionDir)" + OUT + "\\" + X86 + "\\bin\" \"$(SolutionDir)$(Configuration)\" /y")
                        )
                    )
                );

                // Save new project file
                project.Save(slnDir + "\\" + projects[ selectedIndex ].RelativePath);

                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                MessageBox.Show( e.ToString() );
#endif
                return false;
            }
        }

        /// <summary>
        /// Copy chosen files to output directory
        /// </summary>
        /// <param name="outputDir">Directory to copy files to</param>
        /// <returns>Success</returns>
        public bool CopyFiles()
        {
            try
            {
                // Copy GLUT
                if (glut)
                    ColdenCullen.Directory.Copy(Path.Combine(INPUT, GLUT, X86), Path.Combine(slnDir, OUT, X86));

                // Copy freeglut
                if (freeglut)
                    ColdenCullen.Directory.Copy(Path.Combine(INPUT, FREE_GLUT, X86), Path.Combine(slnDir, OUT, X86));

                // Copy GLEW
                if (glew)
                    ColdenCullen.Directory.Copy(Path.Combine(INPUT, GLEW, X86), Path.Combine(slnDir, OUT, X86));

                // Copy GLFW
                if (glfw)
                    ColdenCullen.Directory.Copy(Path.Combine(INPUT, GLFW, X86), Path.Combine(slnDir, OUT, X86));

                // Copy SOIL
                if (freeimage)
                    ColdenCullen.Directory.Copy(Path.Combine(INPUT, FREEIMAGE, X86), Path.Combine(slnDir, OUT, X86));

                return true;
            }
            catch( Exception e )
            {
#if DEBUG
                MessageBox.Show( e.ToString() );
#endif
                return false;
            }
        }
        #endregion
        #endregion
    }
}