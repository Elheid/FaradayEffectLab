using UnityEngine;

public class HornRotator : MonoBehaviour
{
    public Transform hornTransform;

    public Axis rotationAxis = Axis.Y;

    [Header("Если true — angle задаёт абсолютный поворот\nЕсли false — угол добавляется к начальному положению")]
    public bool useAbsoluteAngle = false;

    private Vector3 _initialEulerAngles;

    public enum Axis { X, Y, Z }

    void Awake()
    {
        if (hornTransform != null)
        {
            _initialEulerAngles = hornTransform.localEulerAngles;
        }
        else
        {
            Debug.LogError("hornTransform не назначен!", this);
        }
    }

    public void SetAngle(float angle)
    {
        if (hornTransform == null) return;

        Vector3 targetEulers = useAbsoluteAngle
            ? _initialEulerAngles  // будем перезаписывать одну компоненту
            : _initialEulerAngles; // или добавлять — но логика ниже учитывает оба случая

        if (useAbsoluteAngle)
        {
            // Заменяем угол на абсолютное значение от 0 до 360
            switch (rotationAxis)
            {
                case Axis.X: targetEulers.x = angle; break;
                case Axis.Y: targetEulers.y = angle; break;
                case Axis.Z: targetEulers.z = angle; break;
            }
        }
        else
        {
            // Добавляем смещение к начальному углу
            switch (rotationAxis)
            {
                case Axis.X: targetEulers.x = _initialEulerAngles.x + angle; break;
                case Axis.Y: targetEulers.y = _initialEulerAngles.y + angle; break;
                case Axis.Z: targetEulers.z = _initialEulerAngles.z + angle; break;
            }
        }

        hornTransform.localEulerAngles = targetEulers;
    }
}