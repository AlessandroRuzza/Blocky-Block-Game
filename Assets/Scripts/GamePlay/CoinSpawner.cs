using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*******************        \\ is DONE
* Actual COIN: ideas
*   \\- Little ding sound on pickup
*   - Make nice particle effects (shiny)
*   - Add a rotation effect (might look bad)
* 
****************************/
public class CoinSpawner : MonoBehaviour
{
    [SerializeField] GameObject coinRoot;
    [SerializeField] GameObject healthBarRef, coinClusterRef;
    public int coinsOnScreen=0, MAX_COINS_ON_SCREEN=12;
    [SerializeField] Player playerRef;
    new AudioSource audio;
    [SerializeField] AudioClip pickupSound;
    float maxOffsetX, maxOffsetY;
    [SerializeField] float startWait=3f, coinSpawnRate=2f;
    public bool isSpawning { get; private set; }    // forces use of StartSpawn() and StopSpawn() 
    float timeWaited=0;
    void Start()
    {  
        audio = gameObject.GetComponent<AudioSource>();
        playerRef.OnCoinPickup += PlayPickupSound;
        maxOffsetX = CameraUtils.halfWidth - coinRoot.transform.localScale.x;
        maxOffsetY = CameraUtils.halfHeight - 2; // will avoid coins overlapping CoinCounter
        Invoke("StartSpawn", startWait);     //waits a bit after start of round
        playerRef.OnDeath += StopSpawn;
        playerRef.OnObjectiveReached += StopSpawn;
    }
    void Update(){
        if(timeWaited > coinSpawnRate && isSpawning){   // timeWaited accumulates value until it's more than coinspawn
            timeWaited = 0;     // resets timeWaited before next spawn
            SpawnCoin();
        }
        timeWaited += Time.deltaTime;
    }
    void SpawnCoin(){
        if(coinsOnScreen > MAX_COINS_ON_SCREEN){
            Debug.LogWarning("Too many coins on screen", this);
            return;
        }
        Vector3 spawnPoint = transform.position + RandomSpawnOffset();

        // then check if spawnPoint is over UI element
        float minDistX, minDistY;
        Vector3 distanceToHealthBar = healthBarRef.transform.position - spawnPoint;
        minDistX=2; minDistY=5;
        if(Mathf.Abs(distanceToHealthBar.x) < minDistX && Mathf.Abs(distanceToHealthBar.y) < minDistY){
            SpawnCoin();    // repeat the process (hoping it will be further away)
            return;         // quits after respawning coin
        }
        // check if spawnPoint is too close to player
        Vector3 distanceToPlayer = playerRef.transform.position - spawnPoint;
        minDistX=2; minDistY=3;
        if(Mathf.Abs(distanceToPlayer.x) < minDistX && Mathf.Abs(distanceToPlayer.y) < minDistY){
            SpawnCoin();    // repeat the process (hoping it will be further away)
            return;         // quits after respawning coin
        }

        GameObject coinHandle = Instantiate<GameObject>(coinRoot, spawnPoint, Quaternion.identity, coinClusterRef.transform);
        coinsOnScreen++;
    }
    public void StartSpawn(){
        isSpawning=true;
    }
    public void StopSpawn(){
        isSpawning=false;
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
