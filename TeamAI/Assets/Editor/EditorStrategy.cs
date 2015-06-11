using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using TeamAI;
using System.Xml;
using System;

public class EditorStrategy : EditorWindow 
{
    [MenuItem ("TeamAI/Strategy")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EditorStrategy));
    }

    bool runOnce = false;
    Texture2D field;
    Texture2D player;

    Rect generalWindowRect;
    Rect personalWindowRect;

    List<Formation> formations;
    int currentSelectedPlayer = 0;
    int currentSelectedStrategy = 0;

    List<Strategy> strategies;
    public void init()
    {
        field = (Resources.LoadAssetAtPath("Assets/Textures/EditorField.png", typeof(Sprite)) as Sprite).texture;
        player = (Resources.LoadAssetAtPath("Assets/Textures/EditorPlayer.png", typeof(Sprite)) as Sprite).texture;
        generalWindowRect = new Rect(field.width + 20.0f, 10.0f, 200.0f, 300.0f);
        personalWindowRect = new Rect(10.0f, 10.0f, 200.0f, 200.0f);

        formations = new List<Formation>();

        strategies = new List<Strategy>();// = new Strategy();
        //strategies.Add(new Strategy());

        loadFormations();
        loadXML();

        runOnce = true;
    }

    void loadFormations()
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

            formations.Add(formation);
        }

        //Formation formation = new Formation();
        //formation.FormationName = formationList[0].FirstChild.InnerText;

        int stop = 0;
    }

    void loadXML()
    {
        if (!System.IO.File.Exists("Assets/Scripts/Strategies/Strategies.xml"))
            return;

        XmlDocument doc = new XmlDocument();
        doc.Load("Assets/Scripts/Strategies/Strategies.xml");

        XmlNodeList strategyList = doc.GetElementsByTagName("Strategy");

        for (int i = 0; i < strategyList.Count; i++)
        {
            XmlNode node = strategyList.Item(i);
            Strategy s = new Strategy();
            XmlNode child = node.FirstChild;
            s.formation = Convert.ToInt32(child.InnerText);
            child = child.NextSibling;
            s.risk = Convert.ToInt32(child.InnerText);
            child = child.NextSibling;
            s.attackLine = Convert.ToInt32(child.InnerText);
            child = child.NextSibling;
            s.midfieldLine = Convert.ToInt32(child.InnerText);
            child = child.NextSibling;
            s.defendLine = Convert.ToInt32(child.InnerText);
            //child = child.NextSibling;

            //formation.m_playersInfo = new List<PlayerInfo>();
            for (int j = 0; j < 5; j++)
            {
                child = child.NextSibling;

                PersonalBehavior pb = new PersonalBehavior();
                pb.behavior = Convert.ToInt32(child.FirstChild.InnerText);
                pb.zoneID = Convert.ToInt32(child.FirstChild.NextSibling.InnerText);

                s.m_personal.Add(pb);
            }
            strategies.Add(s);
        }

        //Formation formation = new Formation();
        //formation.FormationName = formationList[0].FirstChild.InnerText;

        int stop = 0;
    }

    void OnGUI()
    {
        if (!runOnce)
            init();

        //return;

        if (Event.current.type == EventType.mouseDown)
        {
            //Debug.Log(Event.current.rawType);

            for (int i = 0; i < formations[strategies[currentSelectedStrategy].formation].m_playersInfo.Count; i++)
            {
                PlayerInfo pi = formations[strategies[currentSelectedStrategy].formation].m_playersInfo[i] as PlayerInfo;
                Vector2 mousePos = Event.current.mousePosition;

                if (mousePos.x > pi.position.x - player.width / 2 && mousePos.x < pi.position.x + player.width - player.width/2 &&
                    mousePos.y > pi.position.y - player.height / 2 && mousePos.y < pi.position.y + player.height - player.height / 2)
                {
                    currentSelectedPlayer = i;
                    Repaint();
                    break;
                }
            }
        }

        GUI.DrawTexture(new Rect(0, 0, field.width, field.height), field);
        for (int i = 0; i < formations[strategies[currentSelectedStrategy].formation].m_playersInfo.Count; i++)
        {
            PlayerInfo pi = formations[strategies[currentSelectedStrategy].formation].m_playersInfo[i];
            GUI.DrawTexture(new Rect(pi.position.x - player.width/2.0f, pi.position.y - player.height/2.0f, player.width, player.height), player);
        }

        BeginWindows();
        generalWindowRect = GUI.Window(0, generalWindowRect, generalWindow, "General");
        personalWindowRect = GUI.Window(1, personalWindowRect, personalWindow, "Personal");
        EndWindows();

        float w = field.width / 4.0f;
        float h = field.height / 3.0f;
        Vector3 current = Vector3.zero + new Vector3(0.0f, field.height - h, 0.0f);

        Handles.BeginGUI();
        Handles.DrawWireDisc(formations[strategies[currentSelectedStrategy].formation].m_playersInfo[currentSelectedPlayer].position, Vector3.forward, 20.0f);
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                Vector3 p1 = current;
                Vector3 p2 = current + new Vector3(w, 0.0f, 0.0f);
                Vector3 p3 = current + new Vector3(w, h, 0.0f);
                Vector3 p4 = current + new Vector3(0.0f, h, 0.0f);

                Handles.DrawLine(p1, p2);
                Handles.DrawLine(p2, p3);
                Handles.DrawLine(p3, p4);
                Handles.DrawLine(p4, p1);
                Handles.Label(current, (y * 4 + x).ToString());

                current.x += w;
            }
            current.y -= h;
            current.x = 0;
        }

        for (int i = 0; i < strategies[currentSelectedStrategy].m_personal.Count; i++)
        {
            PersonalBehavior pb = strategies[currentSelectedStrategy].m_personal[i];
            if (pb.behavior == 1)
            {
                Vector3 pos = formations[strategies[currentSelectedStrategy].formation].m_playersInfo[i].position;
                /*
                 * int endY = p.destinationPos / 4;
        int endX = p.destinationPos - (endY * 4);
        endPos.x = w * (float)endX + w / 2.0f;
        endPos.y = field.height - (h * (float)endY + h / 2.0f);*/

                int y = pb.zoneID / 4;
                int x = pb.zoneID - (y * 4);
                Vector3 end = new Vector3(w * (float)x + w / 2.0f, field.height - (h * (float)y + h / 2.0f), 0.0f);
                Handles.DrawLine(pos, end);
            }
        }
        Handles.EndGUI();

    }

    int individualBehaviorChoise = 0;
    void personalWindow(int windowID)
    {
        PlayerInfo pi = formations[strategies[currentSelectedStrategy].formation].m_playersInfo[currentSelectedPlayer];
        PersonalBehavior pb = strategies[currentSelectedStrategy].m_personal[currentSelectedPlayer];
        EditorGUILayout.LabelField(pi.designation.ToString().Substring(1, pi.designation.ToString().Length - 1));

        List<string> individualBehavior = new List<string>();
        individualBehavior.Add("Man-to-man");
        individualBehavior.Add("Zone");
        individualBehavior.Add("Chase ball");
        pb.behavior = EditorGUILayout.Popup("Behavior", pb.behavior, individualBehavior.ToArray());

        switch (pb.behavior)
        {
            case 0:
                EditorGUILayout.EnumPopup(eDesignation.eAttacker);
                break;
            case 1:
                //EditorGUILayout.
                pb.zoneID = EditorGUILayout.IntField("Zone", pb.zoneID);
                break;
            case 2:
                break;
            default:
                break;
        }

        GUI.DragWindow();
    }

    void generalWindow(int windowID)
    {
        List<string> formationNames = new List<string>();
        for (int i = 0; i < formations.Count; i++)
        {
            formationNames.Add(formations[i].FormationName);
        }
        strategies[currentSelectedStrategy].formation = EditorGUILayout.Popup(strategies[currentSelectedStrategy].formation, formationNames.ToArray());

        strategies[currentSelectedStrategy].risk = EditorGUILayout.IntSlider("Risk", strategies[currentSelectedStrategy].risk, 0, 100);

        List<string> lineOptions = new List<string>();
        lineOptions.Add("Attack oriented");
        lineOptions.Add("Neutral");
        lineOptions.Add("Defense oriented");
        strategies[currentSelectedStrategy].attackLine = EditorGUILayout.Popup("Attackers", strategies[currentSelectedStrategy].attackLine, lineOptions.ToArray());
        strategies[currentSelectedStrategy].midfieldLine = EditorGUILayout.Popup("Midfielders", strategies[currentSelectedStrategy].midfieldLine, lineOptions.ToArray());
        strategies[currentSelectedStrategy].defendLine = EditorGUILayout.Popup("Defenders", strategies[currentSelectedStrategy].defendLine, lineOptions.ToArray());

        if (GUILayout.Button("Save"))
        {
            //Debug.Log("Button pressed");
            saveXML();
        }

        GUI.DragWindow();
    }

    void saveXML()
    {
        XmlWriter xml = XmlWriter.Create("Assets/Scripts/Strategies/Strategies.xml");
        xml.WriteStartDocument();
        xml.WriteStartElement("Root");

        for (int i = 0; i < strategies.Count; i++)
        {
            //Formation f = formations[i] as Formation;
            Strategy s = strategies[i];
            xml.WriteStartElement("Strategy");

            xml.WriteStartElement("formation");
            xml.WriteString(s.formation.ToString());
            xml.WriteEndElement();

            xml.WriteStartElement("risk");
            xml.WriteString(s.risk.ToString());
            xml.WriteEndElement();

            xml.WriteStartElement("attackLine");
            xml.WriteString(s.attackLine.ToString());
            xml.WriteEndElement();

            xml.WriteStartElement("midfieldLine");
            xml.WriteString(s.midfieldLine.ToString());
            xml.WriteEndElement();

            xml.WriteStartElement("defendLine");
            xml.WriteString(s.defendLine.ToString());
            xml.WriteEndElement();

            for (int j = 0; j < s.m_personal.Count; j++)
            {
                PersonalBehavior pb = s.m_personal[j];
                xml.WriteStartElement("Personal");

                xml.WriteStartElement("behavior");
                xml.WriteString(pb.behavior.ToString());
                xml.WriteEndElement();

                xml.WriteStartElement("zoneID");
                xml.WriteString(pb.zoneID.ToString());
                xml.WriteEndElement();

                xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }
        xml.WriteEndElement();
        xml.WriteEndDocument();

        xml.Close();
    }

    void OnDestroy()
    {
        runOnce = false;
    }
}
