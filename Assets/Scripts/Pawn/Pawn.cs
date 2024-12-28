using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pawn : MonoBehaviour
{
    [SerializeField] private float _health;
    [SerializeField] private float _speed;
    [SerializeField] private float _force;
    [SerializeField] private float _knockback;
    [SerializeField] private float _knockbackDelay;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _visibleDistance;
    [SerializeField] private string _enemyTeam;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _attackTime;

    public float _price;
    public float _trainTime;

    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileMaxSpeed;
    [SerializeField] private float _projectileForce;
    [SerializeField] private float _projectileKnockback;
    [SerializeField] private AnimationCurve trajectoryAnimataionCurve;
    [SerializeField] private AnimationCurve axisCorrectionAnimationCurve;
    [SerializeField] private AnimationCurve projectileSpeedAnimationCurve;
    [SerializeField] private float _projectileMaxHeight;

    private bool isDying = false;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private NavMeshAgent _agent;
    private Animator animator;
    public GameObject[] _enemies;
    public GameObject _nearestEnemy;
    [SerializeField] private DamageFlashScript _DamageFlash;

    private void Start()
    {
        PawnSelectionManager.Instance.allPawns.Add(gameObject);
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        _enemies = GameObject.FindGameObjectsWithTag(_enemyTeam);
    }

    private void OnDestroy()
    {
        PawnSelectionManager.Instance.allPawns.Remove(gameObject);
    }

    private void Update()
    {
        _enemies = GameObject.FindGameObjectsWithTag(_enemyTeam);
        if (GetClosestEnemy(_enemies) != null)
        {
            _nearestEnemy = GetClosestEnemy(_enemies);
        }

        if (_attackTime < _attackCooldown)
        {
            _attackTime += Time.deltaTime;
        }

        if (_nearestEnemy != null && Vector2.Distance(transform.position, _nearestEnemy.transform.position) <= _visibleDistance && Vector2.Distance(transform.position, GetClosestEnemy(_enemies).transform.position) > _attackRange && _attackTime >= _attackCooldown && isDying == false && GetComponent<PawnMovement>()._priorityMovement == false)
        {
            animator.SetBool("isRun", true);
            animator.SetBool("Attack", false);
            _agent.SetDestination(_nearestEnemy.transform.position);
            if (transform.position.x < _nearestEnemy.transform.position.x)
            {
                _sr.flipX = false;
            }
            else if (transform.position.x > _nearestEnemy.transform.position.x)
            {
                _sr.flipX = true;
            }
        }
        if (_nearestEnemy == null || Vector2.Distance(_nearestEnemy.transform.position, transform.position) > _visibleDistance || _attackTime < _attackCooldown || isDying == true)
        {
            if (GetComponent<PawnMovement>()._priorityMovement == false && GetComponent<PawnMovement>()._isMovement == false)
            {
                animator.SetBool("isRun", false);
            }
            animator.SetBool("Attack", false);
        }
        if (_nearestEnemy != null && Vector2.Distance(transform.position, _nearestEnemy.transform.position) <= _attackRange && _attackTime >= _attackCooldown && isDying == false && GetComponent<PawnMovement>()._priorityMovement == false)
        {
            if (transform.position.x < _nearestEnemy.transform.position.x)
            {
                _sr.flipX = false;
            }
            else if (transform.position.x > _nearestEnemy.transform.position.x)
            {
                _sr.flipX = true;
            }
            _agent.SetDestination(transform.position);
            animator.SetBool("isRun", false);
            animator.SetBool("Attack", true);
            if (GameObject.FindGameObjectWithTag("Ground").GetComponent<PawnSelectionManager>().selectedPawns.Contains(gameObject))
            {
                GameObject.FindGameObjectWithTag("Ground").GetComponent<PawnSelectionManager>()._destinationMarker.SetActive(false);
            }
        }
    }

    GameObject GetClosestEnemy(GameObject[] enemies)
    {
        GameObject tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in enemies)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

    public void sendDamage()
    {
        if (_nearestEnemy != null)
        {
            _nearestEnemy.GetComponent<Pawn>().TakeDamage(_force, _knockback, gameObject);
        }
    }

    public void TakeDamage(float damage, float knockback, GameObject attacker)
    {
        Vector2 direction = (transform.position - attacker.transform.position).normalized;

        _health -= damage;
        _DamageFlash.callDamageFlash();
        _rb.AddForce(direction * knockback, ForceMode2D.Impulse);
        attacker.GetComponent<Pawn>()._attackTime = 0f;
        StartCoroutine(ResetKnockback(_rb));
        if (_health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDying = true;

        if (_sr.flipX == false)
        {
            Destroy(gameObject, _knockbackDelay);
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }
        else if (_sr.flipX == true)
        {
            Destroy(gameObject, _knockbackDelay);
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
        }
    }

    public void executeProjectile()
    {
        Projectile projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.InitializeProjectile(_nearestEnemy, gameObject, _projectileMaxSpeed, _projectileForce, _projectileKnockback);
            projectile.InitializeAnimationCurve(trajectoryAnimataionCurve, axisCorrectionAnimationCurve, projectileSpeedAnimationCurve, _projectileMaxHeight);
        }
        _attackTime = 0f;
    }

    private IEnumerator ResetKnockback(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(_knockbackDelay);
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }
}
