using UnityEngine.UI;
using UnityEngine;

namespace HeroLeft.Auth {
    public class ErrorText : MonoBehaviour {
        public string AnimName;

        public void SendError(string err) {
            GetComponent<Text>().text = err;
            GetComponent<Animator>().Play(AnimName);
        }
    }
}
