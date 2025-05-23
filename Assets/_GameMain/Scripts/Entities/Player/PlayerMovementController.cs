using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    public event Action<float> OnSpeedChanged;

    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _rotationSpeed = 12f;
    [SerializeField] private float _acceleration = 24f;
    [SerializeField] private float _inputSmooth = 10f;
    
    private Rigidbody _rb;
    private Transform _cameraTransform;
    private Vector3 _input;
    private Vector3 _smoothedInput;
    private bool _isGrabbed;
    private bool _isMovementBlocked;
    private IInputService _inputService;

    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    public void Initialize(Transform cameraTransform, Vector3 spawnPos)
    {
        _cameraTransform = cameraTransform;
        _isGrabbed = false;
        _isMovementBlocked = false;
        _rb.position = spawnPos;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    public void Grabbed()
    {
        _isGrabbed = true;
        _rb.linearVelocity = Vector3.zero;
    }

    public bool IsGrabbed()
    {
        return _isGrabbed;
    }

    public void SetMovementBlocked(bool blocked)
    {
        _isMovementBlocked = blocked;
        if (blocked)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    // _____________ Private _____________

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_isGrabbed || _isMovementBlocked) return;

        float h = _inputService?.GetHorizontal() ?? Input.GetAxisRaw("Horizontal");
        float v = _inputService?.GetVertical() ?? Input.GetAxisRaw("Vertical");

        if (_cameraTransform != null)
        {
            var forward = _cameraTransform.forward; forward.y = 0;
            var right = _cameraTransform.right; right.y = 0;
            _input = (forward.normalized * v + right.normalized * h).normalized;
        }
        else
        {
            _input = new Vector3(h, 0, v).normalized;
        }
        _smoothedInput = Vector3.Lerp(_smoothedInput, _input, _inputSmooth * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (_isGrabbed || _isMovementBlocked) return;

        var targetVel = _smoothedInput * _moveSpeed;
        _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, targetVel, _acceleration * Time.fixedDeltaTime);

        if (_smoothedInput.sqrMagnitude > 0.01f)
        {
            var rot = Quaternion.LookRotation(_smoothedInput);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, _rotationSpeed * Time.fixedDeltaTime);
        }

        OnSpeedChanged?.Invoke(_rb.linearVelocity.magnitude / _moveSpeed);
    }
}
