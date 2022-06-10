using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] private Controller controller;
    
    [Header("HUD")]
    [SerializeField] private Image speedArrow;
    [SerializeField] private Image altBigArrow;
    [SerializeField] private Image altLittleArow;
    [SerializeField] private Image compassArrow;
    [SerializeField] private GameObject brakesOn;
    [SerializeField] private GameObject brakesOff;
    [SerializeField] private Text flapText;
    [SerializeField] private Image flapArrow;
    [SerializeField] private Image thrustArrow;
    [SerializeField] private Image altIndUpDown;
    [SerializeField] private Image altIndLeftRight;

    [Header("Inputs")]
    [SerializeField] private TMP_Text[] inputsTexts;
    [SerializeField] private TMP_SpriteAsset keyboard_SA;
    [SerializeField] private TMP_SpriteAsset ps4_SA;
    [SerializeField] private TMP_SpriteAsset xbox_SA;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        musicSlider.value = AudioManager.instance.GetMusicVolume();
        sfxSlider.value = AudioManager.instance.GetSFXVolume();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpeed();
        UpdateAltitude();
        UpdateCompass();
        UpdateBrakes();
        UpdateFlaps();
        UpdateThrust();
        UpdateAltInd();

        switch (controller.currentControlScheme)
        {
            case ControlScheme.Keyboard:
                SetSpriteAsset(keyboard_SA);
                break;
            case ControlScheme.Gamepad:
                SetSpriteAsset(Gamepad.current.name.Contains("DualShock") ? ps4_SA : xbox_SA);
                break;
            case ControlScheme.Hotas:
                break;
        }
        
        pauseMenu.SetActive(!GameManager.Instance.gameIsOn);
    }

    void UpdateSpeed()
    {
        float speed = (controller.rb.velocity.magnitude * 3.6f / 1.852f);

        if (speed < 40)
        {
            speedArrow.transform.eulerAngles = Vector3.back * 0.35f * speed;
        }
        else
        {
            speedArrow.transform.eulerAngles = Vector3.back * 1.8f * (speed - 40) + Vector3.back * 0.35f * 40;
        }
    }

    void UpdateAltitude()
    {
        float alt = (controller.transform.position.y * 0.33f) * 10;
        float thousands = alt / 1000;
        float hundreds = (alt - (int) thousands * 1000) / 100;
        
        altBigArrow.transform.eulerAngles = Vector3.back * 36f * thousands;
        altLittleArow.transform.eulerAngles = Vector3.back * 36f * hundreds;
    }

    void UpdateCompass()
    {
        compassArrow.transform.eulerAngles = Vector3.forward * controller.transform.rotation.eulerAngles.y;
    }

    void UpdateBrakes()
    {
        brakesOff.SetActive(!(controller.brakesTorque > 0));
        brakesOn.SetActive(controller.brakesTorque > 0);
    }

    void UpdateFlaps()
    {
        float flaps = controller.Flap * 100;
        flapText.text = flaps + "°";

        switch ((int)flaps)
        {
            case 0:
                flapArrow.rectTransform.anchoredPosition = new Vector2(7, 32f);
                break;
            case 10:
                flapArrow.rectTransform.anchoredPosition = new Vector2(7, 10.5f);
                break;
            case 20:
                flapArrow.rectTransform.anchoredPosition = new Vector2(7, -10.5f);
                break;
            case 30:
                flapArrow.rectTransform.anchoredPosition = new Vector2(7, -32f);
                break;
        }
    }

    void UpdateThrust()
    {
        float thrust = controller.currentThrust / 100;

        thrustArrow.transform.eulerAngles = new Vector3(0, 0, 139f) - Vector3.forward * 7.8f * thrust;
    }

    void UpdateAltInd()
    {
        float xRot = controller.transform.eulerAngles.x;
        xRot = (xRot > 180) ? xRot - 360 : xRot;
        xRot = Mathf.Clamp(xRot, -45, 45);
        
        altIndUpDown.rectTransform.localPosition = new Vector3(0, xRot, 0);

        float zRot = controller.transform.eulerAngles.z;
        zRot = (zRot > 180) ? zRot - 360 : zRot;
        zRot = Mathf.Clamp(zRot, -90, 90);
        
        altIndLeftRight.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -zRot));
        altIndUpDown.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -zRot));
    }

    void SetSpriteAsset(TMP_SpriteAsset _sa)
    {
        foreach (var text in inputsTexts)
        {
            text.spriteAsset = _sa;
        }
    }
    
    #region PAUSE_MENU
    public void Resume()
    {
        Cursor.visible = false;
        GameManager.Instance.PauseGame();
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
        AudioManager.instance.Stop("Ambient1");
        AudioManager.instance.Stop("Engine");
        AudioManager.instance.Play("Menu");
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
    #endregion
}
