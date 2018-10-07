
USR_SRC     := src
LIB_SRC     := lib
CORE_SRC    := $(CORE_BASE)

BIN         := bin
USR_BIN     := $(BIN)/src
CORE_BIN    := $(BIN)/core
LIB_BIN     := $(BIN)/lib
CORE_LIB    := $(BIN)\core.a
TARGET_HEX  := $(BIN)\$(TARGET_NAME).hex
TARGET_ELF  := $(BIN)\$(TARGET_NAME).elf
TARGET_LST  := $(BIN)\$(TARGET_NAME).lst 

#******************************************************************************
# BINARIES
#******************************************************************************
CC          := $(GCC_BASE)\bin\arm-none-eabi-gcc
CXX         := $(GCC_BASE)\bin\arm-none-eabi-g++
AR          := $(GCC_BASE)\bin\arm-none-eabi-gcc-ar
OBJCOPY     := $(GCC_BASE)\bin\arm-none-eabi-objcopy
SIZE        := $(GCC_BASE)\bin\arm-none-eabi-size
OBJDUMP     := $(GCC_BASE)\bin\arm-none-eabi-objdump
UPL_PJRC    := "$(UPL_PJRC_B)\teensy_post_compile" -test -file=$(TARGET_NAME) -path=$(BIN) -tools="$(UPL_PJRC_B)" -board=$(BOARD_ID) -reboot
UPL_TYCMD   := $(UPL_TYCMD_B)\tyCommanderC upload $(TARGET_HEX) --autostart --wait --multi
UPL_CLICMD  := $(UPL_CLICMD_B)\teensy_loader_cli -mmcu=$(MCU) -v $(TARGET_HEX)

#******************************************************************************
# Source and Include Files
#******************************************************************************
# Recursively create list of source and object files in USR_SRC and CORE_SRC 
# and corresponding subdirectories. 
# The function rwildcard is taken from http://stackoverflow.com/a/12959694)

rwildcard =$(wildcard $1$2) $(foreach d,$(wildcard $1*),$(call rwildcard,$d/,$2))

#User Sources -----------------------------------------------------------------
USR_C_FILES    := $(call rwildcard,$(USR_SRC)/,*.c)
USR_CPP_FILES  := $(call rwildcard,$(USR_SRC)/,*.cpp)
USR_S_FILES    := $(call rwildcard,$(USR_SRC)/,*.S)
USR_OBJ        := $(USR_S_FILES:$(USR_SRC)/%.S=$(USR_BIN)/%.o) $(USR_C_FILES:$(USR_SRC)/%.c=$(USR_BIN)/%.o) $(USR_CPP_FILES:$(USR_SRC)/%.cpp=$(USR_BIN)/%.o) 

# Core library sources --------------------------------------------------------
CORE_CPP_FILES := $(call rwildcard,$(CORE_SRC)/,*.cpp)
CORE_C_FILES   := $(call rwildcard,$(CORE_SRC)/,*.c)
CORE_OBJ       := $(CORE_S_FILES:$(CORE_SRC)/%.S=$(CORE_BIN)/%.o) $(CORE_C_FILES:$(CORE_SRC)/%.c=$(CORE_BIN)/%.o) $(CORE_CPP_FILES:$(CORE_SRC)/%.cpp=$(CORE_BIN)/%.o) 


# User library sources  (see) https://github.com/arduino/arduino/wiki/arduino-ide-1.5:-library-specification
LIB_DIRS_SHARED  := $(foreach d, $(LIBS_SHARED), $(LIBS_SHARED_BASE)/$d/ $(LIBS_SHARED_BASE)/$d/utility/)  # base and /utility 
LIB_DIRS_SHARED  += $(foreach d, $(LIBS_SHARED), $(LIBS_SHARED_BASE)/$d/src/ $(dir $(call rwildcard,$(LIBS_SHARED_BASE)/$d/src/,*/.)))                           # src and all subdirs of base

