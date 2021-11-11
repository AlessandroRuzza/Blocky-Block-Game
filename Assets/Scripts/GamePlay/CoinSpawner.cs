using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*******************
* COIN SPAWNER:  goals
*   - Generate coins around the screen
*   - Don't let coins collide with UI elements
* 
* Actual COIN: goals
*   - Make nice particle effects on coin pickup
*   - Little ding sound on pickup
*   - Add a rotation effect (might look bad)
* 
****************************/
public class CoinSpawner : MonoBehaviour
{
    public GameObject coinRoot;
    public Player playerRef;
    void Start()
    {  
        InvokeRepeating("SpawnCoin", 3, 2); // starts spawning 3s after start of round, then once every 2s
    }

    void Update()
    {

    }

    void SpawnCoin(){
        Vector3 spawnPoint = transform.position + RandomSpawnOffset();
        // then check if spawnPoint is over UI element

        GameObject coinHandle = Instantiate<GameObject>(coinRoot, spawnPoint, Quaternion.identity);
    }

    Vector3 RandomSpawnOffset(){
        // offset must be within the camera
        float maxOffsetX = halfScreenWidth;
        float maxOffsetY = halfScreenHeight;
        
        float offsetX = Random.Range(-maxOffsetX, maxOffsetX);
        float offsetY = Random.Range(-maxOffsetY, maxOffsetY);

        Vector3 direction = new Vector3(offsetX, offsetY);
        return direction;   
    }
}
