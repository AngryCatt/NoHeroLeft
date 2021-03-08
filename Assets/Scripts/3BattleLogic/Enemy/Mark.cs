using UnityEngine;
using UnityEngine.UI;
using HeroLeft.BattleLogic;

namespace HeroLeft.Interfaces {
    public class Mark : MonoBehaviour {

        public Sprite Active;
        public Sprite inActive;
        public void First()
        {
            GetComponent<Image>().sprite = Active;
            HeroDrag.heroDrag.selectedPos = int.Parse(name);
        }

        public void Second()
        {
            GetComponent<Image>().sprite = inActive;
            HeroDrag.heroDrag.selectedPos = 999;
        }

        private void OnEnable()
        {
            GetComponent<Image>().sprite = inActive;

            if(BattleControll.heroLogic.transform.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1,1,1);
            }
            else
            {
                transform.localScale = Vector3.one;
            }
        }
    }
}