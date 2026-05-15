; ============================================================================
; WindowsGrep NSIS installer
; ----------------------------------------------------------------------------
; Build a release binary first:   cargo build --release
; Then compile this script:       makensis installer.nsi
; Output:                         WindowsGrep-<version>-Setup.exe
; ============================================================================

Unicode true
SetCompressor /SOLID lzma

!include "MUI2.nsh"
!include "LogicLib.nsh"
!include "WinMessages.nsh"
!include "FileFunc.nsh"
!include "StrFunc.nsh"

; Declare StrFunc macros used below
${StrStr}
${UnStrStr}
${UnStrRep}

; ----------------------------------------------------------------------------
; Application metadata
; ----------------------------------------------------------------------------

!define APP_NAME        "WindowsGrep"
!define APP_VERSION     "5.0.0"
!define APP_VERSION_4   "5.0.0.0"
!define APP_PUBLISHER   "sLill"
!define APP_EXE         "grep.exe"
!define APP_URL         "https://github.com/sLill/Windows-Grep"
!define APP_REG_ROOT    "Software\${APP_NAME}"
!define APP_UNINST_KEY  "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"
!define ENV_REG_KEY     "SYSTEM\CurrentControlSet\Control\Session Manager\Environment"
!define CTX_BG_KEY      "Directory\Background\shell\${APP_NAME}"
!define CTX_DIR_KEY     "Directory\shell\${APP_NAME}"

; ----------------------------------------------------------------------------
; Installer settings
; ----------------------------------------------------------------------------

Name "${APP_NAME} ${APP_VERSION}"
OutFile "WindowsGrep-${APP_VERSION}-Setup.exe"
InstallDir "$PROGRAMFILES64\${APP_NAME}"
InstallDirRegKey HKLM "${APP_REG_ROOT}" "InstallDir"
RequestExecutionLevel admin
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
; Init — operate on the 64-bit registry view on 64-bit Windows.
; ----------------------------------------------------------------------------

Function .onInit
  SetRegView 64
FunctionEnd

Function un.onInit
  SetRegView 64
FunctionEnd

; ----------------------------------------------------------------------------
; Install sections
; ----------------------------------------------------------------------------

Section "Core files (required)" SecCore
  SectionIn RO
  SetOutPath "$INSTDIR"
  SetOverwrite on

  File "target\release\${APP_EXE}"
  File "/oname=LICENSE.txt" "..\LICENSE"

  ; Track install
  WriteRegStr HKLM "${APP_REG_ROOT}" "InstallDir" "$INSTDIR"
  WriteRegStr HKLM "${APP_REG_ROOT}" "Version"    "${APP_VERSION}"

  ; Uninstaller
  WriteUninstaller "$INSTDIR\uninstall.exe"

  ; Add / Remove Programs
  WriteRegStr   HKLM "${APP_UNINST_KEY}" "DisplayName"          "${APP_NAME}"
  WriteRegStr   HKLM "${APP_UNINST_KEY}" "DisplayVersion"       "${APP_VERSION}"
  WriteRegStr   HKLM "${APP_UNINST_KEY}" "Publisher"            "${APP_PUBLISHER}"
  WriteRegStr   HKLM "${APP_UNINST_KEY}" "URLInfoAbout"         "${APP_URL}"
  WriteRegStr   HKLM "${APP_UNINST_KEY}" "DisplayIcon"          "$INSTDIR\${APP_EXE}"
  WriteRegStr   HKLM "${APP_UNINST_KEY}" "InstallLocation"      "$INSTDIR"
  WriteRegStr   HKLM "${APP_UNINST_KEY}" "UninstallString"      '"$INSTDIR\uninstall.exe"'
  WriteRegStr   HKLM "${APP_UNINST_KEY}" "QuietUninstallString" '"$INSTDIR\uninstall.exe" /S'
  WriteRegDWORD HKLM "${APP_UNINST_KEY}" "NoModify" 1
  WriteRegDWORD HKLM "${APP_UNINST_KEY}" "NoRepair" 1

  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  WriteRegDWORD HKLM "${APP_UNINST_KEY}" "EstimatedSize" $0
SectionEnd

