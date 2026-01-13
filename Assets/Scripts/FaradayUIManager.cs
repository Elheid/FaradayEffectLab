using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FaradayUIManager : MonoBehaviour
{
    [Header("Ссылка на эксперимент")]
    public FaradayExperimentAlt experiment;

    [Header("Блок питания электромагнита")]
    public Slider currentSlider;
    public TextMeshProUGUI currentText;
    public TextMeshProUGUI amperText;

    [Header("Генератор СВЧ")]
    public Slider generatorAttenuationSlider;
    public Toggle generatorSwitchToggle;
    public TextMeshProUGUI generatorText;
    public TextMeshProUGUI attenuationText;
    public TextMeshProUGUI generatorDeviceText;
    public TextMeshProUGUI attenuationDbText;

    [Header("Измерительный усилитель")]
    public Slider receiverGainSlider;
    public TMP_Dropdown multiplicationDropdown;
    public Toggle receiverSwitchToggle;
    public Button setZeroButton;
    public TextMeshProUGUI receiverText;
    public TextMeshProUGUI receiverGainText;
    public TextMeshProUGUI multiplicationText;
    public TextMeshProUGUI amplifierText;
    public TextMeshProUGUI amplifierResultText;

    [Header("Геометрия и тракт")]
    public TextMeshProUGUI hornText;
    public Slider hornSlider;
    public Toggle reverseModeToggle;
    public TextMeshProUGUI reverseModeText;

    [Header("Рычаги и переключатели")]
    public RotaryKnob amplifierCircularAmplification;
    public RotaryKnob generatorCircularAttenuation;
    public RotaryKnob powerCircularCurrent;
    public DiscretKnob amplifierCircularMulti;
    public SwitchLever generatorSwitchLever;
    public SwitchLever receiverSwitchLever;
    public HornRotator horn;

    [Header("Лампы индикации")]
    public Renderer amplifierOnLamp;
    public Renderer powerOnLamp;

    public Material lampOffMaterial;   // дефолтный
    public Material lampOnMaterial;    // BrightRed или любой


    void Start()
    {
        if (experiment != null)
            experiment.OnSignalChanged += RefreshTexts;

        InitUIFromExperiment();
        SubscribeUIEvents();
        RefreshTexts();

        SubscribeLeverEvents(); //

    }

    // ---------- ИНИЦИАЛИЗАЦИЯ ----------
    void InitUIFromExperiment()
    {
        currentSlider.SetValueWithoutNotify(experiment.currentSlider.value);
        generatorAttenuationSlider.SetValueWithoutNotify(experiment.parameters.attenuationDb);
        receiverGainSlider.SetValueWithoutNotify(experiment.parameters.receiverGain);
        hornSlider.SetValueWithoutNotify(experiment.hornAngleDeg);

        multiplicationDropdown.SetValueWithoutNotify(GetMultiplicationIndex());
        reverseModeToggle.SetIsOnWithoutNotify(experiment.parameters.reverseMode == 1);
        generatorSwitchToggle.SetIsOnWithoutNotify(experiment.parameters.generatorSwitch == 1);
        receiverSwitchToggle.SetIsOnWithoutNotify(experiment.parameters.receiverSwitch == 1);

        SetLamp(powerOnLamp, true);
        SetLamp(amplifierOnLamp, experiment.parameters.receiverSwitch == 1);

    }

    // ---------- ПОДПИСКА ----------
    void SubscribeUIEvents()
    {
        currentSlider.onValueChanged.AddListener(value =>
        {
            experiment.currentSlider.value = value;
            powerCircularCurrent.SetValue(value);
            RefreshTexts();
        });

        generatorAttenuationSlider.onValueChanged.AddListener(value =>
        {
            experiment.parameters.SetAttenuationDb(value);
            generatorCircularAttenuation.SetValue(value);
            RefreshTexts();
        });

        generatorSwitchToggle.onValueChanged.AddListener(isOn =>
        {
            experiment.parameters.SetGeneratorSwitch(isOn ? 1 : 0);
            generatorSwitchLever.SetState(isOn);
            //SetLamp(powerOnLamp, isOn);
            RefreshTexts();
        });

        receiverSwitchToggle.onValueChanged.AddListener(isOn =>
        {
            experiment.parameters.SetReceiverSwitch(isOn ? 1 : 0);
            receiverSwitchLever.SetState(isOn);
            SetLamp(amplifierOnLamp, isOn);
            RefreshTexts();
        });
        receiverGainSlider.onValueChanged.AddListener(value =>
        {
            experiment.parameters.SetReceiverGain(value);
            amplifierCircularAmplification.SetValue(value);
            RefreshTexts();
        });



        multiplicationDropdown.onValueChanged.AddListener(index =>
        {
            int value = GetMultiplicationValue(index);
            experiment.parameters.SetMultiplication(value);

            amplifierCircularMulti.SetIndex(index);
            RefreshTexts();
        });

        hornSlider.onValueChanged.AddListener(value =>
        {
            experiment.hornAngleDeg = value;
            horn.SetAngle(value);
            RefreshTexts();
        });

        reverseModeToggle.onValueChanged.AddListener(isOn =>
        {
            experiment.parameters.SetReverseMode(isOn ? 1 : 0);
            RefreshTexts();
        });

        setZeroButton.onClick.AddListener(() =>
        {
            experiment.SetZero();
            RefreshTexts();
        });
    }

    // ---------- ОБНОВЛЕНИЕ ТОЛЬКО ТЕКСТА ----------
    void RefreshTexts()
    {
        if (experiment == null) return;

        amperText.text = $"{experiment.currentSlider.value:F3} A";
        currentText.text = "Ток";

        generatorText.text = $"{(experiment.parameters.generatorSwitch == 1 ? "ВКЛ" : "ВЫКЛ")}";//Генератор: 
        attenuationText.text = "Ослабление";
        generatorDeviceText.text = "9150 MHz";
        attenuationDbText.text = $"{experiment.parameters.attenuationDb:F4}";

        amplifierText.text = $"{experiment.parameters.receiverGain * 100:F2}";//
        amplifierResultText.text = experiment.signalLevel.ToString("F4");//.ToString();//.ToString("F3");

        receiverText.text = $"{(experiment.parameters.receiverSwitch == 1 ? "ВКЛ" : "ВЫКЛ")}";//Усилитель:
        receiverGainText.text = "Усиление";
        multiplicationText.text = $"Множитель";//:{experiment.parameters.multiplication} ";

        hornText.text = $"{experiment.hornAngleDeg:F1}°";
        reverseModeText.text =
            $"{(experiment.parameters.reverseMode == 0 ? "Прямой" : "Обратный")}";//Режим:
    }

    // ---------- ВСПОМОГАТЕЛЬНОЕ ----------

    void SubscribeLeverEvents()
    {
        // Генератор
        generatorSwitchLever.OnLeverSwitched += isOn =>
        {
            experiment.parameters.SetGeneratorSwitch(isOn ? 1 : 0);
            generatorSwitchToggle.SetIsOnWithoutNotify(isOn);
            //SetLamp(powerOnLamp, isOn);

            RefreshTexts();
        };

        // Усилитель
        receiverSwitchLever.OnLeverSwitched += isOn =>
        {
            experiment.parameters.SetReceiverSwitch(isOn ? 1 : 0);

            receiverSwitchToggle.SetIsOnWithoutNotify(isOn);
            SetLamp(amplifierOnLamp, isOn);

            RefreshTexts();
        };
    }


    void SetLamp(Renderer lamp, bool isOn)
    {
        if (lamp == null) return;
        lamp.material = isOn ? lampOnMaterial : lampOffMaterial;
    }


    int GetMultiplicationIndex()
    {
        return experiment.parameters.multiplication switch
        {
            1 => 0,
            10 => 1,
            100 => 2,
            1000 => 3,
            _ => 0
        };
    }

    int GetMultiplicationValue(int index)
    {
        return index switch
        {
            0 => 1,
            1 => 10,
            2 => 100,
            3 => 1000,
            _ => 1
        };
    }
}


