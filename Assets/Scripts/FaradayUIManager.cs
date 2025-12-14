using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class FaradayUIManager : MonoBehaviour
{
    [Header("Ссылка на эксперимент")]
    public FaradayExperimentAlt experiment;

    [Header("Блок питания электромагнита")]
    public Slider currentSlider;            // Ток подмагничивания
    public TextMeshProUGUI currentText;    // Отображение тока
    public TextMeshProUGUI amperText;

    [Header("Генератор СВЧ")]
    public Slider generatorAttenuationSlider; // Ослабление генератора
    public Toggle generatorSwitchToggle;       // ВКЛ/ВЫКЛ генератора
    public TextMeshProUGUI generatorText;     // Текстовое отображение состояния
    public TextMeshProUGUI attenuationText;
    public TextMeshProUGUI generatorDeviceText;
    public TextMeshProUGUI attenuationDbText;

    [Header("Измерительный усилитель")]
    public Slider receiverGainSlider;      // Усиление приемника
    public TMP_Dropdown multiplicationDropdown; // Множитель (1,10,100)
    public Toggle receiverSwitchToggle;    // ВКЛ/ВЫКЛ усилителя
    public Button setZeroButton;           // Кнопка "Уст. 0"
    public TextMeshProUGUI receiverText;   // Текстовое отображение состояния
    public TextMeshProUGUI receiverGainText;
    public TextMeshProUGUI multiplicationText;
    public TextMeshProUGUI amplifierText;


    [Header("Геометрия и тракт")]
    public TextMeshProUGUI hornText;       // Угол рупора
    public Slider hornSlider;
    public Toggle reverseModeToggle;       // Режим тракта (Прямой/Обратный)
    public TextMeshProUGUI reverseModeText;// Текстовое отображение режима

    void Start()
    {
        // --- Инициализация слайдеров и переключателей ---

        // Ток подмагничивания
        currentSlider.onValueChanged.AddListener(value =>
        {
            experiment.currentSlider.value = value;
            RefreshUI();
        });

        // Генератор
        generatorAttenuationSlider.onValueChanged.AddListener(value =>
        {
            experiment.parameters.SetAttenuationDb(value);
            RefreshUI();
        });
        generatorSwitchToggle.onValueChanged.AddListener(isOn =>
        {
            experiment.parameters.SetGeneratorSwitch(isOn ? 1 : 0);
            RefreshUI();
        });

        // Усилитель
        receiverGainSlider.onValueChanged.AddListener(value =>
        {
            experiment.parameters.SetReceiverGain(value);
            RefreshUI();
        });
        receiverSwitchToggle.onValueChanged.AddListener(isOn =>
        {
            experiment.parameters.SetReceiverSwitch(isOn ? 1 : 0);
            RefreshUI();
        });

        // Множитель
        /*multiplicationDropdown.onValueChanged.AddListener(index =>
        {
            int val = 1;
            switch (index)
            {
                case 0: val = 1; break;
                case 1: val = 10; break;
                case 2: val = 100; break;
            }
            experiment.parameters.SetMultiplication(val);
            RefreshUI();
        });*/
        multiplicationDropdown.onValueChanged.AddListener(index =>
        {
            int val = 1;
            switch (index)
            {
                case 0: val = 1; break;
                case 1: val = 10; break;
                case 2: val = 100; break;
            }
            experiment.parameters.SetMultiplication(val);
            RefreshUI();
        });

        hornSlider.onValueChanged.AddListener(value =>
        {
            experiment.hornAngleDeg = value;
            RefreshUI();
        });


        // Режим тракта
        reverseModeToggle.onValueChanged.AddListener(isOn =>
        {
            experiment.parameters.SetReverseMode(isOn ? 1 : 0);
            RefreshUI();
        });

        // Кнопка "Уст. 0"
        setZeroButton.onClick.AddListener(() =>
        {
            experiment.SetZero();
            RefreshUI();
        });

        // Первоначальная настройка UI
        RefreshUI();
    }

    void RefreshUI()
    {
        if (experiment == null) return;

        // --- Ток подмагничивания ---
        currentText.text = $"Ток подмагничивания: {experiment.currentSlider.value:F3} A";
        currentSlider.value = experiment.currentSlider.value;
        amperText.text = $"{experiment.currentSlider.value:F3} A";

        // --- Генератор ---
        generatorAttenuationSlider.value = experiment.parameters.attenuationDb;
        generatorSwitchToggle.isOn = experiment.parameters.generatorSwitch == 1;
        generatorText.text = $"Генератор: {(experiment.parameters.generatorSwitch == 1 ? "ВКЛ" : "ВЫКЛ")}";
        attenuationText.text = $"Ослабление: {experiment.parameters.attenuationDb} дБ\n";

            //Текст на приборе
        generatorDeviceText.text = $"9150 MHz"; // фиксированная частота
        attenuationDbText.text = $"{Mathf.Round(experiment.parameters.attenuationDb)} дБ";
        


        // --- Усилитель ---
        receiverGainSlider.value = experiment.parameters.receiverGain;
        receiverSwitchToggle.isOn = experiment.parameters.receiverSwitch == 1;
            //Текст на приборе
        amplifierText.text = $"{experiment.parameters.receiverGain * 100:F2}";


        // Множитель
        int mult = experiment.parameters.multiplication;
        switch (mult)
        {
            case 1: multiplicationDropdown.value = 0; break;
            case 10: multiplicationDropdown.value = 1; break;
            case 100: multiplicationDropdown.value = 2; break;
        }
        receiverText.text = $"Усилитель: {(experiment.parameters.receiverSwitch == 1 ? "ВКЛ" : "ВЫКЛ")}";
        receiverGainText.text = $"Усиление: {experiment.parameters.receiverGain * 100 :F2}\n";
        multiplicationText.text = $"Множитель: ×{experiment.parameters.multiplication}\n";
                           
        

        // --- Угол рупора ---
        hornText.text = $"Угол рупора: {experiment.hornAngleDeg:F1}°";

        // --- Режим тракта ---
        reverseModeToggle.isOn = experiment.parameters.reverseMode == 1;
        reverseModeText.text = $"Режим тракта: {(experiment.parameters.reverseMode == 0 ? "Прямой" : "Обратный")}";
    }
}
