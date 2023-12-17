using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VerifyInput : MonoBehaviour
{
    [SerializeField] private Button submitButton;
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField confirmUsernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField confirmPasswordField;
    public void VerifyLoginPassword()
    {
        submitButton.interactable = (usernameField.text.Length >= 4 && passwordField.text.Length >= 4);
    }
    public void VerifyUpdatePassword()
    {
        submitButton.interactable = (usernameField.text.Length >= 4 && passwordField.text.Length >= 4 && confirmPasswordField.text == passwordField.text);

    }
    public void VerifyUpdateUsername()
    {
        submitButton.interactable = (passwordField.text.Length >= 4 && usernameField.text.Length >= 4 && usernameField.text == confirmUsernameField.text);
    }
}
