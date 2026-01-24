using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
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
        if(dir.sqrMagnitude > 1)
        dir.Normalize();


        Vector3 targetpos = rb.position + dir * moveSpeed * Time.deltaTime;
        rb.MovePosition(targetpos);

        if(dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetrot = Quaternion.LookRotation(dir);
            Quaternion smoothRot = Quaternion.Slerp(rb.rotation, targetrot, rotationSpeed * Time.deltaTime);
            rb.MoveRotation(smoothRot);
        }
    }
}
