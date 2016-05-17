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

    public UIScore ui_score = null;

	// Use this for initialization
	void Start () {
        instance = this;
        ResetGame();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool isOver = false;

    void OnGUI()
    {
        
        if (GUILayout.Button("Reset"))
        {
            ResetGame();
        }
        if (GUILayout.Button("CheckGameOver"))
        {
            isOver = CheckIsGameOver();
        }
        if (isOver)
            GUILayout.TextArea("Game is OVer");
        else
            GUILayout.TextArea("Game is not OVer");

        if (GUILayout.Button(BlockStar.easyType.ToString()))
        {
            int count = System.Enum.GetValues(typeof(iTween.EaseType)).Length;
            int index = (int)BlockStar.easyType;
            index = (index + 1) % count;
            Debug.Log("count : " + count + " EasyType : index" + index);
            BlockStar.easyType = (iTween.EaseType)index;
        }
        GUILayout.TextArea("EasySpeed");
        BlockStar.easySpeed = GUILayout.HorizontalSlider(BlockStar.easySpeed, 0f, 100f);
    }

    public void ResetGame()
    {
        for (int i = 0; i < MAX_ROW; i++)
        {
            for (int j = 0; j < MAX_COL; j++)
            {
                if (stars_list[i, j] != null)
                {
                    GameObject.Destroy(stars_list[i, j].gameObject);
                    stars_list[i, j] = null;
                }
            }
        }
        stars_selected.Clear();
        GameData.Reset();
        ui_score.UpdateScore(GameData.score);
        InitStars();
        /*FallAllStars();*/
        StartCoroutine("FallAllStarsCoroutine");
    }

    BlockStar CreateStars(BlockStar.StarType type, int row, int col)
    {
        GameObject obj = GameObject.Instantiate(starPrefabs[(int)type]);
        obj.transform.parent = this.transform;
        obj.SetActive(true);
        BlockStar bs = obj.GetComponent<BlockStar>();
        bs.Init();
        /*bs.SetPos(row, col);*/
        bs.InitPos(row, col);
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
        BlockStar.StarType st = (BlockStar.StarType)Random.Range((int)BlockStar.StarType.Star_Blue, (int)BlockStar.StarType.Star_Red +1);
        return st;
    }

    //设置所有相邻的相同星星为选中状态,返回选中的星星个数
    public int CheckStars(int row, int col, BlockStar.StarType type)
    {
        ClearAllStarsSearchState();
        ClearAllSelectedStars();
        CheckSameStars(row,col,type);
        SetAllSelectStarsState();
        PlaySelectEffect();
        return stars_selected.Count;
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
            /*stars_list[row, col].isSelect = true;*/
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

    //设置所有的搜索到的星星为选中状态
    void SetAllSelectStarsState()
    {
        for (int i = 0, imax = stars_selected.Count; i < imax; i++)
        {
            stars_selected[i].isSelect = true;
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
        StartCoroutine("BoomSelectStars");
    }
    //协程消除星星
    IEnumerator BoomSelectStars()
    {
        for (int i = 0, imax = stars_selected.Count; i < imax; i++)
        {
            BlockStar bs = stars_selected[i];
            int row = bs._row;
            int col = bs._col;
            stars_list[row, col] = null;
            FlyTween.instance.FlyText("" + (5 + i * 10), bs.gameObject, StarsManage.instance.ui_score.txt_score.gameObject, 5f);
            bs.DestoryBoom();
            if(i != imax -1)
                yield return new WaitForSeconds(Mathf.Max(0f,(0.1f-i*0.015f)));
        }
        stars_selected.Clear();
        /*CheckAllStarsPosition();*/
        //等待上下位置变动完成后，开始移动左右位置
        float waitTime = CheckAllStarsColPosition();

        yield return new WaitForSeconds(waitTime);

        CheckAllStarsRowPosition();

        //检查游戏是否结束
        isOver = CheckIsGameOver();
        if (isOver)
        {
            PlayBlinkStars();
            Invoke("ResetGame", 3f);
        }

    }
    //检查上下位置有无变动
    public float CheckAllStarsColPosition()
    {
        bool isHaveMove = false;
        //检查每一行可以移动的星星
        List<BlockStar> colStars = new List<BlockStar>();
        for (int i = 0; i < MAX_COL; i++)
        {
            colStars.Clear();
            //找出本列所有不为空的星星
            for (int j = MAX_ROW - 1; j >= 0; j--)
            {
                if (stars_list[i, j] != null)
                    colStars.Add(stars_list[i, j]);
            }
            //将本列星星按照行从下到上设置位置，剩余位置设置为null
            for (int j = MAX_ROW - 1; j >= 0; j--)
            {
                int index = MAX_ROW - 1 - j;
                if (index < colStars.Count)
                {
                    if(colStars[index]._col != j) isHaveMove = true;
                    stars_list[i, j] = colStars[index];
                    colStars[index].SetPos(i, j);
                }
                else
                {
                    stars_list[i, j] = null;
                }
            }
        }
        //返回等待时间，如果上下位置有变化，返回等待时间
        return isHaveMove ? 0.1f : 0f;
    }

    //检查左右位置有无变动
    public void CheckAllStarsRowPosition()
    {
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
        //将所有列设置向左靠拢
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

    //检查所有位置是否合适
    public void CheckAllStarsPosition()
    {
        
        

    }

    //检查游戏是否结束
    public bool CheckIsGameOver()
    {
        if (stars_list[0, MAX_COL - 1] == null) return true;

        for (int i = 0, imax = MAX_ROW; i < imax; i++)
        {
            for (int j = MAX_COL-1; j >0; j--)
            {
                if (stars_list[i, j] == null) break ;
                if (stars_list[i,j-1] == null) break;
                if (stars_list[i, j]._type == stars_list[i, j - 1]._type)
                    return false;
            }
        }

        for (int i = MAX_COL -1; i >0; i--)
        {
            for (int j = 0,jmax = MAX_ROW-1-1; j <jmax; j++)
            {
                if (stars_list[j, i] == null) break;
                if (stars_list[j+1, i] == null) break;
                if (stars_list[j,i]._type == stars_list[j+1,i]._type)
                    return false;
            }
        }
        return true;
    }

    //播放星星闪烁效果
    public void PlayBlinkStars()
    {
        for (int i = 0; i < MAX_ROW; i++)
        {
            for (int j = 0; j < MAX_COL; j++)
            {
                if (stars_list[i, j] == null) continue;
                stars_list[i, j].PlayBlinkEffect(true);
            }
        }

        Invoke("StopBlinkStars", 1.5f);
    }

    public void StopBlinkStars()
    {
        for (int i = 0; i < MAX_ROW; i++)
        {
            for (int j = 0; j < MAX_COL; j++)
            {
                if (stars_list[i, j] == null) continue;
                stars_list[i, j].PlayBlinkEffect(false);
            }
        }
    }

    //初始化游戏时候的星星下落动画
    IEnumerator FallAllStarsCoroutine()
    {
        for (int i = MAX_COL - 1; i >= 0; i--)
        {
            for (int j = 0; j < MAX_ROW; j++)
            {
                stars_list[j, i].FallStar();
                float RowdealyTime = 0f/*Random.Range(0.0f, 0.05f)*/;
                yield return new WaitForSeconds(RowdealyTime);
            }
            float ColdealyTime = 0f/*Random.Range(0.00f, 0.05f)*/;
            yield return new WaitForSeconds(ColdealyTime);
        }
    }

    void PlaySelectEffect()
    {
        for (int i = 0, imax = stars_selected.Count; i < imax; i++)
        {
            BlockStar bs = stars_selected[i];
            if (bs != null)
                bs.PlaySelectTween();
        }
    }

    void FallAllStars()
    {
        for (int i = MAX_COL - 1; i >= 0; i--)
        {
            for (int j = 0; j < MAX_ROW; j++)
            {
                stars_list[j, i].FallStar();
            }
        }
    }

    //根据选中的星星个数获取分数
    public int GetScore(int stars)
    {
        return stars * stars * 5;
    }

    //获取选中的分数
    public int GetSelectScore()
    {
        int sle_cout = stars_selected.Count;
        return GetScore(sle_cout);
    }
}
