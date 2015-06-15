using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using TeamAI;
using System.Xml;

public class EditorFormation : EditorWindow 
{
    [MenuItem ("TeamAI/Formation")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EditorFormation));
    }

    Texture2D field;
    Texture2D player;
    Rect infoWindowRect;
    Rect controlWindowRect;

    ArrayList players;
    ArrayList formations;

    bool runOnce = false;
    public void init()
    {
        Debug.Log("Window opened");

        field = (Resources.LoadAssetAtPath("Assets/Textures/EditorField.png", typeof(Sprite)) as Sprite).texture;
        player = (Resources.LoadAssetAtPath("Assets/Textures/EditorPlayer.png", typeof(Sprite)) as Sprite).texture;

        infoWindowRect = new Rect(field.width + 20.0f, 10.0f, 200.0f, 300.0f);
        controlWindowRect = new Rect(10.0f, 10.0f, 200.0f, 300.0f);

        //minSize = new Vector2(field.width + infoWindowRect.width + 25.0f, minSize.y);
        players = new ArrayList();

        formations = new ArrayList();
        loadFormations();
        numberOfPlayers = (formations[0] as Formation).m_playersInfo.Count;

        runOnce = true;
    }

    bool hasSelectedPlayer = false;
    int selectedPlayerId = 0;
    void OnGUI()
    {
        if (!runOnce)
            init();

        //return;

        updateNumberOfPlayers();

        //if (Event.current.isMouse) Event.current.Use();
        
        //Debug.Log(Event.current);
        Formation f = formations[currentSelectedFormation] as Formation;

        //Debug.Log(Event.current.mousePosition);
        if (Event.current.type == EventType.mouseDown)
        {
            //Debug.Log(Event.current.rawType);

            for (int i = 0; i < f.m_playersInfo.Count; i++)
            {
                PlayerInfo pi = f.m_playersInfo[i] as PlayerInfo;
                Vector2 mousePos = Event.current.mousePosition;

                if (mousePos.x > pi.position.x && mousePos.x < pi.position.x + player.width &&
                    mousePos.y > pi.position.y && mousePos.y < pi.position.y + player.height)
                {
                    selectedPlayerId = i;
                    hasSelectedPlayer = true;
                    break;
                }
            }
        }

        if (Event.current.type == EventType.mouseUp)
            hasSelectedPlayer = false;

        if (Event.current.type == EventType.mouseDrag)
        {
            //Debug.Log(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
            //Debug.Log(Event.current.mousePosition);
            if (hasSelectedPlayer)
            {
                PlayerInfo pi = f.m_playersInfo[selectedPlayerId] as PlayerInfo;
                pi.position = Event.current.mousePosition;
                Repaint();
            }
        }

        //Sprite test = Resources.LoadAssetAtPath("Assets/Textures/Field.png", typeof(Sprite)) as Sprite;

        int stop = 0;
        //test.texture.Resize(test.texture.width / 2, test.texture.height / 2);

        GUI.DrawTexture(new Rect(0, 0, field.width, field.height), field);

        for (int i = 0; i < f.m_playersInfo.Count; i++)
        {
            PlayerInfo pi = f.m_playersInfo[i] as PlayerInfo;
            GUI.DrawTexture(new Rect(pi.position.x, pi.position.y, player.width, player.height), player);
        }

        BeginWindows();
        infoWindowRect = GUI.Window(0, infoWindowRect, infoWindow, "Formation");
        controlWindowRect = GUI.Window(1, controlWindowRect, controlWindow, "General");
        EndWindows();
    }

    string textField = "";
    void controlWindow(int windowID)
    {
        textField = GUILayout.TextField(textField);

        if (GUILayout.Button("Add"))
        {
            Formation f = new Formation();
            f.FormationName = textField;
            formations.Add(f);

            currentSelectedFormation = formations.Count - 1;
            oldNumberOfPlayer = 0;
            numberOfPlayers = 0;
        }

        GUILayout.Button("Remove");

        List<string> formationNames = new List<string>();
        for (int i = 0; i < formations.Count; i++)
        {
            Formation f = formations[i] as Formation;
            formationNames.Add(f.FormationName);

        }
        currentSelectedFormation = EditorGUILayout.Popup(currentSelectedFormation, formationNames.ToArray());





        GUI.DragWindow();
    }

    void updateNumberOfPlayers()
    {
        if (oldNumberOfPlayer == numberOfPlayers)
            return;

        Formation f = formations[currentSelectedFormation] as Formation;

        oldNumberOfPlayer = numberOfPlayers;
        if (numberOfPlayers < f.m_playersInfo.Count)
        {
            f.m_playersInfo.RemoveRange(numberOfPlayers, f.m_playersInfo.Count - numberOfPlayers);
        }
        else
        {
            int difference = numberOfPlayers - f.m_playersInfo.Count;
            for (int i = 0; i < difference; i++)
            {
                PlayerInfo pi = new PlayerInfo();
                pi.designation = eDesignation.eAttacker;
                pi.position = Vector3.zero;
                pi.toggle = true;
                f.m_playersInfo.Add(pi);
            }
        }
    }

    int formationSelected = 0;
    int numberOfPlayers = 0;
    int oldNumberOfPlayer = 0;
    int idesignation = 0;

    int currentSelectedFormation = 0;

    //Vector2 pos = Vector2.zero;
    bool folded = true;
    void infoWindow(int windowID)
    {
        //List<string> a = new List<string>();
        //a.Add("test");
        //a.Add("aap");
        
        //string[] formationNames = new string[] {"Formation A", "Formation B"};
        string[] designation = new string[] { "Attacker", "Midfielder", "Defender", "Goalie" };

        Formation f = formations[currentSelectedFormation] as Formation;

        //formationSelected = EditorGUILayout.Popup(formationSelected, a.ToArray());
        numberOfPlayers = EditorGUILayout.IntField("Number of players", numberOfPlayers);
        for (int i = 0; i < f.m_playersInfo.Count; i++)
        {
            //EditorGUILayout.BeginFadeGroup(0.0f);
            PlayerInfo pi = f.m_playersInfo[i] as PlayerInfo;
            pi.toggle = EditorGUILayout.Foldout(pi.toggle, "Player" + i);
            if (pi.toggle)
            { 
                pi.position = EditorGUILayout.Vector3Field("Position", pi.position);

                int tmpDesignation = 0;
                if (pi.designation == eDesignation.eAttacker)
                    tmpDesignation = 0;
                if (pi.designation == eDesignation.eMidfielder)
                    tmpDesignation = 1;
                if (pi.designation == eDesignation.eDefender)
                    tmpDesignation = 2;
                if (pi.designation == eDesignation.eGoalie)
                    tmpDesignation = 3;

                tmpDesignation = EditorGUILayout.Popup(tmpDesignation, designation);

                if (tmpDesignation == 0)
                    pi.designation = eDesignation.eAttacker;
                if (tmpDesignation == 1)
                    pi.designation = eDesignation.eMidfielder;
                if (tmpDesignation == 2)
                    pi.designation = eDesignation.eDefender;
                if (tmpDesignation == 3)
                    pi.designation = eDesignation.eGoalie;
            }
        }

        if (GUILayout.Button("Save"))
        {
            //Debug.Log("Button pressed");
            saveAllFormations();
        }
        

        GUI.DragWindow();
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
                tmp = tmp.Remove(tmp.Length-1, 1);
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

    void saveAllFormations()
    {
        XmlWriter xml = XmlWriter.Create("Assets/Scripts/Formations/Formations.xml");
        xml.WriteStartDocument();
        xml.WriteStartElement("Root");

        for (int i = 0; i < formations.Count; i++)
        {
            Formation f = formations[i] as Formation;
            xml.WriteStartElement("Formation");

            xml.WriteStartElement("Name");
            xml.WriteString(f.FormationName);
            xml.WriteEndElement();

            xml.WriteStartElement("PlayerCount");
            xml.WriteString(f.m_playersInfo.Count.ToString());
            xml.WriteEndElement();

            for (int j = 0; j < f.m_playersInfo.Count; j++)
            {
                PlayerInfo pi = f.m_playersInfo[j] as PlayerInfo;
                xml.WriteStartElement("Player");

                xml.WriteStartElement("Pos");
                xml.WriteString(pi.position.ToString());
                xml.WriteEndElement();

                xml.WriteStartElement("Designation");
                xml.WriteString(pi.designation.ToString());
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
        Debug.Log("Window closed");
        runOnce = false;
    }
}
