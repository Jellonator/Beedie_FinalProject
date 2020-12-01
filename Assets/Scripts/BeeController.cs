using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeController : MonoBehaviour
{
    /// Amount of time that the bee will fly before returning to the player
    public float maximumShootTime = 5.0f;
    /// Speed at which the bee will move
    public float speed;
    /// This bee's sphere collider which detects walls
    public Collider wallCollider;
    /// This bee's owning player
    private PlayerController m_owner;
    /// Bee state enumeration.
    /// Shooting  - the bee is traveling and will hit objects
    /// Returning - the bee is returning to the player
    /// StuckWall - the bee is stuck in a wall
    public enum BeeState {
        Shooting,
        Returning,
        StuckWall
    }
    /// The Bee's current state
    private BeeState _state;
    public BeeState State
    {
        get {
            return _state;
        }
        set {
            _state = value;
            if (value == BeeState.Returning) {
                wallCollider.enabled = false;
            } else {
                wallCollider.enabled = true;
            }
        }
    }
    /// The Bee's Rigidbody component
    private Rigidbody m_Rigidbody;
    /// The amount of time that the bee has been traveling
    /// Only used when state is 'Shooting'
    private float m_ShootTime = 0.0f;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        State = BeeState.Shooting;
    }

    void Update()
    {
        if (State == BeeState.Shooting) {
            // Update shooting time. Eventually return to player.
            m_ShootTime += Time.deltaTime;
            if (m_ShootTime > maximumShootTime) {
                State = BeeState.Returning;
            }
        } else if (State == BeeState.Returning) {
            // Look at player. Move towards them.
            transform.LookAt(m_owner.transform, Vector3.up);
            Vector3 playerpos = m_owner.GetMinigunWorldPosition();
            m_Rigidbody.velocity = (playerpos - transform.position).normalized * speed;
            if (Vector3.Distance(playerpos, transform.position) < 0.5f) {
                // Return bee to player
                m_owner.AddBee();
                GameObject.Destroy(this.gameObject);
            }
        }
    }
    /// Shoot this bee in the given direction. Always call this after instancing.
    public void Shoot(Vector3 direction, PlayerController owner) {
        // Shoot in the given direction
        transform.LookAt(transform.position + direction, Vector3.up);
        State = BeeState.Shooting;
        GetComponent<Rigidbody>().velocity = direction.normalized * speed;
        m_owner = owner;
        m_ShootTime = 0.0f;
    }

    void OnCollisionEnter(Collision col)
    {
        if (State == BeeState.Returning) {
            return;
        }
        m_Rigidbody.velocity = Vector3.zero;
        State = BeeState.StuckWall;
    }
}
