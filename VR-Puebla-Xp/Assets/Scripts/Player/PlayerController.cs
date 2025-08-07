using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private InputData _inputData;
    
    [SerializeField] public float shieldHp;
    [SerializeField] public float hp;
    [SerializeField] public float maxHpShield;
    [SerializeField] public bool hasShield;
    [SerializeField] private PoolingSystem pool;
    [SerializeField] private Transform shootPos;
    [SerializeField] private ParticleSystem shootParticleSystem;
    private Vector3 _moveInput;
    private Coroutine _shootingCoroutine;
    [SerializeField] private float fireRate = 0.2f;
    private bool _isShooting = false;
    private Coroutine _shieldRegenCoroutine;
    private Coroutine _shieldDelayCoroutine;
    private bool _canRegenerateShield = true;
    [SerializeField] private AudioSource bulletSfx;
    [SerializeField] private EditableTimer timer;
    
    public event Action<float> OnHpChanged;
    public event Action<float, bool> OnShieldChanged;
    
    [SerializeField] private bool canReceiveData = true;

    private void OnEnable()
    {
        EditableTimer.onTimerEnd += EndGame;
    }

    private void OnDisable()
    {
        EditableTimer.onTimerEnd -= EndGame;
    }

    private void EndGame()
    {
        canReceiveData = false;
    }
    
    
    private void Awake()
    {
        _inputData = GetComponent<InputData>();
    }

    private void Update()
    {
        if (!canReceiveData) return;
        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.trigger, out var trigger))
        {
            switch (trigger)
            {
                case > 0.3f:
                    StartShooting();
                    break;
                case < 0.3f:
                    StopShooting();
                    break;
            }
        }
    }
    
    private void StartShooting()
    {
        if (!_isShooting)
        {
            _isShooting = true;
            _shootingCoroutine = StartCoroutine(ShootingCoroutine());
        }
    }

    private void StopShooting()
    {
        _isShooting = false;
        if (_shootingCoroutine != null)
        {
            StopCoroutine(_shootingCoroutine);
            _shootingCoroutine = null;
        }
    }

    private IEnumerator ShootingCoroutine()
    {
        while (_isShooting)
        {
            Shoot();
            shootParticleSystem.Play();
            bulletSfx.Play();
            Debug.Log(shootParticleSystem);
            yield return new WaitForSeconds(fireRate);
        }
    }

    private void Shoot()
    {
        pool.AskForObject(shootPos);
    }


    public void ReceiveDamage(float damage)
    {
        hp -= damage;
        hp = Mathf.Max(0f, hp);
        OnHpChanged?.Invoke(hp);
        
        Debug.Log("hp: " + hp);

        if (hp <= 0)
        {
            Die();
        }
        
    }
    
    private void Die()
    {
        Debug.Log("DED");
        timer.EndTime();
        EditableTimer.onTimerEnd?.Invoke();
    }
}
