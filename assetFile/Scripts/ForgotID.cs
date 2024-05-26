using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgotID : MonoBehaviour
{
    public InputField inputNick;
    public Text yourID;
    // Start is called before the first frame update
    public void Load()
    {
        string savedID = PlayerPrefs.GetString("ID");
        string savedNick = PlayerPrefs.GetString("Nick");
          
        if (string.IsNullOrEmpty(inputNick.text))
        {
            yourID.text = "Please fill in Nickname";
            yourID.fontSize = 30;
            yourID.color = Color.black;
            yourID.gameObject.SetActive(true);
        }
        else if (inputNick.text == savedNick)
        {
            yourID.text = savedID;
            yourID.fontSize = 60;
            yourID.color = Color.blue;
            yourID.gameObject.SetActive(true);
        }
        else if (inputNick.text != savedNick)
        {
            yourID.text = "not exist";
            yourID.fontSize = 60;
            yourID.color = Color.red;
            yourID.gameObject.SetActive(true);
        }
    }
}
