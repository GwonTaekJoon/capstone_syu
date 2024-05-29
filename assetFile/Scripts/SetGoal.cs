using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetGoal : MonoBehaviour
{
    public InputField setGoal; // 목표 값을 입력할 InputField UI 요소
    public Text toastMsg; // 사용자에게 메시지를 보여줄 Text UI 요소

    public void Save()
    {
        // InputField의 텍스트가 비어있는지 확인
        if (string.IsNullOrEmpty(setGoal.text))
        {
            if (toastMsg != null)
            {
                toastMsg.text = "Please set goal.";
                toastMsg.color = Color.black;
                toastMsg.gameObject.SetActive(true);
            }
            return;
        }

        // 텍스트를 정수로 변환
        int goal;
        if (!int.TryParse(setGoal.text, out goal))
        {
            if (toastMsg != null)
            {
                toastMsg.text = "Please enter a valid number.";
                toastMsg.color = Color.black;
                toastMsg.gameObject.SetActive(true);
            }
            return;
        }

        // 목표가 100보다 큰지 확인
        if (goal > 100)
        {
            if (toastMsg != null)
            {
                toastMsg.text = "Please set it to 100 or less.";
                toastMsg.color = Color.black;
                toastMsg.gameObject.SetActive(true);
            }
            return;
        }

        if (goal <= 0)
        {
            if (toastMsg != null)
            {
                toastMsg.text = "Please enter a valid number.";
                toastMsg.color = Color.black;
                toastMsg.gameObject.SetActive(true);
            }
            return;
        }

        // 목표를 PlayerPrefs에 저장
        PlayerPrefs.SetInt("SetGoal", goal);

        if (toastMsg != null)
        {
            toastMsg.text = "Setting finish";
            toastMsg.color = Color.blue;
            toastMsg.gameObject.SetActive(true);
        }
    }
}
