using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class FaradayDebugTester : MonoBehaviour
{
    public FaradayParams parameters;

    [Range(0f, 1f)]
    public float testCurrent = 0f;

    [Range(0f, 360f)]
    public float hornAngle = 0f;

    private float deltaCurrent = 0.02f;
    private float deltaAngle = 5f;

    private InputSystem_Actions actions;

    public TextMeshProUGUI debugText;   // главный UI-текст
    public Slider currentSlider;        // ползунок для тока
    public Slider hornSlider;           // ползунок для угла рупора
    public TextMeshProUGUI currentText; // текст рядом с ползунком тока
    public TextMeshProUGUI hornText;    // текст рядом с ползунком угла
    public Canvas debugCanvas;          // Canvas для скрытия/показа

    private bool canvasVisible = true;

    void Awake()
    {
        actions = new InputSystem_Actions();

        // ======== КНОПКИ ========
        actions.Faraday.IncreaseCurrent.performed += _ =>
        {
            testCurrent = Mathf.Clamp01(testCurrent + deltaCurrent);
            if (currentSlider != null) currentSlider.value = testCurrent; // обновление только при кнопках
        };

        actions.Faraday.DecreaseCurrent.performed += _ =>
        {
            testCurrent = Mathf.Clamp01(testCurrent - deltaCurrent);
            if (currentSlider != null) currentSlider.value = testCurrent;
        };

        actions.Faraday.RotateRight.performed += _ =>
        {
            hornAngle = (hornAngle + deltaAngle) % 360f;
            if (hornSlider != null) hornSlider.value = hornAngle;
        };

        actions.Faraday.RotateLeft.performed += _ =>
        {
            hornAngle = (hornAngle - deltaAngle + 360f) % 360f;
            if (hornSlider != null) hornSlider.value = hornAngle;
        };

        actions.Faraday.Reset.performed += _ =>
        {
            testCurrent = 0f;
            hornAngle = 0f;
            if (currentSlider != null) currentSlider.value = testCurrent;
            if (hornSlider != null) hornSlider.value = hornAngle;
        };

        actions.Faraday.ToggleUI.performed += _ =>
        {
            canvasVisible = !canvasVisible;
            if (debugCanvas != null) debugCanvas.enabled = canvasVisible;
        };

        // ======== ПОДПИСКИ НА ПОЛЗУНКИ ========
        if (currentSlider != null)
            currentSlider.onValueChanged.AddListener(val => testCurrent = val); // мышь сама изменяет значение

        if (hornSlider != null)
            hornSlider.onValueChanged.AddListener(val => hornAngle = val);
    }

    void OnEnable() => actions.Faraday.Enable();
    void OnDisable() => actions.Faraday.Disable();

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        float H0 = FaradayCalculator.ComputeH0(parameters, testCurrent);
        float thetaRad = FaradayCalculator.ComputeThetaRad(parameters, H0);
        float thetaDeg = thetaRad * Mathf.Rad2Deg;

        float signal = FaradayCalculator.ComputeSignal(
            thetaRad,
            hornAngle,
            u0: 1f,
            A_x: 1f,
            A_y: 0.5f,
            delta: 0.1f
        );

        // Главный текст
        if (debugText != null)
        {
            debugText.text =
                $"=== FARADAY LAB DEBUG ===\n" +
                $"Ток: {testCurrent:F3} A\n" +
                $"H0: {H0:F3} A/m\n" +
                $"Угол поворота θ: {thetaDeg:F3}°\n" +
                $"Поворот рупора φ: {hornAngle:F1}°\n" +
                $"Сигнал U: {signal:F4}";
        }

        // Текущие значения возле ползунков
        if (currentText != null) currentText.text = $"Ток: {testCurrent:F3} A";
        if (hornText != null) hornText.text = $"Рупор: {hornAngle:F1}°";
    }
}
