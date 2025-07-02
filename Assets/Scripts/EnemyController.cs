using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float health = 40;
    [SerializeField] float attack = 10;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] float knockback = 0.5f;
    [SerializeField] float speed = 1.5f;
    [SerializeField] float visionRange = 2.5f;

    [SerializeField] int rayCount = 36;
    [SerializeField] LayerMask detectionMask;
    [SerializeField] float raycastXOffset = 0.35f;
    [SerializeField] float raycastYOffset = 0.3f;

    [SerializeField] float wanderRadius = 2f;
    [SerializeField] float stuckTimeThreshold = 1.5f;

    Rigidbody2D rigidbody2d;
    SpriteRenderer spriteRenderer;
    Animator enemyAnimator;

    bool isRunning = false;
    bool isWaiting = false;
    Vector2 wanderTarget;
    Vector2 lastPosition;
    float stuckTimer = 0f;

    Dictionary<GameObject, float> damageCooldowns = new Dictionary<GameObject, float>();

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyAnimator = GetComponent<Animator>();
        CreateWanderTarget();
    }

    void Update()
    {
        UpdateSpriteOrder();
    }

    void FixedUpdate()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        float angleStep = 360f / rayCount;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * angleStep;
            Vector2 direction = AngleToVector2(angle);
            Vector2 origin = (Vector2)transform.position + direction * raycastXOffset + Vector2.up * raycastYOffset;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, visionRange, detectionMask);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Vector2 targetPosition = hit.collider.transform.position;
                Move(targetPosition);
                Debug.DrawRay(origin, direction * visionRange, Color.red);
                return;
            }
            else
            {
                Debug.DrawRay(origin, direction * visionRange, Color.green);
            }
        }

        StopRunning();
        HandleWandering();
    }

    void Move(Vector2 targetPosition)
    {
        isRunning = true;
        enemyAnimator.SetBool("isRunning", true);

        Vector2 currentPosition = rigidbody2d.position;
        Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.fixedDeltaTime);

        Flip(targetPosition.x > currentPosition.x ? 1 : -1);
        rigidbody2d.MovePosition(newPosition);
    }

    void StopRunning()
    {
        isRunning = false;
        enemyAnimator.SetBool("isRunning", false);
    }

    void Flip(int facing)
    {
        transform.localScale = new Vector3(facing, 1, 1);
    }

    void HandleWandering()
    {
        if (isRunning || isWaiting) return;

        float distanceToTarget = Vector2.Distance(rigidbody2d.position, wanderTarget);

        if (distanceToTarget > wanderRadius)
        {
            CreateWanderTarget();
        }

        Move(wanderTarget);

        if (distanceToTarget < 0.1f)
        {
            StartCoroutine(WanderPause());
        }

        float movementSinceLastFrame = Vector2.Distance(rigidbody2d.position, lastPosition);

        if (movementSinceLastFrame < 0.01f)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer >= stuckTimeThreshold)
            {
                CreateWanderTarget();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = rigidbody2d.position;
    }

    void CreateWanderTarget()
    {
        Vector2 currentPos = rigidbody2d.position;
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        wanderTarget = currentPos + randomOffset;
    }

    IEnumerator WanderPause()
    {
        isWaiting = true;
        StopRunning();
        yield return new WaitForSeconds(2f);
        CreateWanderTarget();
        isWaiting = false;
    }

    void UpdateSpriteOrder()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    Vector2 AngleToVector2(float anglerDegrees)
    {
        float rad = anglerDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;

            if (!damageCooldowns.ContainsKey(player))
            {
                damageCooldowns[player] = 0f;
            }

            if (Time.time >= damageCooldowns[player])
            {
                var playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(attack, knockback, transform.position);
                    damageCooldowns[player] = Time.time + attackCooldown;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            if (damageCooldowns.ContainsKey(player))
            {
                damageCooldowns.Remove(player);
            }
        }
    }
}
