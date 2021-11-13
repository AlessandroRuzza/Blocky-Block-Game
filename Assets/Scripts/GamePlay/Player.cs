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
    [SerializeField] float speed;
    public bool isImmortal=false;
    public event System.Action OnDamage, OnDeath, OnObjectiveReached;
    public event System.Action<int> OnCoinPickup; 
    public static int MAX_HEALTH=3, COIN_TARGET=8;
    [SerializeField] int coinCounter=0;
    public int health { get; private set; }
    public bool isAlive {
        get{ return health>0; }
    }
    public Vector3 resetPosition;
    void Awake()
    {
        resetPosition = transform.position;
        coinCounter=0;
        isImmortal=false;
        health=MAX_HEALTH;
        screenBounds.x = CameraUtils.halfWidth ;  //to revert to old exploitable loop, add:   + transform.localScale.x/2f;
        screenBounds.y = CameraUtils.halfHeight - transform.localScale.y/2f;
        TimeUtils.OnReset += Reset;
    }
    public void Reset(){                        
        coinCounter=0;
        health=MAX_HEALTH;
        transform.position = resetPosition;
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
    }
    void OnDestroy(){
        OnDamage=null;
        OnDeath=null;
        OnObjectiveReached=null;
        OnCoinPickup=null;
    }
    void HandleMovement(float speed){
        Vector3 move;
        move = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        move = move.normalized*speed;
        
        transform.Translate(move*Time.deltaTime);
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
        if(triggerCollider.tag == "Cube" && isAlive && !isImmortal){
            health--;
            if(OnDamage != null) OnDamage();
            Destroy(triggerCollider.gameObject);
        }
        else if(triggerCollider.tag == "Coin" && coinCounter < COIN_TARGET){
            coinCounter++;
            if(OnCoinPickup != null) OnCoinPickup(coinCounter);
            //print("coin! " + coinCounter);
            Destroy(triggerCollider.gameObject);
        }
    }
    void CheckDeath(){
        if(!isAlive){
            StartCoroutine(TimeUtils.Pause());
            if(OnDeath != null) OnDeath();
        }
    }
    void CheckObjectiveComplete(){
        if(coinCounter >= COIN_TARGET && !TimeUtils.isPaused && isAlive){
            //print("CONGRATS!");
            isImmortal = true;
            if(OnObjectiveReached != null) OnObjectiveReached();
        }
    }
    public void Kill(){
        health=0;
        CheckDeath();
    }
}
