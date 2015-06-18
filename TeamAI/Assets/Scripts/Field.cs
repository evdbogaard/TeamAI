using UnityEngine;
using System.Collections;
using TeamAI;

public class Field : MonoBehaviour 
{
    GameStateManager gsm;
	// Use this for initialization
	void Start() 
    {
        TeamAI.Global.init();
        GameObject test = TeamAI.Global.sField;
        gsm = GetComponent<GameStateManager>();
        int stop = 0;
	}
	
	// Update is called once per frame
    bool showStrategyGrid = false;
    bool showInfluenceGrid = false;
	void Update() 
    {
        for (int i = 0; i < Global.GridSizeX * Global.GridSizeY; i++)
        {
            Global.Grid[i].score = 0.0f;
        }

        if (Global.gameTime < 0.0f)
        {
            if (Global.firstHalf)
            {
                Global.firstHalf = false;
                Global.gameTime = 150.0f;
                this.GetComponent<GameStateManager>().changeState(new StateKickoff());
                Global.sBall.transform.position = new Vector3(-0.1f, 0.0f, 0.0f);
                Global.sBall.controller = Global.CoachBlue.FieldPlayers[3];
                Global.sBall.controller.coach.newBallHolder();
                Global.sBall.velocity = Vector3.zero;
            }
            else
            {
                Debug.Break();
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            showStrategyGrid = !showStrategyGrid;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            showInfluenceGrid = !showInfluenceGrid;
        }

        if (showInfluenceGrid)
        {
            TeamAI.Global.drawGrid();
        }

        if (showStrategyGrid)
            TeamAI.Global.drawPlanGrid();
	}

    public bool scored = false;
    public bool failed = false;
    void OnGUI()
    {
        /*if (failed)
        {
            GUIStyle test = new GUIStyle();
            test.fontSize = 50;
            GUI.Label(new Rect(300, 200, 100, 100), "FAILED!!!", test);
        }
        else if (scored)
        {
            GUIStyle test = new GUIStyle();
            test.fontSize = 50;
            GUI.Label(new Rect(300, 200, 100, 100), "GOAL!!!", test);
        }*/

        GUIStyle test = new GUIStyle();
        test.fontSize = 50;
        test.normal.textColor = Color.white;
        GUI.Label(new Rect(300, 25, 100, 100), Global.blueGoals.ToString(), test);
        GUI.Label(new Rect(450, 25, 100, 100), Global.redGoals.ToString(), test);
        GUI.Label(new Rect(350, 525, 100, 100), Global.gameTime.ToString("0"), test);
        GUI.Label(new Rect(10, 525, 300, 100), gsm.m_currentState.ToString());
        GUI.Label(new Rect(10, 545, 300, 100), "[S] toggle strategy grid");
        GUI.Label(new Rect(10, 560, 300, 100), "[I] toggle influence grid");
        GUI.Label(new Rect(10, 575, 300, 100), "[P] toggle play lines");

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                GridPoint gp = Global.PlanGrid[y * 4 + x];
                Vector3 min = Camera.main.WorldToScreenPoint(gp.min);
                //GUI.Label(new Rect(gp.cameraMin.x, (Screen.height - gp.cameraMin.y) - (gp.cameraMax.y - gp.cameraMin.y), gp.cameraMax.x - gp.cameraMin.x, gp.cameraMax.y - gp.cameraMin.y), (gp.y * 4 + gp.x).ToString());
            }
        }

        if (!Global.DebugEnabled)
            return;

        Texture2D texture = new Texture2D(1, 1);

        for (int y = 0; y < Global.GridSizeY; y++)
        {
            for (int x = 0; x < Global.GridSizeX; x++)
            {
                GridPoint gp = Global.Grid[y * Global.GridSizeX + x];
                float score = gp.score;

                Color mainColor = (score > 0.0f) ? Color.cyan : Color.red;
                mainColor.a = Mathf.Lerp(0.0f, 1.0f, Mathf.Abs(score));

                texture.SetPixel(0, 0, mainColor);
                texture.Apply();
                GUI.skin.box.normal.background = texture;

                //Vector3 min = Camera.main.WorldToScreenPoint(gp.min);// Camera.main.WorldToScreenPoint(gp.position - new Vector3(Global.sGridWidth / 2.0f, Global.sGridHeight / 2.0f, 0.0f));
                //Vector3 max = Camera.main.WorldToScreenPoint(gp.max);// Camera.main.WorldToScreenPoint(gp.position + new Vector3(Global.sGridWidth / 2.0f, Global.sGridHeight / 2.0f, 0.0f));

                GUI.Label(new Rect(gp.cameraMin.x, (Screen.height - gp.cameraMin.y) - (gp.cameraMax.y - gp.cameraMin.y), gp.cameraMax.x - gp.cameraMin.x, gp.cameraMax.y - gp.cameraMin.y), (gp.y*Global.GridSizeX+gp.x).ToString());
                //GUI.Box(new Rect(gp.cameraMin.x, (Screen.height - gp.cameraMin.y) - (gp.cameraMax.y - gp.cameraMin.y), gp.cameraMax.x - gp.cameraMin.x, gp.cameraMax.y - gp.cameraMin.y), GUIContent.none);
            }
        }
    }
}