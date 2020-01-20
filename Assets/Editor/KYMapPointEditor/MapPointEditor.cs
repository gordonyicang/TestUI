using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System;
using System.IO;
using System.Linq;
using System.Net;
using UnityEditor.SceneManagement;

namespace KYSystem
{
    public class MapPointEditor : EditorWindow {

        public static GameObject terrain_obj = null;           //物体模块
        // public static GameObject terrain_collider = null;   //collider
        public static bool isShowLine = false;                  //显示射线-默认显示
        public static float showlineTime = 60;                 //秒
        public static float proportion = 1.0f;                 //检测间隔（默认1米）

        private static string screenN = "";
        public static float intervalX = 0.2f;                   //打高度点间隔X
        public static float intervalZ = 0.2f;                   //打高度点间隔Z
        public static float maxDistance = 500;                  //射线最大长度
        public static float floatPointHeight = 30;              //射线点高度
        public static string filePath = "";                     //导出导入文件名
        public static string[] layersArray = new string[] {"Terrain"}; //LayerMap层级 Wall Terrain

        private static int maplayerNumber;                      //层级数字
        private static Vector3 _watchPoint;                     //检测点

        private static float numberP;                           //总点数
        private static List<Transform> list;                    //terrain的子类transform

        private static float checkX_Min = 0f;                   //检测场景最小值x
        private static float checkZ_Min = 0f;                   //检测场景最小值z
        private static float checkX_Max = 500f;                //检测场景最大值x
        private static float checkZ_Max = 500f;                //检测场景最大值z

        private bool savePathPointData = true;     //是否生成寻路点数据
        private bool saveWalkData = true;    //是否生成可走点数据
        private bool saveHeightData = true;     //是否生成高度数据
        // private static MapInfo mapInfoRead = null;
        private static GameObject instance = null;

        [MenuItem("KYTool/Map/MapPointEditor")]
        public static void PointEditorBegin()
        {
            MapPointEditor.CreateInstance<MapPointEditor>().Show();
        }

