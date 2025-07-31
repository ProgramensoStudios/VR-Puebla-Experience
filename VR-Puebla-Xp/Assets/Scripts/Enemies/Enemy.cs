using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private PoolingSystem poolingSystem;
    [SerializeField] private FollowPointsManager patrolManager;

    [Header("Disparo")]
    [SerializeField] private float shootInterval = 0.3f;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Vida")]
    [SerializeField] private int maxHealth = 100;

    private NavMeshAgent _agent;
    private int _currentHealth;
    private float _shootTimer;
    private Transform _currentTarget;
    private Transform _patrolTarget;
    
    [SerializeField] private FollowPointsManager.Zones zonaAsignada;
    
    
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int scoreValue = 100;
    [SerializeField, Range(0f, 1f)] private float coinDropChance = 0.2f;

    [SerializeField] private EnemyCountManager enemyCountManager;
    
    public event System.Action<Enemy> OnDeath;

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
            TryFindPlayer();
            PatrolUpdate();
        }
        else
        {
            // Player en la mira: perseguirlo y disparar
            PursuePlayer();
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

        // Buscamos la zona correspondiente
        FollowPointsManager.Zone zona = patrolManager.zone.Find(z => z.zones == zonaAsignada);

        if (zona.points == null || zona.points.Count == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, zona.points.Count);
        _patrolTarget = zona.points[randomIndex];

        if (_patrolTarget != null)
        {
            _agent.SetDestination(_patrolTarget.position);
        }
    }

    private void TryFindPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
        if (hits.Length > 0)
        {
            _currentTarget = hits[0].transform;
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
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (Random.value <= coinDropChance)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        if (scoreManager != null)
        {
            scoreManager.AddScore(scoreValue);
        }

        OnDeath?.Invoke(this);

        gameObject.SetActive(false);
    }
    
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
