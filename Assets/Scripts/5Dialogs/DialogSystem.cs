using UnityEngine;
using System;

namespace HeroLeft.Dialog
{
    [CreateAssetMenu(menuName = "Dialog sys", fileName = "New dialog", order = 51)]
    public class DialogSystem : ScriptableObject
    {
        public phrase[] Phrases;

        [Serializable]
        public class phrase
        {
            public int type;
            public int panelActive = -1;
            [Multiline(4)] public string text;
            public string buttonText = "Далее";
        }
    }
}