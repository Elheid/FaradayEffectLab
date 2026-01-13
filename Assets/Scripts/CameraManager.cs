using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    [SerializeField] private GameObject mainCamera;      // Камера #1
    [SerializeField] private GameObject tutorialCamera;  // Камера #2


    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
    }
    void OnDestroy()
    {
        if (inputActions != null)
            inputActions.Player.Disable();
    }

    void Start()
    {
        // При старте включаем только туториал-камеру
        tutorialCamera.SetActive(true);
        mainCamera.SetActive(false);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape)) Debug.Log($"ESC 1");
        if (inputActions.Player.Cancel.WasPressedThisFrame())//if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log($"ESC 2");
            SwitchToMainCamera();
        }
    }

    public void SwitchToMainCamera()
    {
        tutorialCamera.SetActive(false);
        mainCamera.SetActive(true);
    }

    public void SwitchToTutorialCamera()
    {
        tutorialCamera.SetActive(true);
        mainCamera.SetActive(false);
    }

}