using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Value")]
    [SerializeField] private int value = 1;

    [Header("Parabola Flight")]
    [SerializeField] private float flightTime = 0.25f;
    [SerializeField] private float arcHeight = 1.2f;
    [SerializeField] private float pickupDelay = 0.15f;

    [Header("Magnet")]
    [SerializeField] private float magnetSpeed = 12f;
    [SerializeField] private float magnetAcceleration = 25f;
    [SerializeField] private float collectDistance = 0.25f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Despawn")]
    [SerializeField] private float despawnTime = 20f;


    private bool canPickup = false;
    private bool isFlying = false;
    private bool isMagneting = false;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float t;

    private Transform magnetTarget;
    private float currentMagnetSpeed;

    [SerializeField] private Collider solidCollider;

    private void Awake()
    {
        if (solidCollider == null)
            solidCollider = GetComponent<Collider>();
    }

    public void Launch(Vector3 target, float flightTime, float arcHeight)
    {
        this.flightTime = flightTime;
        this.arcHeight = arcHeight;

        startPos = transform.position;
        targetPos = target;
        t = 0f;

        isFlying = true;
        isMagneting = false;

        canPickup = false;
        CancelInvoke(nameof(EnablePickup));
        Invoke(nameof(EnablePickup), pickupDelay);
    }

    public void StartMagnet(Transform playerRoot)
    {
        if (solidCollider != null)
            solidCollider.enabled = false;

        if (isMagneting) return;
        if (!canPickup) return;      
        if (isFlying) return;          

        isMagneting = true;
        magnetTarget = playerRoot;
        currentMagnetSpeed = magnetSpeed;

    
        if (animator != null)
            animator.enabled = false;


        transform.rotation = Quaternion.identity;

        CancelInvoke(nameof(Despawn));
    }


    private void Update()
    {
        HandleFlight();
        HandleMagnet();
    }

    private void HandleFlight()
    {
        if (!isFlying) return;

        t += Time.deltaTime / Mathf.Max(0.0001f, flightTime);
        float u = Mathf.Clamp01(t);

        Vector3 pos = Vector3.Lerp(startPos, targetPos, u);
        float h = 4f * u * (1f - u) * arcHeight;
        pos.y += h;

        transform.position = pos;
        transform.Rotate(0f, 720f * Time.deltaTime, 0f, Space.World);

        if (u >= 1f)
            Land();
    }

    private void Land()
    {
        isFlying = false;

        transform.position = targetPos;
        transform.rotation = Quaternion.identity;

        if (animator != null)
        {
            animator.enabled = true;
            animator.SetTrigger("Land");
        }

        Invoke(nameof(Despawn), despawnTime);
    }


    private void HandleMagnet()
    {
        if (!isMagneting || magnetTarget == null) return;

        Vector3 target = magnetTarget.position + Vector3.up * 1.0f;

        currentMagnetSpeed = Mathf.MoveTowards(
            currentMagnetSpeed,
            magnetSpeed * 2.5f,
            magnetAcceleration * Time.deltaTime
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            currentMagnetSpeed * Time.deltaTime
        );

        transform.Rotate(0f, 900f * Time.deltaTime, 0f, Space.World);

        if ((transform.position - target).sqrMagnitude <= collectDistance * collectDistance)
            Collect();
    }


    private void Collect()
    {
        PlayerWallet wallet = magnetTarget.GetComponent<PlayerWallet>();
        if (wallet != null)
            wallet.Add(value);

        Destroy(gameObject);
    }

    private void EnablePickup() => canPickup = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canPickup || isMagneting) return;

        PlayerWallet wallet = other.GetComponent<PlayerWallet>();
        if (wallet == null) return;

        CancelInvoke(nameof(Despawn));
        wallet.Add(value);
        Destroy(gameObject);
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }


    public float GetHalfHeight()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
            return col.bounds.extents.y;

        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            return rend.bounds.extents.y;

        return 0.05f;
    }
}
