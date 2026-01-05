using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField]
    private int damage = 10;
    [SerializeField]
    private float speed = 20f;
    [SerializeField]
    private float lifeTime = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);

    }

    public void SetUpArrow(float direction)
    {
        rb.linearVelocityX = speed * direction;

        Vector3 scale = transform.localScale;

        scale.x = transform.localScale.x * direction;
        transform.localScale = scale;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);

        }
        */
    }



}
