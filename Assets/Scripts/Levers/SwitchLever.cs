using UnityEngine;

public class SwitchLever : MonoBehaviour
{
    [Header("Поворот рычага")]
    public Transform leverTransform;
    public float onAngleX = -20f;   // угол по X в положении "включено"
    public float offAngleX = 0f;    // угол по X в положении "выключено"

    public void SetState(bool isOn)
    {
        if (leverTransform == null)
        {
            Debug.LogError("leverTransform не назначен в SwitchLever!", this);
            return;
        }

        Vector3 eulers = leverTransform.localEulerAngles;
        eulers.x = isOn ? onAngleX : offAngleX;
        leverTransform.localEulerAngles = eulers;
    }
}
