using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceUIManager : MonoBehaviour
{
    public static DeviceUIManager Instance;

    [Header("Canvas устройств")]
    public Canvas generatorCanvas;
    public Canvas hornCanvas;
    public Canvas powerUnitCanvas;
    public Canvas amplifierCanvas;

    [Header("UI panel")]
    public GameObject panel;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        HideAll();
        panel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HideAll();
        }
    }

    public void HideAll()
    {
        generatorCanvas.gameObject.SetActive(false);
        powerUnitCanvas.gameObject.SetActive(false);
        amplifierCanvas.gameObject.SetActive(false);
        hornCanvas.gameObject.SetActive(false);
        panel.SetActive(false);
    }
    public void ShowPanel() {
        panel.SetActive(true);
    }

    public void ShowUI(InteractableObject obj) {
      
        switch (obj.interactableType)
        {
            case InteractableType.PowerUnit:
                ShowPowerUnit();
                break;

            case InteractableType.Generator:
                ShowGenerator();
                break;

            case InteractableType.Amplifier:
                ShowAmplifier();
                break;

            case InteractableType.Horn:
                ShowHorn();
                break;

            case InteractableType.None:
            default:
                HideAll();
                break;
        }
        ShowPanel();
    }

    public void ShowGenerator()
    {
        HideAll();
        generatorCanvas.gameObject.SetActive(true);
    }

    public void ShowHorn()
    {
        HideAll();
        hornCanvas.gameObject.SetActive(true);
    }

    public void ShowPowerUnit()
    {
        HideAll();
        powerUnitCanvas.gameObject.SetActive(true);
    }

    public void ShowAmplifier()
    {
        HideAll();
        amplifierCanvas.gameObject.SetActive(true);
    }
}
