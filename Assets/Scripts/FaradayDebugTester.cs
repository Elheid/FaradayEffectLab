using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class FaradayDebugTester : MonoBehaviour
{
    public FaradayExperimentAlt experiment;
    public Slider hornSlider;           // ползунок для угла рупора


    private InputSystem_Actions actions;

    public TextMeshProUGUI debugText;
    public TextMeshProUGUI currentText;
    public TextMeshProUGUI hornText;
    public TextMeshProUGUI amperText;
    public TextMeshProUGUI generatorText;

    public Canvas debugCanvas;

    private float deltaCurrent = 0.02f;
    private float deltaAngle = 5f;
    private bool canvasVisible = true;

    void Awake()
    {
        actions = new InputSystem_Actions();

        // ↑ ток
        actions.Faraday.IncreaseCurrent.performed += _ =>
        {
            experiment.currentSlider.value =
                Mathf.Clamp01(experiment.currentSlider.value + deltaCurrent);
        };

        // ↓ ток
        actions.Faraday.DecreaseCurrent.performed += _ =>
        {
            experiment.currentSlider.value =
                Mathf.Clamp01(experiment.currentSlider.value - deltaCurrent);
        };

        // → рупор
        actions.Faraday.RotateRight.performed += _ =>
        {
            experiment.hornAngleDeg =
                (experiment.hornAngleDeg + deltaAngle) % 360f;
            hornSlider.value = experiment.hornAngleDeg;
            experiment.hornAngleDeg = hornSlider.value;

        };

        // ← рупор
        actions.Faraday.RotateLeft.performed += _ =>
        {
            experiment.hornAngleDeg =
                (experiment.hornAngleDeg - deltaAngle + 360f) % 360f;
            hornSlider.value = experiment.hornAngleDeg;
            experiment.hornAngleDeg = hornSlider.value;
        };

        // R — сброс
        actions.Faraday.Reset.performed += _ =>
        {
            experiment.currentSlider.value = 0f;
            experiment.hornAngleDeg = 0f;
            hornSlider.value = experiment.hornAngleDeg;
        };

        // UI on/off
        actions.Faraday.ToggleUI.performed += _ =>
        {
            canvasVisible = !canvasVisible;
            debugCanvas.enabled = canvasVisible;
        };
    }

    void OnEnable() => actions.Faraday.Enable();
    void OnDisable() => actions.Faraday.Disable();

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (experiment == null) return;

        float currentA = experiment.currentSlider.value;
        float horn = experiment.hornAngleDeg;
        float beta = experiment.betaAngle;
        float signal = experiment.signalLevel;

        // Главный текст
        debugText.text =
            $"=== FARADAY LAB DEBUG ===\n" +
            $"Ток: {currentA:F3} A\n" +
            $"Рупор φ: {horn:F1}°\n" +
            $"β: {beta:F2}°\n" +
            $"Сигнал U: {signal:F3}";

        // Отдельные поля
        currentText.text = $"Ток: {currentA:F3} A";
        hornText.text = $"Рупор: {horn:F1}°";
        amperText.text = $"{currentA:F3} A";
        generatorText.text = $"9150 MHz";
    }
}

/*

Вкл/Выкл генератора Toggle или Button parameters.generatorSwitch
Вкл/Выкл усилителя Toggle или Button parameters.receiverSwitch
Сделано - Ток усилителя if (amperText != null) amperText.text = $"{testCurrent:F3} A";
Ослабление на генераторе (дБ) Slider (0–60) parameters.attenuationDb
Диапазон на усилителе (×1 / ×10) Toggle или Dropdown parameters.multiplication
Кнопка "Уст. 0" на усилителе Button (однократное нажатие) вызывает parameters.zeroSettingSwitch = 1 (временно)
Вроде не нужно?? - Режим невзаимности  на генераторе? Button или Toggle parameters.reverseMode
Показания на шкале усилителя Text signal * parameters.multiplication
*/

