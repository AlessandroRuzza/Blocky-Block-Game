using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCompleteScreen : MonoBehaviour
{
    [SerializeField] Player playerRef;
    [SerializeField] GameObject healthBar;
    void Start()
    {
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
