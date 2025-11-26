using System;

[System.Serializable]
public class FaradayParamsAlt
{
    // Поляризационная характеристика
    public float thetaInitialDeg = 120f; // θ_нач - начальный угол (110°...130°)
    
    // Генератор
    public float attenuationDb = 40f;    // n_г - ослабление
    public int generatorSwitch = 1;      // p_г - выключатель (0/1)
    
    // Приемник
    public float receiverGain = 1f;      // n_п - усиление
    public int receiverSwitch = 1;       // p_п - выключатель (0/1)
    public int multiplication = 1;       // n_x - умножение (1, 10, 100)
    
    // Установка нуля
    public int zeroSettingSwitch = 0;    // p_ун - переключатель (0/1)
    public float zeroSettingLevel = 0.1f; // n_ун - уровень (0.04...0.15)
}
