using UnityEngine;
using System.Diagnostics;
using UnityEditor;

public class ImageChannelSpliterWrapper: ScriptableObject
{
    public static void Execute(string pngPath)
    {
        pngPath = string.Concat(Application.dataPath.Replace("/Assets", "/"), pngPath);
        pngPath += ".png";
        string etcToolPath = string.Concat(Application.dataPath.Replace("/Assets", ""), "/EtcTool/");
        string etcBatPath = etcToolPath + "ImageChannelSpliter.bat";
        string alphaPath = pngPath.Replace(".png", "_alpha.png");
        Process process = new Process();
        string paramContent = string.Format("\"{0}\" \"{1}\" \"{2}\"", pngPath, alphaPath, etcToolPath);
        ProcessStartInfo info = new ProcessStartInfo(etcBatPath, paramContent);
        process.StartInfo = info;
        process.Start();
        process.WaitForExit();
        process.Close();
        AssetDatabase.Refresh();
    }
}
