using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private PoolingSystem poolingSystem;
    [SerializeField] private FollowPointsManager patrolManager;
    [SerializeField] private AudioSource bulletSfx;

    [Header("Disparo")]
    [SerializeField] private float shootInterval = .8f;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Vida")]
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private Transform player;
    [SerializeField] private int sumYCoinTrans;

    [SerializeField] private FollowPointsManager.Zones zonaAsignada;
    [SerializeField] private GameObject coinPrefab;

    [SerializeField] private EnemyCountManager enemyCountManager;

    public event System.Action<Enemy> OnDeath;

    // --- INTERNAS ---
    private NavMeshAgent _agent;
    private int _currentHealth;
    private float _shootTimer;
    private Transform _currentTarget;
    private Transform _patrolTarget;

    // Alerta por daño
    private Vector3 _investigatePosition;
    private bool _isInvestigating = false;
    private float _investigateTimer = 0f;
    [SerializeField] private float investigateDuration = 5f;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _currentHealth = maxHealth;
        enemyCountManager = FindAnyObjectByType<EnemyCountManager>();
    }

    private void Start()
    {
        SetRandomPatrolPoint();
    }

    private void Update()
    {
        _shootTimer += Time.deltaTime;

        if (_currentTarget == null)
        {
            TryFindPlayer(); // Intenta ver al jugador

            if (_currentTarget == null)
            {
                if (_isInvestigating)
                {
                    Investigate(); // Está en modo "buscar quién me atacó"
                }
                else
                {
                    PatrolUpdate(); // Patrulla normal
                }
            }
        }
        else
        {
            PursuePlayer(); // Lo está siguiendo
        }
    }

    private void TryFindPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
        if (hits.Length > 0)
        {
            _currentTarget = hits[0].transform;
            _isInvestigating = false; // Ya lo encontró, no necesita investigar más
        }
    }

    private void PatrolUpdate()
    {
        if (_agent.pathPending) return;

        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            SetRandomPatrolPoint();
        }
    }

    private void SetRandomPatrolPoint()
    {
        if (patrolManager == null) return;

        FollowPointsManager.Zone zona = patrolManager.zone.Find(z => z.zones == zonaAsignada);
        if (zona.points == null || zona.points.Count == 0) return;

        int randomIndex = Random.Range(0, zona.points.Count);
        _patrolTarget = zona.points[randomIndex];

        if (_patrolTarget != null)
        {
            _agent.SetDestination(_patrolTarget.position);
        }
    }

    private void PursuePlayer()
    {
        if (_currentTarget == null) return;

        _agent.SetDestination(_currentTarget.position);

        Vector3 dir = (_currentTarget.position - transform.position);
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
        }

        if (_shootTimer >= shootInterval)
        {
            ShootAt(_currentTarget);
            _shootTimer = 0f;
        }

        float distance = Vector3.Distance(transform.position, _currentTarget.position);
        if (distance > detectionRange * 1.2f)
        {
            _currentTarget = null;
        }
    }

    private void ShootAt(Transform target)
    {
        GameObject bullet = poolingSystem.AskForObject(shootPoint);
        if (bullet.TryGetComponent(out BulletEnem bulletScript))
        {
            Vector3 dir = (target.position - shootPoint.position).normalized;
            bulletScript.SetDirection(dir);
        }
        bulletSfx.Play();
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        // Activamos modo investigación
        _investigatePosition = player.position; // Guardamos de dónde vino el ataque
        _isInvestigating = true;
        _investigateTimer = investigateDuration;
        _currentTarget = null;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Investigate()
    {
        _investigateTimer -= Time.deltaTime;

        if (_investigateTimer <= 0f)
        {
            _isInvestigating = false;
            return;
        }

        _agent.SetDestination(_investigatePosition);

        // Girar hacia la posición investigada
        Vector3 dir = (_investigatePosition - transform.position);
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
        }

        float distance = Vector3.Distance(transform.position, _investigatePosition);
        if (distance <= _agent.stoppingDistance + 0.5f)
        {
            _isInvestigating = false; // Llegó y no encontró nada
        }
    }

    private void Die()
    {
        Instantiate(coinPrefab, new Vector3(transform.position.x, transform.position.y + sumYCoinTrans, transform.position.z), Quaternion.identity);

        OnDeath?.Invoke(this);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
