using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    /// The player's gravity scale
    public float gravityScale = 1.0f;
    /// The pitch speed for camera controls
    public float pitchSpeed = 1.0f;
    /// The yaw speed for camera controls
    public float yawSpeed = 1.0f;
    /// Player's move speed
    public float moveSpeed = 1.0f;
    /// Player's jump speed
    public float jumpSpeed = 1.0f;
    /// Amount of bees that the player can shoot per second
    public float beesPerSecond = 1.0f;
    /// Rev time
    public float revTime = 0.5f;
    /// Rev slowdown
    public float revSpeedScale = 0.5f;
    /// Rev animation speed; degrees per second
    public float revAnimationSpeed = 1.0f;
    /// Maximum ammo that the player can hold
    public int maximumAmmo = 10;
    /// Dot value that controls range in which bees will be returned when right clicking
    public float beeCrosshairRange = 0.5f;
    /// The Player's camera object
    public Transform viewCamera;
    /// The position that the player will shoot from
    public Transform beeShootPosition;
    /// The bee prefab that will be shot
    public GameObject beePrefab;
    /// The text object to update when changing ammo
    public Text ammoText;
    /// The barrel
    public Transform barrelTransform;
    /// The bee return position
    public Transform beeReturnPosition;
    /// Audio player for rev sound
    public AudioSource sfxRev;
    /// Audio player for shüt
    public AudioSource sfxShoot;
    /// Audio player for upgrade
    public AudioSource sfxUpgrade;

    /// The player's current camera yaw
    private float m_yaw = 0.0f;
    /// The player's current camera pitch
    private float m_pitch = 0.0f;

    /// The player's rigidbody component
    private Rigidbody m_Rigidbody;
    /// The amount of time since the player has pressed the jump objects
    private float m_TimeSinceJumpPressed = 1.0f;
    /// the collision mask for solid objects
    private int m_mask;
    /// The timer used to control shooting bees
    private float m_ShootBeeTimer = 0.0f;
    /// Amount of ammo that the player currently has
    private int m_ammo;
    /// Current rev
    private float m_rev = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_mask = LayerMask.GetMask(new string [] {"Solid"});
        m_ammo = maximumAmmo;
        UpdateAmmoText();
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
        // change speed based on rev state
        float actualSpeed = moveSpeed;
        if (m_rev > 0.0f) {
            actualSpeed = revSpeedScale * moveSpeed;
        }
        Vector3 new_velocity = (dir_forward * movement.y + dir_right * movement.x) * actualSpeed;
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
        // handle shoot
        if (Input.GetMouseButton(0)) {
            m_rev = Mathf.Clamp(m_rev + Time.deltaTime, 0.0f, revTime);
        } else {
            m_rev = Mathf.Clamp(m_rev - Time.deltaTime, 0.0f, revTime);
        }
        if (m_ShootBeeTimer <= 0.0f && CanShoot()) {
            m_ShootBeeTimer = 1.0f;
            Shoot();
        }
        if (m_rev < revTime) {
            // Reset bee shoot timer while not fully revved.
            m_ShootBeeTimer = 1.0f;
        } else {
            // otherwise, decrement shoot timer
            m_ShootBeeTimer -= Time.deltaTime * beesPerSecond;
        }
        // rotate barrel
        barrelTransform.Rotate(Vector3.up, Time.deltaTime * revAnimationSpeed * (m_rev / revTime), Space.Self);
        // handle bee return
        if (Input.GetKey(KeyCode.Q)) {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Bee");
            foreach (GameObject beeobject in objects) {
                BeeController bee = beeobject.GetComponent<BeeController>();
                bee.State = BeeController.BeeState.Returning;
            }
        } else if (Input.GetMouseButton(1)) {
            Vector3 direction = viewCamera.forward;
            Vector3 position = viewCamera.position;
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Bee");
            foreach (GameObject beeobject in objects) {
                BeeController bee = beeobject.GetComponent<BeeController>();
                Vector3 beeposition = beeobject.transform.position;
                Vector3 beedirection = (beeposition - position).normalized;
                if (Vector3.Dot(direction, beedirection) >= beeCrosshairRange) {
                    bee.State = BeeController.BeeState.Returning;
                }
            }
        }
        // Rev sound
        if (m_rev <= 0.0f) {
            sfxRev.Stop();
        } else if (!sfxRev.isPlaying) {
            sfxRev.Play();
        }
        sfxRev.pitch = 0.1f + (m_rev / revTime) * 0.6f;
    }
    /// Update the ammo text in the UI
    private void UpdateAmmoText() {
        ammoText.text = m_ammo.ToString() + " / " + maximumAmmo.ToString();
    }
    /// Returns true if the player is able to jump
    private bool CanJump() {
        Vector3 center = new Vector3(0.0f, 0.5f, 0.0f) + transform.position;
        Vector3 extents = new Vector3(0.5f, 0.1f, 0.5f);
        Vector3 direction = Vector3.down;
        Quaternion orientation = Quaternion.identity;
        return Physics.BoxCast(center, extents, direction, orientation, 0.5f, m_mask);
    }
    /// Add a bee to the player's ammo count
    public void AddBee() {
        m_ammo = Math.Min(m_ammo+1, maximumAmmo);
        UpdateAmmoText();
    }
    /// Returns true if the player is able to shoot
    private bool CanShoot() {
        return m_ammo > 0 && m_rev >= revTime;
    }
    /// Tell the player to shoot a bee in the direction they are facing
    private void Shoot() {
        if (!CanShoot()) {
            return;
        }
        GameObject bee = Instantiate(beePrefab, beeShootPosition.position, Quaternion.identity);
        BeeController beeController = bee.GetComponent<BeeController>();
        beeController.Shoot(viewCamera.forward, this);
        m_ammo -= 1;
        UpdateAmmoText();
        sfxShoot.Play();
    }
    /// Get the position of the position that the player will shoot bees out of
    public Vector3 GetMinigunWorldPosition() {
        return beeReturnPosition.position;
    }
    /// Add more maximum ammo
    public void AddMaximumAmmo(int amount) {
        maximumAmmo += amount;
        m_ammo += amount;
        UpdateAmmoText();
        sfxUpgrade.Play();
    }
}
