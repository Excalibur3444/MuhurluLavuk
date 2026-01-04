using UnityEngine;

public class KnightEnemy : MonoBehaviour
{

    [SerializeField]
    private float maxHealth = 100f;
    [SerializeField]
    private float currentHealth = 100f;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float rayDistance;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask wallLayer;
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private Transform ledgeCheck;
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private float playerCheckDistance = 2f;
    private bool playerInRange = false;
    [SerializeField]
    private float attackCoolDown = 1f;
    private bool isAttacking = false;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isMovingRight = true;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

    }

    private void Update()
    {
        rb.linearVelocity = new Vector2(speed, rb.linearVelocityY);

        CheckForCliffsAndWalls();
        CheckForPlayer();


     
    }

    private void CheckForCliffsAndWalls()
    {
        bool hasGroundAhead = Physics2D.Raycast(ledgeCheck.position, Vector2.down, rayDistance, groundLayer);
        bool hasWallAhead = Physics2D.Raycast(wallCheck.position, transform.right, rayDistance, wallLayer);

        if (!hasGroundAhead || hasWallAhead)
        {
            Flip();

        }


    }

    private void Flip()
    {
        isMovingRight = !isMovingRight;
        speed *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

    }

    private void CheckForPlayer()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, playerCheckDistance, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player") && !isAttacking)
        {
            PerformAttack();
        }

    }

    private void PerformAttack()
    {
        isAttacking = true;
        float cooldown = attackCoolDown;
        cooldown -= Time.deltaTime;

        rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsMoving", false);
        animator.SetTrigger("Attack");

        if (cooldown <= 0)
        {
            isAttacking = false;
        }
        

    }

    public void DealDamage()
    {


    }


    public void GetHit(float amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {

            Die();

        }

    }

    public void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", true);
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject);

    }

}
