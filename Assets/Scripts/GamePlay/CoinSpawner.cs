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
    Player playerRef;
    float maxOffsetX, maxOffsetY;
    void Start()
    {  
        playerRef = GameObject.FindObjectOfType<Player>();
        // whole coin must be within the camera
        maxOffsetX = CameraUtils.halfWidth - coinRoot.transform.localScale.x;
        maxOffsetY = CameraUtils.halfHeight - coinRoot.transform.localScale.y;
        InvokeRepeating("SpawnCoin", 3, 2); // starts spawning 3s after start of round, then once every 2s
        playerRef.OnDeath += CancelInvoke;
        playerRef.OnObjectiveReached += CancelInvoke;
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
        float offsetX = Random.Range(-maxOffsetX, maxOffsetX);
        float offsetY = Random.Range(-maxOffsetY, maxOffsetY);

        Vector3 position = new Vector3(offsetX, offsetY);
        return position;   
    }
}
