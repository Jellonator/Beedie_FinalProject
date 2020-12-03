using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liftable : MonoBehaviour
{
    /// Number of bees required to lift this object
    public int beeRequirement = 1;
    /// Base Velocity when metting bee requirement
    public float liftBase = 0.5f;
    /// Extra velocity for every extra bee
    public float liftScale = 0.1f;
    /// Text Mesh to show attatched bees
    public TextMesh beeCounter;

    /// Number of attached bees
    private int m_bees = 0;
    /// RigidBody component
    Rigidbody m_Rigidbody;
    /// Height to position the bee counter at
    private Vector3 m_beeCounterOffset;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.useGravity = false;
        m_beeCounterOffset = beeCounter.transform.position - transform.position;
        UpdateBeeCounter();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_bees >= beeRequirement) {
            float lift = liftBase + liftScale * (m_bees - beeRequirement);
            Vector3 velocity = m_Rigidbody.velocity;
            velocity.y = Mathf.Clamp(velocity.y + lift * Time.deltaTime, velocity.y, lift);
            m_Rigidbody.velocity = velocity;
        } else {
            float gravityScale = 0.1f + 0.9f * (1.0f - ((float)m_bees / (float)beeRequirement));
            m_Rigidbody.velocity += Vector3.down * gravityScale * 9.8f * Time.deltaTime;
        }
    }
    /// Add a bee to this liftable's bee count
    public void AddBee() {
        m_bees += 1;
        UpdateBeeCounter();
    }
    /// Add a bee to this liftable's bee count
    public void RemoveBee() {
        m_bees -= 1;
        UpdateBeeCounter();
    }
    /// Update the bee counter text
    private void UpdateBeeCounter() {
        beeCounter.text = m_bees.ToString() + "/" + beeRequirement.ToString();
    }

    void LateUpdate() {
        // Make the bee counter look at the camera
        beeCounter.transform.position = transform.position + m_beeCounterOffset;
        beeCounter.transform.LookAt(Camera.main.transform.position, Vector3.up);
    }
}
