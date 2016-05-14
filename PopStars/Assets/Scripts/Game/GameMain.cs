using UnityEngine;
using System.Collections;

public class GameMain : MonoBehaviour {

    public static GameMain instance = null;

	// Use this for initialization
	void Start () 
    {
        instance = this;
	}

    void OnDestroy()
    {
        instance = null;
    }

	// Update is called once per frame
	void Update () {
	
	}
}
