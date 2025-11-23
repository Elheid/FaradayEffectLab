using UnityEngine;
using UnityEngine.UI;

public class FaradayExperiment : MonoBehaviour
{
    public FaradayParams parameters;  // параметры для расчётов
    public Slider currentSlider;      // ползунок для регулировки тока

    [Range(0f, 360f)]
    public float hornAngleDeg = 0f;  // угол поворота рупора

    public float thetaDeg;      // угол поворота в градусах
    public float signalLevel;   // показания усилителя

    void Update()
    {
        float currentA = currentSlider.value; // берём значение тока с ползунка
        float H0 = FaradayCalculator.ComputeH0(parameters, currentA); // вычисляем магнитное поле

        // Вычисляем угол поворота в радианах и преобразуем в градусы
        float thetaRad = FaradayCalculator.ComputeThetaRad(parameters, H0);
        thetaDeg = thetaRad * Mathf.Rad2Deg;

        // Считаем сигнал усилителя
        signalLevel = FaradayCalculator.ComputeSignal(thetaRad, hornAngleDeg);

        // Выводим для отладки
        Debug.Log($"Signal Level: {signalLevel}");
        Debug.Log($"Theta Angle: {thetaDeg}");
    }

    // Генерация поляризационной кривой для текущего тока
    public float[] GeneratePolarizationCurve(int points = 24)
    {
        float[] values = new float[points];
        float currentA = currentSlider.value;
        float H0 = FaradayCalculator.ComputeH0(parameters, currentA);
        float thetaRad = FaradayCalculator.ComputeThetaRad(parameters, H0);

        for (int i = 0; i < points; i++)
        {
            float phi = i * (360f / points);
            values[i] = FaradayCalculator.ComputeSignal(thetaRad, phi);
        }

        return values;
    }
}
