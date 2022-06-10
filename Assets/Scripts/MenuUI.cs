using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        musicSlider.value = AudioManager.instance.GetMusicVolume();
        sfxSlider.value = AudioManager.instance.GetSFXVolume();
    }

    public void Pilot()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("Game");
        AudioManager.instance.Stop("Menu");
        AudioManager.instance.Play("Ambient1");
    }
    
   public void Quit()
   {
        Application.Quit();
   }

   public void ChangeMusicVolume(System.Single _value)
   {
       AudioManager.instance.SetVolume(true, _value);
   }

   public void ChangeSFXVolume(System.Single _value)
   {
       AudioManager.instance.SetVolume(false, _value);
   }

   public void Click()
   {
       AudioManager.instance.Play("UIClick");
   }
   
   public void Select()
   {
       AudioManager.instance.Play("UISelect");
   }
}