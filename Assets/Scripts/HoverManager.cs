using UnityEngine;

public class HoverManager : MonoBehaviour
{
    private HoverSelectable currentHovered;
    [SerializeField] LayerMask hoverableLayer;

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, hoverableLayer);

        if (hit.collider != null)
        {
            HoverSelectable ec = hit.collider.GetComponent<HoverSelectable>();

            if (ec != null)
            {
                if (currentHovered != ec)
                {
                    if (currentHovered != null)
                        currentHovered.RemoveHover();

                    ec.ApplyHover();
                    currentHovered = ec;
                }
            }
        }
        else
        {
            if (currentHovered != null)
            {
                currentHovered.RemoveHover();
                currentHovered = null;
            }
        }
    }
}
