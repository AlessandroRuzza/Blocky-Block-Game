using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/**********
* Tutorial Structure
* - Indicate HealthBar and Player
* - Drop 1 cube for simulation
*   - If avoided:  praise and do practice round until damaged
* - At first damage:  pause and explain damage and show HealthBar
* - Start/Continue practice round until death
* - Show DeathScreen and finish tutorial
*********/
public class TutorialManager : MonoBehaviour
{
    
    public Player playerRef;
    public CubeSpawner tutorialSpawnerRef;
    public GameObject showPlayerRef, coinRoot;
    public CanvasGroup firstDamageGuideRef;
    public Animator tutorialStartRef, warningTextRef, coinGuideRef, cubeTestPassedAnimator;
    public GameObject cube;
    GameObject testCube;
    public AudioClip pickupSound;
    int state=0;
    bool hasBeenDamaged=false;
    void Start()
    {
        playerRef.OnDamage += OnFirstDamage;
        playerRef.OnCoinPickup += PickedCoin;
        tutorialStartRef.SetTrigger("fadeIn");
    }
    void Update()
    {
        if(state==0){       //only during first tutorial phase
            HandleShowPlayerMovement();
            if(Input.GetKeyDown(KeyCode.Space))
                Advance();
        }
        else if(state == 1 && testCube == null && !hasBeenDamaged){    // if testCube has not hit player
            StartCoroutine("CubeTestPassed");
            state++;
        }
    }
    /* Possible States of tutorial
        
        0:  Indicate HealthBar and Player
        1:  Drop 1 cube for simulation
            Praise if avoided, joke if damaged
            Continue dropping round until damage
            Pause and explain damage and show HealthBar
        2:  Start test round
        3:  Show DeathScreen and finish tutorial   
    */
    void Advance(){
        state++;
        switch(state){
            case 1:
                tutorialStartRef.SetTrigger("fadeOut");
                warningTextRef.SetTrigger("fadeIn");
                testCube = tutorialSpawnerRef.SpawnCube(false);
                break;
            case 2:
            case 3:  
                coinGuideRef.SetTrigger("fadeIn");
                Instantiate(coinRoot, Vector3.zero, Quaternion.identity);
                break;
            
            default: 
                Debug.Log("Index too high!");
                break;
        }
    }
    void HandleShowPlayerMovement(){
        Vector3 move;
        float speed = playerRef.speed;
        if(Input.GetKey(KeyCode.LeftShift))
            speed *= 2;
        move = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        move = move.normalized*speed;
        
        showPlayerRef.transform.position += move*Time.deltaTime;
    }
    IEnumerator CubeTestPassed(){
        warningTextRef.SetTrigger("fadeOut");
        StartCoroutine(TimeUtils.Pause());
        cubeTestPassedAnimator.SetTrigger("fadeIn");
        while(!Input.GetKeyDown(KeyCode.Space)){   
            yield return null;
        }
        cubeTestPassedAnimator.SetTrigger("fadeOut");
        StartCoroutine(TimeUtils.Resume());
        ToggleSpawner(true);
    }
    void OnFirstDamage(){
        warningTextRef.SetTrigger("fadeOut");
        if(!hasBeenDamaged){
            playerRef.isImmortal=true;
            hasBeenDamaged = true; 
            ToggleSpawner(false); 
            playerRef.OnDamage -= OnFirstDamage;
            StartCoroutine(TimeUtils.Pause(0.5f));
            firstDamageGuideRef.alpha = 1f;
            firstDamageGuideRef.interactable = true;
            firstDamageGuideRef.blocksRaycasts = true;
            StartCoroutine("AfterFirstDamage");
        }
    } 
    IEnumerator AfterFirstDamage(){
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
        firstDamageGuideRef.alpha = 0f;
        firstDamageGuideRef.interactable = false;
        firstDamageGuideRef.blocksRaycasts = false;
        playerRef.transform.position = playerRef.resetPosition;
        StartCoroutine(TimeUtils.Resume());
        Advance();
    }
    void ToggleSpawner(bool spawn){
        if(spawn){    // on wrong state => exit function
            tutorialSpawnerRef.StartSpawner();
        }
        else{
            tutorialSpawnerRef.StopSpawner();
        }
    }
    void PickedCoin(){
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot(pickupSound);
        playerRef.Kill();
        coinGuideRef.gameObject.SetActive(false);
        playerRef.gameObject.SetActive(false); //hide player
    }
}