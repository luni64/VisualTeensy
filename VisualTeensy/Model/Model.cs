using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using VisualTeensy.Properties;

namespace VisualTeensy.Model
{
    public class Model
    {
        public LibManager libManager { get; }

        public SetupData data { get; }

        public List<Board> boards { get; private set; }
        public Board selectedBoard { get; set; }

        void loadSettings()
        {
            if (Settings.Default.updateNeeded)
            {
                Settings.Default.Upgrade();
                Settings.Default.updateNeeded = false;
                Settings.Default.Save();
                Settings.Default.Reload();
            }

            data.loadSettings();
            data.fromArduino = false;

            if (String.IsNullOrWhiteSpace(data.arduinoBase))
            {
                data.arduinoBase = FileHelpers.findArduinoFolder();
            }
            if (String.IsNullOrWhiteSpace(data.projectBase))
            {
                var user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                String basePath = Path.Combine(user, "source", "new_project");

                int i = 1;
                data.projectBase = basePath;
                while (Directory.Exists(data.projectBase))
                {
                    data.projectBase = $"{basePath}({i++})";
                }
            }
            if (String.IsNullOrWhiteSpace(data.makeExePath))
            {
                var curDir = Directory.GetCurrentDirectory();
                var makeExePath = Path.Combine(curDir, "make.exe");
                if (File.Exists(makeExePath))
                {
                    data.makeExePath = makeExePath;
                }
            }
            if (String.IsNullOrWhiteSpace(data.uplTyBase))
            {
                data.uplTyBase = FileHelpers.findTyToolsFolder();
            }
            if (data.arduinoBaseError == null)
            {
                if (String.IsNullOrWhiteSpace(data.uplPjrcBase)) data.uplPjrcBase = FileHelpers.getToolsFromArduino(data.arduinoBase);
                if (String.IsNullOrWhiteSpace(data.boardTxtPath)) data.boardTxtPath = FileHelpers.getBoardFromArduino(data.arduinoBase);
                if (String.IsNullOrWhiteSpace(data.coreBase)) data.coreBase = FileHelpers.getCoreFromArduino(data.arduinoBase);
                if (String.IsNullOrWhiteSpace(data.compilerBase)) data.compilerBase = Path.Combine(FileHelpers.getToolsFromArduino(data.arduinoBase), "arm");
            }

            data.fromArduino = true;

        }
        public void saveSettings()
        {
            data.saveSettings();
        }

        public bool openProjectPath()
        {
            vtTransferData transferData;

            var configFile = Path.Combine(data.projectBase, ".vscode", "visual_teensy.json");
            try
            {
                var reader = new StreamReader(configFile);
                var jsonString = reader.ReadToEnd();
                var json = new JavaScriptSerializer();
                transferData = json.Deserialize<vtTransferData>(jsonString);

                if (transferData != null)
                {
                    data.fromArduino = transferData.quickSetup;
                    data.arduinoBase = transferData.arduinoBase;
                    data.compilerBase = transferData.compilerBase;
                    data.makeExePath = transferData.makeExePath;

                    data.boardTxtPath = transferData.boardTxtPath.StartsWith("\\") ? Path.Combine(data.projectBase, transferData.boardTxtPath.Substring(1)) : transferData.boardTxtPath;
                    data.coreBase = transferData.coreBase.StartsWith("\\") ? Path.Combine(data.projectBase, transferData.coreBase.Substring(1)) : transferData.coreBase;

                    parseBoardsTxt();

                    selectedBoard = boards?.FirstOrDefault(b => b.name == transferData.board.name);
                    if (selectedBoard != null)
                    {
                        foreach (var tos in transferData.board.options)
                        {
                            var os = selectedBoard.optionSets.FirstOrDefault(x => x.name == tos.Key);
                            if (os != null)
                            {
                                os.selectedOption = os.options.FirstOrDefault(x => x.name == tos.Value);
                            }
                        }
                    }
                }

                generateFiles();
                return true;
            }
            catch (Exception)
            {
                return false;
            }





        }
        public void generateFiles()
        {
            data.makefile = data.tasks_json = data.props_json = null;

            bool ok = selectedBoard != null && data.uplTyBaseError == null && data.projectBaseError == null;
            if (data.fromArduino)
            {
                ok = ok && data.arduinoBaseError == null;
            }
            else
            {
                ok = ok && data.corePathError == null && data.compilerPathError == null;
            }

            if (ok)
            {
                Console.WriteLine("generate files");

                var options = selectedBoard.getAllOptions();

                data.makefile = generateMakefile(options);
                data.props_json = generatePropertiesFile(options);
                data.vsSetup_json = generateVisualTeensySetup();
            }
            data.tasks_json = generateTasksFile();
        }

