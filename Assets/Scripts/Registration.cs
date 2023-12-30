using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Registration : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField passwordConfirmField;
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private Button submitButton;
    public void CallRegister()
    {
        StartCoroutine(Register());
    }
    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", nameField.text);
        form.AddField("password", passwordField.text);
        WWW www = new WWW("http://localhost/sqlconnect/Register.php", form);
        yield return www;
        if (www.text == "0")
        {
            Debug.Log("User created successfully");
            SceneManager.LoadScene(0);
        }
        else
        {
            errorMessage.gameObject.SetActive(true);
        }
    }
    public void VerifyInputs()
    {
        submitButton.interactable = (nameField.text.Length >= 4 && passwordField.text.Length >= 4 && passwordField.text == passwordConfirmField.text);
        if (passwordField.text != "" && passwordConfirmField.text != "")
        {
            if (passwordField.text.Length < 4 || passwordConfirmField.text.Length < 4)
            {
                if (passwordField.text != passwordConfirmField.text)
                {
                    errorMessage.SetText("Nilai dari confirm password harus sama dengan nilai pada password.");
                }
                else
                {
                    errorMessage.SetText("Password harus lebih dari 4 karakter!");
                }
                errorMessage.gameObject.SetActive(true);
            }
            else
            {       
                if (passwordField.text != passwordConfirmField.text)
                {
                    errorMessage.SetText("Nilai dari confirm password harus sama dengan nilai pada password.");
                    errorMessage.gameObject.SetActive(true);
                }
                else
                {
                    errorMessage.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            errorMessage.gameObject.SetActive(false);
        }
    }
}
