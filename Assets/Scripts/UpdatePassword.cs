using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdatePassword : MonoBehaviour
{
    [SerializeField] private Button submitButton;
    [SerializeField] private TextMeshProUGUI submitButtonInfoText;
    [SerializeField] private TMP_InputField oldPasswordField;
    [SerializeField] private TMP_InputField newPasswordField;
    [SerializeField] private TMP_InputField confirmNewPasswordField;
    [SerializeField] private GameObject statusGameObject;
    [SerializeField] private TextMeshProUGUI statusInfoText;
    public void RestoreUI()
    {
        submitButtonInfoText.text = "";
        submitButtonInfoText.gameObject.SetActive(false);
        oldPasswordField.text = "";
        newPasswordField.text = "";
        confirmNewPasswordField.text = "";
    }
    public void ResetSubmitButtonInfoText()
    {
        submitButtonInfoText.gameObject.SetActive(false);
    }
    public void UPDATE_PASSWORD()
    {
        if (oldPasswordField.text.Length < 4)
        {
            submitButtonInfoText.gameObject.SetActive(true);
            submitButtonInfoText.text = "Old password must be greater than 4 characters";
        }
        else if (newPasswordField.text.Length < 4)
        {
            submitButtonInfoText.gameObject.SetActive(true);
            submitButtonInfoText.text = "New password must be greater than 4 characters";
        }
        else if (confirmNewPasswordField.text != newPasswordField.text)
        {
            submitButtonInfoText.gameObject.SetActive(true);
            submitButtonInfoText.text = "New password does not match";
        }
        else
        {
            submitButtonInfoText.gameObject.SetActive(false);
            StartCoroutine(IENUM_UpdatePassword());
        }
    }
    private IEnumerator IENUM_UpdatePassword()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        form.AddField("oldpassword", oldPasswordField.text);
        form.AddField("newpassword", confirmNewPasswordField.text);
        WWW www = new WWW("http://localhost/sqlconnect/UpdatePassword.php", form);
        yield return www;
        if (www.text == "0")
        {
            statusInfoText.SetText("Password telah berhasil diubah!");
            statusInfoText.color = Color.white;
            statusGameObject.SetActive(true);
            oldPasswordField.text = "";
            newPasswordField.text = "";
            confirmNewPasswordField.text = "";
            submitButton.interactable = false;
            gameObject.SetActive(false);
        }
        else if (www.text == "1")
        {
            statusInfoText.SetText("Password lama salah.");
            statusInfoText.color = Color.red;
            statusGameObject.SetActive(true);
        }
        else
        {
            statusInfoText.SetText("Gagal mengubah password.");
            statusInfoText.color = Color.red;
            statusGameObject.SetActive(true);
        }
    }
}
