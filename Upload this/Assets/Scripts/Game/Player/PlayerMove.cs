using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    private Rigidbody2D _rigidbody;
    private Vector2 _moveInput;

    private void Awake()
    {
        //Upon play
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //Moves character
        _rigidbody.linearVelocity = _moveInput * _speed;
    }

    private void OnMove(InputValue move)
    {
        //Gets input
        _moveInput = move.Get<Vector2>();
    }
}
