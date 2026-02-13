using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    public float collectionRange;
    public LayerMask coinMask;
    private Collider[] hitResults = new Collider[20]; // Speicher-Reservierung für NonAlloc


    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 10 == 0)
        {
            CollectCoin();
        }
    }
public void CollectCoin()
{
    // Wallet nur 1x holen
    PlayerWallet wallet = gameObject.GetComponentInParent<PlayerWallet>();
    if (wallet == null)
    {
        Debug.LogError("Keine Playerwallet!");
        return;
    }

    int hitCounts = Physics.OverlapSphereNonAlloc(transform.position, collectionRange, hitResults, coinMask);

    for (int i = 0; i < hitCounts; i++)
        {
            // Wir holen uns den Collider aus dem aktuellen Index
            Collider hitCollider = hitResults[i];

            if (hitCollider == null) continue; // Sicherheits-Check

            CoinDrop coin = hitCollider.GetComponentInParent<CoinDrop>();
            if (coin == null) continue;

            // Schon unterwegs? Dann nicht nochmal zaehlen/triggern
            if (coin.flyingToPlayer)continue;

            coin.LocatePlayer(gameObject);
            coin.flyingToPlayer = true;
            

            wallet.Add(1);
        }
}

}
