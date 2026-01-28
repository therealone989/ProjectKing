using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class TowerCannon : MonoBehaviour
{
    [Header("Aim")]
    [SerializeField] private Transform yawTransform;      // nur Y
    [SerializeField] private Transform pitchTransform;    // nur X (local)
    [SerializeField] private Transform muzzleTransform;   // Spawn

    [Header("Combat")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int damage = 10;
    [SerializeField] private float shotsPerSecond = 1f;

    [Header("Rotation Speeds")]
    [SerializeField] private float yawSpeed = 360f;
    [SerializeField] private float pitchSpeed = 240f;
    [SerializeField] private float minPitch = -10f;
    [SerializeField] private float maxPitch = 35f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string fireTrigger = "Fire";

    [Header("Aim Gate (erstmal AUS lassen)")]
    [SerializeField] private bool requireAim = false;
    [SerializeField, Range(0f, 1f)] private float aimDotThreshold = 0.85f;

    private readonly List<Enemy> enemiesInRange = new();
    private Enemy currentTarget;

    private float nextFireTime;
    private bool fireQueued;

    [SerializeField] private Transform aimForward;

    private void Awake()
    {
        // Trigger & Rigidbody “hart” korrekt setzen
        var sc = GetComponent<SphereCollider>();
        sc.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // Animator robust holen (Root ODER Child)
        if (animator == null)
            animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Minimal sanity logs (nur einmal)
        if (animator == null) Debug.LogError("[TowerCannon] Animator fehlt!");
        if (yawTransform == null) Debug.LogError("[TowerCannon] yawTransform fehlt!");
        if (pitchTransform == null) Debug.LogError("[TowerCannon] pitchTransform fehlt!");
        if (muzzleTransform == null) Debug.LogError("[TowerCannon] muzzleTransform fehlt!");
        if (projectilePrefab == null) Debug.LogError("[TowerCannon] projectilePrefab fehlt!");
    }

    private void Update()
    {
        enemiesInRange.RemoveAll(e => e == null);
        currentTarget = enemiesInRange.Count > 0 ? enemiesInRange[0] : null;

        if (currentTarget == null)
        {
            fireQueued = false;
            return;
        }

        Vector3 targetPos = currentTarget.transform.position;

        AimYaw(targetPos);
        AimPitch(targetPos);

        TryFire(targetPos);
    }

    private void AimYaw(Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - yawTransform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f) return;

        Quaternion targetYaw = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        yawTransform.rotation = Quaternion.RotateTowards(
            yawTransform.rotation,
            targetYaw,
            yawSpeed * Time.deltaTime
        );
    }

    private void AimPitch(Vector3 targetPos)
    {
        Vector3 local = yawTransform.InverseTransformPoint(targetPos);

        float desiredPitch = Mathf.Atan2(local.y, local.z) * Mathf.Rad2Deg;
        desiredPitch = Mathf.Clamp(desiredPitch, minPitch, maxPitch);

        float currentPitch = NormalizeAngle(pitchTransform.localEulerAngles.x);
        float newPitch = Mathf.MoveTowardsAngle(currentPitch, desiredPitch, pitchSpeed * Time.deltaTime);

        pitchTransform.localRotation = Quaternion.Euler(newPitch, 0f, 0f);
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

    private void TryFire(Vector3 targetPos)
    {
        if (Time.time < nextFireTime) return;
        if (fireQueued) return;

        if (requireAim && !IsAimedWellEnough(targetPos)) return;

        fireQueued = true;
        nextFireTime = Time.time + (1f / Mathf.Max(0.01f, shotsPerSecond));

        // Wenn Animator fehlt oder Trigger nicht greift: Fallback schießen
        if (animator != null && animator.isActiveAndEnabled)
        {
            animator.SetTrigger(fireTrigger);
        }
        else
        {
            AnimEvent_Fire();
        }
    }

    private bool IsAimedWellEnough(Vector3 targetPos)
    {
        if (aimForward == null) aimForward = muzzleTransform; // fallback

        Vector3 dir = (targetPos - aimForward.position).normalized;

        Debug.DrawRay(aimForward.position, aimForward.forward * 2f, Color.blue);
        Debug.DrawRay(aimForward.position, dir * 2f, Color.red);

        float dot = Vector3.Dot(aimForward.forward, dir);
        return dot >= aimDotThreshold;
    }



    // Animation Event
    public void AnimEvent_Fire()
    {
        fireQueued = false;

        if (currentTarget == null) return;
        if (projectilePrefab == null || muzzleTransform == null) return;

        GameObject proj = Instantiate(projectilePrefab, muzzleTransform.position, muzzleTransform.rotation);

        var p = proj.GetComponent<Projectile>();
        if (p != null)
            p.Init(currentTarget, damage);
        else
            Debug.LogError("[TowerCannon] Projectile Script fehlt auf projectilePrefab!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent(out Enemy e))
        {
            if (!enemiesInRange.Contains(e))
            {
                e.OnDeath += RemoveEnemy;
                enemiesInRange.Add(e);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent(out Enemy e))
        {
            RemoveEnemy(e);
        }
    }

    private void RemoveEnemy(Enemy e)
    {
        if (e == null) return;
        e.OnDeath -= RemoveEnemy;
        enemiesInRange.Remove(e);
    }
}
