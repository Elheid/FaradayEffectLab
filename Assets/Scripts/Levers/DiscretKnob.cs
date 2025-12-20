using UnityEngine;
using System;

public class DiscretKnob : MonoBehaviour
{
    public Transform knobTransform;

    [Header("Диапазон поворота")]
    public float minAngle = -90;
    public float maxAngle = 45f;

    [Header("Дискретность")]
    [Min(1)] public int stepCount = 4;

    public event Action<int> OnIndexChanged;

    private float[] _cachedAngles;
    private int _currentIndex = -1;

    private void OnValidate()
    {
        RecalculateAngles();
    }

    private void RecalculateAngles()
    {
        _cachedAngles = new float[stepCount];
        for (int i = 0; i < stepCount; i++)
        {
            float t = stepCount > 1 ? (float)i / (stepCount - 1) : 0f;
            _cachedAngles[i] = Mathf.Lerp(minAngle, maxAngle, t);
        }
    }

    // 🔹 Используется ТОЛЬКО для визуального обновления
    public void SetIndex(int index)
    {
        if (knobTransform == null) return;

        if (_cachedAngles == null || _cachedAngles.Length != stepCount)
            RecalculateAngles();

        if (index < 0 || index >= _cachedAngles.Length) return;

        _currentIndex = index;
        knobTransform.localRotation =
            Quaternion.Euler(0f, -90f, _cachedAngles[index]);
    }

    // 🔹 Вызывать ТОЛЬКО при пользовательском вводе (клик, drag и т.п.)
    public void UserSetIndex(int index)
    {
        if (index == _currentIndex) return;

        SetIndex(index);
        OnIndexChanged?.Invoke(index);
    }
}



//using UnityEngine;

//public class DiscretKnob : MonoBehaviour
//{
//    public Transform knobTransform;

//    [Header("Диапазон поворота")]
//    public float minAngle = -90;  // начальный угол (в градусах, вокруг Z)
//    public float maxAngle = 45f;   // конечный угол

//    [Header("Дискретность")]
//    [Min(1)] public int stepCount = 4; // количество позиций (минимум 1)

//    // Кэшируем углы при старте или при изменении параметров (опционально)
//    private float[] _cachedAngles;

//    private void OnValidate()
//    {
//        // Обновляем кэш в редакторе при изменении stepCount/min/max
//        RecalculateAngles();
//    }

//    private void RecalculateAngles()
//    {
//        _cachedAngles = new float[stepCount];
//        for (int i = 0; i < stepCount; i++)
//        {
//            float t = stepCount > 1 ? (float)i / (stepCount - 1) : 0f;
//            _cachedAngles[i] = Mathf.Lerp(minAngle, maxAngle, t);
//            //Debug.Log(_cachedAngles[i]);

//        }
//    }

//    public void SetIndex(int index)
//    {
//        if (knobTransform == null)
//        {
//            Debug.LogError("knobTransform не назначен!", this);
//            return;
//        }

//        if (_cachedAngles == null || _cachedAngles.Length != stepCount)
//            RecalculateAngles();

//        if (index < 0 || index >= _cachedAngles.Length) return;

//        knobTransform.localRotation = Quaternion.Euler(0f, -90f, _cachedAngles[index]);

//    }
//}