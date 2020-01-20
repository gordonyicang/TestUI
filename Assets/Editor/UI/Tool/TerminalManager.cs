using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEditor;
using System.IO;


public class TerminalManager : MonoBehaviour {

	private static string PROCESS_MATCH = "nothing to do";

	public static bool cmd(string command, string argument)
	{
		ProcessStartInfo start = new ProcessStartInfo(command);
		start.Arguments = argument;
		start.CreateNoWindow = false;
		start.ErrorDialog = true;
		start.UseShellExecute = false;

		if (start.UseShellExecute)
		{
			start.RedirectStandardOutput = false;
			start.RedirectStandardError = false;
			start.RedirectStandardInput = false;
		}
		else
		{
			start.RedirectStandardOutput = true;
			start.RedirectStandardError = true;
			start.RedirectStandardInput = true;
			start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
			start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
		}
		Process p = Process.Start(start);
		string outPut = "";
		if (!start.UseShellExecute)
		{
			outPut = p.StandardOutput.ReadToEnd();
			UnityEngine.Debug.Log("###ter output:"+outPut);
			UnityEngine.Debug.Log("###ter error:"+p.StandardError.ReadToEnd());
		}
		p.WaitForExit();
		p.Close();
		AssetDatabase.Refresh();

		bool hasChange = outPut.IndexOf(PROCESS_MATCH) == -1;
		return hasChange;
		// BuildTexturePacker();
		// ClearOtherFiles();
		// AssetDatabase.Refresh();
	}
}
