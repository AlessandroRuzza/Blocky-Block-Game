using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SceneChanger : MonoBehaviour {
    public static event Action OnPlay, OnTutorial, OnQuit, OnTitleScreen;
    void Start(){
        //DontDestroyOnLoad(gameObject);
    }
    public static void ChangeScene(string name){
        TimeUtils.ResetTime();
        switch(name){
            case "GamePlay":
                if(OnPlay != null) OnPlay();
                break;
            case "Tutorial":
                if(OnTutorial != null) OnTutorial();
                break;
            case "TitleScreen":
                if(OnTitleScreen != null) OnTitleScreen();
                break;

            default: print("Nome scena errato:  " + name);  // DEBUG
                break;
        }
        
        SceneManager.LoadScene(name);
    }
    public static void Quit(){
        if(OnQuit != null) OnQuit();

        Application.Quit(0);
    }
}
