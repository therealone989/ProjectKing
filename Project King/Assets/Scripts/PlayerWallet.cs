using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public int Coins { get; private set; }
    
    public void Add(int amount)
    {
        Coins += amount;
    }

    public int GetCoins()
    {
        return Coins;
    }
}