        /// <summary>
        /// Editor GUI
        /// </summary>
        public void OnGUI()
        {
            GUILayout.Label("###导出导入地图数据工具###", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            isShowLine = EditorGUILayout.Toggle("显示射线", isShowLine);
            if (isShowLine)
            {
                EditorGUILayout.LabelField("射线显示时间(秒)");
                showlineTime = EditorGUILayout.FloatField(showlineTime);
            }
            EditorGUILayout.LabelField("检测点间隔（米）");
            proportion = EditorGUILayout.FloatField(proportion);
            EditorGUILayout.LabelField("当前场景:"+screenN);
            if (GUILayout.Button("选择场景文件", GUILayout.Height(20)))
            {
                OpenFilePanel();
            }

            EditorGUILayout.LabelField("MAP高度检测间隔X-float");
            intervalX = EditorGUILayout.FloatField(intervalX);
            
            EditorGUILayout.LabelField("MAP高度检测间隔Z-float");
            intervalZ = EditorGUILayout.FloatField(intervalZ);
            
            EditorGUILayout.LabelField("检测射线长度-float");
            maxDistance = EditorGUILayout.FloatField(maxDistance);
            
            EditorGUILayout.LabelField("检测射线高度-float");
            floatPointHeight = EditorGUILayout.FloatField(floatPointHeight);

            EditorGUILayout.LabelField("导出导入文件名-string");
            filePath = screenN;
            filePath = EditorGUILayout.TextField(filePath);

            //if (GUILayout.Button("生成地图高度"))
            //{
            //    Debug.Log("#开始生成高度map");
            //    if(screenN != "" && screenN != null)
            //    {
            //        getsceneSize();
            //        writeMapData();
            //    }
            //    else
            //    {
            //        Debug.Log("场景为空,重新选择场景文件.");
            //    }
            //}
            

            savePathPointData = EditorGUILayout.Toggle("生成寻路点数据", savePathPointData);
            saveWalkData = EditorGUILayout.Toggle("生成可走点数据", saveWalkData);
            saveHeightData = EditorGUILayout.Toggle("生成高度数据", saveHeightData);

            if (GUILayout.Button("一键生成", GUILayout.Height(20)))
            {
                Debug.Log("#开始生成数据");
                if (screenN != "" && screenN != null)
                {
                    getSceneSize();
                    if(saveHeightData) writeMapHeightData();
                    if (saveWalkData) writeWalkData();
                    if (savePathPointData) wirtePathPointData();
                }
                else
                {
                    Debug.Log("场景为空,重新选择场景文件.");
                }
            }

            if (GUILayout.Button("生成可走点Cube", GUILayout.Height(20)))
            {
                if (screenN != "" && screenN != null)
                {
                    CreateWalkableCube();
                }
                else
                {
                    Debug.Log("场景为空,重新选择场景文件.");
                }
            }

            if (GUILayout.Button("分割地图", GUILayout.Height(20)))
            {
                SplitMeshAndSave();
                // StartBake();
            }
        }

        //可走点数据
        private void writeWalkData()
        {
            MapInfoMgr.instance.OutputWayPointToFile();
        }

        //寻路点数据
        private void wirtePathPointData()
        {
            SpaceDataMgr.SaveToFile();
        }

        private void CreateWalkableCube()
        {
            GameObject goArea = new GameObject("walkableArea");
            int w = MapInfoMgr.hWidth;
            int h = MapInfoMgr.hHeight;
            for(int i = 0;i < w;i ++)
            {
                for(int j = 0;j < h;j ++)
                {
                    if(MapInfoMgr.instance.isCanWalk((int)(i),(int)(j)))
                    {
                        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        go.transform.localPosition = new Vector3(i+0.5f,50,j+0.5f);
                        go.transform.SetParent(goArea.transform);
                    }
                }
            }
        }

        private static void SplitMeshAndSave()
        {
            // MoveTerrainAndWarter();
            SplitMesh();
            // EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        public static void MoveTerrainAndWarter()
        {
            string name = "static";
            GameObject staticGo = GameObject.Find(name);
            GameObject root = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()[0];
            if (staticGo == null)
            {
                staticGo = new GameObject(name);
                staticGo.transform.parent = root.transform;
            }
            //地形
            GameObject terrain = GameObject.Find("Terrain");
            if (terrain)
            {
                terrain.transform.parent = staticGo.transform;
            }
            //水
            int transformCount = root.transform.childCount;
            GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];
            foreach (GameObject child in gos)
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer && renderer.sharedMaterial)
                {
                    Shader shader = renderer.sharedMaterial.shader;
                    if (shader.name.IndexOf("Water") > -1)
                    {
                        child.transform.parent = staticGo.transform;
                        Debug.Log(child.name + "  :" + shader.name);
                    }
                }
            }
        }

        public static void SplitMesh()
        {
            GameObject mesh = GameObject.Find("Mesh");
            int count = 0;
            if (mesh)
            {
                int transformCount = mesh.transform.childCount;
                for (int j = transformCount - 1; j >= 0; j--)
                {
                    Transform childTrans = mesh.transform.GetChild(j);
                    if (childTrans.gameObject.name == "Terrain") continue;
                    float x = childTrans.position.x;
                    float z = childTrans.position.z;
                    int XCount = Mathf.FloorToInt(x / 50);
                    int ZCount = Mathf.FloorToInt(z / 50);
                    if (XCount < 0) { XCount = 1000 - XCount; }
                    if (ZCount < 0) { ZCount = 1000 - ZCount; }

                    string name = "Mesh" + XCount.ToString() + "_" + ZCount.ToString();
                    GameObject gridGo = GameObject.Find(name);
                    if (gridGo == null)
                    {
                        gridGo = new GameObject(name);
                        gridGo.isStatic = true;
                        gridGo.transform.parent = childTrans.root;
                    }
                    childTrans.parent = gridGo.transform;
                    count++;
                }
            }
            if (count > 0)
            {
                Debug.Log("地图分割完成！");
            }
            else
            {
                Debug.Log("没有可分割对象！");
            }
        }

