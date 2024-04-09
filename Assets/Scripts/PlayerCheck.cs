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

        if (inputID.text == savedID && inputPW.text == savedPW)
        {
            // ID와 PW가 일치할 때 다음 페이지로 이동
            SceneManager.LoadScene("WorkoutPage"); // 다음 화면의 씬 이름을 입력하세요
        }
        else
        {
            // ID나 PW가 일치하지 않을 때 처리
            errorMessageText.text = "ID or PW is not match.";
            errorMessageText.gameObject.SetActive(true);
        }
    }
}