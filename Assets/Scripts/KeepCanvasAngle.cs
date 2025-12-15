using UnityEngine;

public class CanvasBillboardFix : MonoBehaviour
{
    public Camera uiCamera; // основная камера сцены

    private RectTransform _rect;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (uiCamera == null)
        {
            uiCamera = Camera.main;
        }

        if (uiCamera != null)
        {
            // Сохраняем позицию (она наследуется от родителя)
            Vector3 worldPos = transform.position;

            // Поворачиваем Canvas так, чтобы он смотрел "в камеру", но без наклона (только Y-поворот)
            // Или просто делаем его "плоским" — как обычный UI
            Vector3 forward = transform.position - uiCamera.transform.position;
            forward.y = 0; // убираем вертикальный наклон
            if (forward.magnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(forward.normalized, Vector3.up);
                transform.rotation = targetRot;
            }

            // Восстанавливаем позицию (на всякий случай)
            transform.position = worldPos;
        }
    }
}