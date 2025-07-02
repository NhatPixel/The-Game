using UnityEngine;
using System.Collections.Generic;

public class ItemController : MonoBehaviour
{
    [SerializeField] Item item;
    SpriteRenderer spriteRenderer;
    float attractSpeed = 5f;
    [SerializeField] InventoryUI inventoryUI;

    List<Transform> playersInRange = new();

    Dictionary<Transform, float> ignoreCooldowns = new();
    float ignoreDuration = 3f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.Icon;
    }

    void Update()
    {
        UpdateSpriteOrder();
        UpdateCooldowns();
        MoveToNearestPlayer();
    }

    void UpdateSpriteOrder()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRange.Add(other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRange.Remove(other.transform);
            ignoreCooldowns.Remove(other.transform);
        }
    }

    void MoveToNearestPlayer()
    {
        if (playersInRange.Count == 0) return;

        Transform nearest = playersInRange[0];
        float minDist = Vector3.Distance(transform.position, nearest.position);

        foreach (Transform t in playersInRange)
        {
            if (ignoreCooldowns.ContainsKey(t)) continue;

            float dist = Vector3.Distance(transform.position, t.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = t;
            }
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            nearest.position,
            attractSpeed * Time.deltaTime
        );

        if (minDist < 0.2f)
        {
            PickUp(nearest);
        }
    }

    void PickUp(Transform player)
    {
        if (inventoryUI.AddItem(item))
        {
            Destroy(gameObject);
        }
        else
        {
            ignoreCooldowns[player] = Time.time + ignoreDuration;
        }
    }

    void UpdateCooldowns()
    {
        List<Transform> toRemove = new();

        foreach (var kvp in ignoreCooldowns)
        {
            if (Time.time >= kvp.Value)
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var t in toRemove)
        {
            ignoreCooldowns.Remove(t);
        }
    }
}
