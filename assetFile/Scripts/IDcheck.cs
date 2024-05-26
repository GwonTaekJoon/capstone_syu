using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IDcheck : MonoBehaviour
{
    public InputField inputID;
    public Text ToastMsg;
    public void idcheck()
    {
        string savedID = PlayerPrefs.GetString("ID");
        
        if (string.IsNullOrEmpty(inputID.text))
        {
            ToastMsg.text = "Please enter your ID!";
            ToastMsg.color = Color.black; 
            ToastMsg.gameObject.SetActive(true);
        }
        else if (inputID.text == savedID)
        {
            ToastMsg.text = "This is a duplicate ID!";
            ToastMsg.color = Color.red;
            ToastMsg.gameObject.SetActive(true);
        }
        else if (inputID.text != savedID)
        {
            ToastMsg.text = "ID is available!";
            ToastMsg.color = Color.blue; 
            ToastMsg.gameObject.SetActive(true);
        }
    }
}
