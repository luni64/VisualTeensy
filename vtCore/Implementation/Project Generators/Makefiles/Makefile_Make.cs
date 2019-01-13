﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace vtCore
{
    static internal class Makefile_Make
    {
        static public string generate(IProject project, LibManager libManager, SetupData setup)
        {
            var cfg = project.selectedConfiguration;
            var board = cfg.selectedBoard;
            var options = board.getAllOptions();
                      
            StringBuilder mf = new StringBuilder();

            mf.Append("#******************************************************************************\n");
            mf.Append("# Generated by VisualTeensy (https://github.com/luni64/VisualTeensy)\n");
            mf.Append("#\n");
            mf.Append($"# {"Board",-18} {board.name}\n");
            foreach (var o in board.optionSets)
            {
                mf.Append($"# {o.name,-18} {o.selectedOption?.name}\n");
            }
            mf.Append("#\n");
            mf.Append($"# {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}\n");
            mf.Append("#******************************************************************************\n");

            mf.Append($"SHELL            := cmd.exe\nexport SHELL\n\n");
            mf.Append($"TARGET_NAME      := {project.name?.Replace(" ", "_")}\n");
            mf.Append(makeEntry("BOARD_ID         := ", "build.board", options) + "\n\n");
            mf.Append(makeEntry("MCU              := ", "build.mcu", options) + "\n\n");

            mf.Append($"LIBS_SHARED_BASE := {Helpers.getShortPath(libManager.sharedRepositoryPath)}\n");
            mf.Append($"LIBS_SHARED      := ");
            foreach (var lib in cfg.sharedLibs)
            {
                mf.Append($"{lib.path ?? "ERROR"} "); //hack, improve library to distinguish between libraries to download and loacal libs
            }
            mf.Append("\n\n");

            mf.Append($"LIBS_LOCAL_BASE  := lib\n");
            mf.Append($"LIBS_LOCAL       := ");
            foreach (var lib in cfg.localLibs)
            {
                mf.Append($"{lib.path ?? lib.name} "); //hack, improve library to distinguish between libraries to download and loacal libs
            }
            mf.Append("\n\n");

            if (cfg.setupType == SetupTypes.quick)
            {
                //mf.Append($"CORE_BASE        := {Helpers.getShortPath(setup.arduinoCore)}\n");
                mf.Append($"CORE_BASE        := {Helpers.getShortPath(Path.Combine(setup.arduinoCore, cfg.selectedBoard.core))}\n");
                mf.Append($"GCC_BASE         := {Helpers.getShortPath(setup.arduinoCompiler)}\n");
                mf.Append($"UPL_PJRC_B       := {Helpers.getShortPath(setup.arduinoTools)}\n");
            }
            else
            {
                mf.Append($"CORE_BASE        := {((cfg.copyCore || (Path.GetDirectoryName(cfg.coreBase) == project.path)) ? "core" : Helpers.getShortPath(Path.Combine(cfg.coreBase,cfg.selectedBoard.core)))}\n");
                mf.Append($"GCC_BASE         := {Helpers.getShortPath(cfg.compilerBase)}\n");
                mf.Append($"UPL_PJRC_B       := {Helpers.getShortPath(setup.uplPjrcBase)}\n");
            }
            mf.Append($"UPL_TYCMD_B      := {Helpers.getShortPath(setup.uplTyBase)}\n");
            mf.Append($"UPL_CLICMD_B     := {Helpers.getShortPath(setup.uplCLIBase)}\n\n");

          

            mf.Append(makeEntry("FLAGS_CPU   := ", "build.flags.cpu", options) + "\n");
            mf.Append(makeEntry("FLAGS_OPT   := ", "build.flags.optimize", options) + "\n");
            mf.Append(makeEntry("FLAGS_COM   := ", "build.flags.common", options) + makeEntry(" ", "build.flags.dep", options) + "\n");
            mf.Append(makeEntry("FLAGS_LSP   := ", "build.flags.ldspecs", options) + "\n");

            mf.Append("\n");
            mf.Append(makeEntry("FLAGS_CPP   := ", "build.flags.cpp", options) + "\n");
            mf.Append(makeEntry("FLAGS_C     := ", "build.flags.c", options) + "\n");
            mf.Append(makeEntry("FLAGS_S     := ", "build.flags.S", options) + "\n");

            mf.Append(makeEntry("FLAGS_LD    := ", "build.flags.ld", options).Replace("{build.core.path}", "$(CORE_BASE)") + "\n");

            mf.Append("\n");
            mf.Append(makeEntry("LIBS        := ", "build.flags.libs", options) + "\n");
            //mf.Append(makeEntry("LD_SCRIPT   := ", "build.mcu", options) + ".ld\n");

            mf.Append("\n");
            mf.Append(makeEntry("DEFINES     := ", "build.flags.defs", options) + " -DARDUINO=10807\n");
            mf.Append("DEFINES     += ");
            mf.Append(makeEntry("-DF_CPU=", "build.fcpu", options) + " " + makeEntry("-D", "build.usbtype", options) + " " + makeEntry("-DLAYOUT_", "build.keylayout", options) + "\n");

            mf.Append($"\n");
            mf.Append("CPP_FLAGS   := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_CPP)\n");
            mf.Append("C_FLAGS     := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_C)\n");
            mf.Append("S_FLAGS     := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_S)\n");
            mf.Append("LD_FLAGS    := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_LSP) $(FLAGS_LD)\n");
            mf.Append("AR_FLAGS    := rcs\n");

            if (cfg.setupType == SetupTypes.expert && !String.IsNullOrWhiteSpace(cfg.makefileExtension))
            {
                mf.Append("\n");
                mf.Append(cfg.makefileExtension);
                mf.Append("\n");
            }

            mf.Append(setup.makefile_fixed);

            return mf.ToString();
        }

        private static string makeEntry(String txt, String key, Dictionary<String, String> options)
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

