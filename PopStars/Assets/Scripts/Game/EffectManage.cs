using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EffectManage {

    EffectManage(){}
    static EffectManage _instance = null;
    public static EffectManage instance
    {
        get 
        {
            if (_instance == null) _instance = new EffectManage();
            return _instance;
        }
    }

	
}
