using UnityEngine;

public class CoinMagnet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Coin coin = other.GetComponentInParent<Coin>();
        if (coin != null)
            coin.StartMagnet(transform.root); 
    }
}
