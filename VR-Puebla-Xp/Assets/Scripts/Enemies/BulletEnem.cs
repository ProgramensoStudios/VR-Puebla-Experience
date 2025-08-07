using System;
using UnityEngine;

public class BulletEnem : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float damage;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private RefShotHit reference;
    RaycastHit hit;
    

    private Vector3 _direction;
    private float _lifeTimer;

    private void Awake()
    {
        reference = FindAnyObjectByType<RefShotHit>();
        if (reference != null)
        {
            Debug.Log("Reference Found");
        } 
    }

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
        if (Physics.Raycast(transform.position - _direction * 0.5f, _direction, out hit, 1f))
        {
            reference.transform.position = hit.point;
            reference.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            reference.transform.position = transform.position;
            reference.GetComponent<ParticleSystem>().Play();
        }
        gameObject.SetActive(false);
    }
    
}
