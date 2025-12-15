using UnityEngine;

[ExecuteAlways]
public class AutoRootCollider : MonoBehaviour
{
    void OnEnable()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        foreach (var r in renderers)
            bounds.Encapsulate(r.bounds);

        var box = GetComponent<BoxCollider>();
        if (!box) box = gameObject.AddComponent<BoxCollider>();

        box.center = transform.InverseTransformPoint(bounds.center);
        box.size = bounds.size;
    }
}
