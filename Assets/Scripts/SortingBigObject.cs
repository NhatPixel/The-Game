using UnityEngine;
using System.Collections.Generic;

public class SortingBigObject : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Color defaultColor;
    Color fadeColor;
    HashSet<GameObject> playersInside = new HashSet<GameObject>();

    void Start()
    {
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
        fadeColor = defaultColor;
        fadeColor.a = 0.7f;
    }

    void Update()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    void FadeOut()
    {
        spriteRenderer.color = fadeColor;
    }

    void FadeIn()
    {
        spriteRenderer.color = defaultColor; 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SpriteRenderer playerSpriteRenderer = other.gameObject.GetComponent<SpriteRenderer>();
            if (playerSpriteRenderer.sortingOrder < spriteRenderer.sortingOrder)
            {
                playersInside.Add(other.gameObject);
                FadeOut();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInside.Remove(other.gameObject);
            if (playersInside.Count == 0)
                FadeIn();
        }
    }
}
