using UnityEngine;
using UnityEngine.UI;
using System;

public class FaradayExperimentAlt : MonoBehaviour
{
    [SerializeField]
    public FaradayParamsAlt parameters;  // параметры для расчётов
    public Slider currentSlider;      // ползунок для регулировки тока

    [Range(0f, 360f)]
    public float hornAngleDeg = 0f;  // угол поворота рупора

    public float betaAngle;      // угол наклона поляризационной характеристики
    public float signalLevel;    // показания усилителя

    private float zeroOffset = 0f; // сохранённый фон

    public event Action OnSignalChanged;


    private float lastInvokeTime = 0f;
private const float MIN_UPDATE_INTERVAL = 0.1f; // 10 раз в секунду

void Update()
{
    float currentA = currentSlider.value;
    
    betaAngle = FaradayCalculatorAlt.ComputeBeta(currentA, parameters.thetaInitialDeg);
    float rawSignal = FaradayCalculatorAlt.ComputeSignal(hornAngleDeg, betaAngle, parameters);

    if (rawSignal > 90_000f)
    {
        signalLevel = 0f;
    }
    else
    {
        rawSignal += GenerateNoise();
        signalLevel = Mathf.Max(0f, rawSignal - zeroOffset);
    }

    // Вызываем событие не чаще чем раз в MIN_UPDATE_INTERVAL секунд
    if (Time.time - lastInvokeTime > MIN_UPDATE_INTERVAL)
    {
        lastInvokeTime = Time.time;
        OnSignalChanged?.Invoke();
    }
}

    //void Update()
    //{
    //    float currentA = currentSlider.value; // берём значение тока с ползунка
        
    //    // 1) Вычисляем угол β
    //    betaAngle = FaradayCalculatorAlt.ComputeBeta(currentA, parameters.thetaInitialDeg);
        
    //    // 2) Вычисляем сигнал на приемнике
    //    float rawSignal = FaradayCalculatorAlt.ComputeSignal(hornAngleDeg, betaAngle, parameters);

    //    if (rawSignal > 90_000f)
    //    {
    //        signalLevel = 0f;
    //    }
    //    else
    //    {
    //        rawSignal += GenerateNoise();
    //        signalLevel = Mathf.Max(0f, rawSignal - zeroOffset);
    //    }

    //    OnSignalChanged?.Invoke(); // 🔥 событие уведомления


    //    // Выводим для отладки
    //    //Debug.Log($"Signal: {signalLevel}");
   // }

    // Генерация шума
    float GenerateNoise()
    {
        return UnityEngine.Random.Range(0.0002f, 0.001f) * parameters.receiverGain;
    }

    // Установка 0
    public void SetZero()
    {
        if (parameters.generatorSwitch == 0)
        {
            float background = FaradayCalculatorAlt.ComputeSignal(
                hornAngleDeg,
                betaAngle,
                parameters
            );

            background += GenerateNoise();
            zeroOffset = background;
        }
        else
        {
            Debug.LogWarning("Нельзя устанавливать ноль при включённом генераторе!");
        }
    }


    // Генерация поляризационной кривой для текущего тока
    public float[] GeneratePolarizationCurve(int points = 24)
    {
        float[] values = new float[points];
        float currentA = currentSlider.value;
        float beta = FaradayCalculatorAlt.ComputeBeta(currentA, parameters.thetaInitialDeg);
        
        for (int i = 0; i < points; i++)
        {
            float angle = i * (360f / points);
            float raw = FaradayCalculatorAlt.ComputeSignal(angle, beta, parameters);
            raw += GenerateNoise();
            values[i] = Mathf.Max(0f, raw - zeroOffset);
        }
        return values;
    }
}
