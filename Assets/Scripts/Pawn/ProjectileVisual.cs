using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileVisual : MonoBehaviour
{
    [SerializeField] private Transform _projectileVisual;
    [SerializeField] private Transform _projectileShadow;
    [SerializeField] private Projectile _projectile;

    private Vector3 _target;
    private Vector3 _trajectoryStartPosition;

    private float shadowPositionDivider = 6f;

    private void Start()
    {
        _trajectoryStartPosition = transform.position;
    }

    private void Update()
    {
        UpdateProjectileRotation();
        UpdateShadowPosition();

        float trajectoryProgressMagnitude = (transform.position - _trajectoryStartPosition).magnitude;
        float trajectoryMagnitude = (_target - _trajectoryStartPosition).magnitude;

        float trajectoryProgressNormalized = trajectoryProgressMagnitude / trajectoryMagnitude;

        if (trajectoryProgressNormalized < .7f)
        {
            UpdateProjectileShadowRotation();
        }
    }

    private void UpdateShadowPosition()
    {
        Vector3 trajectoryRange = _target - _trajectoryStartPosition;
        Vector3 newPosition = transform.position;

        if (Mathf.Abs(trajectoryRange.normalized.x) < Mathf.Abs(trajectoryRange.normalized.y))
        {
            newPosition.x = _trajectoryStartPosition.x + _projectile.GetNextXTrajectoryPosition() / shadowPositionDivider + _projectile.GetNextPositionXCorrectionAbsolute();
        } else
        {
            newPosition.y = _trajectoryStartPosition.y + _projectile.GetNextYTrajectoryPosition() / shadowPositionDivider + _projectile.GetNextPositionYCorrectionAbsolute();
        }

        _projectileShadow.position = newPosition;
    }

    private void UpdateProjectileRotation()
    {
        Vector3 projectileMoveDir = _projectile.GetProjectileMoveDir();

        _projectileVisual.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(projectileMoveDir.y, projectileMoveDir.x) * Mathf.Rad2Deg);
    }

    private void UpdateProjectileShadowRotation()
    {
        Vector3 projectileMoveDir = _projectile.GetProjectileMoveDir();

        _projectileShadow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(projectileMoveDir.y, projectileMoveDir.x) * Mathf.Rad2Deg);
    }

    public void SetTarget(Vector3 target)
    {
        this._target = target;
    }
}
