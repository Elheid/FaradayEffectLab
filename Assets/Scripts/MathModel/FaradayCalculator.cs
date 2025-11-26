using UnityEngine;

public static class FaradayCalculator
{
    // 1) Магнитное поле катушки
    public static float ComputeH0(FaradayParams p, float currentA)
    {
        return (p.turns * currentA) / (Mathf.Sqrt((p.coilLength * p.coilLength) + (p.coilD * p.coilD)));
    }

    // 2) Кривая намагничивания (простая линейка + насыщение)
    public static float ComputeM0(FaradayParams p, float H0)
    {
        if (H0 >= p.Hsat) 
        {
            return p.Msat;
        }
        
        return p.Msat * (H0 / p.Hsat);
    }

    // 3) Частоты f0 и fm (в МГц, если всё остальное тоже в МГц)
    public static void ComputeFrequencies(FaradayParams p, float H0, out float f0, out float fm)
    {
        float M0 = ComputeM0(p, H0);
        f0 = p.gammaMHzPerOe * H0;
        fm = p.gammaMHzPerOe * M0;
    }

    // 4) Магнитные проницаемости для правой и левой круговых волн
    public static void ComputeMuPlusMinus(FaradayParams p, float H0,
                                          out float muPlus, out float muMinus)
    {
        float f = p.frequencyMHz;
        ComputeFrequencies(p, H0, out float f0, out float fm);

        // формулы (4.7) и (4.9)
        muPlus  = 1f - fm / (f0 - f);
        muMinus = 1f - fm / (f0 + f);
    }

    // 5) Волновые числа k+ и k- и угол поворота θ
    public static float ComputeThetaRad(FaradayParams p, float H0)
    {
        ComputeMuPlusMinus(p, H0, out float muPlus, out float muMinus);

        float fHz = p.frequencyMHz * 1e6f;
        float omega = 2f * Mathf.PI * fHz;
        float c = 3e8f;

        float kPlus  = omega / c * Mathf.Sqrt(p.epsilonR * muPlus);
        float kMinus = omega / c * Mathf.Sqrt(p.epsilonR * muMinus);

        // θ = 0.5 * (k+ - k-) * l
        float theta = 0.5f * (kPlus - kMinus) * p.rodLength; // рад
        return theta;
    }

    // 6) Сигнал усилителя при угле рупора φ (в градусах)
    public static float ComputeSignal(float thetaRad, float hornAngleDeg, float u0 = 1f, float A_x = 1f, float A_y = 1f, float delta = 0f)
    {
        float phiRad = hornAngleDeg * Mathf.Deg2Rad;

        // Расчёт сигнала с учётом эллиптической поляризации
        // U = A_x**2 * cos**2(φ − θ) + A_y**2 * sin**2(φ − θ + δ)
        float signal = A_x * A_x * Mathf.Cos(phiRad - thetaRad) * Mathf.Cos(phiRad - thetaRad) +
                       A_y * A_y * Mathf.Sin(phiRad - thetaRad + delta) * Mathf.Sin(phiRad - thetaRad + delta);

        return u0 * signal;
    }
}
