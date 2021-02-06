using System;
using UnityEngine;
using UnityEngine.UI;

namespace HeroLeft.Interfaces {
    public class ResourcesHud : MonoBehaviour {
        public MonoBehaviour component;
        public string ParamName;
        public string Prefix;

        private void OnEnable() {
            GetResources();
        }

        [ContextMenu("Get")]
        public void GetResources() {
            Type type = (component != null) ? component.GetType() : typeof(GameManager);
            
            if (type.GetField(ParamName) != null) {
                GetComponent<Text>().text = (Prefix.Length > 0) ? Prefix + " " + type.GetField(ParamName).GetValue(component).ToString() : type.GetField(ParamName).GetValue(component).ToString();
            }
        }
    }
}
