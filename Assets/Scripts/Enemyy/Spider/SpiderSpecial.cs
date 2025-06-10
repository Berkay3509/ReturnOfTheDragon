using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderSpecial : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private Rect attackArea = new Rect(-1, -1, 2, 2);
    [SerializeField] private float attackInterval = 2f; // Saldýrý aralýðý (saniye)
    [SerializeField] private LayerMask playerLayer;

    private Animator _animator;
    private float _attackCooldown;
    private bool _playerInRange;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null) ;
    }

    private void Update()
    {
        _playerInRange = IsPlayerInAttackBox();

        if (_playerInRange)
        {
            if (_attackCooldown <= 0)
            {
                StartAttack();
                _attackCooldown = attackInterval;
            }
            else
            {
                _attackCooldown -= Time.deltaTime;
            }
        }
        else
        {
            _attackCooldown = 0; // Player menzilden çýkýnca cooldown'u sýfýrla
        }
    }

    private bool IsPlayerInAttackBox()
    {
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(
            attackArea.x * transform.localScale.x,
            attackArea.y
        );

        return Physics2D.OverlapBox(boxCenter,
            new Vector2(attackArea.width, attackArea.height),
            0, playerLayer) != null;
    }

    private void StartAttack()
    {
        _animator.SetTrigger("SpecialATrigger");
    }

    // Animasyon eventinden çaðrýlacak
    public void SpecialDamagePlayer()
    {
        if (!_playerInRange) return; // Menzilde deðilse hasar verme

        Vector2 boxCenter = (Vector2)transform.position + new Vector2(
            attackArea.x * transform.localScale.x,
            attackArea.y
        );

        var hit = Physics2D.OverlapBox(boxCenter,
                     new Vector2(attackArea.width, attackArea.height),
                     0, playerLayer);

        if (hit != null && hit.TryGetComponent<Health>(out var health))
        {
            health.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(
            attackArea.x * transform.localScale.x,
            attackArea.y
        );
        Gizmos.DrawWireCube(boxCenter, new Vector3(attackArea.width, attackArea.height, 1));
    }
}
