using System;
using UnityEngine;

public class PlayerHub : Player
{
    private bool _interact;
    public const float InteractDistance = 2;

    private void Awake()
    {
        Init();
        Anim.Attack += Interact;
        Anim.AttackEnd += InteractEnd;
    }

    private void Interact()
    {
        if (Vector3.Distance(GameManager.Instance.machineCenter, transform.position) < InteractDistance)
        {
            GameManager.Instance.Enter();
        }
    }

    private void InteractEnd()
    {
        _interact = false;
    }

    private void FixedUpdate()
    {
        if (_interact)
        {
            return;
        }
        
        Movement();
    }
    
    private void Update()
    {
        if (!Action || _interact)
        {
            return;
        }
        Anim.SetInteract();
        _interact = true;
        Action = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,InteractDistance);
    }
#endif
}