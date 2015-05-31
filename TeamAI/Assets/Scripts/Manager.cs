using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TeamAI;

public class Manager : MonoBehaviour 
{
    private List<Player> m_fieldPlayers;
    //private GridPoint[] m_influenceGrid

    private Formation m_currentFormation;
    public int selectedFormation = 0;
    private int oldSelectedFormation = 0;

	// Use this for initialization
	void Start() 
    {
        m_fieldPlayers = new List<Player>();

        m_currentFormation = Global.sFormations[0] as Formation;

        for (int i = 0; i < this.transform.childCount; i++)
        {
            Player player = this.transform.GetChild(i).GetComponent<Player>();
            if (player)
            {
                PlayerInfo pi = m_currentFormation.m_playersInfo[i] as PlayerInfo;
                Vector3 temp = pi.position - new Vector3(36.0f, 68.0f, 0.0f);
                temp.z = 0.0f;

                // Field is 600 x 382
                // Ingame X: 36 - 700 or -6 6
                float x = (pi.position.x / 600.0f * 12.0f) - 6.0f;
                float y = (pi.position.y / 382.0f * 7.5f) - 3.75f;
                //x = (x / 600.0f * 12.0f) - 6.0f;
                //x = (x * 12.0f) - 6.0f;

                //Vector3 testCoord = Camera.main.ScreenToWorldPoint(temp);
                player.IdlePosition = new Vector3(x, -y, 0.0f);
                player.designation = pi.designation.ToString();
                m_fieldPlayers.Add(player);
            }
        }        
	}
	
	// Update is called once per frame
	void Update() 
    {
        if (selectedFormation != oldSelectedFormation)
        {
            oldSelectedFormation = selectedFormation;
            m_currentFormation = Global.sFormations[selectedFormation] as Formation;
            for (int i = 0; i < m_currentFormation.m_playersInfo.Count; i++)
            {
                PlayerInfo pi = m_currentFormation.m_playersInfo[i] as PlayerInfo;
                Player player = m_fieldPlayers[i] as Player;
                float x = (pi.position.x / 600.0f * 12.0f) - 6.0f;
                float y = (pi.position.y / 382.0f * 7.5f) - 3.75f;
                player.IdlePosition = new Vector3(x, -y, 0.0f);
                player.designation = pi.designation.ToString();
            }
        }

        int stop = 0;
        //Debug.Log(Input.mousePosition);
        //addScoreToGrid(0, 1.0f);
        //addScoreToGrid(Global.GridSizeX + 1, 1.0f);
        //addScoreToGrid((Global.GridSizeY - 1) * Global.GridSizeX, 1.0f);

        calculateInfluenceMaps();
	}

    private void calculateInfluenceMaps()
    {
        for (int i = 0; i < m_fieldPlayers.Count; i++)
        {
            int id = getGridID(m_fieldPlayers[i]);

            addScoreToGrid(id, 1.0f);

            addScoreToGrid(id + 1, 0.5f);
            addScoreToGrid(id - 1, 0.5f);
            addScoreToGrid(id + Global.GridSizeX, 0.5f);
            addScoreToGrid(id - Global.GridSizeX, 0.5f);
            addScoreToGrid(id + Global.GridSizeX + 1, 0.5f);
            addScoreToGrid(id + Global.GridSizeX - 1, 0.5f);
            addScoreToGrid(id - Global.GridSizeX + 1, 0.5f);
            addScoreToGrid(id - Global.GridSizeX - 1, 0.5f);
        }
    }

    private void addScoreToGrid(int id, float score)
    {
        if (id < 0 || id >= Global.GridSizeX * Global.GridSizeY)
            return;

        float currentScore = Global.Grid[id].score;
        float addedValue = score;

        if (this.name.Contains("Red"))
            addedValue *= -1.0f;

        float finalScore = currentScore + addedValue;
        if (finalScore > 0.0f)
            finalScore = Mathf.Min(1.0f, finalScore);
        else
            finalScore = Mathf.Max(-1.0f, finalScore);

        Global.Grid[id].score = finalScore;
    }

    private void addScoreToSurroundingGrids(int id, float score, int range)
    {

    }

    private int getGridID(Player player)
    {
        for (int i = 0; i < Global.GridSizeX * Global.GridSizeY; i++)
        {
            GridPoint gp = Global.Grid[i];
            if (player.transform.position.x > gp.min.x && player.transform.position.x < gp.max.x &&
                player.transform.position.y > gp.min.y && player.transform.position.y < gp.max.y)
            {
                return i;
            }
        }
        return 0;
    }

    public List<Player> FieldPlayers
    {
        get { return m_fieldPlayers; }
    }
}
