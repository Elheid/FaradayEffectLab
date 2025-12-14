using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("ћатериал подсветки")]
    public Material outlineMaterial;

    private Renderer[] renderers;
    private Material[][] originalMaterials; // сохран€ем материалы каждого рендера

    private void Awake()
    {
        // находим все рендереры в объекте и дочерних объектах
        renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("No Renderer found for InteractableObject on " + gameObject.name);
            return;
        }

        // сохран€ем оригинальные материалы
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
        if (renderers == null || renderers.Length == 0) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (highlighted)
            {
                // замен€ем все материалы на outlineMaterial
                Material[] mats = new Material[renderers[i].materials.Length];
                for (int j = 0; j < mats.Length; j++)
                    mats[j] = outlineMaterial;

                renderers[i].materials = mats;
            }
            else
            {
                // возвращаем оригинальные материалы
                renderers[i].materials = originalMaterials[i];
            }
        }
    }
}