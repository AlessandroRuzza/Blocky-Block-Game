using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

struct screenBounds{
    public static float x;
    public static float y;
}
public class Player : MonoBehaviour
{
    public float speed;
    public bool isImmortal;
    public event System.Action OnReset, OnDamage, OnDeath, OnObjectiveReached;
    public event System.Action<int> OnCoinPickup; 
    public static int MAX_HEALTH=3, COIN_TARGET=8;
    int coinCounter, health;
    public int hp { get { return health; } }
    public Vector3 resetPosition;
    void Awake()
    {
        resetPosition = transform.position;
        isImmortal=false;
        coinCounter=0;
        health=MAX_HEALTH;
        screenBounds.x = CameraUtils.halfWidth + transform.localScale.x/2f;
        screenBounds.y = CameraUtils.halfHeight - transform.localScale.y/2f;
        TimeUtils.OnReset += Reset;
    }
    public void Reset(){                        
        coinCounter=0;
        health=MAX_HEALTH;
        if(OnReset != null)
            OnReset();
    }
    void Update()
    {
        // Handle Player Events
        HandleMovement(speed);
        CheckLoopAround();
        if(!TimeUtils.isPaused){
            CheckDeath();
            CheckObjectiveComplete();
        }
        // reset on R pressed   ------------  TEMP DEBUG
        if(Input.GetKeyDown(KeyCode.R)){
            SceneChanger.ReloadCurrentScene();  
        }
    }
    void OnDestroy(){
        OnReset=null;
        OnDamage=null;
        OnDeath=null;
        OnObjectiveReached=null;
        OnCoinPickup=null;
    }
    void HandleMovement(float speed){
        Vector3 move;
        if(Input.GetKey(KeyCode.LeftShift))
            speed *= 2;
        move = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        move = move.normalized*speed;
        
        transform.position += move*Time.deltaTime;
    }
    void CheckLoopAround(){
        if(transform.position.x < -screenBounds.x)
            transform.position = new Vector3(screenBounds.x, transform.position.y);
        else if(transform.position.x > screenBounds.x)
            transform.position = new Vector3(-screenBounds.x, transform.position.y);

        if(transform.position.y < -screenBounds.y)
            transform.position = new Vector3(transform.position.x, -screenBounds.y);
        else if(transform.position.y > screenBounds.y)
            transform.position = new Vector3(transform.position.x, screenBounds.y);
    }
    void OnTriggerEnter2D(Collider2D triggerCollider){
        if(triggerCollider.tag == "Cube" && IsAlive() && !isImmortal){
            health--;
            if(OnDamage != null) OnDamage();
            Destroy(triggerCollider.gameObject);
        }
        else if(triggerCollider.tag == "Coin" && coinCounter < COIN_TARGET){
            coinCounter++;
            if(OnCoinPickup != null) OnCoinPickup(coinCounter);
            print("coin! " + coinCounter);
            Destroy(triggerCollider.gameObject);
        }
    }
    public bool IsAlive(){
        return health>0;
    }
    void CheckDeath(){
        if(!IsAlive()){
            StartCoroutine(TimeUtils.Pause());
            if(OnDeath != null) OnDeath();
        }
    }
    void CheckObjectiveComplete(){
        if(coinCounter >= COIN_TARGET && !TimeUtils.isPaused && IsAlive()){
            print("CONGRATS!");
            StartCoroutine(TimeUtils.Pause());
            if(OnObjectiveReached != null) OnObjectiveReached();
        }
    }
    public void Kill(){
        health=0;
        CheckDeath();
    }
}
