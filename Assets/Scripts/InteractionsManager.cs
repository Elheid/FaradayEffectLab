using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class InteractionsManager : MonoBehaviour
{
    public static InteractionsManager Instance;

    public Camera mainCamera;
    public CameraController cameraController; // <-- новое поле
    public float focusDistance = 2.0f;
    public float moveDuration = 0.5f;

    private InputSystem_Actions inputActions;
    private InteractableObject currentFocus;
    private Vector3 originalPosition;
    private bool isFocused = false;
    private InteractableObject hoveredObject = null;

    private Coroutine moveCoroutine = null;
    private Transform originalParent;
    public float minFocusDistance = 0.5f; // минимальное расстояние от камеры к центру объекта
    public float extraPadding = 1.05f; // немного расстояния, чтобы объект точно помещался
    private Quaternion originalRotation;
    private Vector3 originalEuler;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }


    private void Update()
    {
        if (isFocused)
        {
            if (inputActions.Player.Cancel.WasPressedThisFrame()) // Escape
            {
                ExitFocusMode();
            }
            return;
        }

        Vector2 mousePos = inputActions.Player.PointerPosition.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        RaycastHit hit;

        InteractableObject newHovered = null;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            newHovered = hit.collider.GetComponent<InteractableObject>();
        }
        // Если под указателем другой объект — переключаем подсветку
        if (newHovered != hoveredObject)
        {
            if (hoveredObject != null)
            {
                hoveredObject.SetHighlighted(false);
            }

            hoveredObject = newHovered;

            if (hoveredObject != null)
            {
                hoveredObject.SetHighlighted(true);
            }
        }

        // Если есть объект под курсором — обработать клик
        if (newHovered != null)
        {
            if (inputActions.Player.LeftClick.WasPressedThisFrame()) // замените на RightClick, если у вас ПКМ в другом действии
            {
                // Начинаем фокусировку
                FocusOnObject(newHovered);
            }
            return;
        }

    }

    public void FocusOnObject(InteractableObject obj)
    {
        if (isFocused || obj == null) return;

        // Сохраняем оригинальные данные, чтобы вернуть назад
        originalParent = obj.transform.parent;
        originalPosition = obj.transform.position;//orig position
        originalRotation = obj.transform.rotation;//orig rotation
        originalEuler = obj.transform.eulerAngles;
        currentFocus = obj;
        isFocused = true;

        // Блокируем камеру
        if (cameraController != null)
            cameraController.SetLock(true);

        // Убираем подсветку у hoveredObject, чтобы не конфликтовать (подсветку можно оставить, но логично убрать)
        if (hoveredObject != null && hoveredObject == obj)
        {
            hoveredObject.SetHighlighted(false);
            hoveredObject = null;
        }

        // Стартуем плавное перемещение
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveObjectToFront(obj));
    }

    private IEnumerator MoveObjectToFront(InteractableObject obj)
    {
        // Найдём рендереры и bounds
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0)
        {
            // Если рендереров нет — просто перемещаем к фиксированному месту перед камерой
            yield return MoveDirect(obj, mainCamera.transform.position + mainCamera.transform.forward * minFocusDistance);
            yield break;
        }

        // Объединяем bounds
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        Vector3 boundsCenterWorld = bounds.center;
        float radius = bounds.extents.magnitude; // аппроксимация радиусом сферы

        // Рассчитываем расстояние, на котором объект полностью помещается в поле зрения камеры
        float fovRad = mainCamera.fieldOfView * Mathf.Deg2Rad;
        float requiredDistance = 0f;
        if (fovRad > 0f)
        {
            // Чтобы вписать сферу радиуса r в вертикальный FOV:
            // r = distance * tan(fov/2)  => distance = r / tan(fov/2)
            requiredDistance = radius / Mathf.Tan(fovRad * 0.5f);
        }
        else
        {
            requiredDistance = minFocusDistance;
        }

        float desiredDistance = Mathf.Max(minFocusDistance, requiredDistance * extraPadding);

        // Точка в пространстве, куда хотим поместить центр bounds
        Vector3 targetCenterPos = mainCamera.transform.position + mainCamera.transform.forward * desiredDistance;

        // Смещение, которое нужно применить объекту, чтобы его bounds.center оказался в targetCenterPos
        Vector3 translation = targetCenterPos - boundsCenterWorld;

        Vector3 startPos = obj.transform.position;
        Vector3 endPos = startPos + translation; // перемещаем весь объект на нужный вектор

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);
            obj.transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = endPos;

        // ПОВОРОТ КАМЕРЫ ЛИЦОМ К ОБЪЕКТУ
        // Горизонтальное направление камеры
        Vector3 camForwardFlat = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);

        // Поворот только по горизонтали
        Quaternion lookRot = Quaternion.LookRotation(camForwardFlat, Vector3.up);

        // Применяем только Y-поворот камеры,
        // оставляя X и Z как у объекта
        obj.transform.rotation = Quaternion.Euler(
            originalEuler.x,        // не трогаем наклон объекта
            lookRot.eulerAngles.y,  // только поворот вокруг Y
            originalEuler.z         // оставляем "наклон" объекта прежним
        );

        moveCoroutine = null;
    }

    private IEnumerator MoveDirect(InteractableObject obj, Vector3 targetPos)
    {
        Vector3 startPos = obj.transform.position;
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);
            obj.transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = targetPos;
    }


    public void ExitFocusMode()
    {
        if (!isFocused) return;

        isFocused = false;

        // Остановим корутину, если она ещё идёт
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        if (currentFocus != null)
        {
            // Возвращаем объект на место. Если сцена меняла позицию (например, объекты динамичны), то
            // лучше сохранить originalPosition и вернуть туда.
            currentFocus.transform.position = originalPosition;
            currentFocus.transform.rotation = originalRotation;
            currentFocus.transform.eulerAngles = originalEuler;
            currentFocus.transform.parent = originalParent;
            currentFocus.SetHighlighted(false);
            currentFocus = null;
        }

        // Разблокируем камеру
        if (cameraController != null)
            cameraController.SetLock(false);
    }


    public bool IsFocused => isFocused;
}

