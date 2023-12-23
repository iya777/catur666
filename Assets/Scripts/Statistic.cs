using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Statistic : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public void Start()
    {
        READ_STATISTICS();
    }
    public void READ_STATISTICS()
    {
        if(DBManager.username != null)
        {
            StartCoroutine(ReadStatistics());
        }
    }
    private IEnumerator ReadStatistics()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        WWW www = new WWW("http://localhost/sqlconnect/GetStatistics.php", form);
        yield return www;
        infoText.text = www.text;
    }
}
