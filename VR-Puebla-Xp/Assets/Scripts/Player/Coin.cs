using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Oscillation Settings")]
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatFrequency = 2f;
    [SerializeField] private float rotateSpeed = 90f;

    [Header("Detection Settings")]
    [SerializeField] private string playerLayerName = "Player";

    [Header("Coin Settings")]
    [SerializeField] private int coinValue = 1;
    [SerializeField] private CurrencyManager currencyManager;

    private Vector3 _startPos;

    private void Awake()
    {
        currencyManager = FindAnyObjectByType<CurrencyManager>();
    }

    private void Start()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = _startPos + new Vector3(0, newY, 0);
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (currencyManager != null)
        {
            currencyManager.AddCoins(coinValue);
        }
        
        Destroy(gameObject);
    }
}