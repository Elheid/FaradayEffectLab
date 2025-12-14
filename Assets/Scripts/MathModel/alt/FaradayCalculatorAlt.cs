using UnityEngine;

public static class FaradayCalculatorAlt
{
    // 1) Сигнал усилителя при угле рупора φ (альтернативная формула)
    public static float ComputeSignal(float hornAngleDeg, float betaDeg, FaradayParamsAlt p)
    {
        if (p.generatorSwitch == 0 || p.receiverSwitch == 0)
        {
            return 0f;
        }

        // U_г = 10^((40 - n_г)/20)
        float Ug = Mathf.Pow(10f, (40f - p.attenuationDb) / 20f);
    
        // Основная формула
        float deltaThetaRad = (hornAngleDeg - betaDeg) * Mathf.Deg2Rad;
        float cosTerm = Mathf.Abs(Mathf.Cos(deltaThetaRad));

        float forwardFactor = 1f;

        // При обратном включении: множитель 10^(-ΔA/20)
        if (p.reverseMode == 1)
        {
            forwardFactor = Mathf.Pow(10f, -p.deltaADb / 20f);
        }
    
         float Us = Ug * p.receiverGain * cosTerm * forwardFactor *
         (p.generatorSwitch / (float)p.multiplication);

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
