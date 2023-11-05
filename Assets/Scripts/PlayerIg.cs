using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIg : Player, IDamageable, ILevelManager
{
    public LevelManager LevelManager { get; set; }

    public bool OnSwitchRoom
    {
        get => onSwitchRoom;
        set
        {
            onSwitchRoom = value;
            Init();
            Collider.enabled = !onSwitchRoom;
        }
    }
    [SerializeField] private int lifeMax = 3;
    private int _life;
    
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector3 attackBox;
    
    private bool _attack;
    public bool onSwitchRoom;
    
    private void Awake()
    {
        Init();
        _life = lifeMax;
        Anim.Attack += Attack;
        Anim.AttackEnd += AttackEnd;
    }
    
    private void Start()
    {
        LevelManager.ui.InitPlayer(lifeMax);
    }
    
    private void AttackEnd()
    {
        _attack = false;
    }

    private void FixedUpdate()
    {
        if (_attack || OnSwitchRoom)
        {
            return;
        }

        Movement();
    }

    private void Update()
    {
        if (!Action || _attack || OnSwitchRoom)
        {
            return;
        }
        Anim.SetInteract();
        _attack = true;
        Action = false;
    }
    
    public void Attack()
    {
        Transform t = transform;
        Collider[] colliders = Physics.OverlapBox(t.position, t.lossyScale*.5f, t.rotation, 256, QueryTriggerInteraction.Collide);
        List<Enemy> enemies = new List<Enemy>();

        foreach (Collider c in colliders)
        {
            if (c.TryGetComponent(out Enemy e))
            {
                enemies.Add(e);
            }
        }
        
        enemies.ForEach(e => e.Hit());
    }
    
    
    public void Hit(int damage = 1)
    {
        if (onSwitchRoom)
        {
            return;
        }
        
        Anim.SetHit();
        
        _life -= damage;
        
        LevelManager.ui.PlayerHeart(_life);

        if (_life <= 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        GameManager.Instance.Lose();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (attackPoint == null)
        {
            return;
        }
        
        Transform t = attackPoint;

        Gizmos.color = Color.blue;

        Matrix4x4 oldMat = Gizmos.matrix;
        
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(t.position, t.rotation, attackBox);
        Gizmos.matrix = rotationMatrix;
        
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        
        Gizmos.matrix = oldMat;
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 9)
        {
            return;
        }

        Destroy(other.gameObject);
        LevelManager.SockCollected++;
    }
}