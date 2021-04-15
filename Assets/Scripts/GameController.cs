using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler{

	public Vector2 MapSize;
	public Vector2 GameSize;
	public Grid[,] Grids;
	public GameObject GridGo;
	public RectTransform GamePanel;
	[SerializeField]
	public List<Histroy> Histroys; 
	private int score;
	public int Score {
		get {
			return score;
		}
		set
        {
			score = value;
			TxtScore.text = string.Format("分数：" + score);
        }
	}
	public Text TxtScore;

	[System.Serializable]
	public struct Histroy
    {
		public int score;
		public List<int> nums;
    }

	void Awake()
    {
		Grids = new Grid[(int)GameSize.x, (int)GameSize.y];
		GamePanel.sizeDelta = MapSize;
		GamePanel.anchoredPosition = -MapSize / 2;
		Histroys = new List<Histroy>();
	}

	// Use this for initialization
	void Start () {
        for (int i = 0; i < GameSize.x; i++)
        {
            for (int j = 0; j < GameSize.y; j++)
            {
				var go = Instantiate(GridGo);
				go.name = string.Format("({0},{1})", i, j);
				go.transform.SetParent(GamePanel, false);
				Grids[i, j] = go.GetComponent<Grid>();
			}
        }
		Restart();
	}

	public void Restart()
    {
		Score = 0;
		var grid_size = new Vector2(MapSize.x / GameSize.x, MapSize.y / GameSize.y);
		for (int i = 0; i < GameSize.x; i++)
		{
			for (int j = 0; j < GameSize.y; j++)
			{
				var grid = Grids[i, j];
				grid.Size = grid_size;
				grid.Pos = new Vector2(i, j);
				grid.Num = 0;
			}
		}
		AddNum();
		AddNum();
	}

	public int[] RangeNums = { 2, 4 };

	public void AddNum()
    {
		var zero_grid_list = new List<Grid>();
		for (int i = 0; i < GameSize.x; i++)
		{
			for (int j = 0; j < GameSize.y; j++)
			{
				var grid = Grids[i, j];
				if (grid.Num == 0)
                {
					zero_grid_list.Add(grid);
				}
			}
		}
		if(zero_grid_list.Count == 0)
        {
			return;
        }
		var pos = Random.Range(0, zero_grid_list.Count);
		zero_grid_list[pos].Num = RangeNums[Random.Range(0, RangeNums.Length)];
	}

	void SaveHistroy()
    {
		var list = new List<int>();
		for (int i = 0; i < GameSize.x; i++)
		{
			for (int j = 0; j < GameSize.y; j++)
			{
				var grid = Grids[i, j];
				list.Add(grid.Num);
			}
		}
		var histroy = new Histroy();
		histroy.score = Score;
		histroy.nums = list;
		Histroys.Insert(0, histroy);
		if(Histroys.Count >= 10)
        {
			print("记录大于10，删掉第10个");
			Histroys.RemoveAt(9);
        }
	}

	public void LoadHistroy()
	{
		if (Histroys.Count == 0)
        {
			print("没有历史");
			return;
        }
		var histroy = Histroys[0];
		Histroys.RemoveAt(0);
		Score = histroy.score;
		for (int i = 0; i < GameSize.x; i++)
		{
			for (int j = 0; j < GameSize.y; j++)
			{
				var grid = Grids[i, j];
				var idx = i * (int)GameSize.x + j;
				grid.Num = histroy.nums[idx];
			}
		}
	}

	bool CheckOver()
    {
        for (int i = 0; i < GameSize.x; i++)
        {
            for (int j = 0; j < GameSize.y; j++)
            {
                var grid = Grids[i, j];
                for (int k = -1; k <= 1; k++)
                {
                    if (k != 0)
                    {
						if(i + k >= 0 && i + k < GameSize.x)
                        {
							var lr_grid = Grids[i + k, j];
							if(lr_grid.Num == grid.Num)
                            {
								return false;
                            }
                        }
						if (j + k >= 0 && j + k < GameSize.y)
						{
							var tb_grid = Grids[i, j + k];
							if (tb_grid.Num == grid.Num)
							{
								return false;
							}
						}
                    }
                }
            }
        }
        return true;
    }

	void LeftDrag()
    {
		AllUnLock();
		var have_move = false;
		for (int i = 1; i < GameSize.x; i++)
		{
			for (int j = 0; j < GameSize.y; j++)
			{
				var grid = Grids[i, j];
				if(grid.Num > 0)
                {
					for (int k = (int)grid.Pos.x - 1; k >= 0; k--)
					{
						var _grid = Grids[k, j];
						if (_grid.IsLock)
						{
							break;
						}
						if (_grid.Num == 0)
						{
							_grid.Num = grid.Num;
							grid.Num = 0;
							grid = _grid;
							have_move = true;
						}
                        else if(_grid.Num == grid.Num)
                        {
							var tar_num = grid.Num + _grid.Num;
							_grid.Num = tar_num;
							Score = Score + tar_num;
							grid.Num = 0;
							_grid.IsLock = true;
							have_move = true;
							break;
						}
                        else if(_grid.Num != grid.Num)
                        {
							break;
                        }
					}
                }
			}
		}
        if (have_move)
        {
			AddNum();
        }
		if (CheckOver() == true)
		{
			print("游戏结束");
		}
	}

	void RightDrag()
	{
		AllUnLock();
		var have_move = false;
		for (int i = (int)GameSize.x - 1; i >= 0; i--)
		{
			for (int j = 0; j < GameSize.y; j++)
			{
				var grid = Grids[i, j];
				if (grid.Num > 0)
				{
					for (int k = (int)grid.Pos.x + 1; k < GameSize.x; k++)
					{
						var _grid = Grids[k, j];
						if (_grid.IsLock)
						{
							break;
						}
						if (_grid.Num == 0)
						{
							_grid.Num = grid.Num;
							grid.Num = 0;
							grid = _grid;
							have_move = true;

						}
						else if (_grid.Num == grid.Num)
						{
							var tar_num = grid.Num + _grid.Num;
							_grid.Num = tar_num;
							Score = Score + tar_num;
							grid.Num = 0;
							_grid.IsLock = true;
							have_move = true;
							break;
						}
						else if (_grid.Num != grid.Num)
						{
							break;
						}
					}
				}
			}
		}
		if (have_move)
		{
			AddNum();
		}
		if (CheckOver() == true)
		{
			print("游戏结束");
		}
	}
	void TopDrag()
	{
		AllUnLock();
		var have_move = false;
		 for (int j = (int)GameSize.y - 1; j >= 0; j--)
		{
			for (int i = 0; i < GameSize.x; i++)
			{
				var grid = Grids[i, j];
				if (grid.Num > 0)
				{
					for (int k = (int)grid.Pos.y + 1; k < GameSize.y; k++)
					{
						var _grid = Grids[i, k];
						if (_grid.IsLock)
						{
							break;
						}
						if (_grid.Num == 0)
						{
							_grid.Num = grid.Num;
							grid.Num = 0;
							grid = _grid;
							have_move = true;

						}
						else if (_grid.Num == grid.Num)
						{
							var tar_num = grid.Num + _grid.Num;
							_grid.Num = tar_num;
							Score = Score + tar_num;
							grid.Num = 0;
							_grid.IsLock = true;
							have_move = true;
							break;
						}
						else if (_grid.Num != grid.Num)
						{
							break;
						}
					}
				}
			}
		}
		if (have_move)
		{
			AddNum();
		}
		if (CheckOver() == true)
		{
			print("游戏结束");
		}
	}
	void DownDrag()
	{
		AllUnLock();
		var add_num = false;
		 for (int j = 1; j < GameSize.y; j++)
			{
			for (int i = 0; i < GameSize.x; i++)
			{
				var grid = Grids[i, j];
				if (grid.Num > 0)
				{
					for (int k = (int)grid.Pos.y - 1; k >= 0; k--)
					{
						var _grid = Grids[i, k];
                        if (_grid.IsLock)
                        {
							break;
                        }
						if (_grid.Num == 0)
						{
							_grid.Num = grid.Num;
							grid.Num = 0;
							grid = _grid;
							add_num = true;
						}
						else if (_grid.Num == grid.Num)
						{
							var tar_num = grid.Num + _grid.Num;
							_grid.Num = tar_num;
							Score = Score + tar_num;
							grid.Num = 0;
							_grid.IsLock = true;
							add_num = true;
							break;
						}
						else if (_grid.Num != grid.Num)
						{
							break;
						}
					}
				}
			}
		}
		if (add_num)
		{
			AddNum();
		}
		if (CheckOver() == true)
		{
			print("游戏结束");
		}
	}
	void AllUnLock()
    {
		for (int i = 0; i < GameSize.x; i++)
		{
			for (int j = 0; j < GameSize.y; j++)
			{
				var grid = Grids[i, j];
				grid.IsLock = false;
			}
		}
	}

	private Vector2 beginDragPos;
	public void OnBeginDrag(PointerEventData eventData)
	{
		beginDragPos = eventData.position;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		var pos = eventData.position - beginDragPos;
		var abs_x = Mathf.Abs(pos.x);
		var abs_y = Mathf.Abs(pos.y);
		var max = Mathf.Max(abs_x, abs_y);
		if(max < 30)
        {
			return;
        }
		SaveHistroy();
		if (Mathf.Abs(pos.x) > Mathf.Abs(pos.y))
        {
			if (pos.x > 0)
            {
				RightDrag();
            }
            else
            {
				LeftDrag();
			}
        }
        else
        {
			if (pos.y > 0)
			{
				TopDrag();
			}
			else
			{
				DownDrag();
			}
		}
	}

    public void OnDrag(PointerEventData eventData)
    {
    }
}
