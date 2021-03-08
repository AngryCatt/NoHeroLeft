using UnityEngine;

namespace HeroLeft.Misc
{
    public class missindicator : MonoBehaviour
    {
        public int availScene = 2;

        void Start()
        {
            if (Application.loadedLevel != availScene)
                gameObject.SetActive(false);
        }
    }
}