LIB_DIRS_LOCAL   := $(foreach d, $(LIBS_LOCAL), $(LIBS_LOCAL_BASE)/$d/ $(LIBS_LOCAL_BASE)/$d/utility/ )    # base and /utility  
LIB_DIRS_LOCAL   += $(foreach d, $(LIBS_LOCAL), $(LIBS_LOCAL_BASE)/$d/src/ $(dir $(call rwildcard,$(LIBS_LOCAL_BASE)/$d/src/,*/.)))                             # src and all subdirs of base

LIB_CPP_SHARED  := $(foreach d, $(LIB_DIRS_SHARED),$(call wildcard,$d*.cpp))
LIB_C_SHARED    := $(foreach d, $(LIB_DIRS_SHARED),$(call wildcard,$d*.c))

LIB_CPP_LOCAL   := $(foreach d, $(LIB_DIRS_LOCAL),$(call wildcard,$d/*.cpp))
LIB_C_LOCAL     := $(foreach d, $(LIB_DIRS_LOCAL),$(call wildcard,$d/*.c))

LIB_OBJ         := $(LIB_CPP_SHARED:$(LIBS_SHARED_BASE)/%.cpp=$(LIB_BIN)/%.o)  $(LIB_CPP_LOCAL:$(LIBS_LOCAL_BASE)/%.cpp=$(LIB_BIN)/%.o) 
LIB_OBJ         += $(LIB_C_SHARED:$(LIBS_SHARED_BASE)/%.c=$(LIB_BIN)/%.o)  $(LIB_C_LOCAL:$(LIBS_LOCAL_BASE)/%.c=$(LIB_BIN)/%.o) 

# Includes -------------------------------------------------------------
INCLUDE        := -I.\$(USR_SRC) -I$(CORE_SRC) 
INCLUDE        += $(foreach d, $(LIB_DIRS_SHARED), -I$d)
INCLUDE        += $(foreach d, $(LIB_DIRS_LOCAL), -I$d)

# Generate directories --------------------------------------------------------
DIRECTORIES :=  $(sort $(dir $(CORE_OBJ) $(USR_OBJ) $(LIB_OBJ)))
generateDirs := $(foreach d, $(DIRECTORIES), $(shell if not exist "$d" mkdir "$d"))

#$(info dirs: $(DIRECTORIES))

#$(info dirs: $(LIB_DIRS_LOCAL))
#$(info )
#$(info cpp: $(LIB_CPP_SHARED))
#$(info include: ${INCLUDE})

#******************************************************************************
# Rules:
#******************************************************************************

.PHONY: directories all rebuild upload uploadTy uploadCLI clean cleanUser cleanCore


all:  $(TARGET_LST) $(TARGET_HEX)

rebuild: cleanUser all

clean:   cleanUser cleanCore cleanLib

upload: $(TARGET_LST) $(TARGET_HEX)
	@$(UPL_PJRC)

uploadTy: $(TARGET_LST) $(TARGET_HEX)
	@$(UPL_TYCMD)

uploadCLI: $(TARGET_LST) $(TARGET_HEX)
	@$(UPL_CLICMD)


# Core library ----------------------------------------------------------------
$(CORE_BIN)/%.o: $(CORE_SRC)/%.S
	@echo CORE [ASM] $(notdir $<)
	@"$(CC)" $(S_FLAGS) $(INCLUDE) -o $@ -c $< 

$(CORE_BIN)/%.o: $(CORE_SRC)/%.c
	@echo CORE [CC]  $(notdir $<)
	@"$(CC)" $(C_FLAGS) $(INCLUDE) -o $@ -c $< 

$(CORE_BIN)/%.o: $(CORE_SRC)/%.cpp
	@echo CORE [CPP] $(notdir $<)
	@"$(CXX)" $(CPP_FLAGS) $(INCLUDE) -o $@ -c $< 

$(CORE_LIB) : $(CORE_OBJ)
	@echo CORE [AR]  $@
	@$(AR) $(AR_FLAGS) $@ $^
	@echo Teensy core built successfully &&echo.

# Shared Libraries ------------------------------------------------------------
$(LIB_BIN)/%.o: $(LIBS_SHARED_BASE)/%.S
	@echo LIB  [ASM] $(notdir $<)
	@"$(CC)" $(S_FLAGS) $(INCLUDE) -o $@ -c $< 

$(LIB_BIN)/%.o: $(LIBS_SHARED_BASE)/%.cpp 
	@echo LIB  [CPP] $(notdir $<)	
	@"$(CXX)" $(CPP_FLAGS) $(INCLUDE) -o $@ -c $< 

$(LIB_BIN)/%.o: $(LIBS_SHARED_BASE)/%.c
	@echo LIB  [CC]  $(notdir $<)	
	@"$(CC)" $(C_FLAGS) $(INCLUDE) -o $@ -c $< 
	
# Local Libraries -------------------------------------------------------------
$(LIB_BIN)/%.o: $(LIBS_LOCAL_BASE)/%.S
	@echo LIB  [ASM] $(notdir $<)
	@"$(CC)" $(S_FLAGS) $(INCLUDE) -o $@ -c $< 

$(LIB_BIN)/%.o: $(LIBS_LOCAL_BASE)/%.cpp
	@echo LIB  [CPP] $(notdir $<)	
	@"$(CXX)" $(CPP_FLAGS) $(INCLUDE) -o $@ -c $< 

$(LIB_BIN)/%.o: $(LIBS_LOCAL_BASE)/%.c
	@echo LIB  [CC]  $(notdir $<)	
	@"$(CC)" $(C_FLAGS) $(INCLUDE) -o $@ -c $< 
	
# Handle user sources ---------------------------------------------------------
$(USR_BIN)/%.o: $(USR_SRC)/%.S
	@echo USER [ASM] $<	
	@"$(CC)" $(S_FLAGS) $(INCLUDE) -o $@ -c $<

$(USR_BIN)/%.o: $(USR_SRC)/%.c
	@echo USER [CC]  $(notdir $<)	
	@"$(CC)" $(C_FLAGS) $(INCLUDE) -o "$@" -c $<

$(USR_BIN)/%.o: $(USR_SRC)/%.cpp
	@echo USER [CPP] $(notdir $<)	
	@"$(CXX)" $(CPP_FLAGS) $(INCLUDE) -o "$@" -c $<

# Linking ---------------------------------------------------------------------
$(TARGET_ELF): $(CORE_LIB) $(LIB_OBJ) $(USR_OBJ) 
	@echo [LD]  $@
	@$(CC) $(LD_FLAGS) -T$(CORE_SRC)/$(LD_SCRIPT) -o "$@" $(USR_OBJ) $(LIB_OBJ) $(CORE_LIB) $(LIBS)
	@echo User code built and linked to libraries &&echo.

%.lst: %.elf
	@echo [LST] $(notdir $@)
	@$(OBJDUMP) -d -S --demangle --no-show-raw-insn --syms "$<"  > "$@"
	@echo Listfile generated &&echo.

%.hex: %.elf
	@echo [HEX] $@
	@$(SIZE) "$<"
	@$(OBJCOPY) -O ihex -R.eeprom "$<" "$@"
	@echo Sucessfully built project &&echo.

# Cleaning --------------------------------------------------------------------
cleanUser:
	@echo Cleaning user binaries...
	@if exist $(USR_BIN) rd /s/q "$(USR_BIN)"

	@if exist $(TARGET_HEX) del  $(TARGET_HEX)
	@if exist $(TARGET_ELF) del  $(TARGET_ELF)
	@if exist $(TARGET_LST) del  $(TARGET_LST)

cleanCore:
	@echo Cleaning core binaries...
	@if exist $(CORE_BIN) rd /s/q "$(CORE_BIN)"
	@if exist $(CORE_LIB) del  "$(CORE_LIB)"

cleanLib:
	@echo Cleaning user library binaries...
	@if exist $(LIB_BIN) rd /s/q "$(LIB_BIN)"
	

# compiler generated dependency info ------------------------------------------
-include $(CORE_OBJ:.o=.d)
-include $(USR_OBJ:.o=.d)
-include $(LIB_OBJ:.o=.d)