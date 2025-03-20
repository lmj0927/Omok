using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public Constants.CELL_TYPE stoneType;
    private bool isDisposable = true;
    private List<Collider> _colliders;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _colliders = GetComponents<Collider>().ToList();
    }

    void Update()
    {
        if (isDisposable)
        {
            if (transform.position.y < -10)
            {
                Destroy(gameObject);
            }
        }
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