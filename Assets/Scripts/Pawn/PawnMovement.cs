using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PawnMovement : MonoBehaviour
{
    public SpriteRenderer _sr;
    public NavMeshAgent _agent;
    public Animator _animator;
    private PawnSelectionManager _pawnSelectionManager;
    public bool _priorityMovement = false;
    public bool _isMovement = false;
    public bool _isMovementAvailable;
    public LayerMask _groundMask;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _pawnSelectionManager = GameObject.FindGameObjectWithTag("Ground").GetComponent<PawnSelectionManager>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (Vector2.Distance(_agent.destination, transform.position) <= _agent.stoppingDistance)
        {
            _priorityMovement = false;
            _isMovement = false;
            _animator.SetBool("isRun", _isMovement);
        }
    }
    private void OnDestroy()
    {
        if (gameObject && GameObject.FindGameObjectWithTag("Ground"))
        {
            _pawnSelectionManager.SelectPawn(gameObject, false);
            _pawnSelectionManager.selectedPawns.Remove(gameObject);
        }
    }
}
