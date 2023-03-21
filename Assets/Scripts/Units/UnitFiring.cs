using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float fireRange = 11f;
    [SerializeField] private float rotationSpeed = 180f;

    private float lastFireTime;

    [ServerCallback]
    private void Update() 
    {
        Targetable target = targeter.GetTarget();

        if(target == null) return;

        if(!CanFireAtTarget()) return;

        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if(Time.time > (1/ fireRate + lastFireTime))
        {
            lastFireTime = Time.time;

            Quaternion projectileRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position);

            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);

            NetworkServer.Spawn(projectileInstance, connectionToClient);
        }
    }

    [Server]
    private bool CanFireAtTarget()
    {
        return ((targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= fireRange * fireRange); 
    }
}
