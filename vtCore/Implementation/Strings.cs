namespace vtCore
{
    public static class Strings
    {
        public static string mainCpp { get; } =
            "#include \"Arduino.h\"\n\n" +

            "void setup()\n" +
            "{\n" +
            "  pinMode(LED_BUILTIN,OUTPUT);\n" +
            "}\n\n" +

            "void loop()\n" +
            "{\n" +
            "  digitalWriteFast(LED_BUILTIN,!digitalReadFast(LED_BUILTIN));\n" +
            "  delay(500);\n" +
            "}\n";

        public static string sketchIno { get; } =
            "void setup()\n" +
            "{\n" +
            "  pinMode(LED_BUILTIN,OUTPUT);\n" +
            "}\n\n" +

            "void loop()\n" +
            "{\n" +
            "  digitalWriteFast(LED_BUILTIN,!digitalReadFast(LED_BUILTIN));\n" +
            "  delay(500);\n" +
            "}\n";


        //public static string makeFileEnd { get; } =        
        //    "\n" +
        //    "USR_SRC     := src\n" +
        //    "CORE_SRC    := $(CORE_BASE)\n" +
        //    "\n" +
        //    "BIN         := bin\n" +
        //    "USR_BIN     := $(BIN)\\src\n" +
        //    "CORE_BIN    := $(BIN)\\core\n" +
        //    "CORE_LIB    := $(BIN)\\core.a\n" +
        //    "TARGET_HEX  := $(BIN)\\$(TARGET_NAME).hex\n" +
        //    "TARGET_ELF  := $(BIN)\\$(TARGET_NAME).elf\n" +
        //    "TARGET_LST  := $(BIN)\\$(TARGET_NAME).lst\n" +
        //    "\n\n" +

        //    "#******************************************************************************\n" +
        //    "# BINARIES\n" +
        //    "#******************************************************************************\n" +
        //    "CC          := $(GCC_BASE)/bin/arm-none-eabi-gcc\n" +
        //    "CXX         := $(GCC_BASE)\\bin\\arm-none-eabi-g++\n" +
        //    "AR          := $(GCC_BASE)\\bin\\arm-none-eabi-gcc-ar\n" +
        //    "OBJCOPY     := $(GCC_BASE)\\bin\\arm-none-eabi-objcopy\n" +
        //    "SIZE        := $(GCC_BASE)\\bin\\arm-none-eabi-size\n" +
        //    "OBJDUMP     := $(GCC_BASE)\\bin\\arm-none-eabi-objdump\n" +
        //    "UPL_PJRC    := \"$(UPL_PJRC_B)\\teensy_post_compile\" -test -file=$(TARGET_NAME) -path=$(BIN) -tools=\"$(UPL_PJRC_B)\" -board=$(BOARD_ID) -reboot\n" +
        //    "UPL_TYCMD   := $(UPL_TYCMD_B)\\tyCommanderC upload $(TARGET_HEX) --autostart\n" +
        //    "\n" +

        //    "#******************************************************************************\n" +
        //    "# Source and Include Files\n" +
        //    "#******************************************************************************\n" +
        //    "# Recursively create list of source and object files in USR_SRC and CORE_SRC \n" +
        //    "# and corresponding subdirectories. \n" +
        //    "# The function rwildcard is taken from http://stackoverflow.com/a/12959694)\n\n" +

        //    "rwildcard =$(wildcard $1$2) $(foreach d,$(wildcard $1*),$(call rwildcard,$d/,$2))\n\n" +

        //    "#User Sources -----------------------------------------------------------------\n" +
        //    "USR_C_FILES    := $(call rwildcard,$(USR_SRC)/,*.c)\n" +
        //    "USR_CPP_FILES  := $(call rwildcard,$(USR_SRC)/,*.cpp)\n" +
        //    "USR_S_FILES    := $(call rwildcard,$(USR_SRC)/,*.S)\n" +
        //    "USR_OBJ        := $(USR_S_FILES:$(USR_SRC)/%.S=$(USR_BIN)/%.o) $(USR_C_FILES:$(USR_SRC)/%.c=$(USR_BIN)/%.o) $(USR_CPP_FILES:$(USR_SRC)/%.cpp=$(USR_BIN)/%.o) \n" +
        //    "\n" +

        //    "# Core library sources -------------------------------------------------------- \n" +
        //    "CORE_CPP_FILES := $(call rwildcard,$(CORE_SRC)/,*.cpp)\n" +
        //    "CORE_C_FILES   := $(call rwildcard,$(CORE_SRC)/,*.c)\n" +
        //    "CORE_OBJ       := $(CORE_S_FILES:$(CORE_SRC)/%.S=$(CORE_BIN)/%.o) $(CORE_C_FILES:$(CORE_SRC)/%.c=$(CORE_BIN)/%.o) $(CORE_CPP_FILES:$(CORE_SRC)/%.cpp=$(CORE_BIN)/%.o) \n" +
        //    "\n" +
        //    "#$(info INFO: ${USR_OBJ})\n" +
        //    "#$(info INFO: ${CORE_OBJ})\n\n" +

        //    "INCLUDE        := -I.\\$(USR_SRC) -I$(CORE_SRC)\n\n\n" +

        //    "#******************************************************************************\n" +
        //    "# Rules:\n" +
        //    "#******************************************************************************\n" +
        //    "\n" +
        //    ".PHONY: all rebuild upload uploadTy clean cleanUser cleanCore\n\n" +

        //    "all:     $(TARGET_LST) $(TARGET_HEX)\n" +
        //    "rebuild: cleanUser all\n" +
        //    "clean:   cleanUser cleanCore\n\n" +

        //    "upload: $(TARGET_LST) $(TARGET_HEX)\n" +
        //       "\t@$(UPL_PJRC)\n\n" +

        //    "uploadTy: $(TARGET_LST) $(TARGET_HEX)\n" +
        //       "\t@$(UPL_TYCMD)\n\n" +


        //    "# Core library ----------------------------------------------------------------\n" +
        //    "$(CORE_BIN)/%.o: $(CORE_SRC)/%.S\n" +
        //       "\t@echo [ASM] CORE $(notdir $<)\n" +
        //       "\t@if not exist $(dir $@)  @mkdir \"$(dir $@)\"\n" +
        //       "\t@\"$(CC)\" $(S_FLAGS) $(INCLUDE) -o $@ -c $< \n\n" +

        //    "$(CORE_BIN)/%.o: $(CORE_SRC)/%.c\n" +
        //       "\t@echo [CC]  CORE $(notdir $<)\n" +
        //       "\t@if not exist $(dir $@)  @mkdir \"$(dir $@)\"\n" +
        //       "\t@\"$(CC)\" $(C_FLAGS) $(INCLUDE) -o $@ -c $< \n\n" +

        //    "$(CORE_BIN)/%.o: $(CORE_SRC)/%.cpp\n" +
        //       "\t@echo [CPP] CORE $(notdir $<)\n" +
        //       "\t@if not exist $(dir $@)  @mkdir \"$(dir $@)\"\n" +
        //       "\t@\"$(CXX)\" $(CPP_FLAGS) $(INCLUDE) -o $@ -c $< \n\n" +

        //    "$(CORE_LIB) : $(CORE_OBJ)\n" +
        //       "\t@echo [AR]  $@\n" +
        //       "\t@$(AR) $(AR_FLAGS) $@ $^\n" +
        //       "\t@echo Teensy core built successfully &&echo.\n\n" +


        //    "# Handle user sources ---------------------------------------------------------\n" +
        //       "$(USR_BIN)/%.o: $(USR_SRC)/%.S\n" +
        //       "\t@echo [ASM] $<\n" +
        //       "\t@if not exist $(dir $@)  @mkdir \"$(dir $@)\"\n" +
        //       "\t@\"$(CC)\" $(S_FLAGS) $(INCLUDE) -o $@ -c $<\n\n" +

        //    "$(USR_BIN)/%.o: $(USR_SRC)/%.c\n" +
        //       "\t@echo [CC]  $(notdir $<)\n" +
        //       "\t@if not exist $(dir $@)  @mkdir \"$(dir $@)\"\n" +
        //       "\t@\"$(CC)\" $(C_FLAGS) $(INCLUDE) -o \"$@\" -c $<\n\n" +

        //    "$(USR_BIN)/%.o: $(USR_SRC)/%.cpp\n" +
        //       "\t@echo [CPP] $<\n" +
        //       "\t@if not exist $(dir $@)  @mkdir \"$(dir $@)\"\n" +
        //       "\t@\"$(CXX)\" $(CPP_FLAGS) $(INCLUDE) -o \"$@\" -c $<\n\n" +


        //    "# Linking ---------------------------------------------------------------------\n" +
        //    "$(TARGET_ELF): $(CORE_LIB) $(USR_OBJ)\n" +
        //       "\t@echo [LD]  $@\n" +
        //       "\t@$(CC) $(LD_FLAGS) -T$(CORE_SRC)/$(LD_SCRIPT) -o \"$@\" $(USR_OBJ) $(CORE_LIB) $(LIBS)\n" +
        //       "\t@echo User code built and linked to core lib &&echo.\n\n" +

        //    "%.lst: %.elf\n" +
        //       "\t@echo [LST] $@\n" +
        //       "\t@$(OBJDUMP) -d -S --demangle --no-show-raw-insn --syms \"$<\"  > \"$@\"\n" +
        //       "\t@echo Listfile generated &&echo.\n\n" +

        //    "%.hex: %.elf\n" +
        //       "\t@echo [HEX] $@\n" +
        //       "\t@$(SIZE) \"$<\"\n" +
        //       "\t@$(OBJCOPY) -O ihex -R.eeprom \"$<\" \"$@\"\n" +
        //       "\t@echo Sucessfully built project &&echo.\n\n" +


        //    "# Cleaning --------------------------------------------------------------------\n" +
        //    "cleanUser:\n" +
        //       "\t@echo Cleaning user binaries...\n" +
        //       "\t@if exist $(USR_BIN) rd /s/q \"$(USR_BIN)\"\n\n" +

        //    "\t@if exist $(TARGET_HEX) del  $(TARGET_HEX)\n" +
        //       "\t@if exist $(TARGET_ELF) del  $(TARGET_ELF)\n" +
        //       "\t@if exist $(TARGET_LST) del  $(TARGET_LST)\n\n" +

        //    "cleanCore:\n" +
        //       "\t@echo Cleaning core binaries...\n" +
        //       "\t@if exist $(CORE_BIN) rd /s/q \"$(CORE_BIN)\"\n" +
        //       "\t@if exist $(CORE_LIB) del  \"$(CORE_LIB)\"\n\n" +


        //    "# compiler generated dependency info ------------------------------------------\n" +
        //    "-include $(CORE_OBJ:.o=.d)\n" +
        //    "-include $(USR_OBJ:.o=.d)\n";
    }
}
