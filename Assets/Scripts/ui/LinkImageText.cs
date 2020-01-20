using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Globalization;

namespace GameLib.ui
{
    [Serializable]
    public class HrefClickEvent : UnityEvent<int> { }
/// <summary>
/// 文本控件，支持超链接、图片
/// </summary>
    [AddComponentMenu("UI/LinkImageText", 10)]
    public class LinkImageText : Text, IPointerClickHandler
    {
        /// <summary>
        /// 解析完最终的文本
        /// </summary>
        private string m_OutputText;

        /// <summary>
        /// 图片池
        /// </summary>
        protected readonly List<GameObject> m_ImagesPool = new List<GameObject>();
        public List<Vector2> m_positionList = new List<Vector2>();

        /// <summary>
        /// 图片的最后一个顶点的索引
        /// </summary>
        private readonly List<int> m_ImagesVertexIndex = new List<int>();


        /// <summary>
        /// 超链接信息列表
        /// </summary>
        private readonly List<HrefInfo> m_HrefInfos = new List<HrefInfo>();

        /// <summary>
        /// 文本构造器
        /// </summary>
        protected static readonly StringBuilder s_TextBuilder = new StringBuilder();

        

        [SerializeField]
        private HrefClickEvent m_OnHrefClick = new HrefClickEvent();

        /// <summary>
        /// 超链接点击事件
        /// </summary>
        public HrefClickEvent onLinkClick
        {
            get { return m_OnHrefClick; }
            set { m_OnHrefClick = value; }
        }

        /// <summary>
        /// 正则取出所需要的属性  （size占位高度），（size * width 占位宽度）
        /// </summary>
        private static readonly Regex s_ImageRegex =
            new Regex(@"<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?)/>", RegexOptions.Singleline);//width=(\d*\.?\d+%?)

        /// <summary>
        /// 超链接正则
        /// </summary>
        private static readonly Regex s_HrefRegex =
            new Regex(@"<a href=([^>\n\s]+) u=([01])>(.*?)(</a>)", RegexOptions.Singleline);

        /// <summary>
        /// 加载精灵图片方法
        /// </summary>
        public Func<string, GameObject> funLoadSprite;

        public int textHeight = 22;

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            UpdateQuadImage();
        }

        protected void UpdateQuadImage()
        {
            m_OutputText = GetOutputText(text);
            m_ImagesVertexIndex.Clear();
            foreach (Match match in s_ImageRegex.Matches(m_OutputText))
            {
                var picIndex = match.Index;
                var endIndex = picIndex * 4 + 3;
                m_ImagesVertexIndex.Add(endIndex);
            }
        }

        /// <summary>
        /// 获取超链接解析后的最后输出文本  @"<a href=([^>\n\s]+)>(.*?)(</a>)"
        /// </summary>
        /// <returns></returns>
        protected virtual string GetOutputText(string outputText)
        {
            s_TextBuilder.Length = 0;
            m_HrefInfos.Clear();
            var indexText = 0;
            foreach (Match match in s_HrefRegex.Matches(outputText))
            {
                s_TextBuilder.Append(outputText.Substring(indexText, match.Index - indexText));
                //s_TextBuilder.Append("<color=blue>");  // 超链接颜色
                var tmp = color;
                var group = match.Groups[1];
                var hrefInfo = new HrefInfo
                {
                    startIndex = s_TextBuilder.Length * 4, // 超链接里的文本起始顶点索引
                    endIndex = (s_TextBuilder.Length + match.Groups[3].Length - 1) * 4 + 3,
                    name = group.Value,
                    hasUnderline = match.Groups[2].Value == "1",
                    //color = HexToColor("#00ffff")
                    color = tmp
                };
                m_HrefInfos.Add(hrefInfo);

                s_TextBuilder.Append(match.Groups[3].Value);
                //s_TextBuilder.Append("</color>");
                indexText = match.Index + match.Length;
            }
            s_TextBuilder.Append(outputText.Substring(indexText, outputText.Length - indexText));
            return s_TextBuilder.ToString();
        }

        readonly UIVertex[] m_TempVerts = new UIVertex[4];
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            var orignText = m_Text;
            m_Text = m_OutputText;
            base.OnPopulateMesh(toFill);
            m_Text = orignText;

            m_positionList.Clear();
            UIVertex vert = new UIVertex();
            for (var i = 0; i < m_ImagesVertexIndex.Count; i++)
            {
                var endIndex = m_ImagesVertexIndex[i];
                //var rt = m_ImagesPool[i].transform as RectTransform;
                //var size = rt.sizeDelta;
                if (endIndex < toFill.currentVertCount)
                {
                    toFill.PopulateUIVertex(ref vert, endIndex);
                    //rt.anchoredPosition = new Vector2(vert.position.x + 2, vert.position.y + size.y);
                    m_positionList.Add(new Vector2(vert.position.x, vert.position.y));

                    // 抹掉左下角的小黑点
                    toFill.PopulateUIVertex(ref vert, endIndex - 3);
                    var pos = vert.position;
                    for (int j = endIndex, m = endIndex - 3; j > m; j--)
                    {
                        toFill.PopulateUIVertex(ref vert, endIndex);
                        vert.position = pos;
                        toFill.SetUIVertex(vert, j);
                    }
                }
            }

            if (m_ImagesVertexIndex.Count != 0)
            {
                m_ImagesVertexIndex.Clear();
            }

