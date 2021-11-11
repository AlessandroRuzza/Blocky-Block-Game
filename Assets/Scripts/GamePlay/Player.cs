using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed;
    public event System.Action OnReset, OnDamage, OnDeath, OnPause, OnResume; 
    public static int MAX_HEALTH=4, COIN_TARGET=5;
    int coinCounter=0, health=MAX_HEALTH;
    public int hp { get {return health; } }
    public Vector3 resetPosition;
    float halfScreenWidth, halfScreenHeight;
    Vector3 move;
    void Start()
    {
        halfScreenWidth = Camera.main.orthographicSize * Camera.main.aspect + transform.localScale.x/2f;
        halfScreenHeight = Camera.main.orthographicSize - transform.localScale.y/2f;
    }
    public void Reset(){                        //TODO:  Fix the reset to SceneManager.LoadScene()
        if(OnReset != null)
            OnReset();
        TimeUtils.ResetTime();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);   // reloads current scene
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
            Reset();  
        }
    }
    void OnDisable(){
        OnReset=null;
        OnDamage=null;
        OnDeath=null;
        OnPause= null;
        OnResume= null;
    }
    void HandleMovement(float speed){
        if(Input.GetKey(KeyCode.LeftShift))
            speed *= 2;
        move = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        move = move.normalized*speed;
        
        transform.position += move*Time.deltaTime;
    }
    void CheckLoopAround(){
        if(transform.position.x < -halfScreenWidth)
            transform.position = new Vector3(halfScreenWidth, transform.position.y);
        else if(transform.position.x > halfScreenWidth)
            transform.position = new Vector3(-halfScreenWidth, transform.position.y);

        if(transform.position.y < -halfScreenHeight)
            transform.position = new Vector3(transform.position.x, -halfScreenHeight);
        else if(transform.position.y > halfScreenHeight)
            transform.position = new Vector3(transform.position.x, halfScreenHeight);
    }
    void OnTriggerEnter2D(Collider2D triggerCollider){
        if(triggerCollider.tag == "Cube" && IsAlive()){
            health--;
            if(OnDamage != null)
                OnDamage();
            Destroy(triggerCollider.gameObject);
        }
        else if(triggerCollider.tag == "Coin"){
            print("coin!");
            coinCounter++;
            Destroy(triggerCollider.gameObject);
        }
    }
    public bool IsAlive(){
        return health>0;
    }
    void CheckDeath(){
        if(!IsAlive()){
            Pause();
            if(OnDeath != null) OnDeath();
        }
    }
    void CheckObjectiveComplete(){
        if(coinCounter >= COIN_TARGET && !TimeUtils.isPaused){
            print("CONGRATS!");
            Pause();
            if(OnPause != null) OnPause();
        }
    }
    void Pause(){
        StartCoroutine(TimeUtils.Pause());
        if(OnPause != null) OnPause();
    }
}
