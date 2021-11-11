using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    static GameObject instance;
    void Awake()
    {
        if(instance != null){
            Destroy(gameObject);
        }
        else{
            instance = gameObject;
            DontDestroyOnLoad(gameObject);
        }
    }
}
