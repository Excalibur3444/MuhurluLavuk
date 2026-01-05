using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private AudioSource audioSource;

    [SerializeField]
    private TextMeshProUGUI berserkTimeText;

    // Ses efektleri
    [SerializeField]
    private AudioClip footstepSound;
    [SerializeField]
    private AudioClip jumpSound;
    [SerializeField]
    private AudioClip damageSound;
    [SerializeField]
    private float footstepVolume = 0.3f;
    [SerializeField]
    private float jumpVolume = 0.5f;
    [SerializeField]
    private float damageVolume = 0.7f;
    private float footstepTimer = 0f;
    [SerializeField]
    private float footstepCooldown = 0.3f;

    // about berserk mode
    [SerializeField]
    private float berserkSpeedBoost = 2f;
    [SerializeField]
    private float berserkJumpBoost = 2f;
    [SerializeField]
    private float berserkDuration = 20f;
    [SerializeField]
    private Color berserkColor = Color.red;
    [SerializeField]
    private Color normalColor = Color.white;
    private bool isBerserk = false;
    private float berserkTimer;
    private SpriteRenderer spriteRenderer;


    [SerializeField]
    private float jumpForce = 10f;
    [SerializeField]
    private float maxSpeed = 10f;
    [SerializeField]
    private float groundAcceleration = 50f;
    [SerializeField]
    private float groundDeceleration = 50f;
    [SerializeField]
    private float airAcceleration = 25f;
    [SerializeField]
    private float airDeceleration = 5f;

    [SerializeField]
    private float groundRayLength = 0.05f;

    public LayerMask groundLayer;

    public LayerMask wallLayer;

    [SerializeField]
    private Vector2 wallJumpForce = new Vector2(5, 5);

    public Transform wallCheck;

    [SerializeField]
    private float wallSlideSpeed = 2f;

    [SerializeField]
    private float wallCheckRadius = 0.2f;

    private bool isOnWall;

    private bool isWallSliding;

    private Vector2 moveInput;

    private bool isGrounded;

    private bool isFacingRight = true;

    [SerializeField]
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [SerializeField]
    private float jumpBufferTime = 0.2f;
    private float jumpBufferTimeCounter;

    private float jumpHeightCutMultiplier = 0.5f;

    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private Transform bowSpawnPoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }



    private void Update()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (jumpBufferTimeCounter > 0)
        {
            jumpBufferTimeCounter -= Time.deltaTime;
        }

        if (footstepTimer > 0)
        {
            footstepTimer -= Time.deltaTime;
        }

        if (isBerserk)
        {
            GameManager.Instance.currentHealth -= Time.deltaTime;
            berserkTimeText.text = Mathf.CeilToInt(GameManager.Instance.currentHealth).ToString();
            if (berserkTimer <= 0)
            {
                ExitBerserkMode();
                // Also Death

            }

            groundAcceleration = groundAcceleration * berserkSpeedBoost;
            groundDeceleration = groundDeceleration * berserkSpeedBoost;
            airAcceleration = airAcceleration * berserkSpeedBoost;
            airDeceleration = airDeceleration * berserkSpeedBoost;
        }

        if (!GameManager.Instance.IsInPast)
        {
            ExitBerserkMode();

        }

    }

    private void FixedUpdate()
    {

        CheckGround();
        CheckIfOnWall();
        MovePlayer();
        DetermineWallSlide();
        DetermineWhichJump();
        PlayFootstepSound();

        UpdateAnimations();


    }


    public void MoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

    }


    private void MovePlayer()
    {

        float currentMaxSpeed = isBerserk ? maxSpeed * berserkSpeedBoost : maxSpeed;

        float targetSpeed = moveInput.x * currentMaxSpeed;

        // or use another currentChangeOfSpeed temporary variable and make it equal to air or ground acceleration and deceleration depending on if is grounded
        float changeOfSpeed;

        if (isGrounded)
        {
            changeOfSpeed = (Mathf.Abs(targetSpeed) > 0.01) ? groundAcceleration : groundDeceleration;
        }
        else
        {
            changeOfSpeed = (Mathf.Abs(targetSpeed) > 0.01) ? airAcceleration : airDeceleration;
        }

        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, targetSpeed, changeOfSpeed * Time.fixedDeltaTime);


        if (moveInput.x != 0)
        {
            CheckFacingDirection(moveInput.x > 0);
        }

    }


    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpBufferTimeCounter = jumpBufferTime;
        }

        if (context.canceled && rb.linearVelocityY > 0)
        {
            rb.linearVelocityY = rb.linearVelocityY * jumpHeightCutMultiplier;

        }

    }

    private void PerformJump()
    {
        float currentJumpVelocity = isBerserk ? jumpForce * berserkJumpBoost : jumpForce;

        rb.linearVelocityY = currentJumpVelocity;
        animator.SetTrigger("Jump");
        PlaySound(jumpSound, jumpVolume);
        coyoteTimeCounter = 0f;
        jumpBufferTimeCounter = 0f;

    }

    private void PerformWallJump()
    {
        // if facing right, jump left if facing left jump right
        float jumpDirection = isFacingRight ? -1f : 1f;

        rb.linearVelocity = new Vector2(wallJumpForce.x * jumpDirection, wallJumpForce.y);

        jumpBufferTimeCounter = 0f;
        coyoteTimeCounter = 0f;

        CheckFacingDirection(jumpDirection > 0);
        animator.SetTrigger("Jump");
        PlaySound(jumpSound, jumpVolume);
    }

    private void DetermineWhichJump()
    {
        if (jumpBufferTimeCounter > 0)
        {
            if (!isGrounded && isOnWall)
            {
                PerformWallJump();
            }
            else if (coyoteTimeCounter > 0)
            {
                PerformJump();
            }


        }


    }


    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, groundRayLength, groundLayer);

        if (hit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void CheckIfOnWall()
    {
        Collider2D overlap = Physics2D.OverlapCircle(wallCheck.transform.position, wallCheckRadius, wallLayer);

        if (overlap != null)
        {
            isOnWall = true;
        }
        else
        {
            isOnWall = false;
        }

        // or simply isOnWall = (overlap != null)

    }


    private void DetermineWallSlide()
    {
        if (!isGrounded && isOnWall && rb.linearVelocityY < 0 && moveInput.x != 0)
        {
            isWallSliding = true;
            PerformWallSlide();

        }
        else
        {
            isWallSliding = false;
        }

    }

    private void PerformWallSlide()
    {
        if (isWallSliding)
        {
            rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -wallSlideSpeed, 9999);
            // 9999 is just a very high number to not limit wall jumps, you could use float.MaxValue too
        }



    }




    private void CheckFacingDirection(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
        {
            FlipPlayer();
        }

    }

    private void FlipPlayer()
    {
        isFacingRight = !isFacingRight;

        Vector3 localScale = gameObject.transform.localScale;

        localScale.x *= -1;

        gameObject.transform.localScale = localScale;

    }

    private void UpdateAnimations()
    {

        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocityX));

        animator.SetFloat("YVelocity", rb.linearVelocityY);

        animator.SetBool("IsGrounded", isGrounded);

        animator.SetBool("IsWallSliding", isWallSliding);

    }

    public void TimeTravelInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.Instance.MakeTimeTravel();

        }


    }



    public void BerserkModeInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isBerserk && GameManager.Instance.IsInPast == true)
            {
                EnterBerserkMode();
            }

            else if (isBerserk)
            {
                ExitBerserkMode();
            }

        }

    }

    private void EnterBerserkMode()
    {
        isBerserk = true;
        berserkTimer = berserkDuration;
        spriteRenderer.color = berserkColor;
        berserkTimeText.gameObject.SetActive(true);
        berserkTimeText.text = berserkDuration.ToString();

    }

    private void ExitBerserkMode()
    {
        isBerserk = false;
        spriteRenderer.color = normalColor;
        berserkTimeText.gameObject.SetActive(false);

    }


    public void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PerformAttack();

        }


    }

    private void PerformAttack()
    {
        animator.SetTrigger("Attack");


    }

    public void BowAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartBowAttack();

        }


    }

    private void StartBowAttack()
    {
        animator.SetTrigger("BowAttack");
        rb.linearVelocity = Vector2.zero;

    }

    public void ShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, bowSpawnPoint.position, Quaternion.identity);
        float direction = transform.localScale.x > 0 ? 1 : -1;

        arrow.GetComponent<Arrow>().SetUpArrow(direction);

    }

    private void PlayFootstepSound()
    {
        if (isGrounded && Mathf.Abs(moveInput.x) > 0.01f && footstepTimer <= 0)
        {
            PlaySound(footstepSound, footstepVolume);
            footstepTimer = footstepCooldown;
        }
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public void TakeDamage()
    {
        PlaySound(damageSound, damageVolume);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spear"))
        {
            ResetLevel();
        }
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
