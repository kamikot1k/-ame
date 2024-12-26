using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PawnMovement : MonoBehaviour
{
    private Camera _camera;
    NavMeshAgent _agent;
    public LayerMask _groundMask;

    private void Awake()
    {
        _camera = Camera.main;
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 ray = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D _hit = Physics2D.Raycast(ray, Vector2.zero, Mathf.Infinity, _groundMask);

            if (_hit)
            {
                _agent.SetDestination(_hit.point);
            }
        }
    }
}
