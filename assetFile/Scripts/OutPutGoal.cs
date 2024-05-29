using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutPutGoal : MonoBehaviour
{
    public Text toastMsg;

    void Start()
    {
        // PlayerPrefs에서 goal 값을 불러옵니다. 기본값으로 12를 사용합니다.
        int goal = PlayerPrefs.GetInt("SetGoal", 12);

        // goal 값을 Toast 메시지로 출력합니다.
        if (goal > 0)
        {
            toastMsg.text = "" + goal;
            toastMsg.color = Color.black;
            toastMsg.gameObject.SetActive(true);
        }
    }
}
