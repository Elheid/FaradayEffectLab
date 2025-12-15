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
        generatorCanvas.enabled = false;
        hornCanvas.enabled = false;
        powerUnitCanvas.enabled = false;
        amplifierCanvas.enabled = false;
    }

    public void ShowGenerator()
    {
        HideAll();
        generatorCanvas.enabled = true;
    }

    public void ShowHorn()
    {
        HideAll();
        hornCanvas.enabled = true;
    }

    public void ShowPowerUnit()
    {
        HideAll();
        powerUnitCanvas.enabled = true;
    }

    public void ShowAmplifier()
    {
        HideAll();
        amplifierCanvas.enabled = true;
    }
}
