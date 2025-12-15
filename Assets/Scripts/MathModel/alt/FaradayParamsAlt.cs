using System;

[System.Serializable]
public class FaradayParamsAlt
{
    // Поляризационная характеристика
    public float thetaInitialDeg = 120f; // θ_нач - начальный угол (110°...130°)
    
    // Генератор
    public float attenuationDb = 40f;    // n_г - ослабление
    public int generatorSwitch = 0;      // p_г - выключатель (0/1)
    
    // Приемник
    public float receiverGain = 1f;      // n_п - усиление
    public int receiverSwitch = 0;       // p_п - выключатель (0/1)
    public int multiplication = 1;       // n_x - умножение (1, 10, 100)

    // Новые параметры
    public int reverseMode = 0; // 0 = прямое, 1 = обратное
    public float deltaADb = 15f; // разница ослаблений (10…20 дБ)

    public void SetThetaInitial(float value)
    {
        thetaInitialDeg = Clamp(value, 110f, 130f);
    }

    public void SetAttenuationDb(float value)
    {
        attenuationDb = Clamp(value, 0f, 60f);
    }

    public void SetGeneratorSwitch(int value)
    {
        generatorSwitch = value == 0 ? 0 : 1;
    }

    public void SetReceiverGain(float value)
    {
        receiverGain = Math.Max(0f, value);
    }

    public void SetReceiverSwitch(int value)
    {
        receiverSwitch = value == 0 ? 0 : 1;
    }

    public void SetMultiplication(int value)
    {
        if (value == 1 || value == 10 || value == 100)
            multiplication = value;
        else
            multiplication = 1;
    }

    public void SetReverseMode(int value)
    {
        reverseMode = value == 0 ? 0 : 1;
    }

    public void SetDeltaADb(float value)
    {
        deltaADb = Clamp(value, 10f, 20f);
    }

    private float Clamp(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}
