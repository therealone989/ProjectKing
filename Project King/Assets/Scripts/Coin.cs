using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Value")]
    [SerializeField] private int value = 1;

    [Header("Parabola")]
    [SerializeField] private float flightTime = 0.25f;   // schneller = snappier
    [SerializeField] private float arcHeight = 1.2f;     // Höhe des Bogens
    [SerializeField] private float pickupDelay = 0.15f;  // nicht instant einsammeln

    private bool canPickup = false;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Despawn")]
    [SerializeField] private float despawnTime = 20f;
    private Vector3 startPos;
    private Vector3 targetPos;
    private float t;
    private bool isFlying = false;

    public void Launch(Vector3 target, float flightTime, float arcHeight)
    {
        this.flightTime = flightTime;
        this.arcHeight = arcHeight;

        startPos = transform.position;
        targetPos = target;
        t = 0f;
        isFlying = true;

        canPickup = false;
        CancelInvoke(nameof(EnablePickup));
        Invoke(nameof(EnablePickup), pickupDelay);
    }

    // misst die “halbe Höhe” des physischen Colliders am Root
    public float GetHalfHeight()
    {
        // Der physische Collider ist bei dir am Root (Münze) und NICHT trigger
        Collider col = GetComponent<Collider>();

        if (col != null && !col.isTrigger)
            return col.bounds.extents.y;

        // Fallback, falls doch was anders ist:
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null) return rend.bounds.extents.y;

        return 0.05f; // Notfallwert
    }


    private void Update()
    {
        if (!isFlying) return;

        t += Time.deltaTime / Mathf.Max(0.0001f, flightTime);
        float u = Mathf.Clamp01(t);

        // Basis-Lerp
        Vector3 pos = Vector3.Lerp(startPos, targetPos, u);

        // Parabel-Offset (0->1->0)
        float h = 4f * u * (1f - u) * arcHeight;
        pos.y += h;

        transform.position = pos;

        // optional: spin
        transform.Rotate(0f, 720f * Time.deltaTime, 0f, Space.World);

        if (u >= 1f)
        {
            Land();
        }
    }

    private void EnablePickup() => canPickup = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canPickup) return;

        var wallet = other.GetComponent<PlayerWallet>();
        if (wallet == null) return;
        CancelInvoke(nameof(Despawn));
        wallet.Add(value);
        Destroy(gameObject);
    }
    public void Configure(float flightTime, float arcHeight)
    {
        this.flightTime = flightTime;
        this.arcHeight = arcHeight;
    }
    private void Land()
    {
        isFlying = false;

        // exakte Endposition
        transform.position = targetPos;

        // Rotation zurücksetzen
        transform.rotation = Quaternion.identity;

        // Animation abspielen
        if (animator != null)
            animator.SetTrigger("Land");

        Invoke(nameof(Despawn), despawnTime);
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }
}
