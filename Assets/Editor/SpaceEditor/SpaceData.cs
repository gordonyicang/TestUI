
using System.Collections.Generic;

namespace KYSystem
{
    class SpaceData
    {
        public string mapName = string.Empty;
        public string spaceName = string.Empty;
        public string trapName = string.Empty;

        //场景全局设置
        public string ambientLight;
        public string objDiffuse;
        public string objSpecular;
        public string charDiffuse;
        public string charSpecular;
        public string fogColor1;
        public string fogColor2;
        public string fogStart;
        public string fogEnd;
        public string fogIntensity;

        public string cameraNearFar;
        public string lightmaps;
        public string lightProbes;

        public string spacePath { get { return SpaceDataMgr.spaceFolder + spaceName + ".xml"; } }
        public string mapPath { get { return SpaceDataMgr.mapFolder + mapName + "/" +  mapName + ".prefab"; } }
    }
}
