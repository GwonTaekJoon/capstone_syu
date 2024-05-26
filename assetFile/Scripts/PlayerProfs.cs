using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerProfs : MonoBehaviour
{
    public InputField inputID;
    public InputField inputPW;
    public InputField inputCheckPW;
    public InputField inputNick;
    public InputField inputPhoneNum;
    public InputField inputHeight;
    public InputField inputWeight;
    public InputField inputGender;
    public Text ToastMsg;    
    // Start is called before the first frame update
    public void Save()
    {
        if (string.IsNullOrEmpty(inputID.text) || string.IsNullOrEmpty(inputPW.text) || 
            string.IsNullOrEmpty(inputPhoneNum.text) || string.IsNullOrEmpty(inputNick.text) ||
            string.IsNullOrEmpty(inputHeight.text) || string.IsNullOrEmpty(inputWeight.text) ||
            string.IsNullOrEmpty(inputGender.text) || string.IsNullOrEmpty(inputCheckPW.text))
        {
            ToastMsg.text = "Please fill in all fields.";
            ToastMsg.color = Color.black;
            ToastMsg.gameObject.SetActive(true);
            return;
        }

        if (inputPW.text != inputCheckPW.text)
        {
            ToastMsg.text = "Password and confirm password do not match.";
            ToastMsg.color = Color.red;
            ToastMsg.gameObject.SetActive(true);
            return;
        }

        PlayerPrefs.SetString("ID",inputID.text);
        PlayerPrefs.SetString("PW",inputPW.text);
        PlayerPrefs.SetString("CheckPW",inputCheckPW.text);
        PlayerPrefs.SetString("Nick",inputNick.text);
        PlayerPrefs.SetString("PhoneNum",inputPhoneNum.text);
        PlayerPrefs.SetString("Gender",inputGender.text);
        PlayerPrefs.SetFloat("Height",float.Parse(inputHeight.text));
        PlayerPrefs.SetFloat("Weight",float.Parse(inputWeight.text));

        ToastMsg.text = "User data is saved.";
        ToastMsg.color = Color.blue;
        ToastMsg.gameObject.SetActive(true);

    }
}
