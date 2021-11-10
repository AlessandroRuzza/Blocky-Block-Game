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
    public Action<int> OnAdvance;
    /* Possible arguments
        
        0:  Indicate HealthBar and Player
        1:  Drop 1 cube for simulation
            Pause and explain damage and show HealthBar
        2:  Start/Continue practice round until death
        3:  Show DeathScreen and finish tutorial   
    */
    public Player playerRef;
    public CubeSpawner tutorialSpawnerRef;
    public GameObject showPlayerRef;
    public CanvasGroup tutorialStartRef, firstDamageGuideRef;
    public Animator warningTextRef;
    public Animator cubeTestPassedAnimator;
    public GameObject cube;
    GameObject testCube;
    int state=0;
    bool hasBeenDamaged=false, damageTutorialDone=false;
    void Start()
    {
        playerRef.OnDamage += OnFirstDamage;
        OnAdvance += TutorialStart;
        OnAdvance += CubeTest;
        OnAdvance += StartSpawner;
        TutorialStart();
    }
    void Update()
    {
        if(state==0){
            showPlayerHandleMovement();
        }
        else if(state == 1 && !damageTutorialDone && testCube == null && !hasBeenDamaged){    // if testCube has not hit player
                StartCoroutine("CubeTestPassed");
                damageTutorialDone = true;
        }

        if(Input.GetKeyDown(KeyCode.Space)){
                state++;
                print(state);
                OnAdvance(state);
            }
    }
    void TutorialStart(int state=0){
        if(state == 0){
            tutorialStartRef.alpha = 1f;
            tutorialStartRef.interactable = true;
            tutorialStartRef.blocksRaycasts = true;
        }
        else{
            tutorialStartRef.alpha = 0f;
            tutorialStartRef.interactable = false;
            tutorialStartRef.blocksRaycasts = false;
        }
    }
    void showPlayerHandleMovement(){
        Vector3 move;
        float speed = playerRef.speed;
        if(Input.GetKey(KeyCode.LeftShift))
            speed *= 2;
        move = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        move = move.normalized*speed;
        
        showPlayerRef.transform.position += move*Time.deltaTime;
    }
    void CubeTest(int state){
        if(state == 1){    // on wrong state => exit function
            warningTextRef.SetTrigger("fadeIn");
            testCube = tutorialSpawnerRef.SpawnCube(false);
        }
    }
    IEnumerator CubeTestPassed(){
        warningTextRef.SetTrigger("fadeOut");
        playerRef.Pause();
        cubeTestPassedAnimator.SetTrigger("fadeIn");
        while(!Input.GetKeyDown(KeyCode.Space)){   
            yield return null;
        }
        cubeTestPassedAnimator.SetTrigger("fadeOut");
        playerRef.Resume();
    }
    void OnFirstDamage(){
        if(!hasBeenDamaged){
            hasBeenDamaged = true;  
            playerRef.OnDamage -= OnFirstDamage;
            playerRef.Pause(0.5f);
            firstDamageGuideRef.alpha = 1f;
            firstDamageGuideRef.interactable = true;
            firstDamageGuideRef.blocksRaycasts = true;
            StartCoroutine("AfterFirstDamage");
        }
    } 
    IEnumerator AfterFirstDamage(){
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
        damageTutorialDone = true;
        firstDamageGuideRef.alpha = 0f;
        firstDamageGuideRef.interactable = false;
        firstDamageGuideRef.blocksRaycasts = false;
        playerRef.Resume();
    }
    void StartSpawner(int state){
        if(state == 2 && !tutorialSpawnerRef.isSpawning){    // on wrong state => exit function
            tutorialSpawnerRef.StartSpawner();
        }
    }
}