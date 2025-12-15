using UnityEngine;

public class RotaryKnob : MonoBehaviour
{
    public Transform knobTransform;

    [Header("Диапазон поворота")]
    public float minAngle = -135f;
    public float maxAngle = 135f;

    [Header("Диапазон значений")]
    public float minValue = 0f;
    public float maxValue = 1f;

    public void SetValue(float value)
    {
        float t = Mathf.InverseLerp(minValue, maxValue, value);
        float angle = Mathf.Lerp(minAngle, maxAngle, t);
        // Вращение вокруг оси Z (X=0, Y=0, Z=angle)
        knobTransform.localRotation = Quaternion.Euler(0f, -90f, angle);
    }
}
