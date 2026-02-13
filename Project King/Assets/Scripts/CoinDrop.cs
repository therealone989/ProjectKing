using UnityEngine;

public class CoinDrop : MonoBehaviour
{
    private ObjectPool pool;

    private Vector3 startPos;
    private Vector3 targetOffset;
    private float timer;

    public bool flyingToPlayer = false;
    GameObject player;
    private void OnEnable()
    {
        flyingToPlayer = false;
        player = null;
        timer = 0f;
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
            gameObject.GetComponent<Collider>().isTrigger = true;
            Vector3 targetPos = player.transform.position + Vector3.up;

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                Time.fixedDeltaTime * 10f
            );
            Vector3 diff = transform.position - player.transform.position;
            if (diff.sqrMagnitude < 1.44f)
            {
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
    public void LocatePlayer(GameObject go)
    {
        player = go;
    }
}
