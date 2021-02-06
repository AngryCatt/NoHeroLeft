using UnityEngine;
using UnityEngine.UI;


namespace HeroLeft.BattleLogic {
    public class BattleLog : MonoBehaviour {

        public static BattleLog battleLog;

        [Header("Transforms")]
        public Transform ScrollView;
        public Transform Content;
        public Text TextPrefab;
        public Transform ShowButton;

        [Header("Values")]
        public float logHeight = 18.75f;
        public int MaxChilds = 16;
        public float MaxSize = 300;
        public float dopSize = 4f;
        public Vector2 showedSize;
        private Vector2 standartSize;

        bool show = false;

        private void Start() {
            battleLog = this;
            standartSize = ScrollView.GetComponent<RectTransform>().sizeDelta;
        }

        public void HideAndShow() {
            if (show) {
                ScrollView.GetComponent<RectTransform>().sizeDelta = standartSize;
                ScrollView.GetComponent<RectTransform>().localPosition = Vector2.zero;
                ShowButton.localScale = new Vector2(1, -1);
            } else {
                ScrollView.GetComponent<RectTransform>().sizeDelta = showedSize;
                ScrollView.GetComponent<RectTransform>().localPosition = new Vector2(0, (showedSize.y - standartSize.y) / 2);
                ShowButton.localScale = new Vector2(1, 1);
            }
            show = !show;
        }

        public void addLog(string txt) {
            Instantiate<Text>(TextPrefab, Content).text = txt;
            if (Content.childCount > 16)
                Destroy(Content.GetChild(0).gameObject);
            Content.GetComponent<RectTransform>().sizeDelta = new Vector2(Content.GetComponent<RectTransform>().sizeDelta.x,Mathf.Clamp(logHeight * Content.childCount + dopSize, 0, MaxSize));
        }
    }
}
