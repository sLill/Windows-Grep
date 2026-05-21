; ============================================================================
; WindowsGrep NSIS installer
; ----------------------------------------------------------------------------
; x64 build (default):
;   cargo build --release
;   makensis installer.nsi
;   -> WindowsGrep-<version>-Setup.exe
;
; ARM64 build:
;   rustup target add aarch64-pc-windows-msvc
;   cargo build --release --target aarch64-pc-windows-msvc
;   makensis /DTARGET_DIR=target\aarch64-pc-windows-msvc\release /DARCH_SUFFIX=-arm64 installer.nsi
;   -> WindowsGrep-<version>-arm64-Setup.exe
; ----------------------------------------------------------------------------
; The installer offers two modes via MultiUser.nsh:
;   * "Install for me only"   — no admin needed, installs to %LOCALAPPDATA%,
;                               writes everything under HKCU.
;   * "Install for everyone"  — needs admin, installs to Program Files,
;                               writes everything under HKLM/HKCR.
; If the user picks "for everyone" from a non-elevated launch, MultiUser
; re-launches the installer via UAC (so admins only see UAC when they ask
; for a system-wide install).
; ============================================================================

Unicode true
SetCompressor /SOLID lzma

; ----------------------------------------------------------------------------
; Application metadata
; ----------------------------------------------------------------------------

!define APP_NAME        "WindowsGrep"
!define APP_DISPLAY     "Windows Grep"
!define APP_VERSION     "6.0.1"
!define APP_VERSION_4   "6.0.1.0"
!define APP_PUBLISHER   "sLill"
!define APP_EXE         "grep.exe"
!define APP_URL         "https://github.com/sLill/Windows-Grep"
!define APP_REG_ROOT    "Software\${APP_NAME}"
!define APP_UNINST_KEY  "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"
!define ENV_KEY_HKLM    "SYSTEM\CurrentControlSet\Control\Session Manager\Environment"
!define ENV_KEY_HKCU    "Environment"
!define CTX_BG_SUBKEY   "Directory\Background\shell\${APP_NAME}"
!define CTX_DIR_SUBKEY  "Directory\shell\${APP_NAME}"

; Override these from the makensis command line to build for other targets, e.g.
;   makensis /DTARGET_DIR=target\aarch64-pc-windows-msvc\release /DARCH_SUFFIX=-arm64 installer.nsi
!ifndef TARGET_DIR
  !define TARGET_DIR    "target\release"
!endif
!ifndef ARCH_SUFFIX
  !define ARCH_SUFFIX   ""
!endif

; ----------------------------------------------------------------------------
; MultiUser configuration — must be defined before MultiUser.nsh is included.
;
; MULTIUSER_EXECUTIONLEVEL=Highest:
;   Standard users launch with no UAC and can install per-user. Admin users
;   get one UAC prompt at launch (same as the old admin-only installer) and
;   then choose per-user or all-users on the install-mode page. NSIS's
;   built-in MultiUser.nsh requires Highest/Admin/Power for the mode page;
;   "Standard" with on-demand elevation needs the Drizin NsisMultiUser
;   plugin, which isn't installed.
;
; ALLOW_BOTH_INSTALLATIONS=0:
;   If an existing install of the other mode is detected, prompt to remove
;   it first instead of letting two installs coexist.
; ----------------------------------------------------------------------------

!define MULTIUSER_EXECUTIONLEVEL                          Highest
!define MULTIUSER_MUI
!define MULTIUSER_INSTALLMODE_COMMANDLINE
!define MULTIUSER_INSTALLMODE_DEFAULT_CURRENTUSER
!define MULTIUSER_INSTALLMODE_ALLOW_BOTH_INSTALLATIONS    0
!define MULTIUSER_INSTALLMODE_INSTDIR                     "${APP_NAME}"
!define MULTIUSER_INSTALLMODE_INSTDIR_REGISTRY_KEY        "${APP_REG_ROOT}"
!define MULTIUSER_INSTALLMODE_INSTDIR_REGISTRY_VALUENAME  "InstallDir"
!define MULTIUSER_INSTALLMODE_DEFAULT_REGISTRY_KEY        "${APP_REG_ROOT}"
!define MULTIUSER_INSTALLMODE_DEFAULT_REGISTRY_VALUENAME  "InstallMode"
!define MULTIUSER_USE_PROGRAMFILES64

