using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Runtime.InteropServices;

public class FileManager
{
	/**
	 * 拷贝目录
	 * 
	 * srcDir 来源文件夹
	 * tarDir 目标文件夹
	 * 
	 * 注:srcDir tarDir 需要统一格式(相对路径or绝对路径)
	*/
	public static void copyDir(string srcDir,string tarDir) {
		if (!Directory.Exists (srcDir)) {
			return;
		}
		if (!Directory.Exists (tarDir)) {
			Directory.CreateDirectory (tarDir);
		}
		string[] srcFiles = Directory.GetFiles (srcDir,"*",SearchOption.AllDirectories);
		foreach (string srcFile in srcFiles) {
			// 生成拷贝的目录结构
			string tarFile = srcFile.Replace (srcDir,tarDir);
			string tarFileParent = Path.GetDirectoryName (tarFile);
			if (!Directory.Exists (tarFileParent)) {
				Directory.CreateDirectory (tarFileParent);
			}
			if (!File.Exists (tarFile)) {
				File.Copy (srcFile,tarFile);
			}
		}
		AssetDatabase.Refresh ();
	}
}


