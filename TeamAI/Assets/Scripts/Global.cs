using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace TeamAI
{
    public struct GridPoint
    {
        public Vector3 position;
        public Vector3 min;
        public Vector3 cameraMin;
        public Vector3 max;
        public Vector3 cameraMax;
        public float score;
        public int x;
        public int y;
    }

    static public class Global
    {
        static public GameObject sField = null;
        static public Bounds sFieldBounds;

        static public float sGridWidth = 0.0f;
        static public float sGridHeight = 0.0f;

        public const int GridSizeX = 20;
        public const int GridSizeY = 20;

        static public bool DebugEnabled = false;

        public static GridPoint[] Grid = new GridPoint[GridSizeX * GridSizeY];
        public static List<Formation> sFormations;

        static public void init()
        {
            sField = GameObject.Find("Field");
            sFieldBounds = sField.GetComponent<Collider2D>().bounds;


            sFormations = new List<Formation>();

            loadFormations();
            createStartegyGrid();
        }

        static public void loadFormations()
        {
            if (!System.IO.File.Exists("Assets/Scripts/Formations/Formations.xml"))
                return;

            XmlDocument doc = new XmlDocument();
            doc.Load("Assets/Scripts/Formations/Formations.xml");

            XmlNodeList formationList = doc.GetElementsByTagName("Formation");

            for (int i = 0; i < formationList.Count; i++)
            {
                XmlNode node = formationList.Item(i);
                Formation formation = new Formation();
                XmlNode child = node.FirstChild;
                formation.FormationName = child.InnerText;
                child = child.NextSibling;
                int playerCount = System.Convert.ToInt32(child.InnerText);
                //formation.m_playersInfo = new List<PlayerInfo>();
                for (int j = 0; j < playerCount; j++)
                {
                    child = child.NextSibling;
                    PlayerInfo pi = new PlayerInfo();
                    string tmp = child.FirstChild.InnerText;
                    tmp = tmp.Remove(0, 1);
                    tmp = tmp.Remove(tmp.Length - 1, 1);
                    string[] positions = tmp.Split(',');
                    pi.position = new Vector3(System.Convert.ToSingle(positions[0].Trim()), System.Convert.ToSingle(positions[1].Trim()), System.Convert.ToSingle(positions[2].Trim()));

                    tmp = child.FirstChild.NextSibling.InnerText;
                    if (tmp == "eAttacker")
                        pi.designation = eDesignation.eAttacker;
                    if (tmp == "eMidfielder")
                        pi.designation = eDesignation.eMidfielder;
                    if (tmp == "eDefender")
                        pi.designation = eDesignation.eDefender;
                    if (tmp == "eGoalie")
                        pi.designation = eDesignation.eGoalie;

                    formation.m_playersInfo.Add(pi);
                }

                sFormations.Add(formation);
            }
        }

        static private void createStartegyGrid()
        {
            Vector3 min = sFieldBounds.min;
            Vector3 max = sFieldBounds.max;
            Vector3 current = min;

            sGridWidth = (max.x - min.x) / (float)GridSizeX;
            sGridHeight = (max.y - min.y) / (float)GridSizeY;

            for (int y = 0; y < GridSizeY; y++)
            {
                for (int x = 0; x < GridSizeX; x++)
                {
                    Vector3 center = current + new Vector3(sGridWidth / 2.0f, sGridHeight / 2.0f, 0.0f);

                    GridPoint gp = new GridPoint();
                    gp.position = center;
                    gp.min = current;
                    gp.cameraMin = Camera.main.WorldToScreenPoint(gp.min);
                    gp.max = current + new Vector3(sGridWidth, sGridHeight, 0.0f);
                    gp.cameraMax = Camera.main.WorldToScreenPoint(gp.max);
                    gp.score = 0.0f;
                    gp.x = x;
                    gp.y = y;

                    Grid[y * GridSizeX + x] = gp;
                    current.x += sGridWidth;
                }
                current.y += sGridHeight;
                current.x = min.x;
            }
        }

        static public void drawGrid()
        {
            Vector3 min = sFieldBounds.min;
            Vector3 max = sFieldBounds.max;
            Vector3 current = min;
            for (int y = 0; y < GridSizeY; y++)
            {
                for (int x = 0; x < GridSizeX; x++)
                {
                    Debug.DrawLine(current, current + new Vector3(sGridWidth, 0.0f, 0.0f));
                    Debug.DrawLine(current, current + new Vector3(0.0f, sGridHeight, 0.0f));
                    Debug.DrawLine(current + new Vector3(sGridWidth, 0.0f, 0.0f), current + new Vector3(sGridWidth, sGridHeight, 0.0f));
                    Debug.DrawLine(current + new Vector3(0.0f, sGridHeight, 0.0f), current + new Vector3(sGridWidth, sGridHeight, 0.0f));

                    //GUI.Box(new Rect(current.x, current.y, sGridWidth, sGridHeight), GUIContent.none);

                    current.x += sGridWidth;
                }
                current.y += sGridHeight;
                current.x = min.x;
            }
        }
    }
}