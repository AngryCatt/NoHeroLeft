using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeroLeft.Misc
{
    public class dmgfistindicator : MonoBehaviour
    {
        public Transform firstPart;
        public Transform secondPart;

        public Vector2 startPos;
        public Vector2 endPos;


        void Start()
        {
            firstPart.localPosition = new Vector3(Random.Range(startPos.x, endPos.x), Random.Range(startPos.y, endPos.y), 0);
            secondPart.localPosition = new Vector3(-firstPart.localPosition.x, -firstPart.localPosition.y, 0);
        }
    }
}