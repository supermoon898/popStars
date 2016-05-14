using UnityEngine;
using System.Collections;

public class BlockStar : MonoBehaviour {

    //星星的类型
    public enum StarType
    {
        Star_Blue,
        Star_Green,
        Star_Purple,
        Star_Red,
    }

    Vector2 starPos = new Vector2(-3.08f, 1.94f);

    const float space = 4f;

    //星星类型
    public StarType _type;

    //星星图片
    public SpriteRenderer _sprite = null;

    //缓存Transform
    Transform cacheTransform = null;

    //选中效果
    public GameObject _selected = null;

    //爆炸效果
    public GameObject effect_boom = null;

    //是否是选中状态
    bool _isSelect;
    public bool isSelect
    {
        set 
        {
            _isSelect = value;
            _selected.SetActive(_isSelect);
        }
        get { return _isSelect; }
    }

    //是否被查找过
    public bool isSearched = false;

    //星星的位置
    public int _row;
    public int _col;

    float _width;
    float _height;

	// Use this for initialization
	void Start () {
        
	}

    public void Init()
    {
        cacheTransform = transform;
        _height = _sprite.sprite.rect.height;
        _width = _sprite.sprite.rect.width;
        isSelect = false;
    }

    public void SetPos(int row, int col)
    {
        _row = row;
        _col = col;
        UpdatePosition();
    }

    void UpdatePosition()
    {
        Vector3 pos = Vector3.zero;
        pos.x = starPos.x + _row * (_width + space) * 0.01f;
        pos.y = starPos.y - _col * (_height + space) * 0.01f;
        pos.z = 10f;
        //transform.localPosition = pos;
        iTween.MoveTo(gameObject, pos, 0.5f);
    }

    public void OnMouseUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (isSelect)
            {
                StarsManage.instance.DestroyAllSelectStars();
                return;
            }
            isSelect = !isSelect;
            StarsManage.instance.CheckStars(_row, _col, _type);
            Debug.Log("isSelect is " + isSelect);
        }
    }

    //销毁自己
    public void DestoryBoom()
    {
        Debug.Log(string.Format("销毁 row {0},col {1},type {2}", _row, _col, _type));
        isSelect = false;
        _sprite.gameObject.SetActive(false);
        effect_boom.SetActive(true);
        GameObject.Destroy(gameObject, 1f);
    }

}
