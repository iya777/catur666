using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeleteAccount : MonoBehaviour
{
    [SerializeField] private Button passwordNextButton;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private GameObject statusGameObject;
    [SerializeField] private TextMeshProUGUI statusInfoText;
    [SerializeField] private GameObject passwordFieldStatusText;
    public void DELETE_ACCOUNT()
    {
        StartCoroutine(IENUM_DeleteAccount());
    }
    public void RestoreUI()
    {
        passwordField.text = "";
    }
    public void CheckPasswordField()
    {
        if (passwordField.text == "")
        {
            passwordFieldStatusText.SetActive(true);
            passwordNextButton.interactable = false;
        }
        else
        {
            passwordFieldStatusText.SetActive(false);
            passwordNextButton.interactable = true;
        }
    }
    private IEnumerator IENUM_DeleteAccount()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        form.AddField("password", passwordField.text);
        WWW www = new WWW("http://localhost/sqlconnect/DeleteAccount.php", form);
        yield return www;
        if (www.text == "0")
        {
            DBManager.LogOut();
        }
        else if (www.text == "1")
        {
            statusInfoText.SetText("Password salah.");
            statusInfoText.color = Color.red;
            statusGameObject.SetActive(true);
        }
        else
        {
            statusInfoText.SetText("Gagal menghapus akun.");
            statusInfoText.color = Color.red;
            statusGameObject.SetActive(true);
        }
        passwordField.text = "";
    }
}