            // 处理超链接包围框
            foreach (var hrefInfo in m_HrefInfos)
            {
                hrefInfo.boxes.Clear();
                if (hrefInfo.startIndex >= toFill.currentVertCount)
                {
                    continue;
                }

                // 将超链接里面的文本顶点索引坐标加入到包围框
                toFill.PopulateUIVertex(ref vert, hrefInfo.startIndex);
                var pos = vert.position;
                var bounds = new Bounds(pos, Vector3.zero);
                for (int i = hrefInfo.startIndex, m = hrefInfo.endIndex; i <= m; i++)
                {
                    if (i >= toFill.currentVertCount)
                    {
                        //Debug.Log("breakbreak");
                        break;
                    }

                    toFill.PopulateUIVertex(ref vert, i);
                    pos = vert.position;
                    //Debug.Log("pos:" + pos);
                    if (pos.x < bounds.min.x && Math.Abs(pos.x - bounds.min.x) > 0.1) // 换行重新添加包围框
                    {
                        hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                        bounds = new Bounds(pos, Vector3.zero);
                    }
                    else
                    {
                        bounds.Encapsulate(pos); // 扩展包围框
                    }
                }
                hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
            }

            //添加下划线
            Vector2 extents = rectTransform.rect.size;
            var settings = GetGenerationSettings(extents);
            TextGenerator _UnderlineText = new TextGenerator();
            _UnderlineText.Populate("—", settings);
            IList<UIVertex> _TUT = _UnderlineText.verts;
            foreach (var item in m_HrefInfos)
            {
                if(item.hasUnderline == false)continue;
                //Debug.Log("================="+ item.boxes.Count);
                for (int i = 0; i < item.boxes.Count; i++)
                {
                    //计算下划线的位置
                    float offsetX = (float)Convert.ToDouble(item.boxes[i].width * 0.045);
                    Vector3[] _ulPos = new Vector3[4];
                    _ulPos[0] = item.boxes[i].position + new Vector2(-offsetX, -2.0f);
                    _ulPos[1] = item.boxes[i].position + new Vector2(item.boxes[i].width + offsetX, -2.0f);
                    _ulPos[2] = item.boxes[i].position + new Vector2(item.boxes[i].width + offsetX, 3.0f);
                    _ulPos[3] = item.boxes[i].position + new Vector2(-offsetX, 3.0f);

                    //绘制下划线
                    for (int j = 0; j < 4; j++)
                    {
                        m_TempVerts[j] = _TUT[j];
                        m_TempVerts[j].color = item.color;
                        m_TempVerts[j].position = _ulPos[j];
                        // if (j == 3)
                        //     toFill.AddUIVertexQuad(m_TempVerts);
                    }
                    //画多次加深颜色
                    for (int n = 0; n < 1; n++)
                    {
                        toFill.AddUIVertexQuad(m_TempVerts);
                    }
                }
            }
        }

        /// <summary>
        /// 点击事件检测是否点击到超链接文本
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out lp);

            for (var j = 0; j < m_HrefInfos.Count; j++)
            {
                var linkInfo = m_HrefInfos[j];
                var boxes = linkInfo.boxes;
                for (var i = 0; i < boxes.Count; ++i)
                {
                    if (boxes[i].Contains(lp))
                    {
                        int index = j + 1;
                        m_OnHrefClick.Invoke(index);
                        return;
                    }
                }
            }
        }

        private Color HexToColor(string hexStr)
        {
            if (hexStr != null)
            {
                Byte rByte = Byte.Parse(hexStr.Substring(1, 2), NumberStyles.HexNumber);
                Byte gByte = Byte.Parse(hexStr.Substring(3, 2), NumberStyles.HexNumber);
                Byte bByte = Byte.Parse(hexStr.Substring(5, 2), NumberStyles.HexNumber);
                return new Color32(rByte, gByte, bByte, Byte.MaxValue);
            }
            else
                return Color.white;
        }

        public override float preferredHeight
        {
            get
            {
                var settings = GetGenerationSettings(new Vector2(GetPixelAdjustedRect().size.x, 0.0f));
                var layout_text = m_OutputText;
                return cachedTextGeneratorForLayout.GetPreferredHeight(layout_text, settings) / pixelsPerUnit;
                // var settings = GetGenerationSettings(new Vector2(GetPixelAdjustedRect().size.x, 0.0f));
                // var layout_text = m_OutputText;
                // cachedTextGeneratorForLayout.Populate(layout_text, settings);
                // var lineCount = cachedTextGeneratorForLayout.lineCount;
                // return fontSize * lineCount + (lineCount - 1) * (lineSpacing - 1) * fontSize;
            }
        }

        public override float preferredWidth
        {
            get
            {
                var settings = GetGenerationSettings(new Vector2(GetPixelAdjustedRect().size.x, 0.0f));
                //var layout_text = s_HrefRegex.Replace(m_Text, string.Empty);
                var layout_text = m_OutputText;
                var tmpw = cachedTextGeneratorForLayout.GetPreferredWidth(layout_text, settings) / pixelsPerUnit;
                if(tmpw > rectTransform.rect.size.x)tmpw = rectTransform.rect.size.x;
                return tmpw;
            }
        }

        /// <summary>
        /// 超链接信息类
        /// </summary>
        private class HrefInfo
        {
            public int startIndex;

            public int endIndex;

            public string name;

            public Color32 color; 

            public bool hasUnderline;
            public readonly List<Rect> boxes = new List<Rect>();
        }
    }
}