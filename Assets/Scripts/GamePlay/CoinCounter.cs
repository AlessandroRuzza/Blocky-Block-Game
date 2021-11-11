using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCounter : MonoBehaviour
{
    Player playerRef;
    public GameObject coinUIRoot;
    GameObject[] coins;
    void Start()
    {
        playerRef = GameObject.FindObjectOfType<Player>();
        coins = new GameObject[Player.COIN_TARGET];
        AddCoinsToScreen();
        playerRef.OnCoinPickup += FillCoin;
    }

    void AddCoinsToScreen(){
        for(int i=0; i < Player.COIN_TARGET; i++){
            GameObject coin = Instantiate<GameObject>(coinUIRoot, gameObject.transform.position, Quaternion.identity, gameObject.transform);
            coin.transform.SetAsLastSibling();
            coin.name = "CoinUI_" + (i+1);
            Destroy(coin.GetComponent<Collider2D>());
            coin.tag = "Untagged";
            coin.layer = 5;
            coins[i] = coin;
        }
    }

    void FillCoin(int coinIndex){
        coinIndex--; // adjust value to actual position in coins array
        if(coinIndex > Player.COIN_TARGET){
            Debug.Log("Coin Index too high, can't fill!");
            return;
        }
        Image coinToFill = coins[coinIndex].GetComponent<Image>();
        coinToFill.fillCenter = true;
    }
}
