using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float visionRange = 2.5f;
    [SerializeField] int rayCount = 36;
    [SerializeField] LayerMask detectionMask;
    [SerializeField] float speed = 1.5f;
    [SerializeField] float yOffset = 0.3f;
    [SerializeField] float wanderRadius = 2f;
    SpriteRenderer enemySpriteRenderer;
    bool isRunning = false;
    bool isWaiting = false;
    Rigidbody2D rigidbody2d;
    Animator enemyAnimator;
    Vector2 wanderTarget;
    Vector2 lastPosition;
    float stuckTimer = 0f;
    [SerializeField] float stuckTimeThreshold = 1.5f;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
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
            Vector2 origin = (Vector2)transform.position + direction * 0.35f + Vector2.up * yOffset;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, visionRange, detectionMask);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Vector2 targetPosition = hit.collider.transform.position;
                MoveEnemy(targetPosition);

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

    void MoveEnemy(Vector2 targetPosition)
    {
        isRunning = true;
        enemyAnimator.SetBool("isRunning", isRunning);

        Vector2 currentPosition = rigidbody2d.position;
        Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.fixedDeltaTime);

        if (targetPosition.x > currentPosition.x)
        {
            Flip(1);
        }
        else
        {
            Flip(-1);
        }

        rigidbody2d.MovePosition(newPosition);
    }

    void Flip(int facing)
    {
        transform.localScale = new Vector3(facing, 1, 1);
    }

    void StopRunning()
    {
        isRunning = false;
        enemyAnimator.SetBool("isRunning", isRunning);
    }

    void HandleWandering()
    {
        if (!isRunning && !isWaiting)
        {
            float distanceToTarget = Vector2.Distance(rigidbody2d.position, wanderTarget);

            if (distanceToTarget > wanderRadius)
            {
                CreateWanderTarget();
            }

            MoveEnemy(wanderTarget);

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
    }

    void CreateWanderTarget()
    {
        Vector2 currentPos = rigidbody2d.position;
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        wanderTarget = currentPos + randomOffset;
    }

    System.Collections.IEnumerator WanderPause()
    {
        isWaiting = true;
        StopRunning();
        yield return new WaitForSeconds(2f);
        CreateWanderTarget();

        isWaiting = false;
    }

    void UpdateSpriteOrder()
    {
        enemySpriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    Vector2 AngleToVector2(float anglerDegrees)
    {
        float rad = anglerDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
