using System.IO;
using System.Linq;
using System.Xml.Linq;
using ColdenCullen.DataStructures;
using System.ComponentModel;

//namespace GLIF
//{
//    class Installer : INotifyPropertyChanged
//    {
//        #region Constants
//        // ----------------------------
//        //          Folders
//        // ----------------------------
//        /// <summary>
//        /// 32bit folder name
//        /// </summary>
//        private const string X86 = "x86";
//        /// <summary>
//        /// 64bit folder name
//        /// </summary>
//        private const string X64 = "x64";
//        /// <summary>
//        /// Root folder in output dir for files to be in
//        /// </summary>
//        private const string OUT = "GL";
//        /// <summary>
//        /// Relative location of input files to exe
//        /// </summary>
//        private const string INPUT = "GL";

//        // ----------------------------
//        //          Libraries
//        // ----------------------------
//        /// <summary>
//        /// GLUT folder name
//        /// </summary>
//        private const string GLUT = "glut";
//        /// <summary>
//        /// freeglut folder name
//        /// </summary>
//        private const string FREE_GLUT = "freeglut";
//        /// <summary>
//        /// GLEW folder name
//        /// </summary>
//        private const string GLEW = "glew";
//        /// <summary>
//        /// GLFW folder name
//        /// </summary>
//        private const string GLFW = "glfw";
//        /// <summary>
//        /// GLEW folder name
//        /// </summary>
//        private const string SOIL = "soil";
//        #endregion

//        #region Binding
//        public event PropertyChangedEventHandler PropertyChanged;

//        void OnPropertyChanged(string name)
//        {
//            if (PropertyChanged != null)
//                PropertyChanged(this, new PropertyChangedEventArgs("name"));
//        }
//        #endregion

//        #region Fields
//        /// <summary>
//        /// Whether or not to install glut
//        /// </summary>
//        private bool glut;
//        /// <summary>
//        /// Whether or not to install freeglut
//        /// </summary>
//        private bool freeglut;
//        /// <summary>
//        /// Whether or not to install glew
//        /// </summary>
//        private bool glew;
//        /// <summary>
//        /// Whether or not to install glfw
//        /// </summary>
//        private bool glfw;
//        /// <summary>
//        /// Whether or not to install soil
//        /// </summary>
//        private bool soil;
//        /// <summary>
//        /// File path of project file
//        /// </summary>
//        private string slnDir;
//        #endregion

//        #region Properties
//        /// <summary>
//        /// Whether or not to install glut
//        /// </summary>
//        public bool Glut
//        {
//            get { return glut; }
//            set
//            {
//                if (glut != value)
//                {
//                    glut = value;
//                    OnPropertyChanged("Glut");
//                }
//            }
//        }
//        /// <summary>
//        /// Whether or not to install freeglut
//        /// </summary>
//        public bool Freeglut
//        {
//            get { return freeglut; }
//            set
//            {
//                if (freeglut != value)
//                {
//                    freeglut = value;
//                    OnPropertyChanged("Freeglut");
//                }
//            }
//        }
//        /// <summary>
//        /// Whether or not to install glew
//        /// </summary>
//        public bool Glew
//        {
//            get { return glew; }
//            set
//            {
//                if (glew != value)
//                {
//                    glew = value;
//                    OnPropertyChanged("Glew");
//                }
//            }
//        }
//        /// <summary>
//        /// Whether or not to install glfw
//        /// </summary>
//        public bool Glfw
//        {
//            get { return glfw; }
//            set
//            {
//                if (glfw != value)
//                {
//                    glfw = value;
//                    OnPropertyChanged("Glfw");
//                }
//            }
//        }
//        /// <summary>
//        /// Whether or not to install soil
//        /// </summary>
//        public bool Soil
//        {
//            get { return soil; }
//            set
//            {
//                if (soil != value)
//                {
//                    soil = value;
//                    OnPropertyChanged("GlSoilfw");
//                }
//            }
//        }
//        /// <summary>
//        /// File path of project file
//        /// </summary>
//        public string SolutionDir
//        {
//            get { return slnDir; }
//            set
//            {
//                if (slnDir != value)
//                {
//                    slnDir = value;
//                    OnPropertyChanged("SolutionDir");
//                }
//            }
//        }
//        #endregion

//        #region Constructor
//        /// <summary>
//        /// Initialize install values to true given default
//        /// </summary>
//        /// <param name="defaultValue">Default value for bools</param>
//        public Installer(bool defaultValue)
//        {
//            glut = freeglut = glew = defaultValue;
//        }
//        #endregion

