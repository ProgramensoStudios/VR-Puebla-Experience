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
    [SerializeField] public float shieldRecoveryMulti;
    [SerializeField] public float maxHp;
    [SerializeField] public float hp;
    [SerializeField] public int missileAmount;
    [SerializeField] public float maxHpShield;
    [SerializeField] public int missileAmountMax;
    [SerializeField] public bool hasShield;
    [SerializeField] private float speed;
    [SerializeField] private PoolingSystem pool;
    [SerializeField] private Transform shootPos;
    [SerializeField] private ParticleSystem shootParticleSystem;

    [SerializeField] private Camera mainCamera;
    private CharacterController _controller;
    private Vector3 _moveInput;
    private Coroutine _shootingCoroutine;
    [SerializeField] private float fireRate = 0.2f;
    private bool _isShooting = false;
    [SerializeField] private GameObject shield;
    [SerializeField] private float shieldRechargeDelay = 3f; 
    private Coroutine _shieldRegenCoroutine;
    private Coroutine _shieldDelayCoroutine;
    [SerializeField] private bool isRegeneratingShield = false;
    private bool _canRegenerateShield = true;
    
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float verticalVelocity = 0f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    
    public event Action<float> OnHpChanged;
    public event Action<float, bool> OnShieldChanged;
    
    
    private void Awake()
    {
        _inputData = GetComponent<InputData>();
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {

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
        // Siempre cancelar la regeneración y el delay previos
        if (_shieldRegenCoroutine != null)
        {
            StopCoroutine(_shieldRegenCoroutine);
            _shieldRegenCoroutine = null;
        }

        if (_shieldDelayCoroutine != null)
        {
            StopCoroutine(_shieldDelayCoroutine);
            _shieldDelayCoroutine = null;
        }

        _canRegenerateShield = false;

        if (hasShield)
        {
            shieldHp -= damage;
            shieldHp = Mathf.Max(0f, shieldHp);
            OnShieldChanged?.Invoke(shieldHp, hasShield);

            if (shieldHp <= 0)
            {
                hasShield = false;
                shield.SetActive(false);
                OnShieldChanged?.Invoke(shieldHp, hasShield);
            }
        }
        else
        {
            hp -= damage;
            hp = Mathf.Max(0f, hp);
            OnHpChanged?.Invoke(hp);

            if (hp <= 0)
            {
                Die();
            }
        }

        _shieldDelayCoroutine = StartCoroutine(StartShieldRegenAfterDelay());
    }

    
    private IEnumerator StartShieldRegenAfterDelay()
    {
        yield return new WaitForSeconds(shieldRechargeDelay);
        _canRegenerateShield = true;
        _shieldRegenCoroutine = StartCoroutine(RecoverShield());
    }
    private void Die()
    {
        Debug.Log("Jugador ha muerto!");
        gameObject.SetActive(false);
    }

    private IEnumerator RecoverShield()
    {
        isRegeneratingShield = true;

        if (shieldHp <= 0)
        {
            shieldHp = 0;
            hasShield = true;
            shield.SetActive(true);
            OnShieldChanged?.Invoke(shieldHp, hasShield);
        }

        while (shieldHp < maxHpShield && _canRegenerateShield)
        {
            shieldHp += shieldRecoveryMulti * Time.deltaTime;
            shieldHp = Mathf.Min(shieldHp, maxHpShield);

            OnShieldChanged?.Invoke(shieldHp, hasShield);
            yield return null;
        }

        isRegeneratingShield = false;
        _shieldRegenCoroutine = null;
    }

    //Power ups

    public void RestoreHpToFull()
    {
        hp = maxHp;
        OnHpChanged?.Invoke(hp);
    }

    public void IncreaseMaxHp(float amount)
    {
        maxHp += amount;
        hp = maxHp;
        OnHpChanged?.Invoke(hp);
    }

    public void IncreaseShieldMax(float amount)
    {
        maxHpShield += amount;
        shieldHp = maxHpShield;
        hasShield = true;
        shield.SetActive(true);
        OnShieldChanged?.Invoke(shieldHp, hasShield);
    }

    public void RefillMissiles()
    {
        missileAmount = missileAmountMax;
    }

    public void IncreaseMissileCapacity(int amount)
    {
        missileAmountMax += amount;
        missileAmount = missileAmountMax;
    }

    public void DecreaseShieldRegenDelay(float amount)
    {
        shieldRechargeDelay = Mathf.Max(0.5f, shieldRechargeDelay - amount);
    }

}
