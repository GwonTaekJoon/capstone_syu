using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckIDPW : MonoBehaviour
{
    public InputField inputID;
    public InputField inputNick;
    public InputField inputPhoneNum;
    public Text yourID;
    public Text yourPW;
    // Start is called before the first frame update
    public void Load()
    {
        string savedID = PlayerPrefs.GetString("ID");
        string savedPW = PlayerPrefs.GetString("PW");
        string savedNick = PlayerPrefs.GetString("Nick");
        string savedPhoneNum = PlayerPrefs.GetString("PhoneNum");

        if (inputID.text == savedID && inputPhoneNum.text == savedPhoneNum)
        {
            yourPW.text = savedPW;
            yourPW.gameObject.SetActive(true);
        }
        else if (inputID.text != savedID || inputPhoneNum.text != savedPhoneNum)
        {
            yourPW.text = "not exist";
            yourPW.gameObject.SetActive(true);
        }
        
        if (inputNick.text == savedNick)
        {
            yourID.text = savedID;
            yourID.gameObject.SetActive(true);
        }
        else if (inputNick.text != savedNick && inputNick.text is not null)
        {
            yourID.text = "not exist";
            yourID.gameObject.SetActive(true);
        }
    }
}
