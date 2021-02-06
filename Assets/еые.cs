using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class еые : MonoBehaviour
{
    void Start()
    {
    //    Debug.Log("ff");
        Instantiate(Resources.Load("Prefabs/SelectCircle"), transform);
    }
}