        public Model(SetupData data)
        {

            boards = new List<Board>();
            this.data = data;
            loadSettings();
            if (!openProjectPath())
            {
                parseBoardsTxt();
                selectedBoard = boards?.FirstOrDefault();
                generateFiles();
            }

            libManager = new LibManager(data);
        }

        public void parseBoardsTxt()
        {
            Console.WriteLine("parseBoardsTxt");
            boards = FileContent.parse(data.boardTxtPath).Where(b => b.core == "teensy3").ToList();
        }
        private string generateVisualTeensySetup()
        {
            var json = new JavaScriptSerializer();
            return FileHelpers.formatOutput(json.Serialize(new vtTransferData(data, selectedBoard)));
        }
        private string generateTasksFile()
        {
            if (data.makeExePathError != null) return null;

            string makePath = data.makeExePath;

            var tasks = new tasksJson()
            {
                presentation = new Presentation(),
                tasks = new List<Task>()
                {
                    new Task()
                    {
                        label = "Build",
                        group = new Group(),
                        command = makePath.Replace('\\','/'),
                        args = new List<string>{"all","-j","-Otarget"},
                    },
                    new Task()
                    {
                        label = "Rebuild User Code",
                        group = new Group(),
                        command = makePath.Replace('\\','/'),
                        args = new List<string>{"rebuild" ,"-j","-Otarget"},
                    },
                    new Task()
                    {
                        label = "Clean",
                        group = new Group(),
                        command = makePath.Replace('\\','/'),
                        args = new List<string>{"clean"},
                    },
                    new Task()
                    {
                        label = "Upload (Teensy Uploader)",
                        group = new Group(),
                        command = makePath.Replace('\\','/'),
                        args = new List<string>{"upload" ,"-j","-Otarget"},
                    }
                    ,
                    new Task()
                    {
                        label = "Upload (TyCommander)",
                        group = new Group(),
                        command = makePath.Replace('\\','/'),
                        args = new List<string>{"uploadTy" ,"-j","-Otarget"},
                    }
                }
            };
            var json = new JavaScriptSerializer();
            return FileHelpers.formatOutput(json.Serialize(tasks));
        }
        private string generatePropertiesFile(Dictionary<string, string> options)
        {
            if (data.compilerPathError != null)
            {
                return null;
            }

            var props = new PropertiesJson()
            {
                configurations = new List<Configuration>()
                {
                    new Configuration()
                    {
                        name = "VisualTeensy",
                        compilerPath =  Path.Combine(data.compilerBase ,"bin","arm-none-eabi-gcc.exe").Replace('\\','/'),
                        intelliSenseMode = "gcc-x64",
                        includePath = new List<string>()
                        {
                            "src/**",
                            "lib/**",
                            data.coreBase?.Replace('\\','/') + "/**"
                        },
                        defines = new List<string>()
                    }
                }
            };



            if (options.ContainsKey("build.flags.defs"))
            {
                foreach (var define in options["build.flags.defs"].Split(new string[] { "-D" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    props.configurations[0].defines.Add(define.Trim());
                }
            }
            addConfigOption(options, props, "F_CPU=", "build.fcpu");
            addConfigOption(options, props, "", "build.usbtype");
            addConfigOption(options, props, "LAYOUT_", "build.keylayout");
            props.configurations[0].defines.Add("ARDUINO");
            
            //props.configurations[0].defines.Add("F_CPU=" + options["build.fcpu"]);
            //props.configurations[0].defines.Add(options["build.usbtype"]);
            //props.configurations[0].defines.Add("LAYOUT_" + options["build.keylayout"]);

            return FileHelpers.formatOutput(new JavaScriptSerializer().Serialize(props));
        }
        private string generateMakefile(Dictionary<string, string> options)
        {
            StringBuilder mf = new StringBuilder();

            mf.Append("#******************************************************************************\n");
            mf.Append("# Generated by VisualTeensy (https://github.com/luni64/VisualTeensy)\n");
            mf.Append("#\n");
            mf.Append($"# {"Board",-18} {selectedBoard.name}\n");
            selectedBoard.optionSets.ForEach(o => mf.Append($"# {o.name,-18} {o.selectedOption?.name}\n"));
            mf.Append("#\n");
            mf.Append($"# {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}\n");
            mf.Append("#******************************************************************************\n");

            mf.Append("SHELL := cmd.exe\nexport SHELL\n\n");

            mf.Append($"TARGET_NAME      := {data.projectName.Replace(" ", "_")}\n\n");

            mf.Append($"LIBS_SHARED_BASE := \n");
            mf.Append($"LIBS_SHARED      := \n\n");

            mf.Append($"LIBS_LOCAL_BASE  := lib\n");
            mf.Append($"LIBS_LOCAL       := \n\n");

            mf.Append(makeEntry("BOARD_ID    := ", "build.board", options) + "\n");
            mf.Append($"CORE_BASE   := {((data.copyCore || (Path.GetDirectoryName(data.coreBase) == data.projectBase)) ? "core" : data.coreBaseShort)}\n");
            mf.Append($"GCC_BASE    := {data.compilerBaseShort}\n");
            mf.Append($"UPL_PJRC_B  := {data.uplPjrcBaseShort}\n");
            mf.Append($"UPL_TYCMD_B := {data.uplTyBaseShort}\n\n");

            mf.Append(makeEntry("FLAGS_CPU   := ", "build.flags.cpu", options) + "\n");
            mf.Append(makeEntry("FLAGS_OPT   := ", "build.flags.optimize", options) + "\n");
            mf.Append(makeEntry("FLAGS_COM   := ", "build.flags.common", options) + makeEntry(" ", "build.flags.dep", options) + "\n");
            mf.Append(makeEntry("FLAGS_LSP   := ", "build.flags.ldspecs", options) + "\n");

            mf.Append("\n");
            mf.Append(makeEntry("FLAGS_CPP   := ", "build.flags.cpp", options) + "\n");
            mf.Append(makeEntry("FLAGS_C     := ", "build.flags.c", options) + "\n");
            mf.Append(makeEntry("FLAGS_S     := ", "build.flags.S", options) + "\n");
            mf.Append(makeEntry("FLAGS_LD    := ", "build.flags.ld", options) + "\n");

            mf.Append("\n");
            mf.Append(makeEntry("LIBS        := ", "build.flags.libs", options) + "\n");
            mf.Append(makeEntry("LD_SCRIPT   := ", "build.mcu", options) + ".ld\n");

            mf.Append("\n");
            mf.Append(makeEntry("DEFINES     := ", "build.flags.defs", options) +  " -DARDUINO \n");
            mf.Append("DEFINES     += ");
            mf.Append(makeEntry("-DF_CPU=", "build.fcpu", options) + " " + makeEntry("-D", "build.usbtype", options) + " " + makeEntry("-DLAYOUT_", "build.keylayout", options) + "\n");

            mf.Append($"\n");
            mf.Append("CPP_FLAGS   := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_CPP)\n");
            mf.Append("C_FLAGS     := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_C)\n");
            mf.Append("S_FLAGS     := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_S)\n");
            mf.Append("LD_FLAGS    := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_LSP) $(FLAGS_LD)\n");
            mf.Append("AR_FLAGS    := rcs\n");
            mf.Append(data.makefile_fixed);

            return mf.ToString();
        }

        private string makeEntry(String txt, String key, Dictionary<String, String> options)
        {
            if (options.ContainsKey(key))
            {
                return $"{txt}{options[key]}";
            }
            else
            {
                return "";
            }
        }

        void addConfigOption(Dictionary<string, string> options, PropertiesJson props, string prefix, string key)
        {
            var option = options.FirstOrDefault(o => o.Key == key).Value;

            if (option != null)
            {
                props.configurations[0].defines.Add(prefix + option);
            }
        }
    }
}

