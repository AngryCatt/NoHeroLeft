using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HeroLeft.Menu {
    public class _MenuList : MonoBehaviour {
        public Animator animator;
        public string variableName;
        public bool Showed = false;

        public void ShowSwitch() {
            Showed = !Showed;
            animator.SetBool(variableName, Showed);
        }

        private void OnEnable() {
            Showed = false;
        }
    }
}
