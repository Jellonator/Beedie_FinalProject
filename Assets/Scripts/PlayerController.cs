using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float pitchSpeed = 1.0f;
    public float yawSpeed = 1.0f;
    public float moveSpeed = 1.0f;
    public Transform camera;

    private float m_yaw = 0.0f;
    private float m_pitch = 0.0f;

    private Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // handle camera movement
        float mouse_x = Input.GetAxis("Mouse X");
        float mouse_y = Input.GetAxis("Mouse Y");
        m_pitch = Mathf.Clamp(m_pitch - mouse_y * pitchSpeed, -85.0f, 85.0f);
        m_yaw = Mathf.Repeat(m_yaw + mouse_x * yawSpeed, 360.0f);
        camera.transform.localRotation = Quaternion.Euler(m_pitch, m_yaw, 0.0f);
        // handle WASD movement
        float move_x = Input.GetAxis("Horizontal");
        float move_y = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(move_x, move_y);
        if (movement.magnitude > 1.0f) {
            movement = movement.normalized;
        }
        Vector3 dir_forward = camera.forward;
        dir_forward.y = 0.0f;
        dir_forward = dir_forward.normalized;
        Vector3 dir_right = camera.right;
        dir_right.y = 0.0f;
        dir_right = dir_right.normalized;
        Vector3 new_velocity = (dir_forward * movement.y + dir_right * movement.x) * moveSpeed;
        m_Rigidbody.velocity = new Vector3(new_velocity.x, m_Rigidbody.velocity.y, new_velocity.z);
    }
}
