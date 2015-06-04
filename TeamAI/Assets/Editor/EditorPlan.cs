using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using TeamAI;
using System.Xml;

public class EditorPlan : EditorWindow 
{
    [MenuItem ("TeamAI/Plan")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EditorPlan));
    }

    bool runOnce = false;
    Rect controlWindowRect;
    Rect infoWindowRect;
    Texture2D field;
    Texture2D ball;
    Texture2D player;
    bool textureSelected;
    int selectedID;

    Vector2 ballPos;
    Vector2 beginPos;
    Vector2 endPos;

    ArrayList plans;

    int currentSelectedPlan;
    int previousSelectedPlan;


    public void init()
    {
        field = (Resources.LoadAssetAtPath("Assets/Textures/EditorField.png", typeof(Sprite)) as Sprite).texture;
        ball = (Resources.LoadAssetAtPath("Assets/Textures/Ball.png", typeof(Sprite)) as Sprite).texture;
        player = (Resources.LoadAssetAtPath("Assets/Textures/EditorPlayer.png", typeof(Sprite)) as Sprite).texture;

        infoWindowRect = new Rect(field.width + 20.0f, 10.0f, 200.0f, 300.0f);
        controlWindowRect = new Rect(10.0f, 10.0f, 200.0f, 300.0f);

        textureSelected = false;
        ballPos = Vector2.zero;
        beginPos = Vector2.zero;
        endPos = Vector2.zero;
        selectedID = 0;

        currentSelectedPlan = 0;
        previousSelectedPlan = -1;

        plans = new ArrayList();

        loadXML();

        runOnce = true;
    }

    void changeInSelection()
    {
        if (previousSelectedPlan == currentSelectedPlan)
            return;

        previousSelectedPlan = currentSelectedPlan;
        Plan p = plans[currentSelectedPlan] as Plan;

        int y = (p.ballPos / 4);
        int x = p.ballPos - (y * 4);

        float w = field.width / 4.0f;
        float h = field.height / 3.0f;

        ballPos.x = w * (float)x + w/2.0f;
        ballPos.y = field.height - (h * (float)y + h/2.0f);

        int beginy = p.teamPos / 4;
        int beginx = p.teamPos - (beginy * 4);
        beginPos.x = w * (float)beginx + w / 2.0f;
        beginPos.y = field.height - (h * (float)beginy + h / 2.0f);

        int endY = p.destinationPos / 4;
        int endX = p.destinationPos - (endY * 4);
        endPos.x = w * (float)endX + w / 2.0f;
        endPos.y = field.height - (h * (float)endY + h / 2.0f);

        int stop = 0;
    }

    void OnGUI()
    {
        if (!runOnce)
            init();

        //return;

        changeInSelection();

        if (Event.current.type == EventType.mouseDown)
        {
            Vector2 mousePos = Event.current.mousePosition;
            if (mousePos.x > ballPos.x - ball.width / 2 && mousePos.x < ballPos.x + ball.width + ball.width / 2 &&
                mousePos.y > ballPos.y - ball.height / 2 && mousePos.y < ballPos.y + ball.height + ball.height / 2)
            {
                textureSelected = true;
                selectedID = 0;
            }
            else if (mousePos.x > beginPos.x - player.width / 2 && mousePos.x < beginPos.x + player.width + player.width / 2 &&
                mousePos.y > beginPos.y - player.height / 2 && mousePos.y < beginPos.y + player.height + player.height / 2)
            {
                textureSelected = true;
                selectedID = 1;
            }
            else if (mousePos.x > endPos.x - player.width / 2 && mousePos.x < endPos.x + player.width + player.width / 2 &&
                mousePos.y > endPos.y - player.height / 2 && mousePos.y < endPos.y + player.height + player.height / 2)
            {
                textureSelected = true;
                selectedID = 2;
            }
            
        }

        if (Event.current.type == EventType.mouseUp)
        {
            if (textureSelected)
            {
                textureSelected = false;

                if (plans.Count != 0)
                {
                    int x = Mathf.FloorToInt(ballPos.x / field.width * 4.0f);
                    int y = Mathf.FloorToInt(3.0f - (ballPos.y / field.height * 3.0f));

                    int teamX = Mathf.FloorToInt(beginPos.x / field.width * 4.0f);
                    int teamY = Mathf.FloorToInt(3.0f - (beginPos.y / field.height * 3.0f));

                    int destX = Mathf.FloorToInt(endPos.x / field.width * 4.0f);
                    int destY = Mathf.FloorToInt(3.0f - (endPos.y / field.height * 3.0f));

                    Plan p = plans[currentSelectedPlan] as Plan;
                    p.ballPos = y * 4 + x;
                    p.teamPos = teamY * 4 + teamX;
                    p.destinationPos = destY * 4 + destX;
                }
            }

            int stop = 0;
        }

        if (Event.current.type == EventType.mouseDrag)
        {
            if (textureSelected)
            {
                switch (selectedID)
                {
                    case 0:
                        ballPos = Event.current.mousePosition;
                        break;
                    case 1:
                        beginPos = Event.current.mousePosition;
                        break;
                    case 2:
                        endPos = Event.current.mousePosition;
                        break;
                    default:
                        break;
                }
                Repaint();
            }
        }

        GUI.DrawTexture(new Rect(0, 0, field.width, field.height), field);
        GUI.DrawTexture(new Rect(ballPos.x - ball.width / 2, ballPos.y - ball.height / 2, ball.width, ball.height), ball);
        GUI.DrawTexture(new Rect(beginPos.x - player.width / 2, beginPos.y - player.height / 2, player.width, player.height), player);
        GUI.DrawTexture(new Rect(endPos.x - player.width / 2, endPos.y - player.height / 2, player.width, player.height), player);

        //Debug.DrawLine(Vector3.zero, new Vector3(100, 100, 0));

        float w = field.width / 4.0f;
        float h = field.height / 3.0f;
        Vector3 current = Vector3.zero + new Vector3(0.0f, field.height - h, 0.0f);

        Handles.BeginGUI();
        Handles.ArrowCap(0, beginPos + (endPos - beginPos), Quaternion.identity, 1.0f);
        // Arrow
        Handles.DrawLine(beginPos, endPos);
        Handles.DrawWireDisc(endPos, Vector3.forward, 20.0f);
        //Vector2 dir = beginPos - endPos;
        //Vector2 dirN = dir.normalized;
        

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

        //Handles.DrawLine(Vector3.zero, new Vector3(100.0f, 100.0f, 0.0f));
        Handles.EndGUI();


        BeginWindows();
        infoWindowRect = GUI.Window(0, infoWindowRect, infoWindow, "Info");
        controlWindowRect = GUI.Window(1, controlWindowRect, controlWindow, "Control");
        EndWindows();
    }

    void infoWindow(int windowID)
    {
        if (plans.Count == 0)
            return;

        Plan p = plans[currentSelectedPlan] as Plan;

        p.ballPos = EditorGUILayout.IntField("Ball Position", p.ballPos);
        p.teamPos = EditorGUILayout.IntField("Teammate Position", p.teamPos);
        p.destinationPos = EditorGUILayout.IntField("Teammate Destination", p.destinationPos);

        if (GUILayout.Button("Save"))
        {
            saveToXML();
        }

        GUI.DragWindow();
    }

    void loadXML()
    {
        if (!System.IO.File.Exists("Assets/Scripts/Plans/Plans.xml"))
            return;

        XmlDocument doc = new XmlDocument();
        doc.Load("Assets/Scripts/Plans/Plans.xml");

        XmlNodeList xmlPlans = doc.GetElementsByTagName("Plan");
        for (int i = 0; i < xmlPlans.Count; i++)
        {
            XmlNode node = xmlPlans.Item(i);

            Plan p = new Plan();
            XmlNode child = node.FirstChild;
            p.ballPos = System.Convert.ToInt32(child.InnerText);
            child = child.NextSibling;
            p.teamPos = System.Convert.ToInt32(child.InnerText);
            child = child.NextSibling;
            p.destinationPos = System.Convert.ToInt32(child.InnerText);

            plans.Add(p);
        }
    }

    void saveToXML()
    {
        XmlWriter xml = XmlWriter.Create("Assets/Scripts/Plans/Plans.xml");
        xml.WriteStartDocument();
        xml.WriteStartElement("Root");
        for (int i = 0; i < plans.Count; i++)
        {
            Plan p = plans[i] as Plan;
            xml.WriteStartElement("Plan");

            xml.WriteStartElement("BallPos");
            xml.WriteString(p.ballPos.ToString());
            xml.WriteEndElement();

            xml.WriteStartElement("MatePos");
            xml.WriteString(p.teamPos.ToString());
            xml.WriteEndElement();

            xml.WriteStartElement("MateDest");
            xml.WriteString(p.destinationPos.ToString());
            xml.WriteEndElement();

            xml.WriteEndElement();
        }
        xml.WriteEndElement();
        xml.WriteEndDocument();

        xml.Close();
    }

    void controlWindow(int windowID)
    {
        List<string> arrayNames = new List<string>();
        for (int i = 0; i < plans.Count; i++)
        {
            arrayNames.Add(i.ToString());
        }

        currentSelectedPlan = EditorGUILayout.Popup(currentSelectedPlan, arrayNames.ToArray());

        if (GUILayout.Button("Add"))
        {
            Plan p = new Plan();
            plans.Add(p);
        }

        GUI.DragWindow();
    }

    void OnDestroy()
    {
        runOnce = false;
    }
}