//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;


//public class FaradayUIManager : MonoBehaviour
//{
//    [Header("Ссылка на эксперимент")]
//    public FaradayExperimentAlt experiment;

//    [Header("Блок питания электромагнита")]
//    public Slider currentSlider;            // Ток подмагничивания
//    public TextMeshProUGUI currentText;    // Отображение тока
//    public TextMeshProUGUI amperText;

//    [Header("Генератор СВЧ")]
//    public Slider generatorAttenuationSlider; // Ослабление генератора
//    public Toggle generatorSwitchToggle;       // ВКЛ/ВЫКЛ генератора
//    public TextMeshProUGUI generatorText;     // Текстовое отображение состояния
//    public TextMeshProUGUI attenuationText;
//    public TextMeshProUGUI generatorDeviceText;
//    public TextMeshProUGUI attenuationDbText;

//    [Header("Измерительный усилитель")]
//    public Slider receiverGainSlider;      // Усиление приемника
//    public TMP_Dropdown multiplicationDropdown; // Множитель (1,10,100)
//    public Toggle receiverSwitchToggle;    // ВКЛ/ВЫКЛ усилителя
//    public Button setZeroButton;           // Кнопка "Уст. 0"
//    public TextMeshProUGUI receiverText;   // Текстовое отображение состояния
//    public TextMeshProUGUI receiverGainText;
//    public TextMeshProUGUI multiplicationText;
//    public TextMeshProUGUI amplifierText;
//    public TextMeshProUGUI amplifierResultText;//signal

//    [Header("Геометрия и тракт")]
//    public TextMeshProUGUI hornText;       // Угол рупора
//    public Slider hornSlider;
//    public Toggle reverseModeToggle;       // Режим тракта (Прямой/Обратный)
//    public TextMeshProUGUI reverseModeText;// Текстовое отображение режима


//    //Переключатели
//    [Header("Рычаги и переключатели")]
//    public RotaryKnob amplifierCircularAmplification;
//    public RotaryKnob generatorCircularAttenuation;
//    public RotaryKnob powerCircularCurrent;

//    public DiscretKnob amplifierCircularMulti;


