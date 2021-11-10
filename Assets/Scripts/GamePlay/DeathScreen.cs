using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    Player playerRef;
    public GameObject healthBar;
    void Start()
    {
        playerRef = FindObjectOfType<Player>();
        playerRef.OnDeath += Show;
        Hide();
    }

    void Show(){
        CanvasGroup obj = gameObject.GetComponent<CanvasGroup>();
        obj.alpha = 1f;
        obj.interactable = true;
        obj.blocksRaycasts = true;
        healthBar.SetActive(false);
    }
    void Hide(){
        CanvasGroup obj = gameObject.GetComponent<CanvasGroup>();
        obj.alpha = 0f;
        obj.interactable = false;
        obj.blocksRaycasts = false;
        healthBar.SetActive(true);
    }
}
