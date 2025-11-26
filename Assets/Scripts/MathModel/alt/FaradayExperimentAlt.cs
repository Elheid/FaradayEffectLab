using UnityEngine;
using UnityEngine.UI;

public class FaradayExperimentAlt : MonoBehaviour
{
    public FaradayParamsAlt parameters;  // параметры для расчётов
    public Slider currentSlider;      // ползунок для регулировки тока

    [Range(0f, 360f)]
    public float hornAngleDeg = 0f;  // угол поворота рупора

    public float betaAngle;      // угол наклона поляризационной характеристики
    public float signalLevel;    // показания усилителя

    void Update()
    {
        float currentA = currentSlider.value; // берём значение тока с ползунка
        
        // 1) Вычисляем угол β
        betaAngle = FaradayCalculatorAlt.ComputeBeta(currentA, parameters.thetaInitialDeg);
        
        // 2) Вычисляем сигнал на приемнике
        signalLevel = FaradayCalculatorAlt.ComputeSignal(
            hornAngleDeg, betaAngle,
            parameters.attenuationDb, parameters.receiverGain,
            parameters.zeroSettingSwitch, parameters.zeroSettingLevel,
            parameters.generatorSwitch, parameters.receiverSwitch,
            parameters.multiplication
        );

        // Выводим для отладки
        Debug.Log($"Beta: {betaAngle}, Signal: {signalLevel}");
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
            values[i] = FaradayCalculatorAlt.ComputeSignal(
                angle, beta, parameters.attenuationDb, parameters.receiverGain,
                parameters.zeroSettingSwitch, parameters.zeroSettingLevel,
                parameters.generatorSwitch, parameters.receiverSwitch,
                parameters.multiplication
            );
        }
        return values;
    }
}
