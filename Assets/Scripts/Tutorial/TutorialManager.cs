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
    
    [SerializeField] Player playerRef;
    [SerializeField] CubeSpawner tutorialSpawnerRef;
    [SerializeField] GameObject showPlayerRef, coinRoot;
    [SerializeField] CanvasGroup firstDamageGuideRef;
    [SerializeField] Animator advanceGuideRef, tutorialStartRef, warningTextRef, coinGuideRef, cubeTestPassedAnimator;
    [SerializeField] GameObject cube;
    GameObject testCube;
    [SerializeField] AudioClip pickupSound;
    int state=0;
    bool hasBeenDamaged=false, endDamageTutorial=false, continueSpawn=false, cubeTestPassed=false;
    Vector3 offsetToPlayer;
    void Start()
    {
        playerRef.OnDamage += OnFirstDamage;
        playerRef.OnCoinPickup += PickedCoin;
        offsetToPlayer = showPlayerRef.transform.position - playerRef.transform.position;
        tutorialStartRef.SetTrigger("fadeIn");
        Invoke("AdvanceGuideFadeIn", 2); // fades in after 2s
    }
    void Update()
    {
        if(state==0){
            // keeps showPlayer in the same spot relative to Player
            showPlayerRef.transform.position = playerRef.transform.position + offsetToPlayer;
        }
        else if(state == 1 && testCube == null && !hasBeenDamaged && !cubeTestPassed){    // if testCube has not hit player
            StartCoroutine("CubeTestPassed");
            cubeTestPassed=true;
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
    void AdvanceGuideFade(string trigger="fadeIn"){
        advanceGuideRef.SetTrigger(trigger);
    }
    void AdvanceGuideFadeIn(){      // just for the initial Invoke
        AdvanceGuideFade("fadeIn");
    }
    public void Advance(){
        state++;
        Debug.Log("state: " + state);
        switch(state){
            case 1:
                AdvanceGuideFade("fadeOut");
                tutorialStartRef.SetTrigger("fadeOut");
                warningTextRef.SetTrigger("fadeIn");
                testCube = tutorialSpawnerRef.SpawnCube(false);
                break;
            case 2: break;
            case 3: 
                AdvanceGuideFade("fadeOut"); 
                coinGuideRef.SetTrigger("fadeIn");
                Instantiate(coinRoot, Vector3.zero, Quaternion.identity);
                break;
            
            default: 
                Debug.Log("State index too high!");
                break;
        }
    }
    IEnumerator CubeTestPassed(){
        warningTextRef.SetTrigger("fadeOut");
        StartCoroutine(TimeUtils.Pause());
        cubeTestPassedAnimator.SetTrigger("fadeIn");
        AdvanceGuideFade("fadeIn");
        while(!continueSpawn){   
            yield return null;
        }
        cubeTestPassedAnimator.SetTrigger("fadeOut");
        StartCoroutine(TimeUtils.Resume());
        ToggleSpawner(true);
    }
    void OnFirstDamage(){
        if(!hasBeenDamaged){
            warningTextRef.SetTrigger("fadeOut");
            AdvanceGuideFade("fadeIn");
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
    public void ConditionalAdvance(){
        if(hasBeenDamaged)
            endDamageTutorial=true;
        else if(cubeTestPassed)
            continueSpawn=true;
    }
    IEnumerator AfterFirstDamage(){
        while (!endDamageTutorial)
            yield return null;
        firstDamageGuideRef.alpha = 0f;
        firstDamageGuideRef.interactable = false;
        firstDamageGuideRef.blocksRaycasts = false;
        playerRef.Reset();
        StartCoroutine(TimeUtils.Resume());
    }
    void ToggleSpawner(bool spawn){
        if(spawn){    // on wrong state => exit function
            tutorialSpawnerRef.StartSpawner();
        }
        else{
            tutorialSpawnerRef.StopSpawner();
        }
    }
    void PickedCoin(int coinC=0){   // param needed to add to Player's OnCoinPickup action
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot(pickupSound);
        playerRef.Kill();
        coinGuideRef.gameObject.SetActive(false);
        playerRef.gameObject.SetActive(false); //hide player
    }
}