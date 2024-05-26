using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgotPW : MonoBehaviour
{
    public InputField inputID;
    public InputField inputPhoneNum;
    public Text yourPW;

    public void load()
    {
        string savedPW = PlayerPrefs.GetString("PW");
        string savedID = PlayerPrefs.GetString("ID");
        string savedPhoneNum = PlayerPrefs.GetString("PhoneNum");
  
        if (string.IsNullOrEmpty(inputID.text) || string.IsNullOrEmpty(inputPhoneNum.text))
        {
            yourPW.text = "Please fill in ID / Phone Number";
            yourPW.fontSize = 30;
            yourPW.color = Color.black;
            yourPW.gameObject.SetActive(true);
        }
        else if (inputID.text == savedID && inputPhoneNum.text == savedPhoneNum)
        {
            yourPW.text = savedPW;
            yourPW.fontSize = 60;
            yourPW.color = Color.blue;
            yourPW.gameObject.SetActive(true);
        }
        else if (inputID.text != savedID || inputPhoneNum.text != savedPhoneNum)
        {
            yourPW.text = "not exist";
            yourPW.fontSize = 60;
            yourPW.color = Color.red;
            yourPW.gameObject.SetActive(true);
        }
    }
}