        public static void StartBake()
        {
            Lightmapping.Clear();
            Lightmapping.Bake();
        }

        private void OpenFilePanel()
        {
            string root = Path.Combine(Application.dataPath, "Resources/Maps/");
            string filePath = EditorUtility.OpenFilePanel("指定文件", root, "unity");  //unity
            //EditorApplication.OpenScene(filePath);
            EditorSceneManager.OpenScene(filePath);
            string prefabName = Path.GetFileNameWithoutExtension(filePath);
            screenN = prefabName;
            Debug.Log(string.Format("地图 prefabName:{0}", prefabName));
            MapInfoMgr.instance.CreateWayPointInfo(prefabName,proportion);


            SpaceDataMgr.spaceData.spaceName = screenN;
            SpaceDataMgr.LoadPathPoints(screenN);
        }

        /// <summary>  
        /// 获取子对象变换集合  
        /// </summary>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public static List<Transform> GetChildCollection(Transform obj)  
        {  
            List<Transform> list = new List<Transform>();  
            for (int i = 0; i < obj.childCount; i++)  
            {  
                if(obj.GetChild(i).childCount <= 0)
                {
                    // Debug.Log("没有子类");
                    list.Add(obj.GetChild(i)); 
                }
                else
                {
                    Transform childobj = obj.GetChild(i);
                    for (int j = 0; j < childobj.childCount; j++)
                    {
                        // Debug.Log("有子类"+childobj.gameObject.name);
                        list.Add(childobj.GetChild(j)); 
                    }  
                }
            }  
            return list;  
        }

        /// <summary>
        /// 计算场景长宽
        /// </summary>
        private void getSceneSize()
        {
            //GameObject obj = GameObject.Find("NotExport");//可走路径算的是地板,这里为了统一也算地板
            //List<Transform> objList = GetChildCollection(obj.transform);
            //// Debug.Log("计算检测场景区间");
            //List<float> objx = new List<float>();
            //List<float> objz = new List<float>();

            //for(int i = 0;i < objList.Count;i++)
            //{
            //    objx.Add(objList[i].position.x + objList[i].GetComponent<Collider>().bounds.size.x);
            //    objz.Add(objList[i].position.z + objList[i].GetComponent<Collider>().bounds.size.z);
            //}
            //// checkX_Min = objx.Min();
            //checkX_Max = objx.Max();
            //checkX_Min = 0;
            //checkZ_Max = objz.Max();
            //checkZ_Min = 0;

            //或者直接用可走区域计算的xz(因为和开谦的可走区域xz计算的不一致,为了统一)
            checkX_Max = MapInfoMgr.hWidth;
            checkZ_Max = MapInfoMgr.hHeight;

        }

