using KYSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace KYSystem
{
    class SpaceDataMgr
    {
        public static string PathPointName = "{0}_PathPoint";
 
        public static GameObject mapObj;
        

        public static SpaceData spaceData = new SpaceData();
        public static string mapFolder = string.Concat(Application.dataPath, "/Resources/Maps/");
        public static string spaceFolder = string.Concat(Application.dataPath, "/MapTestInfo/source/");

        public static SpaceDataMgr instance = new SpaceDataMgr();

        public SpaceDataMgr() { }


        public static void LoadBaseSettingsPrefab()
        {
            /*spaceSettingObj = GameObject.Find("spaceSetting");
            entitySettingObj = GameObject.Find("entitySetting");
            if (spaceSettingObj == null) spaceSettingObj = new GameObject("spaceSetting");
            if (entitySettingObj == null) entitySettingObj = new GameObject("entitySetting");
            entitySettingObj.transform.SetParent(spaceSettingObj.transform);*/
        }

        private static SecurityElement pathPointList = null;
        public static void LoadSpaceFile(string spaceName, string mapAssetPath)
        {
            spaceData.mapName = spaceName;
            Debug.Log("[Space.Open] mapName=" + spaceData.mapName);
            //SecurityElement rootNode = XMLParser.LoadXML(FileUtils.LoadTextFile(spaceData.spacePath));
            //if (rootNode != null)
            //{
            //    spaceData.mapName = rootNode.SearchForTextOfTag("mapName");
            //    spaceData.trapName = rootNode.SearchForTextOfTag("trapName");
            //    spaceData.spaceName = spaceName;
            //    pathPointList = rootNode.SearchForChildByTag("path_points");
            //}
            //else 
            //{//打开一个没编辑过的.untity文件，获取.unity文件所在路径的父目录作为mapName
            //string dir = Path.GetDirectoryName(mapAssetPath).Replace("\\", "/");
            //spaceData.mapName = dir.Substring(dir.LastIndexOf("/") + 1);
            //Debug.Log("[Space.Open] mapName=" + spaceData.mapName);
            //}
        }

        public static void LoadMapPrefab()
        {
            mapObj = GameObject.Find(SpaceDataMgr.spaceData.mapName);
            if (mapObj == null)
            {
                UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(SpaceDataMgr.spaceData.mapPath);
                mapObj = (GameObject)GameObject.Instantiate(obj);
                mapObj.name = SpaceDataMgr.spaceData.mapName;
            }
            Debug.Log("[LoadMapPrefab] mapName=" + SpaceDataMgr.spaceData.mapName);
        }

        //加载路点配置
        public static void LoadPathPoints(string spaceName)
        {
            bool isNew = false;
            string pathPointName = string.Format(PathPointName, spaceName);
            string pathPointPath = "Assets/MapTestInfo/PathPoint/" + pathPointName + ".prefab";
            GameObject oldPathPoint = GameObject.Find(pathPointName);
            if (oldPathPoint != null)
            {
                GameObject.DestroyImmediate(oldPathPoint);
            }
            GameObject parent = (GameObject)AssetDatabase.LoadAssetAtPath(pathPointPath, typeof(GameObject));
            if (parent == null)
            {
                parent = new GameObject(pathPointName);
                isNew = true;
            }
            else 
            {
                parent = (GameObject)GameObject.Instantiate(parent);
                parent.name = pathPointName;
            }
            AssetDatabase.Refresh();
            parent.transform.localPosition = Vector3.zero;
            parent.transform.localScale = Vector3.one;
            parent.transform.localEulerAngles = Vector3.zero;
            Debug.Log("[LoadPathPoints] isNew=" + isNew);
        }

        public static void SaveSpace()
        {
            if (mapObj == null)
            {
                mapObj = GameObject.Find(SpaceDataMgr.spaceData.mapName);
            }
            Debug.Log("[SaveSpace] mapName=" + SpaceDataMgr.spaceData.mapName + ", mapObj=" + mapObj);
            if (mapObj != null)
            {
                //ResetBaseSettings();
                SaveToFile();
            }
            else 
            {
                EditorUtility.DisplayDialog("提示", "当前没有可编辑场景，请先打开？", "确定");
            }
        }

        private static void ResetBaseSettings()
        {
            SceneLightMgr sceneLightMgr = mapObj.GetComponentInChildren<SceneLightMgr>();
            if (sceneLightMgr == null)
            {
                EditorUtility.DisplayDialog("严重", mapObj.name + ".prefab文件没挂载SceneLightMgr.cs脚本！", "确定");
                return;
            }
            //spaceData.ambientLight = SpaceEditorUtils.FormatColor(sceneLightMgr._Ambient*sceneLightMgr._AmbientIntensity);
            //spaceData.objDiffuse = SpaceEditorUtils.FormatColor(sceneLightMgr._ObjDiffuse * sceneLightMgr._ObjDiffuseIntensity);
            //spaceData.objSpecular = SpaceEditorUtils.FormatColor(sceneLightMgr._ObjDiffuse * sceneLightMgr._ObjDiffuseIntensity);

            //spaceData.charDiffuse = SpaceEditorUtils.FormatColor(sceneLightMgr._CharDiffuse * sceneLightMgr._ObjSpecularIntensity);
            //spaceData.charSpecular = SpaceEditorUtils.FormatColor(sceneLightMgr._CharSpecular * sceneLightMgr._CharSpecularIntensity);
            //spaceData.fogColor1 = SpaceEditorUtils.FormatColor(sceneLightMgr._FogColor1);
            //spaceData.fogColor2 = SpaceEditorUtils.FormatColor(sceneLightMgr._FogColor2);
            //spaceData.fogStart = sceneLightMgr._FogStart.ToString("f3");
            //spaceData.fogEnd = sceneLightMgr._FogEnd.ToString("f3");
            //spaceData.fogIntensity = sceneLightMgr._FogIntensity.ToString("f3");

            //spaceData.lightmaps = SpaceEditorUtils.FormatLightmapData(LightmapSettings.lightmaps);
            //spaceData.lightProbes = SpaceEditorUtils.FormatLightProbes(LightmapSettings.lightProbes);
        }

        public static void SaveToFile()
        {
            StringBuilder stringBuilder = new StringBuilder("");
            //stringBuilder.Append();
            string sceneId = spaceData.spaceName;
            string dirPath = string.Concat(Application.dataPath, "/MapTestInfo/");
            string srcPath = dirPath+"source/";
            string txtFilePath = string.Concat(srcPath, sceneId, "p" ,".txt");
            string bmFilePath = Path.Combine(dirPath, string.Format("{0}p.txt", sceneId));
            // string bmFilePath = Path.Combine(Application.streamingAssetsPath, string.Format("{0}p.txt", sceneId));
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            if (!Directory.Exists(srcPath)) Directory.CreateDirectory(srcPath);
            if (File.Exists(txtFilePath)) File.Delete(txtFilePath);
            if (File.Exists(bmFilePath)) File.Delete(bmFilePath);
            //生成二进制文件
            FileStream fs = new FileStream(bmFilePath, FileMode.Create);
            MapInfoWriter bmWriter = new MapInfoWriter(fs);

            StreamWriter txtWriter = File.CreateText(txtFilePath);
            SavePathPointContent(spaceData.spaceName, bmWriter , txtWriter);
            //txtWriter.WriteLine(result);
            txtWriter.Close();
            bmWriter.Close();
            SavePathPointAsPrefab(spaceData.spaceName);
            Debug.Log("寻路点信息已保存：" + txtFilePath);

            string assetPath = Path.Combine(Application.streamingAssetsPath, string.Format("{0}p.txt", sceneId));
            File.Copy(bmFilePath,assetPath,true);
            // EditorUtility.DisplayDialog("保存成功", "[保存成功] Space信息已保存到=" + txtFilePath, "确定");
        }

        private static string SavePathPointContent(string spaceName, MapInfoWriter writer = null, StreamWriter streamWriter = null)
        {
            string pathPointName = string.Format(PathPointName, spaceName);
            GameObject parent = GameObject.Find(pathPointName);
            if (parent != null)
            {
                Transform child = null;
                int childCount = parent.transform.childCount;
                List<int> checkList = new List<int>();
                StringBuilder str = new StringBuilder("");
                writer.WriteString16(spaceName);
                //生成编辑内容
                writer.WriteInt16((short)childCount);
                short[,] weightList = new short[childCount,childCount];
                List<Vector3> posList = new List<Vector3>(); 
                for (int i = 0; i < childCount; i++)
                {
                    child = parent.transform.GetChild(i);
                    child.name = (i+1).ToString();
                    int pointId = int.Parse(child.name);

                    // writer.WriteInt16((short)pointId);

                    float x = (float)Math.Round(child.position.x, 2);
                    float z = (float)Math.Round(child.position.z, 2);
                    float y = (float)Math.Round(child.position.y, 2);
                    int writeX = Convert.ToInt32(x * 100);
                    int writeY = Convert.ToInt32(y * 100);
                    int writeZ = Convert.ToInt32(z * 100);
                    //Debug.Log("pointId:" + pointId + "  x:" + writeX + "  z:" + writeZ + "  y:" + writeY);
                    writer.WriteInt16((short)writeX);
                    // writer.WriteInt16((short)writeY);
                    writer.WriteInt16((short)writeZ);

                    posList.Add(new Vector3(child.position.x, child.position.y, child.position.z));

                    streamWriter.WriteLine("pointId:" + pointId + "  x:" + writeX + "  z:" + writeZ + "  y:" + writeY);
                }

                for (int i = 0; i < posList.Count; i++)
                {
                    for (int j = 0; j < posList.Count; j++)
                    {
                        int writeDis = 200000;
                        float x1 = posList[i].x;
                        float z1 = posList[i].z;
                        float x2 = posList[j].x;
                        float z2 = posList[j].z;
                        // float dis = getDistance(x1, z1, x2, z2);
                         if (i != j && isCanWalk(x1, z1, x2, z2))
                        {
                            float sqr = (x1 - x2) * (x1 - x2) + (z1 - z2) * (z1 - z2);
                            double s = Math.Sqrt(sqr);
                            writeDis = Convert.ToInt32(s * 100);
                            //Debug.Log("i:" + i + "  j:" + j + "  d:" + s + "  x1:" + x1 + "  z1:" + z1 + "  x2:" + x2 + "  z2:" + z2 + "  sqr:" + sqr);
                        }
                        writer.WriteInt32(writeDis);
                        streamWriter.WriteLine("i:" + i + "  j:" + j + "  dis:" + writeDis);
                    }
                }

                return str.ToString();
            }
            else
            {
                Debug.LogError("找不到PathPoints,无法生成寻路点信息!!");
            }
            return string.Empty;
        }

        private static bool isCanWalk(float startX,float startZ,float endX,float endZ)
        {
            Vector3 normalize = new Vector3(endX - startX , 0 , endZ - startZ);
            float distance = normalize.magnitude;
            normalize.Normalize();
            int len = Convert.ToInt32(Math.Floor(distance));
            for (int i = 1; i < len; i++)
            {
                float nextX = startX + normalize.x * i;
                float nextZ = startZ + normalize.z * i;
                if (MapInfoMgr.instance.PointIsCanWalk(nextX, nextZ) == false)
                {
                    return false;
                }
            }
            return true;
        }


        //private static string SavePathPointContent(string spaceName, MapInfoWriter writer = null)
        //{
        //    string pathPointName = string.Format(PathPointName, spaceName);
        //    GameObject parent = GameObject.Find(pathPointName);
        //    if (parent != null)
        //    {
        //        Transform child = null;
        //        EditPathPoint editPathPoint = null;
        //        int childCount = parent.transform.childCount;
        //        List<int> checkList = new List<int>();
        //        StringBuilder str = new StringBuilder("");
        //        Debug.Log("========== childCount=" + childCount);
        //        //A-1 检查编辑合法性
        //        for (int i = 0; i < childCount; i++)
        //        {
        //            child = parent.transform.GetChild(i);
        //            editPathPoint = child.GetComponent<EditPathPoint>();
        //            if (editPathPoint == null)
        //            {
        //                str.Append(string.Format("id={0}未挂载EditPathPoint脚本\n", child.name));
        //            }
        //            else
        //            {
        //                int id = int.Parse(child.name);
        //                if (checkList.Contains(id))
        //                {
        //                    str.Append(string.Format("id={0}已重复\n", id));
        //                }
        //                else 
        //                {
        //                    checkList.Add(id);
        //                }
        //            }
        //        }
        //        if (str.Length > 0)
        //        {
        //            EditorUtility.DisplayDialog("错误", str.ToString(), "确定");
        //            return null;
        //        }
        //        writer.WriteString16(spaceName);
        //        //A-2 生成编辑内容
        //        str.Remove(0, str.Length);
        //        writer.WriteInt16((short)childCount);
        //        for (int i = 0; i < childCount; i++)
        //        {
        //            child = parent.transform.GetChild(i);
        //            int pointId = int.Parse(child.name);
        //            editPathPoint = child.GetComponent<EditPathPoint>();
        //            editPathPoint.pointId = pointId;
        //            str.Append(editPathPoint.ToStringTxt());

        //            writer.WriteInt16((short)pointId);

        //            float x = (float)Math.Round(child.position.x, 2);
        //            float z = (float)Math.Round(child.position.z, 2);
        //            float y = (float)Math.Round(child.position.y, 2);
        //            int writeX = Convert.ToInt32(x * 100);
        //            int writeY = Convert.ToInt32(y * 100);
        //            int writeZ = Convert.ToInt32(z * 100);
        //            Debug.Log("pointId:" + pointId + "  x:" + writeX + "  z:" + writeZ + "  y:" + writeY);
        //            writer.WriteInt16((short)writeX);
        //            writer.WriteInt16((short)writeY);
        //            writer.WriteInt16((short)writeZ);
        //            List<int> relationtList = editPathPoint.relationPointList;
        //            writer.WriteInt16((short)relationtList.Count);
        //            foreach (int item in relationtList)
        //            {
        //                writer.WriteByte((byte)item);
        //            }
        //        }
        //        return str.ToString();
        //    }
        //    else 
        //    {
        //        Debug.LogError("找不到PathPoints,无法生成寻路点信息!!");
        //    }
        //    return string.Empty;
        //}

        private static void SavePathPointAsPrefab(string spaceName)
        {
            string pathPointName = string.Format(PathPointName, spaceName);
            GameObject parent = GameObject.Find(pathPointName);
            //if (parent != null && parent.transform.childCount > 0)
            {
                string path = Application.dataPath + "/MapTestInfo/PathPoint/" + pathPointName + ".prefab";
                if(File.Exists(path))
                {
                    string name = "Assets/MapTestInfo/PathPoint/" + pathPointName+".prefab";
                    var oldPrefab = AssetDatabase.LoadAssetAtPath(name, typeof(GameObject));
                    Debug.Log(name+"   "+oldPrefab.name);
                    PrefabUtility.ReplacePrefab(parent, oldPrefab);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    Debug.Log(path);
                    PrefabUtility.CreatePrefab("Assets/MapTestInfo/PathPoint/" + pathPointName + ".prefab", parent);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        public static void DrawPathPoint(string spaceName)
        {
            // string pathPointName = string.Format(PathPointName, spaceName);
            // GameObject parent = GameObject.Find(pathPointName);
            // if (parent != null)
            // {
            //     Transform tran = null;
            //     EditPathPoint cur = null;
            //     Dictionary<int, EditPathPoint> dict =new Dictionary<int,EditPathPoint>();
            //     int childCount = parent.transform.childCount;
            //     for (int i = 0; i < childCount; i++)
            //     {
            //         tran = parent.transform.GetChild(i);
            //         cur = tran.GetComponent<EditPathPoint>();
            //         if (cur != null)
            //         {
            //             cur.pointId = int.Parse(cur.transform.name);
            //             if (!dict.ContainsKey(cur.pointId))
            //             {
            //                 dict.Add(cur.pointId, cur);
            //             }
            //             else
            //             {
            //                 Debug.LogError("顶点编号已重复id=" + cur.pointId);
            //             }
            //         }
            //         else
            //         {
            //             Debug.LogError("顶点id=" + tran.name + " 未挂载EditPathPoint脚本");
            //         }
            //     }

            //     foreach (EditPathPoint item in dict.Values)
            //     {
            //         for (int i = 0; i < item.relationPointList.Count; i++)
            //         {
            //             if (dict.ContainsKey(item.relationPointList[i]))
            //             {
            //                 EditPathPoint relation = dict[item.relationPointList[i]];
            //                 Color color = relation.relationPointList.Contains(item.pointId) ? Color.blue : Color.yellow;
            //                 Debug.DrawLine(item.transform.position, relation.transform.position, color, 300);
            //             }
            //             else
            //             {
            //                 Debug.LogError(string.Format("找不到顶点={0}的关联点id={1}信息", item.pointId, item.relationPointList[i]));
            //             }
            //         }
            //     }
            // }
            // else
            // {
            //     Debug.LogError("找不到PathPoints,无法绘制寻路点!!");
            // }
        }

        private static byte[,] mapData = null;
        private static void LoadByPath(string filePath)
        {
            //LoadBytes(FileUtils.LoadByteFile(filePath));
        }

        private static void LoadBytes(byte[] data)
        {
            if (data != null && data.Length > 4)
            {
                int cursor = 0;
                int xl = data[cursor++];
                int xh = data[cursor++];
                int zl = data[cursor++];
                int zh = data[cursor++];
                int width = xl + (xh << 8);
                int height = zl + (zh << 8);
                mapData = new byte[width, height];
                for (int i = 0; i < width; ++i)
                {
                    for (int j = 0; j < height; ++j)
                    {
                        mapData[i, j] = data[cursor++];
                    }
                }
            }
            else
            {
                Debug.Log("map block is null!");
            }
        }

        public static void DrawBlockMap(string filePath, float h)
        {
            LoadByPath(filePath);
            Vector3 start = new Vector3(0, h, 0);
            Vector3 end = new Vector3(0, h, 0);
            for (int x = mapData.GetLowerBound(0); x <= mapData.GetUpperBound(0); x++)
            {
                for (int z = mapData.GetLowerBound(1); z <= mapData.GetUpperBound(1); z++)
                {
                    var subData = mapData[x, z];
                    if (subData > 0)
                    {
                        start.x = x - 0.5f;
                        start.z = z - 0.5f;
                        end.x = x + 0.5f;
                        end.z = z - 0.5f;
                        Debug.DrawLine(start, end, Color.red, 300);


                        start.x = x - 0.5f;
                        start.z = z + 0.5f;
                        end.x = x + 0.5f;
                        end.z = z + 0.5f;
                        Debug.DrawLine(start, end, Color.red, 300);


                        start.x = x - 0.5f;
                        start.z = z + 0.5f;
                        end.x = x - 0.5f;
                        end.z = z - 0.5f;
                        Debug.DrawLine(start, end, Color.red, 300);


                        start.x = x + 0.5f;
                        start.z = z + 0.5f;
                        end.x = x + 0.5f;
                        end.z = z - 0.5f;
                        Debug.DrawLine(start, end, Color.red, 300);
                    }
                }
            }
        }

    }
}
