using UnityEngine;
using System.Linq;

public class InteractableObject : MonoBehaviour
{
    [Header("Тип объекта")]
    public InteractableType interactableType = InteractableType.None;

    private Renderer[] renderers;
    private Material[][] originalMaterials; // сохраняем материалы каждого рендера

    [Header("Параметр приближений больше - дальше")]
    public float focusDistance = 0.009f;//0.009f;
    [Header("Focus Offset  Vector2(0.3f, 0f);  // правее Vector2(-0.3f, 0f); // левее")]
    public Vector2 focusScreenOffset = Vector2.zero;//new Vector2(0.05f, 0.02f); // смещение вправо или влево
    //focusScreenOffset = new Vector2(0.3f, 0f);  // правее
    //focusScreenOffset = new Vector2(-0.3f, 0f); // левее

   // private MaterialPropertyBlock propBlock;





    private void Awake()
    {
        // находим все рендереры в объекте и дочерних объектах
        //propBlock = new MaterialPropertyBlock();

        foreach (var outline in GetComponentsInChildren<Outline>())
            outline.enabled = false; // выключаем подсветку у всех



        renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("No Renderer found for InteractableObject on " + gameObject.name);
            return;
        }

        // сохраняем оригинальные материалы
        originalMaterials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].materials;
        }
    }

    public void BringToFront()
    {
        InteractionsManager.Instance.FocusOnObject(this);
    }

    public void SetHighlighted(bool highlighted)
    {
        foreach (var outline in GetComponentsInChildren<Outline>())
            outline.enabled = highlighted;
    }





    //public void SetHighlighted(bool highlighted)
    //{
    //    if (renderers == null || renderers.Length == 0) return;

    //    for (int i = 0; i < renderers.Length; i++)
    //    {
    //        if (highlighted)
    //        {
    //            // заменяем все материалы на outlineMaterial
    //            Material[] mats = new Material[renderers[i].materials.Length];
    //            for (int j = 0; j < mats.Length; j++)
    //                mats[j] = outlineMaterial;

    //            renderers[i].materials = mats;
    //        }
    //        else
    //        {
    //            // возвращаем оригинальные материалы
    //            renderers[i].materials = originalMaterials[i];
    //        }
    //    }
    //}

}