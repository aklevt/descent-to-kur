using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    
    private Transform currentTarget;

    private void Start()
    {
        ResetToPlayer();
    }

    private void LateUpdate()
    {
        if (currentTarget == null) return;
        transform.position = currentTarget.position + offset;
    }
    
    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }
    
    public void ResetToPlayer()
    {
        currentTarget = playerTarget;
    }
}