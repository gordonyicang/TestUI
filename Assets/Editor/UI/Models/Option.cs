using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Option {
	public int AntiAlias;
	public int Justification;
	public float Leading;

	public string font;
	public string text;
	public string property;
	public string link;
	public Model u3dLink;

	public bool underLine;
	public bool singleLine;
	public bool isScaleImage;

	public int fontSize;
	public float opacity;
	public float colorB;
	public float colorG;
	public float colorR;

	public float width;
	public float height;
	public float x;
	public float y;

	public float bottom;
	public float left;
	public float top;
	public float right;

	public Model bar;
}
