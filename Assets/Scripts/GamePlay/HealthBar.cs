using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Player playerRef;
    Slider healthBar;
    new AudioSource audio;
    [SerializeField] AudioClip damageSound; 
    [SerializeField] ParticleSystem damageEmitter;
    void Start()
    {
        healthBar = gameObject.GetComponent<Slider>();
        audio = gameObject.GetComponent<AudioSource>();
        playerRef.OnDamage += EmitDamage;
        playerRef.OnDamage += PlayDamageSound;
        healthBar.maxValue = Player.MAX_HEALTH;
    }
    public void Reset(){
        healthBar.value = healthBar.maxValue;
    }
    void Update()
    {
        healthBar.value = playerRef.health;
    }

    void EmitDamage(){
        damageEmitter.Play();
    }
    void PlayDamageSound(){
        audio.PlayOneShot(damageSound);
    }
}
