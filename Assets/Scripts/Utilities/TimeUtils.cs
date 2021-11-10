using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TimeUtils : MonoBehaviour
{
    static bool paused, pauseDone;
    public static float startScale=1f, startFixedScale=0.02f;
    public static bool isPaused { get{ return paused; }}
    public static bool isPauseDone { get{ return pauseDone; }}
    public void ResetTime(){
        Time.timeScale = startScale;
        Time.fixedDeltaTime = startFixedScale;
    }
    public void Pause(float transitionTime=1f, int numStep=-50){
        StopCoroutine("TimeTransition");
        paused = true;
        StartCoroutine(TimeTransition(numStep, transitionTime));
    }
    public void Resume(float transitionTime=1f, int numStep=50){
        StopCoroutine("TimeTransition");
        paused = false;
        StartCoroutine(TimeTransition(numStep, transitionTime));
    }
    static IEnumerator TimeTransition(int numStep, float transitionTime){
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
        }
        else{
            Time.timeScale = 0;
            Time.fixedDeltaTime = 0;
            pauseDone=true;
        }
    }
}