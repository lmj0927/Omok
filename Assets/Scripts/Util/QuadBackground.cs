using System;
using UnityEngine;

public class QuadBackground : MonoBehaviour
{
    Camera _main;

    void Start()
    {
        _main = Camera.main;
    }

    private void LateUpdate()
    {
        if (_main == null) return;

        transform.position = _main.transform.position + _main.transform.forward * 10f;

        transform.rotation = _main.transform.rotation;
    }
}