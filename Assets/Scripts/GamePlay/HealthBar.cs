using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Player playerRef;
    Slider healthBar;
    public ParticleSystem damageEmitter;
    void Start()
    {
        healthBar = gameObject.GetComponent<Slider>();
        playerRef = FindObjectOfType<Player>();
        playerRef.OnDamage += EmitDamage;
        healthBar.maxValue = Player.MAX_HEALTH;
    }
    public void Reset(){
        healthBar.maxValue = Player.MAX_HEALTH;
    }
    void Update()
    {
        healthBar.value = playerRef.hp;
    }

    void EmitDamage(){
        damageEmitter.Play();
    }

}
