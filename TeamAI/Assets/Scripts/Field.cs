using UnityEngine;
using System.Collections;
using TeamAI;

public class Field : MonoBehaviour 
{
	// Use this for initialization
	void Start() 
    {
        TeamAI.Global.init();
        GameObject test = TeamAI.Global.sField;

        int stop = 0;
	}
	
	// Update is called once per frame
	void Update() 
    {
        for (int i = 0; i < Global.GridSizeX * Global.GridSizeY; i++)
        {
            Global.Grid[i].score = 0.0f;
        }

        //TeamAI.Global.drawGrid();
        TeamAI.Global.drawPlanGrid();
	}

    void OnGUI()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                GridPoint gp = Global.PlanGrid[y * 4 + x];
                Vector3 min = Camera.main.WorldToScreenPoint(gp.min);
                GUI.Label(new Rect(gp.cameraMin.x, (Screen.height - gp.cameraMin.y) - (gp.cameraMax.y - gp.cameraMin.y), gp.cameraMax.x - gp.cameraMin.x, gp.cameraMax.y - gp.cameraMin.y), (gp.y * 4 + gp.x).ToString());
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