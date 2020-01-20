using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Property {
	public string name = "";
	public string className = "";
	public float scaleX = 0f;
	public float scaleY = 0f;
	public int rotate = 0;
	public bool flipX = false;
	public bool flipY = false;
	public bool visible = false;
	public int zOrder = 0;

	public int opacity = 255;
	public int colorR = 255;
	public int colorG = 255;
	public int colorB = 255;

	public int anchorType = 0;
	// public float[] anchors = new float[2]{0.0f,1.0f};
	public Vector2 anchors = PSDConst.PIVOTPOINT1;

	public string p_path = "";
	//public bool input = false;
	public string selected = "";
	public bool commonImage = false;
	public int isOutpic = 0;
	public string imagePath = "";
	public int scale9_x = 0;
	public int scale9_y = 0;
	public int scale9_width = 0;
	public int scale9_height = 0;
	public bool isInput = false;
	public int width = 0;
	public int height = 0;
	public int direction = 0;
	public int percent = 0;
	public bool selectState = false;
	public bool isHidden = false;
	public bool isRichLabel = false;
	public bool isSavePrefab = false;//是否保存成预设形式

	// 解析Properys属性数组
	public void DeserializeProperty(string[] propertys) {
		for (int i=0;i<propertys.Length;i++) {
			var propItems = propertys [i].Split ('=');
			var propName = propItems [0];

			//Debug.Log ("prop:"+propName);

			if (propName.Equals("N")) {
				this.name = propItems[1];
			}else if(propName.Equals("Type"))
			{
				//T_type
				this.className = propItems[1];
			}
			else if(propName.Equals("Scale"))
			{
				//S_scale
				if(propItems.Length == 2)
				{
					this.scaleX = this.scaleY = Mathf.Round(float.Parse(propItems[1]));
				}
				else if(propItems.Length == 3)
				{
					this.scaleX = Mathf.Round(float.Parse(propItems[1]));
					this.scaleY = Mathf.Round(float.Parse(propItems[2]));
				}
			}
			else if(propName.Equals("SX")) {
				this.scaleX = Mathf.Round(this.width / float.Parse(propItems[1]) * 100) / 100f;
			}
			else if(propName.Equals("SY")) {
				this.scaleY = Mathf.Round(this.height / float.Parse(propItems[1]) * 100) / 100f;
			}
			else if(propName.Equals("Rotation") && !"-1".Equals(propItems[1]))
			{
//				this.rotate = Integer.parseInt(propItems[1]);
				this.rotate = int.Parse (propItems[1]);
			}
			else if(propName.Equals("M"))
			{
				//M_mirrorX_mirrorY
				this.flipX = propItems[1].Equals("1");
				this.flipY = propItems[2].Equals("1");
			}
			else if(propName.Equals("Visible") && !"-1".Equals(propItems[1]))
			{
				//V_visible
				this.visible = propItems[1].Equals("1");
			}
			else if(propName.Equals("ZOrder") && !"-1".Equals(propItems[1]))
			{
				//Z_zorder
//				this.zOrder = Integer.parseInt(propItems[1]);
				this.zOrder = int.Parse(propItems[1]);
			}
			else if(propName.Equals("Opacity") && !"-1".Equals(propItems[1]))
			{
				//O_opacity 
//				this.opacity = Integer.parseInt(propItems[1]); 
				this.opacity = int.Parse(propItems[1]);
			}
			else if(propName.Equals("Color"))
			{
				//C_r_g_b
				if(!propItems[1].Equals("-1")) {
//					this.colorR = Integer.parseInt(propItems[1]);
					this.colorR = int.Parse(propItems[1]);
				}
				if(!propItems[2].Equals("-1")) {
//					this.colorG = Integer.parseInt(propItems[2]);
					this.colorG = int.Parse(propItems[2]);
				}
				if(!propItems[3].Equals("-1")) {
//					this.colorB = Integer.parseInt(propItems[3]);
					this.colorB = int.Parse(propItems[3]);
				}	
			}
			else if(propName.Equals("Anchor"))
			{
				//An_anchor 
				this.anchorType = int.Parse(propItems[1]);

				if (anchorType == -1) 
				{
					//默认左上角
					this.anchorType = 1;
				}
				switch(this.anchorType) 
				{
					case 0:this.anchors = PSDConst.PIVOTPOINT0;break;
					case 1:this.anchors = PSDConst.PIVOTPOINT1;break;
					case 2:this.anchors = PSDConst.PIVOTPOINT2;break;
					case 3:this.anchors = PSDConst.PIVOTPOINT3;break;
					case 4:this.anchors = PSDConst.PIVOTPOINT4;break;
					case 5:this.anchors = PSDConst.PIVOTPOINT5;break;
					case 6:this.anchors = PSDConst.PIVOTPOINT6;break;
					case 7:this.anchors = PSDConst.PIVOTPOINT7;break;
					case 8:this.anchors = PSDConst.PIVOTPOINT8;break;
					default:this.anchors = PSDConst.PIVOTPOINT1;break;
				}
			}
			else if(propName.Equals("P"))
			{
				//P_path_iscommon
				if(propItems.Length == 3)
				{
					this.commonImage = propItems[2].Equals("1");
				}
				else
				{
					this.commonImage = true;
				}
				this.imagePath = propItems[1];
//				this.imagePath = addImage2Output(this.imagePath, imageList);
//				imageAdded = true;
			}
			else if(propName.Equals("S9"))
			{
				//S9_x_y_w_h
				this.scale9_x= int.Parse(propItems[4]);
				this.scale9_y= int.Parse(propItems[1]);
				//				if(propItems.length > 3)
				//				{
				//					this.scale9_width= Integer.parseInt(propItems[3]);
				//					this.scale9_height= Integer.parseInt(propItems[4]);
				//				}
				//				else
				//				{
				this.scale9_width = 1;
				this.scale9_height = 1;
				//				}
			}
			else if(propName.Equals("input"))
			{
				//this.isInput = true;
				this.isInput = (propItems[1].Equals("1")) ? true : false;
			}
			else if(propName.Equals("hidden"))
			{
				this.isHidden = (propItems[1].Equals("1")) ? true : false;
			}
			else if(propName.Equals("Selected"))
			{
				//select_state_unselectpath
				this.selectState = propItems[1].Equals("1");
				//				imageList.add(new ImagePathData(this.unselectPath, this.imagePath,true));
			}
			else if((propName.Equals("out") || propName.Equals("outpic") || propName.Equals("outPic")) && !"0".Equals(propItems[1])) {
				this.commonImage = false;
			}
			else if(propName.Equals("dir"))
			{
				//dir_dirType
				this.direction = int.Parse(propItems[1]);
			}
			else if(propName.Equals("Percent") && !"-1".Equals(propItems[1]))
			{
				//per_percent
				this.percent = int.Parse(propItems[1]);
			}

			if (propName.Equals ("outpic")) { // 外部图片
				this.isOutpic = int.Parse(propItems[1]);
				// this.isOutpic = (propItems[1].Equals("1")) ? true : false;
			}
			if (propName.Equals ("isRichLabel")) { // 是否富文本
				this.isRichLabel = (propItems[1].Equals("1")) ? true : false;
			}
			if (propName.Equals ("isSave")) { // 是否保存成预设
				this.isSavePrefab = (propItems[1].Equals("1")) ? true : false;
			}
		}
			
	}
}
