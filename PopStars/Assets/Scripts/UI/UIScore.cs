using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIScore : MonoBehaviour {

    public Text txt_highScore = null;

    public Text txt_score = null;

    public Text txt_scoreTip = null;

    public Button btn_Back = null;

	// Use this for initialization
	void Start () {
	
	}

    void InitCallBack()
    {
        
    }

    public void UpdateScore(int score)
    {
        txt_score.text = "" + score;
    }

    public void UpdateScoreTip(int sel_stars_count)
    {
        txt_scoreTip.text = string.Format("{0}块 {1}分", sel_stars_count, StarsManage.instance.GetScore(sel_stars_count));
        iTween.FadeTo(txt_scoreTip.gameObject, 1,3f);
    }

   
}
