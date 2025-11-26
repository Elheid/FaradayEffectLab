using UnityEngine;

public static class FaradayCalculatorAlt
{
    // 1) Сигнал усилителя при угле рупора φ (альтернативная формула)
    public static float ComputeSignal(
        float hornAngleDeg,    // θ - угол поворота рупора
        float betaDeg,         // β - угол наклона поляризационной характеристики
        float attenuationDb,   // n_г - ослабление на генераторе (20...80 дБ)
        float receiverGain,    // n_п - усиление на приемнике (0...2)
        int zeroSettingSwitch, // p_ун - переключатель установки нуля (0 или 1)
        float zeroSettingLevel,// n_ун - уровень установки нуля (0...1)
        int generatorSwitch,   // p_г - выключатель генератора (0 или 1)
        int receiverSwitch,    // p_п - выключатель приемника (0 или 1)
        int multiplication     // n_x - умножение уровня (1, 10, 100)
    )
    {
        // U_г = 10^((40 - n_г)/20)
        float Ug = Mathf.Pow(10f, (40f - attenuationDb) / 20f);
    
        // Основная формула
        float deltaThetaRad = (hornAngleDeg - betaDeg) * Mathf.Deg2Rad;
        float cosTerm = Mathf.Abs(Mathf.Cos(deltaThetaRad));
    
        float Us = receiverSwitch * 
                  (Ug * receiverGain * cosTerm * (1f - zeroSettingSwitch) * 
                  (generatorSwitch / (float)multiplication) + zeroSettingLevel);
    
        Us = Mathf.Pow(Us, 2f) * 100f;
    
        return Us;
    }

    // 2) Угол наклона поляризационной характеристики
    public static float ComputeBeta(float currentA, float thetaInitialDeg)
    {
        // β = θ_нач + 100 * (I / (0.15 + I))^1.25
        float part = currentA / (0.15f + currentA);
        float angleDeg = thetaInitialDeg + 100f * Mathf.Pow(part, 1.25f);
        return angleDeg;
    }

}
