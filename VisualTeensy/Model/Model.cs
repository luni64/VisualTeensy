using Board2Make.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Board2Make.Model
{
    public class Model
    {
        public SetupData data { get; } 

        public List<Board> boards { get; private set; } 
        public Board selectedBoard { get; set; }
        
        void loadSettings()
        {
            if (Settings.Default.updateNeeded) 
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.updateNeeded = false;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            }

            data.loadSettings();
            data.fromArduino = true;

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

                //Directory.CreateDirectory(data.projectBase);
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
        }
        public void saveSettings()
        {
            data.saveSettings();
        }
                
        public void openProjectPath()
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
                    data.arduinoBase = transferData.arduinoBase;
                    data.compilerBase = transferData.compilerBase;
                    data.makeExePath = transferData.makeExePath;
                    //  data.projectName = Path.GetFileName(data.projectBase);

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
            }
            catch (Exception) { }





        }               
        public void generateFiles()
        {
            data.makefile = data.tasks_json = data.props_json = null;

            bool ok = selectedBoard != null && data.uplTyBaseError == null && data.projectBaseError == null && data.projectNameError == null;
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
            if (data.makeExePathError == null)
            {
                data.tasks_json = generateTasksFile(data.makeExePath);
            }
        }
        
        public Model()
        {
            boards = new List<Board>();
            data = new SetupData();
            loadSettings();
            openProjectPath();
        }

        private void parseBoardsTxt()
        {
            Console.WriteLine("parseBoardsTxt");
            boards = FileContent.parse(data.boardTxtPath).ToList();
        }
        private string generateVisualTeensySetup()
        {
            var json = new JavaScriptSerializer();
            return FileHelpers.formatOutput(json.Serialize(new vtTransferData(data, selectedBoard)));
        }
        private string generateTasksFile(string makePath)
        {
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
                        args = new List<string>{"all"},
                    },
                    new Task()
                    {
                        label = "Rebuild User Code",
                        group = new Group(),
                        command = makePath.Replace('\\','/'),
                        args = new List<string>{"rebuild"},
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
                        args = new List<string>{"upload"},
                    }
                    ,
                    new Task()
                    {
                        label = "Upload (TyCommander)",
                        group = new Group(),
                        command = makePath.Replace('\\','/'),
                        args = new List<string>{"uploadTy"},
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
                            "${workspaceFolder}/src/**",
                            data.coreBase?.Replace('\\','/') + "/**"
                        },
                        defines = new List<string>()
                    }
                }
            };

            foreach (var define in options["build.flags.defs"].Split(new string[] { "-D" }, StringSplitOptions.RemoveEmptyEntries))
            {
                props.configurations[0].defines.Add(define.Trim());
            }

            props.configurations[0].defines.Add("F_CPU=" + options["build.fcpu"]);
            props.configurations[0].defines.Add(options["build.usbtype"]);
            props.configurations[0].defines.Add("LAYOUT_" + options["build.keylayout"]);

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
            if (data.fromArduino || !data.copyBoardTxt)
            {
                mf.Append($"# {"Boards.txt",-18}  {data.boardTxtPath} \n");
            }
            else
            {
                mf.Append($"# {"Boards.txt",-18} " + $"{Path.Combine(data.projectBase, Path.GetFileName(data.boardTxtPath)) }\n");
            }

            mf.Append("#\n");
            mf.Append($"# {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}\n");
            mf.Append("#******************************************************************************\n\n");

            mf.Append($"TARGET_NAME := {data.projectName}\n\n");

            mf.Append(makeEntry("BOARD_ID    := ", "build.board", options) + "\n");
            mf.Append($"CORE_BASE   := {(data.copyCore ? "core" : data.coreBaseShort)}\n");
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
            mf.Append(makeEntry("DEFINES     := ", "build.flags.defs", options) + "\n");
            mf.Append("DEFINES     += ");
            mf.Append(makeEntry("-DF_CPU=", "build.fcpu", options) + " " + makeEntry("-D", "build.usbtype", options) + " " + makeEntry("-DLAYOUT_", "build.keylayout", options) + "\n");

            mf.Append($"\n");
            mf.Append("CPP_FLAGS   := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_CPP)\n");
            mf.Append("C_FLAGS     := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_C)\n");
            mf.Append("S_FLAGS     := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_S)\n");
            mf.Append("LD_FLAGS    := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_LSP) $(FLAGS_LD)\n");
            mf.Append("AR_FLAGS    := rcs\n");
            mf.Append(Strings.makeFileEnd);

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
    }
}

