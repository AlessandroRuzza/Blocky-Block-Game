using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCompleteScreen : MonoBehaviour
{
    Player playerRef;
    public GameObject healthBar;
    void Start()
    {
        playerRef = FindObjectOfType<Player>();
        playerRef.OnObjectiveReached += Show;
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
