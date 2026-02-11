using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    public int collectionRange;
    public LayerMask coinMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

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

    Collider[] hitColliders = Physics.OverlapSphere(transform.position, collectionRange, coinMask);

    foreach (var hitCollider in hitColliders)
    {
        CoinDrop coin = hitCollider.GetComponentInParent<CoinDrop>();
        if (coin == null) 
            continue;

        // Schon unterwegs? Dann nicht nochmal zaehlen/triggern
        if (coin.flyingToPlayer)
            continue;

        coin.flyingToPlayer = true;
        coin.LocatePlayer(gameObject);

        wallet.Add(1);
    }
}

}
