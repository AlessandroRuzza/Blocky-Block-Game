using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgMusic : MonoBehaviour
{
    Animator audioTransition;
    new AudioSource audio;
    float startVolume;
    void Start(){
        audio = gameObject.GetComponent<AudioSource>();
        startVolume = audio.volume;
        audioTransition = gameObject.GetComponent<Animator>();
        TimeUtils.OnPause += FadeOut;
        TimeUtils.OnResume += FadeIn;
        TimeUtils.OnReset += FadeIn;
        FadeIn();
    }
    void OnDestroy(){
        TimeUtils.OnPause -= FadeOut;
        TimeUtils.OnResume -= FadeIn;
        TimeUtils.OnReset -= FadeIn;
    }
    public void FadeOut(){
        audioTransition.SetTrigger("fadeOut");
    }
    public void FadeIn(){
        audioTransition.SetTrigger("fadeIn");
    }  
    public void Mute(){  
        audio.mute = true;
    }
    public void ResetVolume(){  
        audio.volume = startVolume;
    }
}
