using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
    float health = 100;
    float speed = 3f;
    float currentHealth;
    Color originColor;
    float hurtDuration = 0.2f;
    float antiKnockback = 0;

    Color hurtColor = Color.red;

    Rigidbody2D rigidbody2d;
    Animator animator;
    SpriteRenderer spriteRenderer;

    NetworkVariable<float> horizontal = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    NetworkVariable<float> vertical = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    bool isFacingFront = true;
    bool isRunning = false;
    bool isHurt = false;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            AssignCamera();
        }
    }

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = health;
        originColor = spriteRenderer.color;
    }

    void Update()
    {
        UpdateSpriteOrder();
        UpdateAnimation();
        HandleInput();
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    void HandleInput()
    {
        if (!IsOwner) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != horizontal.Value)
            horizontal.Value = h;

        if (v != vertical.Value)
            vertical.Value = v;

        if (horizontal.Value != 0)
        {
            Flip();
        }

        if (vertical.Value != 0)
        {
            isFacingFront = vertical.Value < 0;
        }

        isRunning = horizontal.Value != 0 || vertical.Value != 0;
    }

    void Move()
    {
        if (isHurt) return;
        Vector2 moveDirection = new Vector2(horizontal.Value, vertical.Value).normalized;
        rigidbody2d.linearVelocity = moveDirection * speed;
    }

    void Flip()
    {
        transform.localScale = new Vector3(horizontal.Value, 1, 1);
    }

    void UpdateAnimation()
    {
        animator.SetBool("isFacingFront", isFacingFront);
        animator.SetBool("isRunning", isRunning);
    }

    void UpdateSpriteOrder()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    void AssignCamera()
    {
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.Target = transform;
        }
    }

    public void TakeDamage(float damage, float knockback, Vector2 hitSourcePosition)
    {
        if (isHurt)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth == 0)
        {
            Die();
        }
        else
        {
            Vector2 knockDirection = (rigidbody2d.position - hitSourcePosition).normalized;
            rigidbody2d.AddForce(knockDirection * Mathf.Max(0f, knockback - antiKnockback), ForceMode2D.Impulse);

            StartCoroutine(FlashHurt());
        }
    }

    void Die()
    {
        StartCoroutine(FadeOutAndDestroy());
    }

    IEnumerator FlashHurt()
    {
        isHurt = true;

        spriteRenderer.color = hurtColor;

        yield return new WaitForSeconds(hurtDuration);

        spriteRenderer.color = originColor;

        isHurt = false;
    }

    IEnumerator FadeOutAndDestroy()
    {
        for (float f = 1f; f >= 0; f -= 0.05f)
        {
            Color c = spriteRenderer.color;
            c.a = f;
            spriteRenderer.color = c;
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(gameObject);
    }
}
