using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D triggerCollider){
        if(triggerCollider.tag == "Cube")
            Destroy(triggerCollider.gameObject);
    }
}
