using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum ControlScheme
{
    Keyboard,
    Gamepad,
    Hotas
}

public class Controller : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField]
    List<WheelCollider> wheels = null;
    
    [Header("Sensibility")]
    [SerializeField]
    float rollControlSensitivity = 0.2f;
    [SerializeField]
    float pitchControlSensitivity = 0.2f;
    [SerializeField]
    float yawControlSensitivity = 0.2f;

    [Header("Inputs values")]
    [Range(-1, 1)]
    public float Pitch;
    [Range(-1, 1)]
    public float Yaw;
    [Range(-1, 1)]
    public float Roll;
    [Range(0, 1)]
    public float Flap;

    public float currentThrust = 1200;
    public float brakesTorque = 100;

    Physics aircraftPhysics;
    public Rigidbody rb;

    private bool powerUp = false;
    private bool powerDown = false;
    private float powerTimer = 0.0f;

    [Header("Animation")]
    [SerializeField] private GameObject helix;

    [Header("Procedures")]
    [SerializeField] private GameObject proceduresCover;
    [SerializeField] private GameObject[] proceduresPages;
    private bool areProceduresOpen = false;
    private int currentProceduresPage = 0;
    [SerializeField] private GameObject landingPoints;
    private bool displayLandingPoints = false;

    private PlayerInput playerInput;
    [HideInInspector] public ControlScheme currentControlScheme;
    
    private void Start()
    {
        aircraftPhysics = GetComponent<Physics>();
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        
        AudioManager.instance.Play("Engine");
    }
    
    private void Update()
    {
        // Power Update
        if (powerUp)
        {
            powerTimer += Time.deltaTime;

            if (powerTimer > 0.05f)
            {
                currentThrust = Mathf.Clamp(currentThrust + 10, 0, aircraftPhysics.thrustMax);
                
                powerTimer = 0.0f;
            }
        }

        if (powerDown)
        {
            powerTimer += Time.deltaTime;

            if (powerTimer > 0.05f)
            {
                currentThrust = Mathf.Clamp(currentThrust - 10, 0, aircraftPhysics.thrustMax);
                
                powerTimer = 0.0f;
            }
        }

        AudioManager.instance.SetPitch("Engine", currentThrust / 1500);
        
        // Helix animation
        UpdateAnimation();

        // Get current control scheme (for UI)
        switch (playerInput.currentControlScheme)
        {
            case "Keyboard-Mouse":
                currentControlScheme = ControlScheme.Keyboard;
                break;
            case "Gamepad":
                currentControlScheme = ControlScheme.Gamepad;
                break;
            case "Hotas":
                currentControlScheme = ControlScheme.Hotas;
                break;
        }
    }

    private void FixedUpdate()
    {
        // Update aircraft physic
        SetControlSurfacesAngles(Pitch, Roll, Yaw, Flap);
        aircraftPhysics.SetCurrentThrust(currentThrust);
        foreach (var wheel in wheels)
        {
            wheel.brakeTorque = brakesTorque;
            wheel.motorTorque = 0.01f;
        }
    }

    private void SetControlSurfacesAngles(float pitch, float roll, float yaw, float flap)
    {
        foreach (var surface in aircraftPhysics.aerodynamicSurfaces)
        {
            if (surface == null || !surface.IsControlSurface) continue;
            switch (surface.InputType)
            {
                case ControlInputType.Pitch:
                    surface.SetFlapAngle(pitch * pitchControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Roll:
                    surface.SetFlapAngle(roll * rollControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Yaw:
                    surface.SetFlapAngle(yaw * yawControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Flap:
                    surface.SetFlapAngle(Flap * surface.InputMultiplyer);
                    break;
            }
        }
    }

    private void UpdateAnimation()
    {
        helix.transform.Rotate(0f, 0f, currentThrust * Time.deltaTime);
    }

    #region INPUT_SYSTEM
    public void UpdateRoll(InputAction.CallbackContext ctx)
    {
        Roll = ctx.ReadValue<float>();
    }
    
    public void UpdatePitch(InputAction.CallbackContext ctx)
    {
        Pitch = ctx.ReadValue<float>();
    }
    
    public void UpdateYaw(InputAction.CallbackContext ctx)
    {
        Yaw = ctx.ReadValue<float>();
    }

    public void UpdatePower(InputAction.CallbackContext ctx)
    {
        // if hotas
        if (ctx.control.displayName == "Z")
        {
            
        }
        
        float value = ctx.ReadValue<float>();

        if (value > 0)
        {
            powerUp = true;
        }
        else if (value < 0)
        {
            powerDown = true;
        }
        else
        {
            powerUp = false;
            powerDown = false;
        }
    }

    public void UpdateBrakes(InputAction.CallbackContext ctx)
    {
        brakesTorque = brakesTorque > 0 ? 0 : 100f;
    }

    public void UpdateFlaps(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();

        if (value > 0)
        {
            switch (Flap)
            {
                case 0:
                    Flap = 0.1f;
                    break;
                case 0.1f:
                    Flap = 0.2f;
                    break;
                case 0.2f:
                    Flap = 0.3f;
                    break;
            }
        }

        if (value < 0)
        {
            switch (Flap)
            {
                case 0.1f:
                    Flap = 0;
                    break;
                case 0.2f:
                    Flap = 0.1f;
                    break;
                case 0.3f:
                    Flap = 0.2f;
                    break;
            }
        }
    }

    public void OpenCloseProcedures(InputAction.CallbackContext ctx)
    {
        areProceduresOpen = !areProceduresOpen;

        proceduresCover.SetActive(!areProceduresOpen);
        proceduresPages[currentProceduresPage].SetActive(areProceduresOpen);
    }

    public void ProceduresNavigation(InputAction.CallbackContext ctx)
    {
        if (areProceduresOpen)
        {
            float value = ctx.ReadValue<float>();

            proceduresPages[currentProceduresPage].SetActive(false);
            
            if (value < 0)
            {
                currentProceduresPage--;
                
                if (currentProceduresPage == -1)
                    currentProceduresPage = proceduresPages.Length - 1;
            }
            else if (value > 0)
            {
                currentProceduresPage++;

                if (currentProceduresPage >= proceduresPages.Length)
                    currentProceduresPage = 0;
            }

            proceduresPages[currentProceduresPage].SetActive(true);
        }
    }

    public void DisplayLandingPoints(InputAction.CallbackContext ctx)
    {
        displayLandingPoints = !displayLandingPoints;
        landingPoints.SetActive(displayLandingPoints);
    }

    public void SetTimeScale(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();

        if (value < 0)
        {
            Time.timeScale = 2;
        }
        else if (value > 0)
        {
            Time.timeScale = 0.5f;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void Pause(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.PauseGame();
    }
    
    #endregion
    
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            SetControlSurfacesAngles(Pitch, Roll, Yaw, Flap);
    }
}
