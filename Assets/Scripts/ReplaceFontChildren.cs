using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CanvasFontChanger : MonoBehaviour
{
    public TMP_FontAsset newFont;

#if UNITY_EDITOR
    [CustomEditor(typeof(CanvasFontChanger))]
    public class CanvasFontChangerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            CanvasFontChanger changer = (CanvasFontChanger)target;
            
            if (GUILayout.Button("Replace Font in All Children"))
            {
                if (changer.newFont == null)
                {
                    Debug.LogError("Assign a font first!");
                    return;
                }
                
                TMP_Text[] texts = changer.GetComponentsInChildren<TMP_Text>(true);
                int count = 0;
                
                foreach (TMP_Text text in texts)
                {
                    text.font = changer.newFont;
                    EditorUtility.SetDirty(text);
                    count++;
                }
                
                Debug.Log($"Replaced {count} fonts");
            }
        }
    }
#endif

    void Start()
    {
        // Автоматическая замена при старте
        if (newFont != null)
        {
            ReplaceFonts();
        }
    }

    public void ReplaceFonts()
    {
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text text in texts)
        {
            text.font = newFont;
        }
    }
}