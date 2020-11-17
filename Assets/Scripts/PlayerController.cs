using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float pitchSpeed = 1.0f;
    public float yawSpeed = 1.0f;
    public Transform camera;

    private float m_yaw = 0.0f;
    private float m_pitch = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float mouse_x = Input.GetAxis("Mouse X");
        float mouse_y = Input.GetAxis("Mouse Y");
        m_pitch = Mathf.Clamp(m_pitch - mouse_y * pitchSpeed, -85.0f, 85.0f);
        m_yaw = Mathf.Repeat(m_yaw + mouse_x * yawSpeed, 360.0f);
        camera.transform.localRotation = Quaternion.Euler(m_pitch, m_yaw, 0.0f);
    }
}
