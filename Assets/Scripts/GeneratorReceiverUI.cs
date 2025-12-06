//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class GeneratorReceiverUI : MonoBehaviour
//{
//    public FaradayExperiment experiment;

//    [Header("=== Генератор ===")]
//    public Slider attenuationSlider;
//    public TMP_Text attenuationText;
//    public Toggle generatorToggle;

//    [Header("=== Приемник ===")]
//    public Slider gainSlider;
//    public TMP_Text gainText;
//    public Toggle ampToggle;
//    public TMP_Dropdown multiplierDropdown;
//    public Toggle zeroSetToggle;
//    public Slider zeroSlider;
//    public TMP_Text zeroText;

//    void Start()
//    {
//        // Инициализация значений
//        attenuationSlider.minValue = 20f;
//        attenuationSlider.maxValue = 80f;
//        gainSlider.minValue = 0f;
//        gainSlider.maxValue = 2f;

//        zeroSlider.minValue = 0f;
//        zeroSlider.maxValue = 0.20f;

//        multiplierDropdown.ClearOptions();
//        multiplierDropdown.AddOptions(new System.Collections.Generic.List<string> { "X1", "X10", "X100" });
//    }

//    void Update()
//    {
//        // === Генератор ===
//        experiment.attenuationDb = attenuationSlider.value;
//        attenuationText.text = $"{experiment.attenuationDb:F1} dB";

//        experiment.generatorOn = generatorToggle.isOn;

//        // === Приемник ===
//        experiment.gain = gainSlider.value;
//        gainText.text = $"{experiment.gain:F2}";

//        experiment.ampOn = ampToggle.isOn;

//        switch (multiplierDropdown.value)
//        {
//            case 0: experiment.multiplier = 1; break;
//            case 1: experiment.multiplier = 10; break;
//            case 2: experiment.multiplier = 100; break;
//        }

//        experiment.zeroSetEnabled = zeroSetToggle.isOn;
//        experiment.zeroValue = zeroSlider.value;
//        zeroText.text = $"{experiment.zeroValue:F3}";
//    }
//}
