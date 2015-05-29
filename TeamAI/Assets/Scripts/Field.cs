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

        TeamAI.Global.drawGrid();
	}

    void OnGUI()
    {
        Texture2D texture = new Texture2D(1, 1);

        for (int y = 0; y < Global.GridSizeY; y++)
        {
            for (int x = 0; x < Global.GridSizeX; x++)
            {
                GridPoint gp = Global.Grid[y * Global.GridSizeX + x];
                float score = gp.score;

                if (y * Global.GridSizeX + x == 268)
                {
                    int stop = 0;
                }

                if (score > 0.4f && score < 0.6f)
                {
                    int stop = 0;
                }

                Color mainColor = (score > 0.0f) ? Color.cyan : Color.red;
                mainColor.a = Mathf.Lerp(0.0f, 1.0f, Mathf.Abs(score));

                texture.SetPixel(0, 0, mainColor);
                texture.Apply();
                GUI.skin.box.normal.background = texture;

                Vector3 maxInv = gp.max;
                maxInv.y *= -1.0f;
                Vector3 minInv = gp.min;
                minInv.y *= -1.0f;
                Vector3 min = Camera.main.WorldToScreenPoint(gp.min);// Camera.main.WorldToScreenPoint(gp.position - new Vector3(Global.sGridWidth / 2.0f, Global.sGridHeight / 2.0f, 0.0f));
                Vector3 max = Camera.main.WorldToScreenPoint(gp.max);// Camera.main.WorldToScreenPoint(gp.position + new Vector3(Global.sGridWidth / 2.0f, Global.sGridHeight / 2.0f, 0.0f));

                GUI.Box(new Rect(min.x, (Screen.height - min.y) - (max.y - min.y), max.x - min.x, max.y - min.y), GUIContent.none);
            }
        }
    }
}