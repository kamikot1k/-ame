using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileVisual _projectileVisual;

    private GameObject _target;
    private Vector3 _targetPosition;
    private GameObject _attacker;
    private float _moveSpeed;
    private float _maxMoveSpeed;
    private float _force;
    private float _knockback;

    private AnimationCurve trajectoryAnimationCurve;
    private AnimationCurve axisCorrectionAnimationCurve;
    private AnimationCurve projectileSpeedAnimationCurve;

    private float trajectoryMaxRelativeHeight;
    private Vector3 trajectoryRange;

    private Vector3 trajectoryStartPoint;
    private Vector3 projectileMoveDir;

    private float nextYTrajectoryPosition;
    private float nextXTrajectoryPosition;
    private float nextPositionYCorrectionAbsolute;
    private float nextPositionXCorrectionAbsolute;

    private float distanceToTargetToDestroyProjectile = 1f;

    private void Start()
    {
        trajectoryStartPoint = transform.position;
    }

    private void Update()
    {
        if (_target != null)
        {
            _targetPosition  = _target.transform.position;

            updateProjectilePosition();

            if (Vector2.Distance(transform.position, _target.transform.position) < distanceToTargetToDestroyProjectile)
            {
                _target.GetComponent<Pawn>().TakeDamage(_force, _knockback, _attacker);
                Destroy(gameObject);
            }
        }
        else
        {
            _projectileVisual.SetTarget(_targetPosition);

            updateProjectilePosition();

            if (Vector2.Distance(transform.position, _targetPosition) < distanceToTargetToDestroyProjectile)
            {
                Destroy(gameObject);
            }
        }
    }
    private void updateProjectilePosition()
    {
        if (_target != null)
        {
            trajectoryRange = _target.transform.position - trajectoryStartPoint;
        }
        else
        {
            trajectoryRange = _targetPosition - trajectoryStartPoint;
        }

        if (Mathf.Abs(trajectoryRange.normalized.x) < Mathf.Abs(trajectoryRange.normalized.y))
        {
            if (trajectoryRange.y < 0)
            {
                _moveSpeed = -_moveSpeed;
            }

            UpdatePositionWithXCurve();
        }
        else
        {
            if (trajectoryRange.x < 0)
            {
                _moveSpeed = -_moveSpeed;
            }

            UpdatePositionWithYCurve();
        }
    }

    private void UpdatePositionWithYCurve()
    {
        float nextPositionX = transform.position.x + _moveSpeed * Time.deltaTime;
        float nextPositionXNormalized = (nextPositionX - trajectoryStartPoint.x) / trajectoryRange.x;

        float nextPositionYNormalized = trajectoryAnimationCurve.Evaluate(nextPositionXNormalized);
        nextYTrajectoryPosition = nextPositionYNormalized * trajectoryMaxRelativeHeight;

        float nextPositionYCorrectionNormalized = axisCorrectionAnimationCurve.Evaluate(nextPositionXNormalized);
        nextPositionYCorrectionAbsolute = nextPositionYCorrectionNormalized * trajectoryRange.y;

        float nextPositionY = trajectoryStartPoint.y + nextYTrajectoryPosition + nextPositionYCorrectionAbsolute;

        Vector3 newPosition = new Vector3(nextPositionX, nextPositionY, 0);

        CalculateNextProjectileSpeed(nextPositionXNormalized);
        projectileMoveDir = newPosition - transform.position;

        transform.position = newPosition;
    }

    private void UpdatePositionWithXCurve()
    {
        float nextPositionY = transform.position.y + _moveSpeed * Time.deltaTime;
        float nextPositionYNormalized = (nextPositionY - trajectoryStartPoint.y) / trajectoryRange.y;

        float nextPositionXNormalized = trajectoryAnimationCurve.Evaluate(nextPositionYNormalized);
        nextXTrajectoryPosition = nextPositionXNormalized * trajectoryMaxRelativeHeight;

        float nextPositionXCorrectionNormalized = axisCorrectionAnimationCurve.Evaluate(nextPositionYNormalized);
        nextPositionXCorrectionAbsolute = nextPositionXCorrectionNormalized * trajectoryRange.x;

        if (trajectoryRange.x > 0 && trajectoryRange.y > 0)
        {
            nextXTrajectoryPosition = -nextXTrajectoryPosition;
        }

        if (trajectoryRange.x < 0 && trajectoryRange.y < 0)
        {
            nextXTrajectoryPosition = -nextXTrajectoryPosition;
        }


        float nextPositionX = trajectoryStartPoint.x + nextXTrajectoryPosition + nextPositionXCorrectionAbsolute;

        Vector3 newPosition = new Vector3(nextPositionX, nextPositionY, 0);

        CalculateNextProjectileSpeed(nextPositionYNormalized);
        projectileMoveDir = newPosition - transform.position;

        transform.position = newPosition;
    }

    private void CalculateNextProjectileSpeed(float nextPositionXNormalized)
    {
        float nextMoveSpeedNormalized = projectileSpeedAnimationCurve.Evaluate(nextPositionXNormalized);

        _moveSpeed = nextMoveSpeedNormalized * _maxMoveSpeed;
    }

    public void InitializeProjectile(GameObject target, GameObject attacker, float maxMoveSpeed, float force, float knockback)
    {
        this._target = target;
        this._targetPosition = target.transform.position;
        this._attacker = attacker;
        this._maxMoveSpeed = maxMoveSpeed;
        this._force = force;
        this._knockback = knockback;

        _projectileVisual.SetTarget(target.transform.position);
    }

    public void InitializeAnimationCurve(AnimationCurve trajectoryAnimationCurve, AnimationCurve axisCorrectionAnimationCurve, AnimationCurve projectileSpeedAnimationCurve, float trajectoryMaxHeight)
    {
        this.trajectoryAnimationCurve = trajectoryAnimationCurve;
        this.axisCorrectionAnimationCurve = axisCorrectionAnimationCurve;
        this.projectileSpeedAnimationCurve = projectileSpeedAnimationCurve;
        float xDistanceToTarget = _target.transform.position.x - transform.position.x;
        this.trajectoryMaxRelativeHeight = Mathf.Abs(xDistanceToTarget) * trajectoryMaxHeight;
    }

    public Vector3 GetProjectileMoveDir()
    {
        return projectileMoveDir;
    }

    public float GetNextYTrajectoryPosition()
    {
        return nextYTrajectoryPosition;
    }

    public float GetNextPositionYCorrectionAbsolute()
    {
        return nextPositionYCorrectionAbsolute;
    }

    public float GetNextXTrajectoryPosition()
    {
        return nextXTrajectoryPosition;
    }

    public float GetNextPositionXCorrectionAbsolute()
    {
        return nextPositionXCorrectionAbsolute;
    }
}
