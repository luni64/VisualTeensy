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
        public SetupData data { get; } = new SetupData();

        public List<Board> boards { get; private set; } = new List<Board>();


        void loadSettings()
        {
            if (Settings.Default.updateNeeded)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.updateNeeded = false;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            }

           // data.loadSettings();
        }
        public void saveSettings()
        {
            data.saveSettings();
        }

        public Model()
        {
            loadSettings();         


            if (data.arduinoBaseError != null) data.arduinoBase = FileHelpers.findArduinoFolder();
            if (String.IsNullOrWhiteSpace(data.projectName)) data.projectName = "new_project";
            if (String.IsNullOrWhiteSpace(data.projectBase))
            {
                var user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                data.projectBase = Path.Combine(user, "source", data.projectName);

                Directory.CreateDirectory(data.projectBase);                
                
            }

            if(String.IsNullOrWhiteSpace(data.makeExePath))
            {
                var curDir = Directory.GetCurrentDirectory();
                var makeExePath = Path.Combine(curDir, "make.exe");
                if(File.Exists(makeExePath))
                {
                    data.makeExePath = makeExePath;
                }
            }
        }

        public void parseBoardsTxt()
        {
            Console.WriteLine("parseBoardsTxt");
            boards = FileContent.parse(data.boardTxtPath).ToList();
        }
        public void generateFiles(Board board)
        {
            data.makefile = data.tasks_json = data.propsFile = null;

            bool ok = board != null && data.uplTyBaseError == null && data.projectBaseError == null && data.projectNameError == null;
            if (data.fromArduino)
            {
                ok = ok && data.arduinoBaseError == null;
            }
            else
            {
                ok = ok && data.corePathError == null && data.compilerPathError == null;
            }

            //if (board != null && data.corePathError == null && data.compilerPathError == null && (data.fromArduino ? data.uplPjrcBaseError == null))
            if (ok)
            {
                var options = board.getAllOptions();

                Console.WriteLine("generate files");

                data.makefile = generateMakefile(board.name, board.optionSets, options, data);
                data.propsFile = generatePropertiesFile(options, data);
            }
            if (data.makeExePathError == null)
            {
                data.tasks_json = generateTasksFile(data.makeExePath);
            }
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
            return FileHelpers.FormatOutput(json.Serialize(tasks));
        }
        private string generatePropertiesFile(Dictionary<string, string> options, SetupData data)
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

            return FileHelpers.FormatOutput(new JavaScriptSerializer().Serialize(props));
        }
        private string generateMakefile(string boardName, List<OptionSet> optionSets, Dictionary<string, string> options, SetupData data)
        {
            StringBuilder mf = new StringBuilder();

            mf.Append("#******************************************************************************\n");
            mf.Append("# Generated by VisualTeensy (https://github.com/luni64/VisualTeensy)\n");
            mf.Append("#\n");
            mf.Append($"# {"Board",-18} {boardName}\n");
            optionSets.ForEach(o => mf.Append($"# {o.name,-18} {o.selectedOption.name}\n"));
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