//        #region Methods
//        /// <summary>
//        /// Splice file paths into VSC++ Project settings file
//        /// </summary>
//        /// <param name="filePath">Path to file to modify</param>
//        public bool SetupProjects(List<SolutionProject> projects, List<int> selectedIndices)
//        {
//            //try
//            {
//                XDocument project;

//                for (int ii = 0; ii < selectedIndices.Count; ++ii)
//                {
//                    // Load project file
//                    project = XDocument.Load(slnDir + "\\" + projects[ ii ].RelativePath);

//                    // Add properties
//                    project.Descendants().First<XElement>().Add(
//                        new XElement(project.Root.GetDefaultNamespace() + "PropertyGroup", new XAttribute("Condition", "'$(Configuration)|$(Platform)'=='Debug|Win32'"),
//                            new XElement(project.Root.GetDefaultNamespace() + "ExecutablePath", "$(SolutionDir)" + OUT + "\\" + X86 + "\\bin;$(ExecutablePath)"),
//                            new XElement(project.Root.GetDefaultNamespace() + "LibraryPath", "$(SolutionDir)" + OUT + "\\" + X86 + "\\lib;$(LibraryPath)"),
//                            new XElement(project.Root.GetDefaultNamespace() + "IncludePath", "$(SolutionDir)" + OUT + "\\" + X86 + "\\include;$(IncludePath)")
//                        ),
//                        new XElement(project.Root.GetDefaultNamespace() + "PropertyGroup", new XAttribute("Condition", @"'$(Configuration)|$(Platform)'=='Release|Win32'"),
//                            new XElement(project.Root.GetDefaultNamespace() + "ExecutablePath", "$(SolutionDir)" + OUT + "\\" + X86 + "\\bin;$(ExecutablePath)"),
//                            new XElement(project.Root.GetDefaultNamespace() + "LibraryPath", "$(SolutionDir)" + OUT + "\\" + X86 + "\\lib;$(LibraryPath)"),
//                            new XElement(project.Root.GetDefaultNamespace() + "IncludePath", "$(SolutionDir)" + OUT + "\\" + X86 + "\\include;$(IncludePath)")
//                        ),
//                        new XElement(project.Root.GetDefaultNamespace() + "ItemDefinitionGroup", new XAttribute("Condition", "'$(Configuration)|$(Platform)'=='Debug|Win32'"),
//                            new XElement(project.Root.GetDefaultNamespace() + "PostBuildEvent",
//                                new XElement(project.Root.GetDefaultNamespace() + "Command", "xcopy \"$(SolutionDir)" + OUT + "\\" + X86 + "\\bin\" \"$(SolutionDir)$(Configuration)\" /y")
//                            )
//                        ),
//                        new XElement(project.Root.GetDefaultNamespace() + "ItemDefinitionGroup", new XAttribute("Condition", "'$(Configuration)|$(Platform)'=='Release|Win32'"),
//                            new XElement(project.Root.GetDefaultNamespace() + "PostBuildEvent",
//                                new XElement(project.Root.GetDefaultNamespace() + "Command", "xcopy \"$(SolutionDir)" + OUT + "\\" + X86 + "\\bin\" \"$(SolutionDir)$(Configuration)\" /y")
//                            )
//                        )
//                    );

//                    // Save new project file
//                    project.Save(slnDir + "\\" + projects[ii].RelativePath);
//                }

//                return true;
//            }
//            //catch( Exception )
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// Copy chosen files to output directory
//        /// </summary>
//        /// <param name="outputDir">Directory to copy files to</param>
//        /// <returns>Success</returns>
//        public bool CopyFiles()
//        {
//            //try
//            {
//                // Copy GLUT
//                if (glut)
//                    ColdenCullen.Directory.Copy(Path.Combine(INPUT, GLUT, X86), Path.Combine(slnDir, OUT, X86));

//                // Copy freeglut
//                if (freeglut)
//                    ColdenCullen.Directory.Copy(Path.Combine(INPUT, FREE_GLUT, X86), Path.Combine(slnDir, OUT, X86));

//                // Copy GLEW
//                if (glew)
//                    ColdenCullen.Directory.Copy(Path.Combine(INPUT, GLEW, X86), Path.Combine(slnDir, OUT, X86));

//                // Copy GLFW
//                if (glfw)
//                    ColdenCullen.Directory.Copy(Path.Combine(INPUT, GLFW, X86), Path.Combine(slnDir, OUT, X86));

//                // Copy SOIL
//                if (soil)
//                    ColdenCullen.Directory.Copy(Path.Combine(INPUT, SOIL, X86), Path.Combine(slnDir, OUT, X86));

//                return true;
//            }
//            //catch (Exception)
//            {
//                return false;
//            }
//        }
//        #endregion
//    }
//}