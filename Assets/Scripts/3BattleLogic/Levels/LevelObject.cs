using UnityEngine;
using HeroLeft.Interfaces;

namespace HeroLeft.BattleLogic {

    [CreateAssetMenu(menuName = "LevelImage", fileName = "New Level", order = 51)]
    public class LevelObject : ScriptableObject {

        [Header("Level Information")]
        public string LevelName;
        public Sprite Ico;
        public Sprite BackGround;

        public XmlManager.LevelXML.LevelInfo LevelInfo {
            get {
                if(levelInfo == null) {
                    levelInfo = XmlManager.LevelXML.GetLevelInfo(LevelName);
                }
                return levelInfo;
            }
        }
        private XmlManager.LevelXML.LevelInfo levelInfo;

        [UnityEngine.Range(1,8)]
        public int EnemiesOnField = 2;

        [UnityEngine.Range(1,4)]
        public int EnemyRows = 2;

        public UnitObject[] Units;
        public EnemySpawn enemySpawn = new EnemySpawn();

        public enum EnemySpawn {
            afterDead,
            inNextTurn
        }
    }
}
