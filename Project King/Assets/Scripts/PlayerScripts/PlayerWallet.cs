using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] TMP_Text coinsText;
    public int Coins { get; private set; }

    private void Awake()
    {
        coinsText.text = Coins.ToString();
    }
    public void Add(int amount)
    {
        Coins += amount;
        coinsText.text = Coins.ToString();
    }
    public void Subtract(int amount)
    {
        Coins -= amount;
        coinsText.text = Coins.ToString();
    }
    public int GetCoins()
    {
        return Coins;
    }
}
