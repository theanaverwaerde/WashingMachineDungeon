using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody),typeof(AnimationBehavior))][DisallowMultipleComponent]
public abstract class Player : MonoBehaviour
{
    private Actions _input;
    private Camera _camera;

    private Vector2 _movement;
    protected bool Action;
    
    
    [SerializeField] private float rotationSpeed = 15;

    private Rigidbody _rb;
    protected Collider Collider;
    protected AnimationBehavior Anim;
    [SerializeField] private float speed;

    private bool _isInit;
    protected void Init()
    {
        if (_isInit)
        {
            return;
        }
        _isInit = true;
        
        _rb = GetComponent<Rigidbody>();
        Anim = GetComponent<AnimationBehavior>();
        Collider = GetComponent<Collider>();
        _input = new Actions();
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _input.Player.Move.Enable();
        _input.Player.Actions.Enable();
        
        _input.Player.Move.started += MoveOnstarted;
        _input.Player.Move.performed += MoveOnstarted;
        _input.Player.Move.canceled += MoveOncanceled;
        
        _input.Player.Actions.started += ActionsOnstarted;
    }

    private void ActionsOnstarted(InputAction.CallbackContext obj)
    {
        Action = true;
    }

    private void MoveOnstarted(InputAction.CallbackContext obj)
    {
        _movement = obj.ReadValue<Vector2>();
    }

    private void MoveOncanceled(InputAction.CallbackContext obj)
    {
        _movement = Vector2.zero;
    }

    protected void Movement()
    {
        Transform camT = _camera.transform;
        Vector3 camForward = camT.forward;
        camForward.y = 0;
        camForward = camForward.normalized;
        Vector3 dir = (_movement.y * camForward + _movement.x * camT.right).normalized;
        dir *= speed;
        dir *= Time.fixedDeltaTime;

        _rb.velocity = dir;
        
        Anim.SetVelocity(dir);

        if (dir.sqrMagnitude <= 0)
        {
            return;
        }
        
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot,rotationSpeed * Time.fixedDeltaTime);
    }

}