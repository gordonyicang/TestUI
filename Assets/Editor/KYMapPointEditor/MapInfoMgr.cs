using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

/*
    计算生成可走区域地图
*/
namespace KYSystem
{
    /// <summary>
    /// 生成地图信息工具类
    /// </summary>
    class MapInfoMgr
    {
        public static int mapX = 0;                                      //地图宽度值(对应mapX)
        public static int mapZ = 0;                                      //地图高度值(对应mapZ)
        public static int hWidth = 0;                                    //取地图高度的区域 宽
        public static int hHeight = 0;                                   //取地图高度的区域 宽
        private byte[,] nodeList;                                        //存储路点信息[0:通路,1:障碍(非0都当成是障碍)]
        private string prefabName;                                       //场景预设名称
        private List<Transform> terrCubeList = new List<Transform>();    //地表列表
        private float proportion = 1.0f;
        public int findCount = 0;

        private byte[,] findList;//找过的列表，初始为-1，可走标记为0，不可走1

        //=================  路点标记常量  ==============//
        public const byte CAN_MOVE = 0;            //通路点
        public const byte CAN_NOT_MOVE = 1;        //障碍点
        // public const byte WALL = 2;                //墙 
        // public const byte SIDE_AREA = 3;           //补边区域
        // public const byte FLY_AREA = 4;            //飞行区域
        // public const byte FLY_WALL = 5;            //飞行墙
        //==============================================//

        //=================  场景用到的Layer名称 =======//
        public const string Terr = "Terr";
        public const string Wall = "Wall";
        // public const string WallFly = "WallFly";
        //==============================================//

        //=================  GameObject关键字   ========//
        public const string MapTerr = "MapTerr";
        // public const string FlyTerr = "FlyTerr";
        // public const string wall_Cube = "wall_cube";
        // public const string terrain_Cube = "terrain_cube";
        //==============================================//

        public static MapInfoMgr instance = new MapInfoMgr();

        private GameObject mapTerrainObj;
        // private GameObject floorObj;

        public MapInfoMgr() { }

        /// <summary>
        /// 生成地图路点信息
        /// </summary>
        /// <param name="prefabName">地图名称</param>
        public void CreateWayPointInfo(string prefabName, float proportion)
        {
            this.prefabName = prefabName;
            this.proportion = proportion;
            GameObject map = GameObject.Find(this.prefabName);
            mapTerrainObj = GetGoByName(map,MapTerr);
            if(mapTerrainObj != null)mapTerrainObj.SetActive(true);

            // GameObject collider = GetGoByName(map,"Collider");
            // floorObj = GetGoByName(collider,"NotExport");
            // if(floorObj != null)floorObj.SetActive(true);

            //A-1、计算地图大小(宽*高),地图起始坐标(x=0,z=0,这个是与美术制作地图时的约定，一定要记住)
            CaluMapWH();
            //A-2、初始化地图路点[0:可行走区域,1:障碍点区域]
            InitNodeList();
            //A-4、设置内墙Wall和外墙WallFly标识为：2
            // SetWallFlag();
            //A-5、设置内墙行走区域标识为：0
            SetMapTerr();
            //A-6、设置内墙Wall和外墙WallFly标识为：4
            // SetFlyTerr();
            //A-7、设置外墙WallFly标识为：4
            // SetWallFlyFlag();
            //A-8、导出路点信息到文本
            //OutputWayPointToFile();
            AssetDatabase.Refresh();
        }

        public void UnActiveTerrain()
        {
            if(mapTerrainObj != null)mapTerrainObj.SetActive(false);
            // if(floorObj != null)floorObj.SetActive(false);
        }

