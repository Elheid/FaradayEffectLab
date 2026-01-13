using UnityEngine;

public class FullscreenController : MonoBehaviour
{
    void Start()
    {
        // Включить полный экран при старте
        Screen.fullScreen = true;
    }

    void Update()
    {
        // Переключение по клавише F11
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;
            Debug.Log("Fullscreen: " + Screen.fullScreen);
        }

        // Или по Escape для выхода
        if (Input.GetKeyDown(KeyCode.Escape) && Screen.fullScreen)
        {
            Screen.fullScreen = false;
        }
    }
}