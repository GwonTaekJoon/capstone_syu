using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject rawImage;

    void Start()
    {
        // 처음에 RawImage를 비활성화합니다.
        rawImage.SetActive(false);
    }

    // 마우스가 버튼 위에 올라갔을 때 호출되는 함수
    public void OnPointerEnter(PointerEventData eventData)
    {
        // RawImage를 활성화합니다.
        rawImage.SetActive(true);
    }

    // 마우스가 버튼에서 벗어났을 때 호출되는 함수
    public void OnPointerExit(PointerEventData eventData)
    {
        // RawImage를 비활성화합니다.
        rawImage.SetActive(false);
    }
}