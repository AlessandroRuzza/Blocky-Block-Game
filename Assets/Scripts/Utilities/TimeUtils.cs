using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class TimeUtils
{
    static bool paused, pauseDone;
    public static event Action OnPause, OnResume;
    static float startScale=1f, startFixedScale=0.02f;
    public static bool isPaused { get{ return paused; }}
    public static bool isPauseDone { get{ return pauseDone; }}
    public static void ResetTime(){
        Time.timeScale = startScale;
        Time.fixedDeltaTime = startFixedScale;
        paused = false;
        pauseDone = false;
    }
    public static void SetTimeScales(float timeScale, float fixedDeltaTime){
        startScale = timeScale;
        startFixedScale = fixedDeltaTime;
        ResetTime();
    }
    public static IEnumerator Pause(float transitionTime=1f){
        paused = true;
        float stepTime = 0.01f;       
        int numStep = Mathf.RoundToInt(transitionTime/stepTime);          // transitionSeconds / interval between steps = number of steps
        float stepSize = startScale/numStep;            
        float stepFixedSize = startFixedScale/numStep;  

        OnPause();
        for(int i=0; i<numStep; i++){
            Time.timeScale -= stepSize;
            Time.fixedDeltaTime -= stepFixedSize;
            // print("Scaled: " + Time.timeScale + " Fixed: " + Time.fixedDeltaTime); debug
            yield return new WaitForSecondsRealtime(stepTime);
        }
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;
        pauseDone=true;
    }
    public static IEnumerator Resume(float transitionTime=1f){
        paused = false;
        float stepTime = 0.01f;       
        int numStep = Mathf.RoundToInt(transitionTime/stepTime);          // transitionSeconds / interval between steps = number of steps
        float stepSize = startScale/numStep;            
        float stepFixedSize = startFixedScale/numStep; 

        OnResume();
        for(int i=0; i<numStep; i++){
            Time.timeScale += stepSize;
            Time.fixedDeltaTime += stepFixedSize;
            //print("Scaled: " + Time.timeScale + " Fixed: " + Time.fixedDeltaTime); debug
            yield return new WaitForSecondsRealtime(stepTime);
        }
        Time.timeScale = startScale;
        Time.fixedDeltaTime = startFixedScale;
        pauseDone=false;
    }
}