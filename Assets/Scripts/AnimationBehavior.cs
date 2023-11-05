using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationBehavior : MonoBehaviour
{
    private Animator _animator;
    private static readonly int Interact = Animator.StringToHash("Interact");
    private static readonly int Velocity = Animator.StringToHash("Velocity");
    private static readonly int Hit = Animator.StringToHash("Hit");

    public Action HitRestore = null;
    public Action Attack = null;
    public Action AttackEnd = null;

    public void MakeAttack()
    {
        Attack?.Invoke();
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetVelocity(float velocity)
    {
        _animator.SetFloat(Velocity,velocity);
    }
    
    public void SetVelocity(Vector2 velocity)
    {
        _animator.SetFloat(Velocity,velocity.magnitude);
    }
    
    public void SetVelocity(Vector3 velocity)
    {
        _animator.SetFloat(Velocity,velocity.magnitude);
    }

    public void SetInteract()
    {
        _animator.SetTrigger(Interact);
    }

    List<Color> originalColors = new();
 
    private int originalColorIndex;
    private static readonly int Color1 = Shader.PropertyToID("_Color");

    public void SetHit()
    {
        MeshRenderer[] children = GetComponentsInChildren<MeshRenderer>();
 
        // Cycle through each child object found with a MeshRenderer
 
        foreach (MeshRenderer rend in children)
        {
            // And for each child, cycle through each material
 
            foreach (Material mat in rend.materials)
            {
                // Store original colors
 
                originalColors.Add(mat.color);
            }
        }

        foreach (MeshRenderer rend in children)
        {
            foreach (Material mat in rend.materials)
            {
                mat.SetColor(Color1, Color.white);
            }
        }

        LeanTween.delayedCall(gameObject, .2f, () =>
        {
            foreach (MeshRenderer rend in children)
            {
                foreach (Material mat in rend.materials)
                {
                    mat.SetColor(Color1, originalColors[originalColorIndex]);

                    // Increment originalColorIndex by 1

                    originalColorIndex += 1;
                }
            }
            
            originalColorIndex = 0;
        });
        
        _animator.SetTrigger(Hit);
    }
}