        private GameObject GetGoByName(GameObject go,string name)
        {
            foreach (Transform child in go.transform)
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }
            }
            return null;
        }

        /// <summary>
        /// A-1、计算地图宽和高
        /// </summary>
        private void CaluMapWH()
        {
            float terrainX = 300; 
            float terrainZ = 300;
            //获取地形 对象
            GameObject terrain =  GameObject.Find("Terrain");
            if (terrain != null)
            {
                //获得宽度
                float size_x = terrain.GetComponent<Collider>().bounds.size.x;
                //获得缩放比例
                float scal_x = terrain.transform.localScale.x;
                //同上
                float size_z = terrain.GetComponent<Collider>().bounds.size.z;
                float scal_z = terrain.transform.localScale.z;
                terrainX = size_x * scal_x;
                terrainZ = size_z * scal_z;
            }

            //悬空地形
            GameObject obj = GameObject.Find(Terr);
            if (obj == null && terrain == null)
            {
                Debug.LogError("场景编辑不对，请先编辑场景的Terr cube信息!");
                return;
            }
            List<float> xList = new List<float>();
            List<float> zList = new List<float>();
            AddTerrainCube(obj.transform, xList, zList);

            // obj = GameObject.Find("NotExport");
            //if (floorObj != null) AddTerrainCube(floorObj.transform, xList, zList);
            xList.Sort();
            zList.Sort();

            float colliderX = 0;
            if(xList.Count > 0)
            {
                colliderX = xList[xList.Count - 1];
            }
            float colliderZ = 0;
            if(zList.Count > 0)
            {
                colliderZ = zList[zList.Count - 1];
            }

            float maxX = Math.Max(colliderX, terrainX);
            float maxZ = Math.Max(colliderZ, terrainZ);

            mapX = Convert.ToInt32(Math.Ceiling(maxX/proportion));
            mapZ = Convert.ToInt32(Math.Ceiling(maxZ / proportion));

            //高度图，不除以proportion
            hWidth = Convert.ToInt32(Math.Ceiling(maxX));
            hHeight = Convert.ToInt32(Math.Ceiling(maxZ));

            Debug.Log(string.Format("A-1、获得地图尺寸 宽mapX:{0},高mapZ:{1}", mapX, mapZ));
        }

        private void AddTerrainCube(Transform tran, List<float> xList, List<float> zList)
        {
            foreach (Transform go in tran)
            {
                // if (go.name == terrain_Cube)
                // {
                    xList.Add(go.position.x - go.lossyScale.x / 2);
                    xList.Add(go.position.x + go.lossyScale.x / 2);
                    zList.Add(go.position.z - go.lossyScale.z / 2);
                    zList.Add(go.position.z + go.lossyScale.z / 2);
                    terrCubeList.Add(go);
                // }
            }
        }


        /// <summary>
        /// A-2、初始化存储路点信息,默认全部为障碍点:1
        /// </summary>
        private void InitNodeList()
        {
            nodeList = new byte[mapX, mapZ];   
            for (int row = 0; row < mapX; ++row)
            {
                for (int col = 0; col < mapZ; ++col)
                {
                    nodeList[row, col] = 1;
                }
            }

            findList = new byte[mapX, mapZ];   
            for (int row = 0; row < mapX; ++row)
            {
                for (int col = 0; col < mapZ; ++col)
                {
                    findList[row, col] = 10;
                }
            }
        }

        // /// <summary>
        // /// A-3、设置墙Wall和飞行墙WallFly标记为：障碍点:2
        // /// </summary>
        // private void SetWallFlag()
        // {
        //     GameObject obj = GameObject.Find(Wall);
        //     if (obj == null)
        //     {
        //         Debug.LogError("场景设置不对，请编辑场景Wall");
        //         return;
        //     }

        //     Transform tran = obj.transform;
        //     foreach (Transform tr in tran)
        //     {
        //         if (tr.gameObject.layer == UnityEngine.LayerMask.NameToLayer(WallFly))
        //         {//外墙
        //             foreach (Transform go in tr)
        //             {
        //                 if (go.name == wall_Cube)
        //                 {
        //                     get_rectpoint(go, WALL);
        //                 }
        //             }
        //         }
        //         else 
        //         {//内墙
        //             if (tr.localScale.x <= 0 || tr.localScale.y <= 0 || tr.localScale.z <= 0)
        //                 Debug.LogError("[SetWallFlag] scale小于0 tr.name=" + tr.name + ",pos=" + tr.localPosition.ToString() + ",scale=" + tr.localScale.ToString());
        //             get_rectpoint(tr, WALL);
        //         }
        //     }
        //     Debug.Log("A-3、设置墙Wall和飞行墙WallFly标记为：障碍点:2");
        // }

        // /// <summary>
        // /// A-4、设置外墙WallFly标识为：障碍点4
        // /// </summary>
        // private void SetWallFlyFlag()
        // {
        //     GameObject obj = GameObject.Find(Wall);
        //     if (obj == null)
        //     {
        //         Debug.LogError("场景设置不对，请编辑场景Wall");
        //         return;
        //     }

        //     Transform tran = obj.transform;
        //     foreach (Transform tr in tran)
        //     {
        //         if (tr.gameObject.layer == UnityEngine.LayerMask.NameToLayer(WallFly))
        //         {//飞行墙
        //             foreach (Transform go in tr)
        //             {
        //                 if (go.name == wall_Cube)
        //                 {
        //                     get_rectpoint(go, FLY_WALL);
        //                 }
        //             }
        //         }
        //     }
        //     Debug.Log("A-4、设置外墙WallFly和内墙Wall之间标记完成");
        // }

        /// <summary>
        /// A-5、根据MapTerr设置为:可走点0
        /// </summary>
        public void SetMapTerr()
        {
            // GameObject obj = GameObject.Find(MapTerr);

            if (mapTerrainObj == null)
            {
                Debug.LogError("该场景未设置MapTerr.");
                return;
            }

            int x = 0;
            int z = 0;
            Transform gos = mapTerrainObj.transform;
            foreach (Transform go in gos)
            {
                // if (go.name == terrain_Cube)
                // {
                    x = (int)(go.transform.position.x / proportion);
                    z = (int)(go.transform.position.z / proportion);
                    CheckPointValue(x,z);
                    Debug.Log("[MapTerr] position=" + go.transform.position.ToString());
                    // try
                    // {
                    //     // SetAroundInfo(x, z, 0);
                    //     Debug.Log("CheckPointValue x:" + x + "  y:" + z);
                    //     CheckPointValue(x,z);
                    // }
                    // catch (Exception ex)
                    // {
                    //     Debug.LogError("[MapTerr] 越界 position=" + go.transform.position.ToString() + ",local=" + go.transform.localPosition + ",error=" + ex.Message);
                    // }
                // }
            }
            Debug.Log("检测点个数：" + findCount);
            Debug.Log("A-5、根据MapTerr设置为:可走点0");
        }

        private int CheckPointValue(int x,int y)
        {
            // if(x >= 50 || x < 0 || y >= 150 || y < 0)return -1;
            if(IsArrayOutOfIndex(x,y))return 1;
            findCount ++;
            if(findList[x,y] != 10)return 2;

            int result = GetPointResult(x,y);
            findList[x,y] = (byte)result;
            if(result == 1)return 3;

            bool dolog = false;
            // if(x == 44 && y == 145)
            // {
            //     dolog = true;
            // }
            // int t1 = CheckPointValue(x - 1,y - 1);
            int t2 = CheckPointValue(x - 1,y);
            // int t3 = CheckPointValue(x - 1,y + 1);
            int t4 = CheckPointValue(x,y - 1);
            int t5 = CheckPointValue(x,y + 1);
            // int t6 = CheckPointValue(x + 1,y - 1);
            int t7 = CheckPointValue(x + 1,y);
            // int t8 = CheckPointValue(x + 1,y + 1);
            // if(dolog)
            // {
            //     Debug.Log("t1:" + t1);
            //     Debug.Log("t2:" + t2);
            //     Debug.Log("t3:" + t3);
            //     Debug.Log("t4:" + t4);
            //     Debug.Log("t5:" + t5);
            //     Debug.Log("t6:" + t6);
            //     Debug.Log("t7:" + t7);
            //     Debug.Log("t8:" + t8);
            // }
            return 0;
        }


        private int GetPointResult(int x,int y)
        {
            // int result = -1;
            // if(nodeList[x,y] == 2)
            // {
            //     result = 1;
            // }
            // else
            // {
            //     result = 0;
            //     nodeList[x,y] = 0;
            // }
            // return result;

            int result = -1;
            int hitCount = 0;
            List<Vector2> checkList = new List<Vector2>();
            checkList.Add(new Vector2(x,y));
            checkList.Add(new Vector2(x+1,y));
            checkList.Add(new Vector2(x,y+1));
            checkList.Add(new Vector2(x+1,y+1));
            checkList.Add(new Vector2(x+0.5f,y+0.5f));
            for(int i = 0;i < checkList.Count;i ++)
            {
                if(hitCount >= 3)break;
                if(rayHitWall(checkList[i].x,checkList[i].y))
                {
                    hitCount ++;
                }
            }
            if(hitCount >= 3)
            {
                result = 1;
            }
            else
            {
                result = 0;
                nodeList[x,y] = 0;
            }

            return result;

            //hit2
            // int result = -1;
            // Vector3 watchPoint = new Vector3();
            // watchPoint.x = x;
            // watchPoint.z = y;
            // watchPoint.y = 50;
            // var ray = new Ray(watchPoint, Vector3.down);
            // //传入碰撞层级,数组
            // string[] layersArray = new string[] {"Wall"};
            // var maplayerNumber = LayerMask.GetMask(layersArray);
            // RaycastHit rayCast;//碰撞点
            // if(Physics.Raycast(ray, out rayCast, 500, maplayerNumber))
            // {
            //     result = 1;
            //     // Debug.Log("bingo---x:" + x + "  y:" + y + "layer:" + LayerMask.LayerToName(rayCast.transform.gameObject.layer));
            // }
            // else
            // {
            //     result = 0;
            //     nodeList[x,y] = 0;
            // }
            // return result;
        }

        private bool rayHitWall(float x,float y)
        {
            Vector3 watchPoint = new Vector3();
            watchPoint.x = x;
            watchPoint.z = y;
            watchPoint.y = 50;
            var ray = new Ray(watchPoint, Vector3.down);
            //传入碰撞层级,数组
            string[] layersArray = new string[] {"Wall"};
            var maplayerNumber = LayerMask.GetMask(layersArray);
            RaycastHit rayCast;//碰撞点
            if(Physics.Raycast(ray, out rayCast, 500, maplayerNumber))
            {
                return true;
            }
            return false;
        }

        // /// <summary>
        // /// A-6、设置外墙WallFly和内墙Wall之间空间为:障碍点4
        // /// </summary>
        // private void SetFlyTerr()
        // {
        //     GameObject obj = GameObject.Find(FlyTerr);
        //     if (obj == null)
        //     {
        //         Debug.Log("该场景未设置飞行区域，如需要请增加，不需要请无视该log");
        //         return;
        //     }
        //     int x = 0;
        //     int z = 0;
        //     Transform gos = obj.transform;
        //     foreach (Transform go in gos)
        //     {
        //         if (go.name == terrain_Cube)
        //         {
        //             x = (int)(go.transform.position.x / proportion);
        //             z = (int)(go.transform.position.z / proportion);
        //             // SetAroundInfo(x, z, FLY_AREA);       //FLY_AREA=4
        //         }
        //     }
        //     Debug.Log("A-6、设置外墙WallFly和内墙Wall之间空间为:障碍点4");
        // }

        /// <summary>
        /// 保存地图路点信息到文件
        /// </summary>
        public void OutputWayPointToFile()
        {
            string sceneId = prefabName; //prefabName.Split('_')[0];
            string dirPath = string.Concat(Application.dataPath, "/MapTestInfo/");
            string srcPath = dirPath+"source/";
            string txtFilePath = string.Concat(srcPath, sceneId, "w", ".txt");
            string bmFilePath = Path.Combine(dirPath, string.Format("{0}w.txt", sceneId));
            // string bmFilePath = Path.Combine(Application.streamingAssetsPath, string.Format("{0}w.txt", sceneId));
            if (!Directory.Exists(srcPath)) Directory.CreateDirectory(srcPath);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            if (File.Exists(txtFilePath)) File.Delete(txtFilePath);
            if (File.Exists(bmFilePath)) File.Delete(bmFilePath);

            //生成txt文本文件
            StringBuilder str = new StringBuilder("");
            StreamWriter txtWriter = File.CreateText(txtFilePath);
            string textStr = "";
     
            //生成二进制文件
            FileStream fs = new FileStream(bmFilePath, FileMode.Create);
            MapInfoWriter bmWriter = new MapInfoWriter(fs);
            bmWriter.WriteString16(sceneId);
            bmWriter.WriteInt16((short)hWidth);//地图实际宽高
            bmWriter.WriteInt16((short)hHeight);
            bmWriter.WriteInt16((short)mapX); //存入地图宽和高值 （实际宽高除以检测间隔）
            bmWriter.WriteInt16((short)mapZ);

            //先按Z轴存，与 A* 对应
            for (int i = 0; i < mapZ; i++)
            {
                textStr = "";
                for (int j = 0; j < mapX; j++)
                {
                    textStr += "z:" + i + "  x:" + j + "  value:" + nodeList[j, i] + "\n";
                    bmWriter.WriteByte((byte)nodeList[j, i]);
                }
                txtWriter.WriteLine(textStr);
            }

            txtWriter.Close();
            bmWriter.Close();
            fs.Close();
            Debug.Log("地图路点信息已保存：" + bmFilePath);

            string assetPath = Path.Combine(Application.streamingAssetsPath, string.Format("{0}w.txt", sceneId));
            File.Copy(bmFilePath,assetPath,true);
        }

        /// <summary>
        /// 设置obj在地图中的标记
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="flag">标记值</param>
        private void get_rectpoint(Transform obj, byte flag)
        {
            //确定地板大小
            float angle = (obj.eulerAngles.y / 180 * (float)Math.PI);
            float xscale = (obj.lossyScale.x) / 2 / proportion;//世界坐标
            float zscale = (obj.lossyScale.z) / 2 / proportion;

            float begin_x = -xscale;
            float begin_z = -zscale;
            float end_x = xscale;
            float end_z = zscale;

            //初始点
            float mapX = begin_x;
            float mapZ = begin_z;

            //设置地板区可行
            for (float _i = begin_x; _i < end_x; _i += 0.1f)
            {
                for (float _j = begin_z; _j < end_z; _j += 0.1f)
                {
                    mapX = _i;
                    mapZ = _j;
                    float x1 = mapZ * (float)Math.Sin(angle) + mapX * (float)Math.Cos(angle);
                    float z1 = mapZ * (float)Math.Cos(angle) - mapX * (float)Math.Sin(angle);
                    x1 += obj.position.x / proportion;
                    z1 += obj.position.z / proportion;
                    //找到障碍采样点对应的整数坐标设置flag
                    int xpos = (int)x1 + ((x1 - (int)x1) > 0.5f ? 1 : 0);
                    int zpos = (int)z1 + ((z1 - (int)z1) > 0.5f ? 1 : 0);

                    if (IsArrayOutOfIndex(xpos, zpos) == false)
                    {
                        nodeList[xpos, zpos] = flag;  //1
                    }
                }
            }
        }

        /// <summary>
        /// 检查类成员nodeList二维数组是否越界
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private bool IsArrayOutOfIndex(int i, int j)
        {
            if (i >= 0 && i < mapX && j >= 0 && j < mapZ)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //是否可走
        public bool isCanWalk(int i,int j)
        {
            // Debug.Log("i:" + i + "  j:" + j + "  v:" + nodeList[i,j]);
            if(IsArrayOutOfIndex(i,j))return false;
            // Debug.Log("i:" + i + "  j:" + j + "  v:" + nodeList[i,j]);
            return nodeList[i,j] == 0;
        }

        //外面传浮点坐标
        public bool PointIsCanWalk(float x, float y)
        {
            int i = Convert.ToInt32(x / proportion);
            int j = Convert.ToInt32(y / proportion);
            return isCanWalk(i,j);
        }
    }
}
