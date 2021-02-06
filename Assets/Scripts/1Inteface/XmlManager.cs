using System.Xml;
using UnityEngine;

namespace HeroLeft.Interfaces {

    public class XmlManager : MonoBehaviour {

        public static bool ResourcableXml(string XmlPath, ref TextAsset xmlAsset) {
            if (!xmlAsset) {
                xmlAsset = Resources.Load<TextAsset>(XmlPath);
            }
            if (!xmlAsset) return false;
            return true;
        }

        public class LevelXML {
            public const string LevelXmlPath = "Xmls/Levels";
            private static TextAsset levelXmlAsset;

            public static LevelInfo GetLevelInfo(string id) {
                if (!ResourcableXml(LevelXmlPath, ref levelXmlAsset)) return null;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(levelXmlAsset.text);

                XmlElement xRoot = xmlDoc.DocumentElement;

                LevelInfo inf = new LevelInfo();

                bool NeedXml = false;
                XmlNode acceptedxnode = null;

                foreach (XmlNode xnode in xRoot) {
                    if (xnode.Attributes.GetNamedItem("lng").Value == GameManager.SelectedLanguage) {
                        acceptedxnode = xnode;
                        break;
                    }
                }

                string recomendedPrefix = "Рекоммендуемый уровень: ";
                if (acceptedxnode != null)
                    foreach (XmlNode xnode in acceptedxnode.ChildNodes) {
                        if (xnode.Name == "recomended") {
                            recomendedPrefix = xnode.ChildNodes[0].InnerText;
                        }

                        if (xnode.Attributes.Count > 0) {
                            XmlNode attr = xnode.Attributes.GetNamedItem("name");
                            if (attr.Value == id) {
                                NeedXml = true;
                            } else {
                                if (NeedXml) return inf;
                            }
                        }

                        if (NeedXml)
                            foreach (XmlNode childnode in xnode.ChildNodes) {
                                if (childnode.Name == "name") {
                                    inf.LevelName = childnode.InnerText;
                                }
                                if (childnode.Name == "description") {
                                    inf.LevelDiscription = childnode.InnerText;
                                }
                                if (childnode.Name == "rec") {
                                    inf.RecomendedLevel = recomendedPrefix + childnode.InnerText;
                                }
                            }
                    }
                return inf;
            }

            public class LevelInfo {
                public string LevelName;
                public string LevelDiscription;
                public string RecomendedLevel;
            }
        }
        public class UnitXML {
            public const string UnitXmlPath = "Xmls/Units";
            private static TextAsset unitXmlAsset;

            public static UnitInfo GetUnitInfo(string id) {
                if (!ResourcableXml(UnitXmlPath, ref unitXmlAsset)) return null;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(unitXmlAsset.text);

                XmlElement xRoot = xmlDoc.DocumentElement;

                UnitInfo inf = new UnitInfo();

                bool NeedXml = false;

                XmlNode acceptedxnode = null;

                foreach (XmlNode xnode in xRoot) {
                    if (xnode.Attributes.GetNamedItem("lng").Value == GameManager.SelectedLanguage) {
                        acceptedxnode = xnode;
                        break;
                    }
                }

                if (acceptedxnode != null)
                    foreach (XmlNode xnode in acceptedxnode.ChildNodes) {
                        if (xnode.Attributes.Count > 0) {
                            XmlNode attr = xnode.Attributes.GetNamedItem("name");
                            if (attr.Value == id) {
                                NeedXml = true;
                            } else {
                                if (NeedXml) return inf;
                            }
                        }

                        if (NeedXml)
                            foreach (XmlNode childnode in xnode.ChildNodes) {
                                if (childnode.Name == "name") {
                                    inf.UnitName = childnode.InnerText;
                                }
                                if (childnode.Name == "description") {
                                    inf.UnitDiscription = childnode.InnerText;
                                }
                            }
                    }
                return inf;
            }

            public class UnitInfo {
                public string UnitName;
                public string UnitDiscription;
            }
        }
    }
}