!include "MultiUser.nsh"
!include "MUI2.nsh"
!include "LogicLib.nsh"
!include "WinMessages.nsh"
!include "FileFunc.nsh"
!include "StrFunc.nsh"

; Declare StrFunc macros used below. StrStr is already declared by
; MultiUser.nsh; we only need to declare UnStrRep for the uninstaller.
${UnStrRep}

; ----------------------------------------------------------------------------
; Installer settings
; ----------------------------------------------------------------------------

Name "${APP_NAME} ${APP_VERSION}"
OutFile "WindowsGrep-${APP_VERSION}${ARCH_SUFFIX}-Setup.exe"
ShowInstDetails show
ShowUninstDetails show
BrandingText "${APP_NAME} v${APP_VERSION}"

VIProductVersion "${APP_VERSION_4}"
VIAddVersionKey "ProductName"     "${APP_NAME}"
VIAddVersionKey "ProductVersion"  "${APP_VERSION}"
VIAddVersionKey "CompanyName"     "${APP_PUBLISHER}"
VIAddVersionKey "FileDescription" "${APP_NAME} Installer"
VIAddVersionKey "FileVersion"     "${APP_VERSION_4}"
VIAddVersionKey "LegalCopyright"  "Copyright (c) Samuel Turner-Lill. MIT licensed."

; ----------------------------------------------------------------------------
; Modern UI
; ----------------------------------------------------------------------------

!define MUI_ABORTWARNING
!define MUI_FINISHPAGE_NOAUTOCLOSE
!define MUI_UNFINISHPAGE_NOAUTOCLOSE

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "..\LICENSE"

; Override the "all users" radio button label to flag that it needs admin.
!define MULTIUSER_INSTALLMODEPAGE_TEXT_ALLUSERS "Install for anyone using this computer (Requires Admin)"
!insertmacro MULTIUSER_PAGE_INSTALLMODE
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

; ----------------------------------------------------------------------------
; Init — operate on the 64-bit registry view and pick install mode.
; ----------------------------------------------------------------------------

Function .onInit
  SetRegView 64
  !insertmacro MULTIUSER_INIT
FunctionEnd

Function un.onInit
  SetRegView 64
  !insertmacro MULTIUSER_UNINIT
FunctionEnd

; ----------------------------------------------------------------------------
; Install sections
;
; SHCTX is the "shell context" hive — HKLM in AllUsers mode, HKCU in
; CurrentUser mode. We use it for everything that should follow the
; install-mode choice; everything else branches explicitly on
; $MultiUser.InstallMode.
; ----------------------------------------------------------------------------

Section "Core files (required)" SecCore
  SectionIn RO
  SetOutPath "$INSTDIR"
  SetOverwrite on

  File "${TARGET_DIR}\${APP_EXE}"
  File "/oname=LICENSE.txt" "..\LICENSE"

  ; Track install
  WriteRegStr SHCTX "${APP_REG_ROOT}" "InstallDir"  "$INSTDIR"
  WriteRegStr SHCTX "${APP_REG_ROOT}" "Version"     "${APP_VERSION}"
  WriteRegStr SHCTX "${APP_REG_ROOT}" "InstallMode" "$MultiUser.InstallMode"

  ; Uninstaller
  WriteUninstaller "$INSTDIR\uninstall.exe"

  ; Add / Remove Programs
  WriteRegStr   SHCTX "${APP_UNINST_KEY}" "DisplayName"          "${APP_NAME}"
  WriteRegStr   SHCTX "${APP_UNINST_KEY}" "DisplayVersion"       "${APP_VERSION}"
  WriteRegStr   SHCTX "${APP_UNINST_KEY}" "Publisher"            "${APP_PUBLISHER}"
  WriteRegStr   SHCTX "${APP_UNINST_KEY}" "URLInfoAbout"         "${APP_URL}"
  WriteRegStr   SHCTX "${APP_UNINST_KEY}" "DisplayIcon"          "$INSTDIR\${APP_EXE}"
  WriteRegStr   SHCTX "${APP_UNINST_KEY}" "InstallLocation"      "$INSTDIR"
  WriteRegStr   SHCTX "${APP_UNINST_KEY}" "UninstallString"      '"$INSTDIR\uninstall.exe"'
  WriteRegStr   SHCTX "${APP_UNINST_KEY}" "QuietUninstallString" '"$INSTDIR\uninstall.exe" /S'
  WriteRegDWORD SHCTX "${APP_UNINST_KEY}" "NoModify" 1
  WriteRegDWORD SHCTX "${APP_UNINST_KEY}" "NoRepair" 1

  ; For per-user installs, tell Windows not to prompt for UAC on uninstall.
  ${If} $MultiUser.InstallMode == "CurrentUser"
    WriteRegDWORD SHCTX "${APP_UNINST_KEY}" "NoElevateOnModify" 1
  ${EndIf}

  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  WriteRegDWORD SHCTX "${APP_UNINST_KEY}" "EstimatedSize" $0
