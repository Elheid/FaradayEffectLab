using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class InteractableObject : MonoBehaviour
{
    // Ссылка на контурный материал (см. ниже)
    public Material outlineMaterial;
    private Material originalMaterial;
    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material; // Сохраняем оригинальный материал
    }


    // Публичный метод для вызова "приближения"
    public void BringToFront()
    {
        InteractionsManager.Instance.FocusOnObject(this);
    }

    // Включить/выключить подсветку
    public void SetHighlighted(bool highlighted)
    {
        //Debug.Log(highlighted ? "Highlighted!" : "Normal");
        rend.material = highlighted ? outlineMaterial : originalMaterial;
    }

}