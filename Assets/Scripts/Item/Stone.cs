using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stone : MonoBehaviour
{
    [SerializeField] private float speedThreshold;
    [SerializeField] private float lifeTimeLimit;
    
    public Constants.CELL_TYPE stoneType;
    private bool isDisposable = true;
    private List<Collider> _colliders;
    private Rigidbody _rigidbody;
    private float _lifeTime;

    public Action<GameObject> OnStop;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _colliders = GetComponents<Collider>().ToList();

        _rigidbody.sleepThreshold = speedThreshold;
    }

    private void OnEnable()
    {
        _lifeTime = 0;
    }

    void Update()
    {
        if (isDisposable)
        {
            if (transform.position.y < -10)
            {
                OnStop?.Invoke(gameObject);
            }

            if (_rigidbody && _rigidbody.IsSleeping())
            {
                OnStop?.Invoke(gameObject);
            }

            if (_lifeTime >= lifeTimeLimit)
            {
                OnStop?.Invoke(gameObject);
            }
        }

        _lifeTime += Time.deltaTime;
    }

    public void SetKinematic(bool isKinematic)
    {
        foreach (var col in _colliders)
        {
            col.enabled = !isKinematic;
        }
        
        _rigidbody.isKinematic = isKinematic;
        
        isDisposable = !isKinematic;
    }
}