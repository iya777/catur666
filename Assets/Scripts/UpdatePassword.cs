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
        submitButton.interactable = (oldPasswordField.text.Length >= 4 && newPasswordField.text.Length >= 4 && confirmNewPasswordField.text.Length >= 4 && newPasswordField.text == confirmNewPasswordField.text);
        if (newPasswordField.text != "" && confirmNewPasswordField.text != "")
        {
            if (newPasswordField.text.Length < 4 || confirmNewPasswordField.text.Length < 4)
            {
                if (newPasswordField.text != confirmNewPasswordField.text)
                {
                    submitButtonInfoText.SetText("Nilai dari confirm password harus sama dengan nilai pada new password.");
                }
                else
                {
                    submitButtonInfoText.SetText("Password harus lebih dari 4 karakter!");
                }
                submitButtonInfoText.gameObject.SetActive(true);
            }
            else
            {
                if (newPasswordField.text != confirmNewPasswordField.text)
                {
                    submitButtonInfoText.SetText("Nilai dari confirm password harus sama dengan nilai pada new password.");
                    submitButtonInfoText.gameObject.SetActive(true);
                }
                else
                {
                    submitButtonInfoText.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            submitButtonInfoText.gameObject.SetActive(false);
        }
    }
    public void UPDATE_PASSWORD()
    {
        StartCoroutine(IENUM_UpdatePassword());
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
