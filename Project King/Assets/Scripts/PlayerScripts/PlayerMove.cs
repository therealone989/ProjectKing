using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    Vector2 movementVector;
    public float moveSpeed = 8f;
    public float rotationSpeed = 10f;
    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    public void InputPlayer(InputAction.CallbackContext _context)
    {
        movementVector = _context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 dir = new Vector3(movementVector.x, 0, movementVector.y);
        if (dir.sqrMagnitude > 1)
            dir.Normalize();


        Vector3 targetpos = rb.position + dir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetpos);

        if (dir.sqrMagnitude > 0.0001f)
        {
            // NUR DREHEN, wenn wir NICHT gerade jemanden angreifen
            // (Dafür brauchen wir eine Referenz auf AutoAttack oder wir machen es einfach so:)
            if (GetComponent<AutoAttack>().attackRange == 0 || dir.magnitude > 0.1f)
            {
                // Wenn du willst, dass Bewegung immer Vorrang hat, lass es wie es ist.
                // Wenn der Schuss wichtiger ist, dreht AutoAttack den Charakter.
            }

            Quaternion targetrot = Quaternion.LookRotation(dir);
            Quaternion smoothRot = Quaternion.Slerp(rb.rotation, targetrot, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothRot);
        }
    }
}
