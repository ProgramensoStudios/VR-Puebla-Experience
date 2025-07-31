using UnityEngine;

public class BulletEnem : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 3f;

    private Vector3 _direction;
    private float _lifeTimer;

    public void SetDirection(Vector3 dir)
    {
        _direction = dir.normalized;
        _lifeTimer = lifeTime;
    }

    private void Update()
    {
        transform.position += _direction * (speed * Time.deltaTime);
        _lifeTimer -= Time.deltaTime;

        if (_lifeTimer <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController health))
        {
            health.ReceiveDamage(damage);
        }

        gameObject.SetActive(false);
    }
}
