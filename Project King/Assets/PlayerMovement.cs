using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{

    Vector2 movementVector;
    public float moveSpeed = 8f;

    public void InputPlayer(InputAction.CallbackContext _context)
    {
        movementVector = _context.ReadValue<Vector2>();
    }

    private void Update()
    {
        Vector3 movement = new Vector3(movementVector.x, 0, movementVector.y);
        movement.Normalize();
        transform.Translate(moveSpeed * movement * Time.deltaTime);
    }
}
