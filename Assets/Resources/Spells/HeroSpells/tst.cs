using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using HeroLeft.BattleLogic;

public class tst : MonoBehaviour
{


    [ContextMenu("fff")]
    public void test()
    {
        Debug.Log(HeroLeft.Helper.lstDamage);
    }
}
