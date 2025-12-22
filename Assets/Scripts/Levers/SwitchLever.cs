using UnityEngine;
using System;

public class SwitchLever : MonoBehaviour
{
    [Header("Поворот рычага")]
    public Transform leverTransform;
    public float onAngleX = -20f;
    public float offAngleX = 0f;

    public bool IsOn { get; private set; }

    // Событие, чтобы UIManager мог подписаться
    public event Action<bool> OnLeverSwitched;

    public void SetState(bool isOn)
    {
        IsOn = isOn;

        if (leverTransform == null)
        {
            Debug.LogError("leverTransform не назначен в SwitchLever!", this);
            return;
        }

        Vector3 eulers = leverTransform.localEulerAngles;
        eulers.x = isOn ? onAngleX : offAngleX;
        leverTransform.localEulerAngles = eulers;
    }

    public void Toggle()
    {
        SetState(!IsOn);
        OnLeverSwitched?.Invoke(IsOn);
    }
}


//using UnityEngine;

//public class SwitchLever : MonoBehaviour
//{
//    [Header("Поворот рычага")]
//    public Transform leverTransform;
//    public float onAngleX = -20f;   // угол по X в положении "включено"
//    public float offAngleX = 0f;    // угол по X в положении "выключено"

//    public void SetState(bool isOn)
//    {
//        if (leverTransform == null)
//        {
//            Debug.LogError("leverTransform не назначен в SwitchLever!", this);
//            return;
//        }

//        Vector3 eulers = leverTransform.localEulerAngles;
//        eulers.x = isOn ? onAngleX : offAngleX;
//        leverTransform.localEulerAngles = eulers;
//    }
//}
