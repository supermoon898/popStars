using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class FlyTween : MonoBehaviour {

    public static FlyTween instance = null;

    List<Text> canUseTexts = new List<Text>();

    List<Text> useTexts = new List<Text>();

    public Text prefab_Text = null;

    public Canvas canvas = null;
	// Use this for initialization
	void Start () {
        instance = this;
	}

    void OnDestroy()
    {
        instance = null;
    }
    /// <summary>
    /// 飘字
    /// </summary>
    /// <param name="content">字的内容</param>
    /// <param name="from">游戏内的物品</param>
    /// <param name="to">UI内的位置</param>
    /// <param name="time">持续时间</param>
    public void FlyText(string content, GameObject from, GameObject to, float time)
    {
        Text txt = GetCanUseText();
        if (txt == null) return;
        txt.text = content;
        GameObject target = txt.gameObject;
        target.SetActive(true);
        if(canvas == null) canvas = GameObject.Find("UI").GetComponent<Canvas>();
        //from转换成屏幕坐标
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main,from.transform.position);
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, screenPos, canvas.worldCamera, out worldPos);
        target.transform.position = worldPos;
        iTween.MoveTo(target, iTween.Hash(iT.MoveTo.position, to.transform.position,iT.MoveTo.islocal,false, iT.MoveTo.time, time,
            iT.MoveTo.easetype, iTween.EaseType.easeOutSine, iT.MoveTo.oncomplete, "FlyTextComplete",iT.MoveTo.oncompletetarget,gameObject,iT.MoveTo.oncompleteparams,target));
        
    }
    //飘字的完成后的回调
    void FlyTextComplete(GameObject target)
    {
        target.SetActive(false);
        Text txt = target.GetComponent<Text>();
        useTexts.Remove(txt);
        canUseTexts.Add(txt);
    }

    //获取可用的文本
    public Text GetCanUseText()
    {
        Text txt = null;
        if (canUseTexts.Count <= 0)
        {
            txt = GameObject.Instantiate(prefab_Text);
            txt.transform.parent = transform;
            txt.transform.localPosition = Vector3.zero;
            txt.gameObject.SetActive(false);
        }
        else
        {
            txt = canUseTexts[0];
            useTexts.Add(txt);
            canUseTexts.RemoveAt(0);
        }
        return txt;
    }
}
