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
    bool hasBeenDamaged=false;
    Vector3 offsetToPlayer;
    void Start()
    {
        playerRef.OnDamage += OnFirstDamage;
        playerRef.OnCoinPickup += PickedCoin;
        offsetToPlayer = showPlayerRef.transform.position - playerRef.transform.position;
        tutorialStartRef.SetTrigger("fadeIn");
        Invoke("AdvanceGuideFadeIn", 2);
    }
    void Update()
    {
        if(state==0){
            // keeps showPlayer in the same spot relative to Player
            showPlayerRef.transform.position = playerRef.transform.position + offsetToPlayer;
            if(Input.GetKeyDown(KeyCode.Space))
                Advance();
        }
        else if(state == 1 && testCube == null && !hasBeenDamaged){    // if testCube has not hit player
            StartCoroutine("CubeTestPassed");
            state++;
        }
    }
    void AdvanceGuideFadeIn(){
        advanceGuideRef.SetTrigger("fadeIn");
    }
    void AdvanceGuideFadeOut(){
        advanceGuideRef.SetTrigger("fadeOut");
    }
    void Advance(){
        state++;
        switch(state){
            case 1:
                AdvanceGuideFadeOut();
                tutorialStartRef.SetTrigger("fadeOut");
                warningTextRef.SetTrigger("fadeIn");
                testCube = tutorialSpawnerRef.SpawnCube(false);
                break;
            case 2:
            case 3:  
                AdvanceGuideFadeOut();
                coinGuideRef.SetTrigger("fadeIn");
                Instantiate(coinRoot, Vector3.zero, Quaternion.identity);
                break;
            
            default: 
                Debug.Log("Index too high!");
                break;
        }
    }
    IEnumerator CubeTestPassed(){
        AdvanceGuideFadeIn();
        warningTextRef.SetTrigger("fadeOut");
        StartCoroutine(TimeUtils.Pause());
        cubeTestPassedAnimator.SetTrigger("fadeIn");
        while(!Input.GetKeyDown(KeyCode.Space)){   
            yield return null;
        }
        cubeTestPassedAnimator.SetTrigger("fadeOut");
        AdvanceGuideFadeOut();
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
        AdvanceGuideFadeIn();
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;
        firstDamageGuideRef.alpha = 0f;
        firstDamageGuideRef.interactable = false;
        firstDamageGuideRef.blocksRaycasts = false;
        playerRef.Reset();
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
    void PickedCoin(int coinC=0){   // param needed to add to Player's OnCoinPickup action
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.PlayOneShot(pickupSound);
        playerRef.Kill();
        coinGuideRef.gameObject.SetActive(false);
        playerRef.gameObject.SetActive(false); //hide player
    }
}