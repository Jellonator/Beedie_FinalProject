using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float gravityScale = 1.0f;
    public float pitchSpeed = 1.0f;
    public float yawSpeed = 1.0f;
    public float moveSpeed = 1.0f;
    public float jumpSpeed = 1.0f;
    public Transform viewCamera;

    private float m_yaw = 0.0f;
    private float m_pitch = 0.0f;

    private Rigidbody m_Rigidbody;
    private float m_TimeSinceJumpPressed = 1.0f;
    private int m_mask;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_mask = LayerMask.GetMask(new string [] {"Solid"});
    }

    void Update()
    {
        // Handle jump
        if (Input.GetKeyDown("space")) {
            m_TimeSinceJumpPressed = 0.0f;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // handle camera movement
        float mouse_x = Input.GetAxis("Mouse X");
        float mouse_y = Input.GetAxis("Mouse Y");
        m_pitch = Mathf.Clamp(m_pitch - mouse_y * pitchSpeed, -85.0f, 85.0f);
        m_yaw = Mathf.Repeat(m_yaw + mouse_x * yawSpeed, 360.0f);
        viewCamera.transform.localRotation = Quaternion.Euler(m_pitch, m_yaw, 0.0f);
        // handle WASD movement
        float move_x = Input.GetAxis("Horizontal");
        float move_y = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(move_x, move_y);
        if (movement.magnitude > 1.0f) {
            movement = movement.normalized;
        }
        Vector3 dir_forward = viewCamera.forward;
        dir_forward.y = 0.0f;
        dir_forward = dir_forward.normalized;
        Vector3 dir_right = viewCamera.right;
        dir_right.y = 0.0f;
        dir_right = dir_right.normalized;
        Vector3 new_velocity = (dir_forward * movement.y + dir_right * movement.x) * moveSpeed;
        m_Rigidbody.velocity = new Vector3(new_velocity.x, m_Rigidbody.velocity.y, new_velocity.z);
        // handle jump
        if (m_TimeSinceJumpPressed < 0.1f) {
            if (CanJump()) {
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, jumpSpeed, m_Rigidbody.velocity.z);
                m_TimeSinceJumpPressed = 1.0f;
            }
        }
        m_TimeSinceJumpPressed += Time.deltaTime;
        m_Rigidbody.velocity += Vector3.down * gravityScale * Time.deltaTime;
    }

    private bool CanJump() {
        Vector3 center = new Vector3(0.0f, 0.5f, 0.0f) + transform.position;
        Vector3 extents = new Vector3(0.5f, 0.1f, 0.5f);
        Vector3 direction = Vector3.down;
        Quaternion orientation = Quaternion.identity;
        return Physics.BoxCast(center, extents, direction, orientation, 0.5f, m_mask);
    }
}
