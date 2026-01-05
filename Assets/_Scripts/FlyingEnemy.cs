using Unity.VisualScripting;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;
    [SerializeField]
    private float currentHealth = 100f;
    [SerializeField]
    private float speed = 5f;

    private int currentPatrolPoint;
    public Transform[] patrolPoints;
    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        Transform targetPosition = patrolPoints[currentPatrolPoint];

        transform.position = Vector2.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);

        if (targetPosition.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);

        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Vector2.Distance(transform.position, targetPosition.position) < 0.1f)
        {
            currentPatrolPoint++;

            if (currentPatrolPoint >= patrolPoints.Length)
            {
                currentPatrolPoint = 0;
            }
        }

    }


}