SectionEnd

Section "Add to PATH" SecPath
  Push $0
  Push $1

  ${If} $MultiUser.InstallMode == "AllUsers"
    ReadRegStr $0 HKLM "${ENV_KEY_HKLM}" "PATH"
  ${Else}
    ReadRegStr $0 HKCU "${ENV_KEY_HKCU}" "PATH"
  ${EndIf}

  ; Wrap with ';' so we don't match a substring of another entry.
  ${StrStr} $1 ";$0;" ";$INSTDIR;"
  ${If} $1 == ""
    ${If} $0 == ""
      StrCpy $1 "$INSTDIR"
    ${Else}
      StrCpy $1 "$0;$INSTDIR"
    ${EndIf}

    ${If} $MultiUser.InstallMode == "AllUsers"
      WriteRegExpandStr HKLM "${ENV_KEY_HKLM}" "PATH" "$1"
    ${Else}
      WriteRegExpandStr HKCU "${ENV_KEY_HKCU}" "PATH" "$1"
    ${EndIf}

    SendMessage ${HWND_BROADCAST} ${WM_WININICHANGE} 0 "STR:Environment" /TIMEOUT=5000
    WriteRegDWORD SHCTX "${APP_REG_ROOT}" "PathAdded" 1
    DetailPrint "Added $INSTDIR to PATH"
  ${Else}
    DetailPrint "$INSTDIR is already in PATH"
  ${EndIf}

  Pop $1
  Pop $0
SectionEnd

Section "Explorer right-click integration" SecContext
  ; HKCR writes go to HKLM\Software\Classes (system-wide). For a per-user
  ; install we use HKCU\Software\Classes instead — Explorer merges the two
  ; on read, so context menus show up either way.
  ${If} $MultiUser.InstallMode == "AllUsers"
    WriteRegStr HKCR "${CTX_BG_SUBKEY}"          ""     "${APP_DISPLAY}"
    WriteRegStr HKCR "${CTX_BG_SUBKEY}"          "Icon" "$INSTDIR\${APP_EXE}"
    WriteRegStr HKCR "${CTX_BG_SUBKEY}\command"  ""     '"$INSTDIR\${APP_EXE}"'
    WriteRegStr HKCR "${CTX_DIR_SUBKEY}"         ""     "${APP_DISPLAY}"
    WriteRegStr HKCR "${CTX_DIR_SUBKEY}"         "Icon" "$INSTDIR\${APP_EXE}"
    WriteRegStr HKCR "${CTX_DIR_SUBKEY}\command" ""     '"cmd.exe" /C start "" /D "%1" "$INSTDIR\${APP_EXE}"'
  ${Else}
    WriteRegStr HKCU "Software\Classes\${CTX_BG_SUBKEY}"          ""     "${APP_DISPLAY}"
    WriteRegStr HKCU "Software\Classes\${CTX_BG_SUBKEY}"          "Icon" "$INSTDIR\${APP_EXE}"
    WriteRegStr HKCU "Software\Classes\${CTX_BG_SUBKEY}\command"  ""     '"$INSTDIR\${APP_EXE}"'
    WriteRegStr HKCU "Software\Classes\${CTX_DIR_SUBKEY}"         ""     "${APP_DISPLAY}"
    WriteRegStr HKCU "Software\Classes\${CTX_DIR_SUBKEY}"         "Icon" "$INSTDIR\${APP_EXE}"
    WriteRegStr HKCU "Software\Classes\${CTX_DIR_SUBKEY}\command" ""     '"cmd.exe" /C start "" /D "%1" "$INSTDIR\${APP_EXE}"'
  ${EndIf}

  WriteRegDWORD SHCTX "${APP_REG_ROOT}" "ContextMenuAdded" 1
