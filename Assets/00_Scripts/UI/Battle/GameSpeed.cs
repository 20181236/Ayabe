using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeed : MonoBehaviour
{
    public Sprite normalSpeedSprite; // 1배속 버튼 이미지
    public Sprite doubleSpeedSprite; // 2배속 버튼 이미지
    public Image buttonImage;        // 버튼의 Image 컴포넌트

    private bool isDoubleSpeed = false;

    public void ToggleSpeed()
    {
        if (isDoubleSpeed)
        {
            ScreenAndTimeEffectController.instance.SetGameSpeed(1f); // 1배속
            buttonImage.sprite = normalSpeedSprite;
        }
        else
        {
            ScreenAndTimeEffectController.instance.SetGameSpeed(2f); // 2배속
            buttonImage.sprite = doubleSpeedSprite;
        }

        isDoubleSpeed = !isDoubleSpeed;
    }
}
