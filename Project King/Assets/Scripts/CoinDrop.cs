using UnityEngine;

public class CoinDrop : MonoBehaviour
{

    private Rigidbody rb;
    private ObjectPool pool;

    private Vector3 startPos;
    private Vector3 targetOffset;
    private float timer;

    bool flyingToPlayer = false;
    Transform player;

    private bool isPickedUp = false;

    private void OnEnable()
    {
        flyingToPlayer = false;
        player = null;
        timer = 0f;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init(ObjectPool pool)
    {
        this.pool = pool;

        timer = 0f;
        startPos = transform.position;

        Vector2 rand = Random.insideUnitCircle * 4.5f;
        targetOffset = new Vector3(rand.x, 0.2f, rand.y);
    }

    void FixedUpdate()
    {
        if (flyingToPlayer)
        {
            Vector3 targetPos = player.position + Vector3.up;

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                Time.fixedDeltaTime * 10f
            );
            if (Vector3.Distance(transform.position, player.position) < 1.2f)
            {
                isPickedUp = false;
                pool.ReturnObject(gameObject);
            }

            return;
        }

        // sanfte Wurfbewegung
        timer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(timer / 0.4f);

        Vector3 arc = Vector3.up * Mathf.Sin(t * Mathf.PI) * 0.2f;
        transform.position = startPos + Vector3.Lerp(Vector3.zero, targetOffset, t) + arc;

        // Rotation
        transform.Rotate(Vector3.up, 100f * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPickedUp) return;
        if (!other.CompareTag("Player")) return;
        isPickedUp = true;
        PlayerWallet wallet = other.GetComponent<PlayerWallet>();
        wallet.Add(1);
        player = other.transform;
        flyingToPlayer = true;
    }

}
