using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using TeamAI;

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

    ArrayList players;

    bool runOnce = false;
    public void init()
    {
        Debug.Log("Window opened");
        runOnce = true;

        field = (Resources.LoadAssetAtPath("Assets/Textures/EditorField.png", typeof(Sprite)) as Sprite).texture;
        player = (Resources.LoadAssetAtPath("Assets/Textures/EditorPlayer.png", typeof(Sprite)) as Sprite).texture;

        infoWindowRect = new Rect(field.width + 20.0f, 10.0f, 200.0f, 300.0f);

        minSize = new Vector2(field.width + infoWindowRect.width + 25.0f, minSize.y);
        players = new ArrayList();
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

        if (Event.current.type == EventType.mouseDown)
        {
            //Debug.Log(Event.current.rawType);

            for (int i = 0; i < players.Count; i++)
            {
                PlayerInfo pi = players[i] as PlayerInfo;
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
            //Debug.Log(Event.current.mousePosition);
            if (hasSelectedPlayer)
            {
                PlayerInfo pi = players[selectedPlayerId] as PlayerInfo;
                pi.position = Event.current.mousePosition;
                Repaint();
            }
        }

        //Sprite test = Resources.LoadAssetAtPath("Assets/Textures/Field.png", typeof(Sprite)) as Sprite;

        int stop = 0;
        //test.texture.Resize(test.texture.width / 2, test.texture.height / 2);

        GUI.DrawTexture(new Rect(0, 0, field.width, field.height), field);

        for (int i = 0; i < players.Count; i++)
        {
            PlayerInfo pi = players[i] as PlayerInfo;
            GUI.DrawTexture(new Rect(pi.position.x, pi.position.y, player.width, player.height), player);
        }

        BeginWindows();
        infoWindowRect = GUI.Window(0, infoWindowRect, infoWindow, "Tools");
        EndWindows();
    }

    void updateNumberOfPlayers()
    {
        if (oldNumberOfPlayer == numberOfPlayers)
            return;

        oldNumberOfPlayer = numberOfPlayers;
        if (numberOfPlayers < players.Count)
        {
            players.RemoveRange(numberOfPlayers, players.Count - numberOfPlayers);
        }
        else
        {
            int difference = numberOfPlayers - players.Count;
            for (int i = 0; i < difference; i++)
            {
                PlayerInfo pi = new PlayerInfo();
                pi.designation = eDesignation.eAttacker;
                pi.position = Vector3.zero;
                pi.toggle = true;
                players.Add(pi);
            }
        }
    }

    int formationSelected = 0;
    int numberOfPlayers = 0;
    int oldNumberOfPlayer = 0;
    int idesignation = 0;

    //Vector2 pos = Vector2.zero;
    bool folded = true;
    void infoWindow(int windowID)
    {
        string[] formationNames = new string[] {"Formation A", "Formation B"};
        string[] designation = new string[] { "Attacker", "Midfielder", "Defender", "Goalie" };

        formationSelected = EditorGUILayout.Popup(formationSelected, formationNames);
        numberOfPlayers = EditorGUILayout.IntField("Number of players", numberOfPlayers);
        for (int i = 0; i < players.Count; i++)
        {
            //EditorGUILayout.BeginFadeGroup(0.0f);
            PlayerInfo pi = players[i] as PlayerInfo;
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
            //EditorGUILayout.LabelField("Player" + i);
            //EditorGUILayout.EndFadeGroup();
        }
        

        GUI.DragWindow();
    }

    void OnDestroy()
    {
        Debug.Log("Window closed");
        runOnce = false;
    }
}