Section "Add to system PATH" SecPath
  Push $0
  Push $1

  ReadRegStr $0 HKLM "${ENV_REG_KEY}" "PATH"
  ; Wrap with ';' so we don't match a substring of another entry.
  ${StrStr} $1 ";$0;" ";$INSTDIR;"
  ${If} $1 == ""
    ${If} $0 == ""
      WriteRegExpandStr HKLM "${ENV_REG_KEY}" "PATH" "$INSTDIR"
    ${Else}
      WriteRegExpandStr HKLM "${ENV_REG_KEY}" "PATH" "$0;$INSTDIR"
    ${EndIf}
    SendMessage ${HWND_BROADCAST} ${WM_WININICHANGE} 0 "STR:Environment" /TIMEOUT=5000
    WriteRegDWORD HKLM "${APP_REG_ROOT}" "PathAdded" 1
    DetailPrint "Added $INSTDIR to system PATH"
  ${Else}
    DetailPrint "$INSTDIR is already in system PATH"
  ${EndIf}

  Pop $1
  Pop $0
SectionEnd

Section "Explorer right-click integration" SecContext
  ; Right-click on folder background -> opens grep REPL in that folder.
  WriteRegStr HKCR "${CTX_BG_KEY}" ""        "Open in ${APP_NAME}"
  WriteRegStr HKCR "${CTX_BG_KEY}" "Icon"    "$INSTDIR\${APP_EXE}"
  WriteRegStr HKCR "${CTX_BG_KEY}\command" "" '"$INSTDIR\${APP_EXE}"'

  ; Right-click on a folder -> opens grep REPL with that folder as CWD.
  WriteRegStr HKCR "${CTX_DIR_KEY}" ""        "Open in ${APP_NAME}"
  WriteRegStr HKCR "${CTX_DIR_KEY}" "Icon"    "$INSTDIR\${APP_EXE}"
  WriteRegStr HKCR "${CTX_DIR_KEY}\command" "" '"cmd.exe" /C start "" /D "%1" "$INSTDIR\${APP_EXE}"'

  WriteRegDWORD HKLM "${APP_REG_ROOT}" "ContextMenuAdded" 1
SectionEnd

; ----------------------------------------------------------------------------
; Section descriptions
; ----------------------------------------------------------------------------

LangString DESC_SecCore    ${LANG_ENGLISH} "Installs ${APP_EXE} and the license. Required."
LangString DESC_SecPath    ${LANG_ENGLISH} "Appends the install directory to the system PATH so '${APP_EXE}' is runnable from any shell."
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
  ReadRegDWORD $0 HKLM "${APP_REG_ROOT}" "PathAdded"
  ${If} $0 == 1
    Push $1
    Push $2
    ReadRegStr $1 HKLM "${ENV_REG_KEY}" "PATH"
    ${If} $1 != ""
      ; Wrap with ';' on each side, replace ";INSTDIR;" with ";", then unwrap.
      StrCpy $2 ";$1;"
      ${UnStrRep} $2 "$2" ";$INSTDIR;" ";"
      ${If} $2 == ";"
        StrCpy $2 ""
      ${Else}
        ; Strip leading and trailing ';'
        StrLen $0 $2
        IntOp $0 $0 - 2
        StrCpy $2 $2 $0 1
      ${EndIf}
      WriteRegExpandStr HKLM "${ENV_REG_KEY}" "PATH" "$2"
      SendMessage ${HWND_BROADCAST} ${WM_WININICHANGE} 0 "STR:Environment" /TIMEOUT=5000
    ${EndIf}
    Pop $2
    Pop $1
  ${EndIf}

  ; --- Remove context menu entries (unconditional cleanup) ---
  DeleteRegKey HKCR "${CTX_BG_KEY}\command"
  DeleteRegKey HKCR "${CTX_BG_KEY}"
  DeleteRegKey HKCR "${CTX_DIR_KEY}\command"
  DeleteRegKey HKCR "${CTX_DIR_KEY}"

  ; --- Files ---
  Delete "$INSTDIR\${APP_EXE}"
  Delete "$INSTDIR\LICENSE.txt"
  Delete "$INSTDIR\uninstall.exe"
  RMDir  "$INSTDIR"

  ; --- Registry cleanup ---
  DeleteRegKey HKLM "${APP_UNINST_KEY}"
  DeleteRegKey HKLM "${APP_REG_ROOT}"
SectionEnd
