using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Player playerRef;
    Slider healthBar;
    new AudioSource audio;
    public AudioClip damageSound; 
    public ParticleSystem damageEmitter;
    void Start()
    {
        healthBar = gameObject.GetComponent<Slider>();
        audio = gameObject.GetComponent<AudioSource>();
        playerRef = FindObjectOfType<Player>();
        playerRef.OnDamage += EmitDamage;
        playerRef.OnDamage += PlayDamageSound;
        healthBar.maxValue = Player.MAX_HEALTH;
    }
    public void Reset(){
        healthBar.value = healthBar.maxValue;
    }
    void Update()
    {
        healthBar.value = playerRef.hp;
    }

    void EmitDamage(){
        damageEmitter.Play();
    }
    void PlayDamageSound(){
        audio.PlayOneShot(damageSound);
    }
}
