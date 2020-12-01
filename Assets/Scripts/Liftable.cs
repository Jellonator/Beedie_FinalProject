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

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.useGravity = false;
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

    public void AddBee() {
        m_bees += 1;
        UpdateBeeCounter();
    }

    public void RemoveBee() {
        m_bees -= 1;
        UpdateBeeCounter();
    }

    private void UpdateBeeCounter() {
        beeCounter.text = m_bees.ToString() + "/" + beeRequirement.ToString();
    }

    void LateUpdate() {
        beeCounter.transform.LookAt(Camera.current.transform.position, Vector3.up);
    }
}
