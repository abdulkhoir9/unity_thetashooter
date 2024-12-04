using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour 
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _rotationSpeed;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private float _screenBorder;

    [SerializeField]
    private PlayerFieldOfView _playerFieldOfView;

    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;
    private Vector2 _mousePosition;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        SetPlayerVelocity();
        RotateInDirectionOfInput();
        _playerFieldOfView.SetOrigin(transform.position);
    }

    private void SetPlayerVelocity()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(
                    _smoothedMovementInput,
                    _movementInput,
                    ref _movementInputSmoothVelocity,
                    0.1f);

        float movementDirection = Vector2.Dot(_smoothedMovementInput.normalized, transform.up);

        // get the value of _fireContinuously from PlayerShoot, to determine whether the player is currently shooting or not
        if (transform.GetComponent<PlayerShoot>()._fireContinuously)
        {
            _rigidbody.velocity = _smoothedMovementInput * (_speed / 3);
            //Debug.Log("velocity (" + _rigidbody.velocity + ") = smoothedMovementInput (" + _smoothedMovementInput + ") + speed (" + _speed / 4 + ")");
        }
        else if(movementDirection < 0)
        {
            _rigidbody.velocity = _smoothedMovementInput * (_speed / 2);
            //Debug.Log("velocity (" + _rigidbody.velocity + ") = smoothedMovementInput (" + _smoothedMovementInput + ") + speed (" + _speed / 3 + ")");
        }
        else 
        {
            _rigidbody.velocity = _smoothedMovementInput * _speed;
            //Debug.Log("velocity (" + _rigidbody.velocity + ") = smoothedMovementInput (" + _smoothedMovementInput + ") + speed (" + _speed + ")");
        }
        
        PreventPlayerGoingOffscreen();  
    }
    private void PreventPlayerGoingOffscreen()
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);

        if((screenPosition.x < _screenBorder && _rigidbody.velocity.x < 0) ||
            screenPosition.x > _camera.pixelWidth - _screenBorder && _rigidbody.velocity.x >0)
        {
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
        }

        if ((screenPosition.y < _screenBorder && _rigidbody.velocity.y < 0) ||
            screenPosition.y > _camera.pixelHeight - _screenBorder && _rigidbody.velocity.y > 0)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
        }
    }

    private void RotateInDirectionOfInput()
    {
        //if(_movementInput != Vector2.zero)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _smoothedMovementInput);
        //    Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        
        //    _rigidbody.MoveRotation(rotation);
        //}
        Vector2 lookDirection = _mousePosition - _rigidbody.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
        _rigidbody.rotation = angle;
        _playerFieldOfView.SetViewDirection(lookDirection);
    }

    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();
    }
}
