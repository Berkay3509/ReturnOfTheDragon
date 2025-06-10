using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private float direction;
    private bool hit;
    private float lifetime;

    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > 5) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger("explode");

        if (collision.tag == "Enemy")
            collision.GetComponent<Health>()?.TakeDamage(1);
            collision.GetComponent<BatHealth>()?.TakeDamage(1);
            collision.GetComponent<GhostKnightHealth>()?.TakeDamage(1);
            collision.GetComponent<GhostHealth>()?.TakeDamage(1);
            collision.GetComponent<ButcherHealth>()?.TakeDamage(1);
            collision.GetComponent<CrystalHealth>()?.TakeDamage(1);
            collision.GetComponent<ImpHealth>()?.TakeDamage(1);
            collision.GetComponent<MagmaHealth>()?.TakeDamage(1);
            collision.GetComponent<RatHealth>()?.TakeDamage(1);
            collision.GetComponent<SkeletonHealth>()?.TakeDamage(1);
            collision.GetComponent<SpiderHealth>()?.TakeDamage(1);
            collision.GetComponent<SuccubusHealth>()?.TakeDamage(1);
            collision.GetComponent<WoodHealth>()?.TakeDamage(1);
            collision.GetComponent<ZombieHealth>()?.TakeDamage(1);
            collision.GetComponent<ZombieWarriorHealth>()?.TakeDamage(1);
            collision.GetComponent<RedDragonHealth>()?.TakeDamage(1);
    }
    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}