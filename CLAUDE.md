# VisualTeensy

WPF tool that generates VS Code projects (makefile, tasks.json, IntelliSense config) for Teensy boards.

## Layout
- `VisualTeensy/` – WPF UI (.NET Framework 4.6.1, packages.config). Ships `make.exe` (GNU make 4.2), copied to the output folder.
- `vtCore/` – generator logic (netstandard2.0, PackageReference). Entry points: `Factory.makeLibManager/makeProject`, `CodeGenerator.getTasks`.
- `tester/` – old experiments, not part of the solution build.

## Build
```
nuget restore VisualTeensy.sln
msbuild VisualTeensy.sln -restore -p:Configuration=Debug
```
- `-restore` only restores vtCore (PackageReference); the WPF project needs `nuget restore`.
- Newtonsoft.Json must be the same version (13.0.x) in `VisualTeensy.csproj`, `packages.config` and `vtCore.csproj`; App.config redirects older versions. A mismatch crashes at startup in `LibraryReader.parseIndexJsonRepository`.
- Close a running VisualTeensy before building, the exe is locked otherwise.

## Where core / compiler / uploader paths come from
- **Quick setup**: everything is derived from `SetupData.arduinoBase` (Setup tab "Arduino folder") every time the makefile is generated.
  - Arduino 1.x folder: `hardware/teensy/avr`, `hardware/tools/arm`, `hardware/tools`.
  - Arduino15 data folder (Teensyduino >= 1.58, boards manager): resolved by `vtCore/Implementation/Arduino15.cs` – newest `packages/teensy/hardware/avr/<ver>`, tool versions (`teensy-compile`, `teensy-tools`) from the platform's `installed.json`.
- **Expert setup**: `coreBase` / `compilerBase` are copied from the Arduino folder when the project is created and then stored per project in `.vsteensy/vsteensy.json`. Changing the Arduino folder later does not affect them (see issue #80).
- Compiler flags come from `boards.txt` (`build.flags.*`), nothing version specific is hardcoded. IntelliSense `cppStandard` is parsed from `build.flags.cpp`.
- User settings (`Properties.Settings`) are written only when the app closes.

## Testing without the GUI
`tools/vt_generate.ps1` loads the Debug build of vtCore and generates a quick-setup project. Run it with Windows PowerShell 5.1 (.NET Framework); PowerShell 7 ships its own Newtonsoft.Json which conflicts with vtCore's:
```
powershell -NoProfile -ExecutionPolicy Bypass -File tools/vt_generate.ps1 -ProjectPath <empty folder> [-ArduinoBase <Arduino 1.x or Arduino15 folder>] [-Board "Teensy 4.1"]
make -C <project> all          # incremental build
make -C <project> uploadTy     # upload via TyCommanderC (if a TyTools folder was found)
```
Check which core/compiler a build used:
- makefile: `CORE_BASE`, `GCC_BASE`, `-DTEENSYDUINO=` (1.62.0 → 160)
- elf: `arm-none-eabi-readelf -p .comment <project>.elf` shows the gcc version

## Known issues / open items
- Issue #80: simplify upgrading to a new core (version display, core dropdown, upgrade hint, automatic core rebuild).
- Arduino-Builder build system only works with an Arduino 1.x installation (no arduino-builder in Arduino15).
- `vcSettingsJson.cs` hardcodes `compiler-explorer.compiler = "g54"`.
- Dependabot reports 2 moderate vulnerabilities on master.
