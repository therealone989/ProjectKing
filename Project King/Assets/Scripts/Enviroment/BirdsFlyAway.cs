using UnityEngine;

public class TriggerFly : MonoBehaviour
{
    public float flySpeed = 2f;   
    public float forwardSpeed = 1f;  

    private bool shouldFly;
    private Animator[] animators;

    void Awake()
    {
        animators = GetComponentsInChildren<Animator>(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        shouldFly = true;

        foreach (var a in animators)
            a.SetTrigger("Fly");
    }

    void Update()
    {
        if (!shouldFly) return;

        float dt = Time.deltaTime;

        foreach (var a in animators)
        {
            Transform t = a.transform;

            Vector3 move = Vector3.up * flySpeed * dt
                         + t.forward * forwardSpeed * dt;

            t.position += move;
        }
    }
}