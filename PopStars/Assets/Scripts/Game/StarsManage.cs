using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class StarsManage : MonoBehaviour {

    public static StarsManage instance = null;

    public GameObject[] starPrefabs;

    public const int MAX_ROW = 10;
    public const int MAX_COL = 10;

    //所有被选中的星星
    List<BlockStar> stars_selected = new List<BlockStar>();

    //所有星星
    BlockStar[,] stars_list = new BlockStar[MAX_ROW, MAX_COL];

	// Use this for initialization
	void Start () {
        instance = this;
        InitStars();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    BlockStar CreateStars(BlockStar.StarType type, int row, int col)
    {
        GameObject obj = GameObject.Instantiate(starPrefabs[(int)type]);
        obj.transform.parent = this.transform;
        obj.SetActive(true);
        BlockStar bs = obj.GetComponent<BlockStar>();
        bs.Init();
        bs.SetPos(row, col);
        return bs;
    }

    public void InitStars()
    {
        for (int i = 0; i < MAX_ROW; i++)
        {
            for (int j = 0; j < MAX_COL; j++)
            {
                BlockStar.StarType st = GenerateStarType();
                stars_list[i, j] = CreateStars(st, i, j);
            }
        }
    }

    public BlockStar.StarType GenerateStarType()
    {
        BlockStar.StarType st = (BlockStar.StarType)Random.Range((int)BlockStar.StarType.Star_Blue, (int)BlockStar.StarType.Star_Red);
        return st;
    }

    //设置所有相邻的相同星星为选中状态
    public void CheckStars(int row, int col, BlockStar.StarType type)
    {
        ClearAllStarsSearchState();
        ClearAllSelectedStars();
        CheckSameStars(row,col,type);
    }

    //查找临近的所有相同的星星,递归遍历
    void CheckSameStars(int row, int col,BlockStar.StarType type)
    {
        //无效的索引
        if (row < 0 || row >= MAX_ROW || col < 0 || col >= MAX_COL) return;
        //星星为null
        if (stars_list[row, col] == null) return;
        //这个位置已经被搜索过了
        if(stars_list[row,col].isSearched) return;
        stars_list[row,col].isSearched = true;
        //找到相同的星星类型，继续搜索
        if (stars_list[row, col]._type == type)
        {
            stars_list[row, col].isSelect = true;
            stars_selected.Add(stars_list[row, col]);
            //向左搜索
            CheckSameStars(row - 1, col,type);
            //向右搜索
            CheckSameStars(row + 1, col,type);
            //向上搜索
            CheckSameStars(row, col - 1, type);
            //向下搜索
            CheckSameStars(row, col + 1, type);
        }
    }

    //清除所有星星的扫描状态
    void ClearAllStarsSearchState()
    {
        for (int i = 0; i < MAX_ROW; i++)
        {
            for (int j = 0; j < MAX_COL; j++)
            {
                if (stars_list[i, j] != null)
                    stars_list[i, j].isSearched = false;
            }
        }
    }
    //清除之前选中的星星的选中状态
    void ClearAllSelectedStars()
    {
        for (int i = 0, imax = stars_selected.Count; i < imax; i++)
        {
            stars_selected[i].isSelect = false;
        }
        stars_selected.Clear();
    }

    //消除所选择的星星
    public void DestroyAllSelectStars()
    {
        if (stars_selected.Count <= 1) return;

        for (int i = 0, imax = stars_selected.Count; i < imax; i++)
        {
            BlockStar bs = stars_selected[i];
            int row = bs._row;
            int col = bs._col;
            stars_list[row, col] = null;
            bs.DestoryBoom();
        }
        stars_selected.Clear();
        CheckAllStarsPosition();
    }

    //检查所有位置是否合适
    public void CheckAllStarsPosition()
    {

        //检查每一行可以移动的星星
        List<BlockStar> colStars = new List<BlockStar>();
        for (int i = 0; i < MAX_COL; i++)
        {
            colStars.Clear();
            //找出本列所有不为空的星星
            for (int j = MAX_ROW - 1; j >= 0; j--)
            {
                if (stars_list[i, j] != null)
                    colStars.Add(stars_list[i,j]);
            }

            //将本列星星按照行从下到上设置位置，剩余位置设置为null
            for (int j = MAX_ROW - 1; j >= 0; j--)
            {
                int index = MAX_ROW - 1 - j;
                if (index < colStars.Count)
                {
                    stars_list[i, j] = colStars[index];
                    colStars[index].SetPos(i, j);
                }
                else
                {
                    stars_list[i, j] = null;
                }
            }
        }
        List<int> colNotNull = new List<int>();
        //检查每一列可以移动的星星,记录所有不为空的列
        for (int i = 0; i < MAX_ROW; i++)
        {
            for (int j = 0; j < MAX_COL; j++)
            {
                if (stars_list[i, j] != null)
                {
                    colNotNull.Add(i);
                    break;
                }
            }
        }
        //将所有列设置向左靠拢 有问题
        for (int i = 0; i < MAX_ROW; i++)
        {
            if (i < colNotNull.Count)
            {
                int index = colNotNull[i];
                if (i == index) continue;
                for (int j = 0; j < MAX_COL; j++)
                {
                    stars_list[i, j] = stars_list[index, j];
                    if (stars_list[i, j] != null)
                        stars_list[i, j].SetPos(i, j);
                }
            }
            else
            {
                for (int j = 0; j < MAX_COL; j++)
                    stars_list[i, j] = null;
            }
        }

    }

    //检查游戏是否结束
    public bool CheckIsGameOver()
    {
        return false;
    }
}
