using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed, startScale, startFixedScale;
    public event System.Action OnReset, OnDamage, OnDeath, OnPause, OnResume; 
    public static int MAX_HEALTH=4, COIN_TARGET=5;
    public int coinCounter, health;
    public bool paused, pauseDone;
    public Vector3 resetPosition;
    float halfScreenWidth, halfScreenHeight;
    Vector3 move;
    void Start()
    {
        resetPosition = transform.position;
        halfScreenWidth = Camera.main.orthographicSize * Camera.main.aspect + transform.localScale.x/2f;
        halfScreenHeight = Camera.main.orthographicSize - transform.localScale.y/2f;
        startScale = Time.timeScale;
        startFixedScale = Time.fixedDeltaTime;
        Reset();
    }
    public void Reset(){
        transform.position = resetPosition;
        Time.timeScale = startScale;
        Time.fixedDeltaTime = startFixedScale;
        health = MAX_HEALTH;
        coinCounter=0;
        if(OnReset != null)
            OnReset();
        paused = false;
    }
    void Update()
    {
        // Handle Player Events
        HandleMovement(speed);
        CheckLoopAround();
        if(!paused){
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
        if(coinCounter >= COIN_TARGET){
            print("CONGRATS!");
            Pause();
            coinCounter = 0;
        }
    }
    public void Pause(float transitionTime=1f, int numStep=-50){
        StopCoroutine("TimeTransition");
        paused = true;
        StartCoroutine(TimeTransition(numStep, transitionTime));
    }
    public void Resume(float transitionTime=1f, int numStep=250){
        StopCoroutine("TimeTransition");
        paused = false;
        StartCoroutine(TimeTransition(numStep, transitionTime));
    }
    IEnumerator TimeTransition(int numStep, float transitionTime){
        float stepTime = Mathf.Abs(transitionTime/numStep);  // transitionSeconds / steps = interval between steps
        float stepSize = startScale/numStep;            // is negative for pausing, positive for unpausing
        float stepFixedSize = startFixedScale/numStep;  // is negative for pausing, positive for unpausing

        for(int i=0; i<Mathf.Abs(numStep); i++){
            Time.timeScale += stepSize;
            Time.fixedDeltaTime += stepFixedSize;
            //print("Scaled: " + Time.timeScale + " Fixed: " + Time.fixedDeltaTime); debug
            yield return new WaitForSecondsRealtime(stepTime);
        }
        if(numStep>0){
            Time.timeScale = startScale;
            Time.fixedDeltaTime = startFixedScale;
            pauseDone=false;
            if(OnResume != null) OnResume();
        }
        else{
            Time.timeScale = 0;
            Time.fixedDeltaTime = 0;
            pauseDone=true;
            if(OnPause != null) OnPause();
        }
    }

    /* FRAME BASED PAUSE (laggy cubes)

    void gradualPause(){
        const float smoothSeconds = 1f;
        float pauseSpeed = startScale/smoothSeconds;
        float reduceBy = pauseSpeed*Time.deltaTime;

        if(Time.timeScale > reduceBy)
            Time.timeScale -= reduceBy;
        else{
            Time.timeScale = 0;
            paused = false; 
            print("Done");
        }
    }
    */

}