        //地图高度数据
        private void writeMapHeightData() 
        {
            _watchPoint = new Vector3();
            _watchPoint.y = floatPointHeight;

            string sceneId = screenN; //prefabName.Split('_')[0];
            string dirPath = string.Concat(Application.dataPath, "/MapTestInfo/");
            string txtFilePath = string.Concat(dirPath, sceneId, "h", ".txt");
            string bmFilePath = Path.Combine(Application.streamingAssetsPath, string.Format("{0}h.txt", sceneId));
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            if (File.Exists(txtFilePath)) File.Delete(txtFilePath);
            if (File.Exists(bmFilePath)) File.Delete(bmFilePath);

            StreamWriter txtWriter = File.CreateText(txtFilePath);

            //txt先写入sceneId
            string txtSceneId = "sceneId:"+sceneId;
            txtWriter.WriteLine(txtSceneId);
            //生成二进制文件
            FileStream fs = new FileStream(bmFilePath, FileMode.Create);
            MapInfoWriter bmWriter = new MapInfoWriter(fs);
            bmWriter.WriteString16(sceneId);
            float gridX = Convert.ToSingle(Math.Ceiling(checkX_Max / intervalX));
            float gridZ = Convert.ToSingle(Math.Ceiling(checkZ_Max / intervalZ));
            bmWriter.WriteInt16((short)gridX);
            bmWriter.WriteInt16((short)gridZ);
            Debug.Log("gridX:" + gridX + "  gridZ:" + gridZ);
            
            int writeCount = 0;
            List<List<float>> writeList = new List<List<float>>();
            for(int i = 0;i < gridX;i ++)
            {
                for(int j = 0;j < gridZ;j ++)
                {
                    float x = checkX_Min + intervalX/2 + i * intervalX;
                    float y = checkZ_Min + intervalZ/2 + j * intervalZ;
                    
                    if(!MapInfoMgr.instance.isCanWalk((int)(x),(int)(y)))
                    {
                        continue;
                    }
                    //else
                    //{
                    //}

                    writeCount ++;
                    float h = Convert.ToSingle(Math.Round(getMapItemPoint(x,y),2));//高度保留2位小数

                    int height32 = Convert.ToInt32(h * 100);
                    string txtString = "i:" + i + "  j:" + j + "  x:" + x +"  y:" + y+"  h:" + height32;
                    txtWriter.WriteLine(txtString);

                    List<float> list = new List<float>();
                    list.Add(Convert.ToSingle(i));
                    list.Add(Convert.ToSingle(j));
                    list.Add(height32);
                    writeList.Add(list);

                    // bmWriter.WriteInt16((short)(h * 100));
                }
                //进度条
                var valF = i / gridX + 0.1f;
                if (valF < 1)
                {
                    EditorUtility.DisplayProgressBar("场景碰撞检测中", "正在拼命发射线...", valF);
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                }
            }

            bmWriter.WriteInt32(writeList.Count);
            for(int i = 0;i < writeList.Count;i ++)
            {
                List<float> l = writeList[i];
                bmWriter.WriteInt16((short)(l[0]));
                bmWriter.WriteInt16((short)(l[1]));
                bmWriter.WriteInt16((short)(l[2]));
            }

            txtWriter.WriteLine("totalGrid:" + gridX * gridZ + "    writeGrid:" + writeCount);
            txtWriter.Close();
            bmWriter.Close();
            fs.Close();
            Debug.Log("地图高度信息已保存：" + bmFilePath);

        }

        /// <summary>
        /// 射线检测,传入检测坐标点,忽略y高度值
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private float getMapItemPoint(float i, float j) 
        {
            // Debug.Log(i + "----" + j);
            _watchPoint.x = i;
            _watchPoint.z = j;
            var ray = new Ray(_watchPoint, Vector3.down);
            //传入碰撞层级,数组
            maplayerNumber = LayerMask.GetMask(layersArray);
            RaycastHit rayCast;//碰撞点
            bool hitTerrain = false;
            if(Physics.Raycast(ray, out rayCast, maxDistance, maplayerNumber)){

                hitTerrain = LayerMask.LayerToName(rayCast.transform.gameObject.layer) == "Terrain";
                if(hitTerrain)return rayCast.point.y;
            }else{
                return 100;//没有碰撞返回100
            }
            return 100;
        }

        /// <summary>
        /// 获取碰撞体
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        Collider getCollider (GameObject go) 
        {
            var collider = go.GetComponent<Collider>();
            var colliders = go.GetComponentsInChildren<Collider>();
            var middle = new Vector3();
            foreach(Collider c in colliders) {
                Debug.Log(c.gameObject.name + ":" + c.bounds.size.ToString());
                middle += c.bounds.center;
            }
            middle = middle / colliders.Length;
            return collider;
        }
        
        void OnInspectorUpdate()
        {
            Repaint();
        }

        void OnDestroy()
        {
            EditorUtility.ClearProgressBar();
            Debug.Log("工具窗口关闭");
            MapInfoMgr.instance.UnActiveTerrain();
            DestroyImmediate(instance);
        }

    }
}
