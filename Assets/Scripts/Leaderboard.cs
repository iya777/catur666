using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ranks;
    [SerializeField] private TextMeshProUGUI usernames;
    [SerializeField] private TextMeshProUGUI scores;
    [SerializeField] private TextMeshProUGUI highests;
    [SerializeField] private TextMeshProUGUI wins;
    [SerializeField] private TextMeshProUGUI loses;
    [SerializeField] private TextMeshProUGUI draws;

    public void READ_LEADERBOARD()
    {
        RestoreUI();
        StartCoroutine(IENUM_ReadLeaderboard());
    }
    public void RestoreUI()
    {
        ranks.text = "";
        usernames.text = "";
        scores.text = "";
        highests.text = "";
        wins.text = "";
        loses.text = "";
        draws.text = "";
    }
    private IEnumerator IENUM_ReadLeaderboard()
    {
        WWWForm form = new WWWForm();
        WWW www = new WWW("http://localhost/sqlconnect/Leaderboard.php", form);
        yield return www;
        string[] rows = www.text.Split('\n');
        for (int i = 0; i < rows.Length - 1; i++)
        {
            string[] data = rows[i].Split('\t');
            if (i > 9)
            {
                ranks.text += (i + 1) + " :\n";
            }
            else
            {
                ranks.text += (i + 1) + "  :\n";
            }
            usernames.text += data[1] + "\n";
            scores.text += data[2] + "\n";
            highests.text += data[3] + "\n";
            wins.text += data[4] + "\n";
            loses.text += data[5] + "\n";
            draws.text += data[6] + "\n";
        }
    }
}
