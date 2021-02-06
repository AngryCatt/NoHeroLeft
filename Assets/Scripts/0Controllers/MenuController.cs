using System;
using UnityEngine;
using HeroLeft.Menu;
using HeroLeft.BattleLogic;
using UnityEngine.SceneManagement;

namespace HeroLeft {
    public class MenuController : MonoBehaviour {
        public int LoadedPage = 0;

        [Header("Pages")]
        public _MenuList MenuList;
        public Page[] Pages;

        [Header("Levels")]
        public string GameLevel;

        public static MenuController menuController;

        private void Start() {
            menuController = this;
            foreach(Page pg in Pages) {
                pg.page.SetActive(false);
            }
            LoadPage(0);
        }

        public void LoadLevel(LevelObject level) {
            BattleControll.LoadedLevel = level;
            SceneManager.LoadScene(GameLevel);
        }

        public void LoadPage(int pageNumber) {
            if (Pages.Length <= pageNumber) {
                throw new Exception("Страницы " + pageNumber.ToString() + " не существует");
            }

            if (MenuList != null)
                MenuList.gameObject.SetActive((Pages[pageNumber].needMenu) ? true : false);

            Pages[LoadedPage].page.SetActive(false);
            Pages[pageNumber].page.SetActive(true);
            LoadedPage = pageNumber;
        }
    }
}
