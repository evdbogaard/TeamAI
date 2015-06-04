using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using TeamAI;

public class EditorStrategy : EditorWindow 
{
    [MenuItem ("TeamAI/Strategy")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EditorStrategy));
    }

    bool runOnce = false;
    Texture2D field;

    Rect overalWindowRect;
    Rect defenceRect;

    public void init()
    {
        field = (Resources.LoadAssetAtPath("Assets/Textures/EditorField.png", typeof(Sprite)) as Sprite).texture;
        overalWindowRect = new Rect(field.width + 20.0f, 10.0f, 200.0f, 300.0f);
        defenceRect = new Rect(10.0f, 10.0f, 200.0f, 200.0f);

        runOnce = true;
    }

    void OnGUI()
    {
        if (!runOnce)
            init();

        GUI.DrawTexture(new Rect(0, 0, field.width, field.height), field);

        BeginWindows();
        overalWindowRect = GUI.Window(0, overalWindowRect, overalWindow, "Overal");
        defenceRect = GUI.Window(1, defenceRect, defenceWindow, "Something");
        EndWindows();

    }

    int chosenAttacker = 0;
    int chosenMidfield = 0;
    int chosenDefence = 0;
    void defenceWindow(int windowID)
    {
        List<string> offenceOptions = new List<string>();
        offenceOptions.Add("Don't defend");
        offenceOptions.Add("Fall back to midfield");
        offenceOptions.Add("Defend everywhere");
        chosenAttacker = EditorGUILayout.Popup("Attackers", chosenAttacker, offenceOptions.ToArray());

        List<string> midfieldOptions = new List<string>();
        midfieldOptions.Add("Defensive");
        midfieldOptions.Add("Neutral");
        midfieldOptions.Add("Offensive");
        chosenMidfield = EditorGUILayout.Popup("Midfielders", chosenMidfield, midfieldOptions.ToArray());

        List<string> defenceOptions = new List<string>();
        defenceOptions.Add("Stay in the back");
        defenceOptions.Add("Support");
        defenceOptions.Add("Full attack");
        chosenDefence = EditorGUILayout.Popup("Defenders", chosenDefence, defenceOptions.ToArray());

        GUI.DragWindow();
    }

    int aggresiveness = 50;
    int risk = 50;
    void overalWindow(int windowID)
    {
        aggresiveness = EditorGUILayout.IntSlider("Aggresiveness", aggresiveness, 0, 100);
        risk = EditorGUILayout.IntSlider("Risk", risk, 0, 100);

        List<string> manmarking = new List<string>();
        manmarking.Add("Man-to-man");
        manmarking.Add("Zone");
        EditorGUILayout.Popup("Man marking", 0, manmarking.ToArray());

        GUI.DragWindow();
    }

    void OnDestroy()
    {
        runOnce = false;
    }
}