//    public SwitchLever generatorSwitchLever;
//    public SwitchLever receiverSwitchLever;

//    public HornRotator horn;


//    //public SwitchLever setZeroSwitchLever;


//    void Start()
//    {

//        // При изменении множителя  multiplicationDropdown
//        //Вида прямого и обратного reverseModeToggle
//        //Нужно обновлять ui и amplifierResultText от нового signalLevel

//        // --- Инициализация слайдеров и переключателей ---

//        // Ток подмагничивания
//        currentSlider.onValueChanged.AddListener(value =>
//        {
//            experiment.currentSlider.value = value;
//            powerCircularCurrent.SetValue(value);
//            RefreshUI();
//        });

//        // Генератор
//        generatorAttenuationSlider.onValueChanged.AddListener(value =>
//        {
//            experiment.parameters.SetAttenuationDb(value);
//            generatorCircularAttenuation.SetValue(value);
//            RefreshUI();
//        });
//        generatorSwitchToggle.onValueChanged.AddListener(isOn =>
//        {
//            experiment.parameters.SetGeneratorSwitch(isOn ? 1 : 0);
//            generatorSwitchLever.SetState(isOn);
//            RefreshUI();
//        });

//        // Усилитель
//        receiverGainSlider.onValueChanged.AddListener(value =>
//        {
//            experiment.parameters.SetReceiverGain(value);
//            amplifierCircularAmplification.SetValue(value);
//            RefreshUI();
//        });
//        receiverSwitchToggle.onValueChanged.AddListener(isOn =>
//        {
//            experiment.parameters.SetReceiverSwitch(isOn ? 1 : 0);
//            receiverSwitchLever.SetState(isOn);
//            RefreshUI();
//        });

//        multiplicationDropdown.onValueChanged.AddListener(index =>
//        {
//            int val = 1;
//            switch (index)
//            {
//                case 0: val = 1; break;
//                case 1: val = 10; break;
//                case 2: val = 100; break;
//                case 3: val = 1000; break;
//            }
//            experiment.parameters.SetMultiplication(val);
//            amplifierCircularMulti.SetIndex(index);
//            RefreshUI();
//        });

//        hornSlider.onValueChanged.AddListener(value =>
//        {
//            experiment.hornAngleDeg = value;
//            horn.SetAngle(value);
//            RefreshUI();
//        });


//        // Режим тракта
//        reverseModeToggle.onValueChanged.AddListener(isOn =>
//        {
//            experiment.parameters.SetReverseMode(isOn ? 1 : 0);
//            RefreshUI();
//        });

//        // Кнопка "Уст. 0"
//        setZeroButton.onClick.AddListener(() =>
//        {
//            experiment.SetZero();

//            RefreshUI();
//        });

//        // Первоначальная настройка UI
//        RefreshUI();
//    }

//    void RefreshUI()
//    {
//        if (experiment == null) return;

//        // --- Ток подмагничивания ---
//        currentText.text = $"Ток"; //подмагничивания: {experiment.currentSlider.value:F3} A";
//        currentSlider.value = experiment.currentSlider.value;
//        amperText.text = $"{experiment.currentSlider.value:F3} A";

//        // --- Генератор ---
//        generatorAttenuationSlider.value = experiment.parameters.attenuationDb;
//        generatorSwitchToggle.isOn = experiment.parameters.generatorSwitch == 1;
//        generatorText.text = $"Генератор: {(experiment.parameters.generatorSwitch == 1 ? "ВКЛ" : "ВЫКЛ")}";

//        //float value = experiment.parameters.attenuationDb;
//        //float rounded = Mathf.Round(value * 1000f) / 1000f;

//        attenuationText.text = $"Ослабление";//: {rounded} –dB\n";
//            //Текст на приборе
//        generatorDeviceText.text = $"9150 MHz"; // фиксированная частота

//        attenuationDbText.text = $"{experiment.parameters.attenuationDb:F4}";//-dB



//        // --- Усилитель ---
//        receiverGainSlider.value = experiment.parameters.receiverGain;
//        receiverSwitchToggle.isOn = experiment.parameters.receiverSwitch == 1;
//        //Текст на приборе
//        amplifierText.text = $"{experiment.parameters.receiverGain * 100:F2}";

//        amplifierResultText.text = $"{experiment.signalLevel}";


//        // Множитель

//        receiverText.text = $"Усилитель: {(experiment.parameters.receiverSwitch == 1 ? "ВКЛ" : "ВЫКЛ")}";
//        receiverGainText.text = $"Усиление";//: {experiment.parameters.receiverGain * 100 :F2}\n";
//        multiplicationText.text = $"Множитель";//: ×{experiment.parameters.multiplication}\n";



//        // --- Угол рупора ---
//        hornText.text = $"Угол рупора: {experiment.hornAngleDeg:F1}°";

//        // --- Режим тракта ---
//        reverseModeToggle.isOn = experiment.parameters.reverseMode == 1;
//        reverseModeText.text = $"Режим: {(experiment.parameters.reverseMode == 0 ? "Прямой" : "Обратный")}";
//    }
//}