SectionEnd

; ----------------------------------------------------------------------------
; Section descriptions
; ----------------------------------------------------------------------------

LangString DESC_SecCore    ${LANG_ENGLISH} "Installs ${APP_EXE} and the license. Required."
LangString DESC_SecPath    ${LANG_ENGLISH} "Appends the install directory to PATH so '${APP_EXE}' is runnable from any shell."
LangString DESC_SecContext ${LANG_ENGLISH} "Adds 'Open in ${APP_NAME}' to the Explorer right-click menu for folders."

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SecCore}    $(DESC_SecCore)
  !insertmacro MUI_DESCRIPTION_TEXT ${SecPath}    $(DESC_SecPath)
  !insertmacro MUI_DESCRIPTION_TEXT ${SecContext} $(DESC_SecContext)
!insertmacro MUI_FUNCTION_DESCRIPTION_END

; ----------------------------------------------------------------------------
; Uninstall
; ----------------------------------------------------------------------------

Section "Uninstall"
  ; --- Remove PATH entry if we added one ---
  ReadRegDWORD $0 SHCTX "${APP_REG_ROOT}" "PathAdded"
  ${If} $0 == 1
    Push $1
    Push $2

    ${If} $MultiUser.InstallMode == "AllUsers"
      ReadRegStr $1 HKLM "${ENV_KEY_HKLM}" "PATH"
    ${Else}
      ReadRegStr $1 HKCU "${ENV_KEY_HKCU}" "PATH"
    ${EndIf}

    ${If} $1 != ""
      ; Wrap with ';' on each side, replace ";INSTDIR;" with ";", then unwrap.
      StrCpy $2 ";$1;"
      ${UnStrRep} $2 "$2" ";$INSTDIR;" ";"
      ${If} $2 == ";"
        StrCpy $2 ""
      ${Else}
        StrLen $0 $2
        IntOp $0 $0 - 2
        StrCpy $2 $2 $0 1
      ${EndIf}

      ${If} $MultiUser.InstallMode == "AllUsers"
        WriteRegExpandStr HKLM "${ENV_KEY_HKLM}" "PATH" "$2"
      ${Else}
        WriteRegExpandStr HKCU "${ENV_KEY_HKCU}" "PATH" "$2"
      ${EndIf}
      SendMessage ${HWND_BROADCAST} ${WM_WININICHANGE} 0 "STR:Environment" /TIMEOUT=5000
    ${EndIf}

    Pop $2
    Pop $1
  ${EndIf}

  ; --- Remove context menu entries ---
  ${If} $MultiUser.InstallMode == "AllUsers"
    DeleteRegKey HKCR "${CTX_BG_SUBKEY}\command"
    DeleteRegKey HKCR "${CTX_BG_SUBKEY}"
    DeleteRegKey HKCR "${CTX_DIR_SUBKEY}\command"
    DeleteRegKey HKCR "${CTX_DIR_SUBKEY}"
  ${Else}
    DeleteRegKey HKCU "Software\Classes\${CTX_BG_SUBKEY}\command"
    DeleteRegKey HKCU "Software\Classes\${CTX_BG_SUBKEY}"
    DeleteRegKey HKCU "Software\Classes\${CTX_DIR_SUBKEY}\command"
    DeleteRegKey HKCU "Software\Classes\${CTX_DIR_SUBKEY}"
  ${EndIf}

  ; --- Files ---
  Delete "$INSTDIR\${APP_EXE}"
  Delete "$INSTDIR\LICENSE.txt"
  Delete "$INSTDIR\uninstall.exe"
  RMDir  "$INSTDIR"

  ; --- Registry cleanup ---
  DeleteRegKey SHCTX "${APP_UNINST_KEY}"
  DeleteRegKey SHCTX "${APP_REG_ROOT}"
SectionEnd
