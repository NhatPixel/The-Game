using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
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
    Rigidbody2D rigidbody2d;
    [SerializeField] float speed = 5f;
    Animator playerAnimator;
    bool isFacingFront = true;
    bool isRunning = false;
    SpriteRenderer playerSpriteRenderer;

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
        playerAnimator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
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
            MovePlayer();
        }
    }

    void HandleInput()
    {
        if (IsOwner)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            if (h != horizontal.Value)
                horizontal.Value = Input.GetAxisRaw("Horizontal");
            if (v != vertical.Value)
                vertical.Value = Input.GetAxisRaw("Vertical");
        }

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

    void Flip()
    {
        transform.localScale = new Vector3(horizontal.Value, 1, 1);
    }

    void UpdateAnimation()
    {
        playerAnimator.SetBool("isFacingFront", isFacingFront);
        playerAnimator.SetBool("isRunning", isRunning);
    }

    void UpdateSpriteOrder()
    {
        playerSpriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    void MovePlayer()
    {
        Vector2 moveDirection = new Vector2(horizontal.Value, vertical.Value).normalized;
        rigidbody2d.linearVelocity = moveDirection * speed;
    }

    void AssignCamera()
    {
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.Target = transform;
        }
    }
}
