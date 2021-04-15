using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public Image ImgBg;
	public Vector2 Size;
	private Vector2 pos;
	public Vector2 Pos
    {
        get
        {
			return pos;
        }
        set
        {
			pos = value;
			transform.localPosition = new Vector2(pos.x * Size.x, pos.y * Size.y);
			transform.GetComponent<RectTransform>().sizeDelta = Size;
		}
	}
	private int num = 0;
	public int Num
    {
		get
		{
			return num;
		}
		set
		{
			num = value;
			if (num > 0)
            {
				TxtNum.text = num.ToString();
            }
            else
            {
				TxtNum.text = "";
            }
			
			var show_color = Colors.Find(c => c.Num == (int)num);
			ImgBg.color = show_color.BgColor;
			TxtNum.color = show_color.NumColor;

		}
    }
	public Text TxtNum;
	public bool IsLock;

	[System.Serializable]
	public struct ShowColor{
		public int Num;
		public Color BgColor;
		public Color NumColor;
	}
	public List<ShowColor> Colors;
}
