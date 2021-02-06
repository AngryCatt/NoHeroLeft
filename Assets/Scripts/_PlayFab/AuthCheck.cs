using UnityEngine;

namespace HeroLeft.Auth {
    public class AuthCheck : MonoBehaviour {
        public GameObject AuthPanel;

        void Start() {
            if (!AuthController.HaveAuthorized && AuthController.HaveInternetConnection) {
                AuthPanel.SetActive(true);
            }
        }
    }
}
