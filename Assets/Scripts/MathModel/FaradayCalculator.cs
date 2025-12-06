using UnityEngine;

public static class FaradayCalculator
{
    // 1) Магнитное поле катушки
    public static float ComputeH0(FaradayParams p, float currentA)
    {
        return (p.turns * currentA) / (Mathf.Sqrt(p.coilLength * p.coilLength + p.coilD * p.coilD));
    }

    // 2) Кривая намагничивания (линейка + насыщение)
    public static float ComputeM0(FaradayParams p, float H0)
    {
        if (H0 >= p.Hsat) return p.Msat;
        return p.Msat * (H0 / p.Hsat);
    }

    // 3) Частоты f0 и fm в МГц (с учётом перевода A/m → Oe)
    public static void ComputeFrequencies(FaradayParams p, float H0, out float f0, out float fm)
    {
        float M0 = ComputeM0(p, H0);

        if (p.Msat == 0f) p.Msat = 1000f; // дефолтные тестовые значения
        if (p.Hsat == 0f) p.Hsat = 2000f;

        float H0_Oe = H0 / 79.577f;
        float M0_Oe = M0 / 79.577f;

        f0 = p.gammaMHzPerOe * H0_Oe;
        fm = p.gammaMHzPerOe * M0_Oe;

        // Защита от деления на ноль
        if (Mathf.Approximately(f0 - p.frequencyMHz, 0f)) f0 += 0.01f;
        if (Mathf.Approximately(f0 + p.frequencyMHz, 0f)) f0 += 0.01f;
    }

    // 4) Магнитные проницаемости μ⁺ и μ⁻
    public static void ComputeMuPlusMinus(FaradayParams p, float H0, out float muPlus, out float muMinus)
    {
        float f = p.frequencyMHz;
        ComputeFrequencies(p, H0, out float f0, out float fm);

        muPlus = 1f - fm / (f0 - f);
        muMinus = 1f - fm / (f0 + f);

        // Защита от отрицательного подкоренного
        muPlus = Mathf.Max(muPlus, 0.0001f);
        muMinus = Mathf.Max(muMinus, 0.0001f);
    }

    // 5) Волновые числа k⁺ и k⁻ и угол поворота θ (в радианах)
    public static float ComputeThetaRad(FaradayParams p, float H0)
    {
        ComputeMuPlusMinus(p, H0, out float muPlus, out float muMinus);

        float fHz = p.frequencyMHz * 1e6f;
        float omega = 2f * Mathf.PI * fHz;
        float c = 3e8f;

        float kPlus = omega / c * Mathf.Sqrt(p.epsilonR * muPlus);
        float kMinus = omega / c * Mathf.Sqrt(p.epsilonR * muMinus);

        float theta = 0.5f * (kPlus - kMinus) * p.rodLength; // рад
        return theta;
    }

    // 6) Сигнал усилителя при угле рупора φ (в градусах)
    public static float ComputeSignal(float thetaRad, float hornAngleDeg, float u0 = 1f, float A_x = 1f, float A_y = 1f, float delta = 0f)
    {
        float phiRad = hornAngleDeg * Mathf.Deg2Rad;

        float signal = A_x * A_x * Mathf.Cos(phiRad - thetaRad) * Mathf.Cos(phiRad - thetaRad) +
                       A_y * A_y * Mathf.Sin(phiRad - thetaRad + delta) * Mathf.Sin(phiRad - thetaRad + delta);

        return u0 * signal;
    }
}