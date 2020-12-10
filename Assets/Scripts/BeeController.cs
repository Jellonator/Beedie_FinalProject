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
        StuckWall,
        StuckObject
    }
    /// The Bee's current state
    private BeeState _state;
    public BeeState State
    {
        get {
            return _state;
        }
        set {
            if (m_Liftable != null && value != BeeState.StuckObject) {
                m_Liftable.RemoveBee();
                m_Liftable = null;
            }
            _state = value;
            if (value == BeeState.Shooting) {
                wallCollider.enabled = true;
            } else {
                wallCollider.enabled = false;
            }
        }
    }
    /// The Bee's Rigidbody component
    private Rigidbody m_Rigidbody;
    /// The amount of time that the bee has been traveling
    /// Only used when state is 'Shooting'
    private float m_ShootTime = 0.0f;
    /// Object that this bee is currently lifting
    Liftable m_Liftable = null;
    /// transform to be applied on top of liftable's transform when attatched
    Matrix4x4 m_LocalTransform = Matrix4x4.identity;

    void Awake()
    {
        // Awake used here since OnCollisionEnter can be called before Start
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
        Liftable liftable = col.gameObject.GetComponent<Liftable>();
        if (liftable != null) {
            m_Liftable = liftable;
            m_LocalTransform = liftable.transform.worldToLocalMatrix * transform.localToWorldMatrix;
            liftable.AddBee();
            State = BeeState.StuckObject;
        } else {
            State = BeeState.StuckWall;
        }
    }

    void LateUpdate() {
        // Update the bee's position to match the object it is attatched to
        if (State == BeeState.StuckObject && m_Liftable != null) {
            Matrix4x4 tx = m_Liftable.transform.localToWorldMatrix * m_LocalTransform;
            transform.position = new Vector3(tx.m03, tx.m13, tx.m23);
            transform.rotation = tx.rotation;
            transform.localScale = tx.lossyScale;
        }
    }
}
