using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*******************        \\ is DONE
* COIN SPAWNER:  goals
*   \\- Generate coins around the screen
*   - Don't let coins collide with UI elements
* 
* Actual COIN: goals
*   - Make nice particle effects (shiny)
*   - Little ding sound on pickup
*   - Add a rotation effect (might look bad)
* 
****************************/
public class CoinSpawner : MonoBehaviour
{
    public GameObject coinRoot;
    public GameObject healthBarRef;
    public static int coinsOnScreen, MAX_COINS_ON_SCREEN=12;
    Player playerRef;
    new AudioSource audio;
    public AudioClip pickupSound;
    float maxOffsetX, maxOffsetY;
    void Start()
    {  
        coinsOnScreen = 0;
        playerRef = GameObject.FindObjectOfType<Player>();
        playerRef.OnCoinPickup += PlayPickupSound;
        audio = gameObject.GetComponent<AudioSource>();
        maxOffsetX = CameraUtils.halfWidth - coinRoot.transform.localScale.x;
        maxOffsetY = CameraUtils.halfHeight - coinRoot.transform.localScale.y;
        InvokeRepeating("SpawnCoin", 3, 2); // starts spawning 3s after start of round, then once every 2s
        playerRef.OnDeath += CancelInvoke;
        playerRef.OnObjectiveReached += CancelInvoke;
    }
    void SpawnCoin(){
        if(coinsOnScreen >= MAX_COINS_ON_SCREEN){
            Vector3 spawnPoint = transform.position + RandomSpawnOffset();
            // then check if spawnPoint is over UI element
            Vector3 distanceToHealthBar = healthBarRef.transform.position - spawnPoint;
            float minDistX=2, minDistY=5;
            if(distanceToHealthBar.x < minDistX && distanceToHealthBar.y < minDistY){
                spawnPoint += Vector3.left*minDistX;
            }
            // check if spawnPoint is too close to player
            Vector3 distanceToPlayer = playerRef.transform.position - spawnPoint;
            float minDistance = 10;
            if(distanceToPlayer.magnitude < minDistance){
                spawnPoint += distanceToPlayer.normalized * minDistance;
            }

            GameObject coinHandle = Instantiate<GameObject>(coinRoot, spawnPoint, Quaternion.identity);
            coinsOnScreen++;
        }
    }
    Vector3 RandomSpawnOffset(){        
        float offsetX = Random.Range(-maxOffsetX, maxOffsetX);
        float offsetY = Random.Range(-maxOffsetY, maxOffsetY);

        Vector3 position = new Vector3(offsetX, offsetY);
        return position;   
    }
    public void PlayPickupSound(int coinCount){
        coinsOnScreen--;
        audio.PlayOneShot(pickupSound);
        // sound goes higher pitch on more coins?
    }
}
