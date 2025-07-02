using UnityEngine;

public class HoverSelectable : MonoBehaviour
{
    [SerializeField] Material hoverMaterial;

    Material originalMaterial;
    Material hoverInstance;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
            hoverInstance = new Material(hoverMaterial);

            if (spriteRenderer.sprite != null)
            {
                hoverInstance.mainTexture = spriteRenderer.sprite.texture;
            }
        }
    }

    public void ApplyHover()
    {
        if (spriteRenderer != null)
            spriteRenderer.material = hoverInstance;
    }

    public void RemoveHover()
    {
        if (spriteRenderer != null)
            spriteRenderer.material = originalMaterial;
    }
}
