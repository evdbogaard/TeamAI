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

    ArrayList plans;

    public void init()
    {
        field = (Resources.LoadAssetAtPath("Assets/Textures/EditorField.png", typeof(Sprite)) as Sprite).texture;

        infoWindowRect = new Rect(field.width + 20.0f, 10.0f, 200.0f, 300.0f);
        controlWindowRect = new Rect(10.0f, 10.0f, 200.0f, 300.0f);

        plans = new ArrayList();

        loadXML();

        runOnce = true;
    }

    void OnGUI()
    {
        if (!runOnce)
            init();

        //return;

        

        GUI.DrawTexture(new Rect(0, 0, field.width, field.height), field);
        //Debug.DrawLine(Vector3.zero, new Vector3(100, 100, 0));

        float w = field.width / 4.0f;
        float h = field.height / 3.0f;
        Vector3 current = Vector3.zero + new Vector3(0.0f, field.height - h, 0.0f);

        Handles.BeginGUI();

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

    int currentSelectedPlan = 0;
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
