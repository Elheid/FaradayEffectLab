using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InteractionsManager : MonoBehaviour
{
    public static InteractionsManager Instance;
    public static DeviceUIManager DeviceUIManager;

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
    public float focusOffset = 0f; // отрицательное значение — ближе к камере

    private Quaternion originalRotation;
    private Vector3 originalEuler;

    private bool eulerBackAfterFocus = true;


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


    //private void Update()
    //{
    //    if (isFocused)
    //    {
    //        if (inputActions.Player.Cancel.WasPressedThisFrame()) // Escape
    //        {
    //            ExitFocusMode();
    //        }
    //        return;
    //    }

    //    Vector2 mousePos = inputActions.Player.PointerPosition.ReadValue<Vector2>();
    //    Ray ray = mainCamera.ScreenPointToRay(mousePos);

    //    RaycastHit hit;

    //    InteractableObject newHovered = null;
    //    if (Physics.Raycast(mainCamera.ScreenPointToRay(inputActions.Player.PointerPosition.ReadValue<Vector2>()), out hit, 100f))
    //    {
    //        // ищем скрипт на самом коллайдере или на родителях
    //        newHovered = hit.collider.GetComponentInParent<InteractableObject>();
    //    }
    //    // Если под указателем другой объект — переключаем подсветку
    //    if (newHovered != hoveredObject)
    //    {
    //        if (hoveredObject != null) hoveredObject.SetHighlighted(false);
    //        hoveredObject = newHovered;
    //        if (hoveredObject != null) hoveredObject.SetHighlighted(true);
    //    }



    //    if (inputActions.Player.LeftClick.WasPressedThisFrame())
    //    {
    //        if (Physics.Raycast(
    //            mainCamera.ScreenPointToRay(inputActions.Player.PointerPosition.ReadValue<Vector2>()),
    //            out hit,
    //            100f))
    //        {
    //            // 1️⃣ СНАЧАЛА проверяем, не кликнули ли по рычагу
    //            SwitchLever lever = hit.collider.GetComponentInParent<SwitchLever>();
    //            if (lever != null)
    //            {
    //                lever.Toggle();
    //                return; // ❗ ВАЖНО
    //            }

    //            // 2️⃣ Если не рычаг — ищем объект фокуса
    //            InteractableObject obj = hit.collider.GetComponentInParent<InteractableObject>();
    //            if (obj != null)
    //            {
    //                FocusOnObject(obj);
    //            }
    //        }
    //    }


    //}
    //private void Update()
    //{
    //    // Escape работает всегда
    //    if (isFocused && inputActions.Player.Cancel.WasPressedThisFrame())
    //    {
    //        ExitFocusMode();
    //        return;
    //    }

    //    Vector2 mousePos = inputActions.Player.PointerPosition.ReadValue<Vector2>();
    //    Ray ray = mainCamera.ScreenPointToRay(mousePos);

    //    RaycastHit hit;

    //    // --- Подсветка ТОЛЬКО если не в фокусе ---
    //    if (!isFocused)
    //    {
    //        InteractableObject newHovered = null;

    //        if (Physics.Raycast(ray, out hit, 100f))
    //            newHovered = hit.collider.GetComponentInParent<InteractableObject>();

    //        if (newHovered != hoveredObject)
    //        {
    //            if (hoveredObject != null) hoveredObject.SetHighlighted(false);
    //            hoveredObject = newHovered;
    //            if (hoveredObject != null) hoveredObject.SetHighlighted(true);
    //        }
    //    }
    //    // --- Подсветка рычагов ---

    //    if (hoveredObject != null)
    //    {
    //        if (isFocused && hoveredObject.interactableType == InteractableType.Lever) hoveredObject.SetHighlighted(true);
    //        else hoveredObject.SetHighlighted(false);
    //    }

    //    // --- КЛИКИ РАБОТАЮТ ВСЕГДА ---
    //    if (inputActions.Player.LeftClick.WasPressedThisFrame())
    //    {
    //        if (Physics.Raycast(ray, out hit, 100f))
    //        {
    //            // 1️⃣ Рычаг — всегда приоритет
    //            SwitchLever lever = hit.collider.GetComponentInParent<SwitchLever>();
    //            if (lever != null)
    //            {
    //                lever.Toggle();
    //                return;
    //            }

    //            // 2️⃣ Фокус — только если НЕ в фокусе
    //            if (!isFocused)
    //            {
    //                InteractableObject obj = hit.collider.GetComponentInParent<InteractableObject>();
    //                if (obj != null)
    //                    FocusOnObject(obj);
    //            }
    //        }
    //    }
    //}

    private List<InteractableObject> hoveredObjects = new List<InteractableObject>();

    private void Update()
    {
        Vector2 mousePos = inputActions.Player.PointerPosition.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        // --- 1️⃣ Escape всегда работает ---
        if (isFocused && inputActions.Player.Cancel.WasPressedThisFrame())
        {
            ExitFocusMode();
            return;
        }

        // --- 2️⃣ Подсветка ---
        if (!isFocused)
        {
            // Не в фокусе: подсвечиваем только устройства под курсором
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
            HashSet<InteractableObject> currentlyHit = new HashSet<InteractableObject>();

            foreach (var hit in hits)
            {
                var obj = hit.collider.GetComponentInParent<InteractableObject>();
                if (obj != null && obj.interactableType != InteractableType.Lever)
                    currentlyHit.Add(obj);
            }

            // Включаем подсветку для новых объектов
            foreach (var obj in currentlyHit)
            {
                if (!hoveredObjects.Contains(obj))
                {
                    obj.SetHighlighted(true);
                    hoveredObjects.Add(obj);
                }
            }

            // Выключаем подсветку для объектов, которых больше нет под курсором
            for (int i = hoveredObjects.Count - 1; i >= 0; i--)
            {
                if (!currentlyHit.Contains(hoveredObjects[i]))
                {
                    hoveredObjects[i].SetHighlighted(false);
                    hoveredObjects.RemoveAt(i);
                }
            }
        }
        else if (isFocused && currentFocus != null)
        {
            // В фокусе: подсвечиваем только рычаги внутри currentFocus
            InteractableObject[] children = currentFocus.GetComponentsInChildren<InteractableObject>();
            foreach (var obj in children)
            {
                if (obj != null)
                    obj.SetHighlighted(obj.interactableType == InteractableType.Lever);
            }
        }

        // --- 3️⃣ Клики работают всегда ---
        if (inputActions.Player.LeftClick.WasPressedThisFrame())
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

            foreach (var hit in hits)
            {
                // 1️⃣ Рычаг — приоритет
                SwitchLever lever = hit.collider.GetComponentInParent<SwitchLever>();
                if (lever != null)
                {
                    lever.Toggle();
                    return;
                }

                // 2️⃣ Фокус — только если не в фокусе
                if (!isFocused)
                {
                    InteractableObject obj = hit.collider.GetComponentInParent<InteractableObject>();
                    if (obj != null)
                        FocusOnObject(obj);
                }
            }
        }
    }






    public void OpenUI(InteractableObject obj) {
        if (DeviceUIManager.Instance != null)
        {
            DeviceUIManager.Instance.ShowUI(obj);
            if (obj.interactableType == InteractableType.Horn)
                eulerBackAfterFocus = false;
            else eulerBackAfterFocus = true;
        }

    }

    public void CloseUI() {
        if (DeviceUIManager.Instance != null)
        {
            DeviceUIManager.Instance.HideAll();
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
        OpenUI(obj);
    }

    private IEnumerator MoveObjectToFront(InteractableObject obj)
    {
        // Найдём рендереры и bounds
        //Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        //if (renderers == null || renderers.Length == 0)
        //{
        //    // Если рендереров нет — просто перемещаем к фиксированному месту перед камерой
        //    yield return MoveDirect(obj, mainCamera.transform.position + mainCamera.transform.forward * minFocusDistance);
        //    yield break;
        //}

        //// Объединяем bounds
        //Bounds bounds = renderers[0].bounds;
        //for (int i = 1; i < renderers.Length; i++)
        //    bounds.Encapsulate(renderers[i].bounds);

        //Vector3 boundsCenterWorld = bounds.center;
        //float radius = bounds.extents.magnitude; // аппроксимация радиусом сферы

        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        if (colliders.Length == 0)
        {
            yield return MoveDirect(obj, mainCamera.transform.position + mainCamera.transform.forward * minFocusDistance);
            yield break;
        }

        Bounds bounds = colliders[0].bounds;
        for (int i = 1; i < colliders.Length; i++)
            bounds.Encapsulate(colliders[i].bounds);

        Vector3 boundsCenterWorld = bounds.center;
        float radius = bounds.extents.magnitude;



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

        float desiredDistance = Mathf.Max(minFocusDistance, requiredDistance * extraPadding) - obj.focusDistance;


        // Точка в пространстве, куда хотим поместить центр bounds
        //Vector3 targetCenterPos = mainCamera.transform.position + mainCamera.transform.forward * desiredDistance;
        Transform cam = mainCamera.transform;

        Vector3 targetCenterPos =
            cam.position +
            cam.forward * desiredDistance +
            cam.right * obj.focusScreenOffset.x * desiredDistance +
            cam.up * obj.focusScreenOffset.y * desiredDistance;


        //Debug.DrawLine(mainCamera.transform.position, boundsCenterWorld, Color.red, 2f);

        //Debug.DrawLine(boundsCenterWorld, boundsCenterWorld + Vector3.up * 0.2f, Color.red, 2f);

        //Debug.DrawLine(obj.transform.position, obj.transform.position + Vector3.up * 0.2f, Color.green, 2f);

        //Debug.DrawLine(mainCamera.transform.position, targetCenterPos, Color.blue, 2f);


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
            lookRot.eulerAngles.y + originalEuler.y,  // только поворот вокруг Y
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
            if (eulerBackAfterFocus) currentFocus.transform.rotation = originalRotation;
            if(eulerBackAfterFocus) currentFocus.transform.eulerAngles = originalEuler;
            currentFocus.transform.parent = originalParent;
            currentFocus.SetHighlighted(false);
            currentFocus = null;
        }

        // Разблокируем камеру
        if (cameraController != null)
            cameraController.SetLock(false);
        CloseUI();
    }


    public bool IsFocused => isFocused;
}

