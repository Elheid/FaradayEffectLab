using System;

[System.Serializable]
public class FaradayParams
{
    // Волна / частоты
    public float frequencyMHz = 9000f; // частота (в МГц)
    public float epsilonR = 10f;       // относительная диэлектрическая проницаемость феррита

    // Феррит
    public float gammaMHzPerOe = 2.8f; // гиромагнитное отношение в МГц/Оэ
    public float Msat;                 // намагниченность насыщения (А/м)
    public float Hsat;                 // поле насыщения (А/м)
    public float rodLength = 0.05f;    // длина стержня феррита (м)

    // Катушка
    public int turns = 1500;            // количество витков
    public float coilLength = 0.14f;    // длина катушки (м)
    public float coilD = 0.03f;         // диаметр катушки (м)  
}
