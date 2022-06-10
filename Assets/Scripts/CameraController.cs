using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float cameraSensitivity = 0.1f;
    
    public void ControlCamera(InputAction.CallbackContext ctx)
    {
        Vector2 look = ctx.ReadValue<Vector2>();
        
        // Horizontal rotation
        transform.rotation *= Quaternion.AngleAxis(look.x * cameraSensitivity, Vector3.up);
        
        // Vertical rotation
        transform.rotation *= Quaternion.AngleAxis(look.y * cameraSensitivity, Vector3.right);

        Vector3 localAngles = transform.localEulerAngles;
        localAngles.z = 0;

        float angleX = transform.localEulerAngles.x;

        if (angleX > 180 && angleX < 300)
        {
            localAngles.x = 300;
        }
        else if (angleX < 180 && angleX > 60)
        {
            localAngles.x = 60;
        }

        transform.localEulerAngles = localAngles;
    }
}
