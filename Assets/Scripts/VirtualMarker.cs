using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualMarker : MonoBehaviour
{
    [SerializeField] private GameObject _environment;
    
    private Vector3 _relativePosition;
    private Quaternion _relativeRotation;

    public Vector3 RelativePosition => _relativePosition;
    public Quaternion RelativeRotation => _relativeRotation;

    
    private void Start()
    {
        SaveStartValues();
    }

    private void SaveStartValues()
    {
        _relativePosition = transform.InverseTransformPoint(_environment.transform.position);
        _relativeRotation = _environment.transform.rotation * Quaternion.Inverse(transform.rotation);
    }
}
