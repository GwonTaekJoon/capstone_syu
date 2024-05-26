using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerCheck : MonoBehaviour
{
    public InputField inputID;
    public InputField inputPW;
    public Text errorMessageText;
    public void Load()
    {
        string savedID = PlayerPrefs.GetString("ID");
        string savedPW = PlayerPrefs.GetString("PW");

        if (string.IsNullOrEmpty(inputID.text) || string.IsNullOrEmpty(inputPW.text))
        {
            errorMessageText.text = "Plase fill in ID / PW";
            errorMessageText.color = Color.black;
            errorMessageText.gameObject.SetActive(true);
        }
        else if (inputID.text == savedID && inputPW.text == savedPW)
        {
            SceneManager.LoadScene("WorkoutPage");
        }
        else if (inputID.text != savedID)
        {
            errorMessageText.text = "This ID does not exist";
            errorMessageText.color = Color.red;
            errorMessageText.gameObject.SetActive(true);
        }
        else if (inputPW.text != savedPW)
        {
            errorMessageText.text = "This PW does not exist";
            errorMessageText.color = Color.red;
            errorMessageText.gameObject.SetActive(true);
        }
    }
}