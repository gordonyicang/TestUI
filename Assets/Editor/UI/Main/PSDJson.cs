using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEditor;
using System;

public class PSDJson {
	private string jsonFile;
	public Model model;

	// 初始化
	public PSDJson(string file) {
		jsonFile = file;
//		Debug.Log (jsonFile);
		model = DeserializeJson(file);
		Debug.Log (model);
	}

	// 解析JSON
	public Model DeserializeJson(string file) {
		if (jsonFile != null) {
			StreamReader jsonReader = File.OpenText(file);
			if (jsonReader != null) {
				string jsonStr = jsonReader.ReadToEnd ();
				/*
//				Debug.Log (jsonStr);
				// JsonUntility序列化
//				PSDUI psdui = new PSDUI();
//				psdui.strTest = "psd str test";
//				psdui.intTest = 123456;
//				string jsonStrTest = JsonUtility.ToJson (psdui);
//				Debug.Log (jsonStrTest);
//				{"strTest":"psd str test","intTest":123456}
				// 使用JsonUntility反序列化 
//				string jsonStrTest = "{\"strTest\":\"psd str test\",\"intTest\":123456}";
//				PSDUI psdui = JsonUtility.FromJson<PSDUI> (jsonStrTest);
//				Debug.Log (psdui.strTest);

//				string jsonStrTest = "{\"PSDUI\":{\"psdSize\":{\"width\":\"1200\",\"height\":\"640\"}}}";
//				Model model = JsonUtility.FromJson<Model>(jsonStr);
//				Debug.Log (model.PSDUI.psdSize.width + "=====psdSize======" + model.PSDUI.psdSize.height);
				*/

				model = JsonUtility.FromJson<Model> (jsonStr);

				/*
//				Debug.Log (model.children[0].children);
//				Debug.Log (model.children[0].name);
				*/
			}
		}
		return model;
	}
}
