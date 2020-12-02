using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HivePickup : MonoBehaviour
{
    /// Amount of ammo to add to the player
    public int ammoUpgrade = 5;
    /// The transform to animate
    public Transform graphic;
    /// timing float
    private float m_animateTimer = 0.0f;

    static float EaseInOutSine(float x)
    {
        return -(Mathf.Cos(Mathf.PI * x) - 1.0f) / 2.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_animateTimer += Time.deltaTime;
        float ypos = EaseInOutSine(Mathf.PingPong(m_animateTimer, 1.0f)) * 0.15f;
        graphic.transform.localPosition = new Vector3(0.0f, ypos, 0.0f);
        graphic.Rotate(new Vector3(0.0f, Time.deltaTime * 82.13f, 0.0f));
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player") {
            PlayerController player = col.GetComponent<PlayerController>();
            player.AddMaximumAmmo(10);
            GameObject.Destroy(this.gameObject);
        }
    }
}
