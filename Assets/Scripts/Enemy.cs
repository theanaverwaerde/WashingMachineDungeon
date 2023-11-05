using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[DisallowMultipleComponent] 
[RequireComponent(typeof(AnimationBehavior))]
public class Enemy : MonoBehaviour, IDamageable
{
    private NavMeshAgent _agent;
    private AnimationBehavior _anim;
    private bool _isReady;
    private PlayerIg _player;
    [SerializeField] private int maxLife = 2;
    private int _life;
    private bool _hitted;
    [SerializeField] private GameObject deadEffect;
    [SerializeField] private float detectRadius = 4;
    [SerializeField] private float limitReach = .1f;

    private bool IsOnDestination => (_agent.destination - transform.position).magnitude < limitReach;

    public int Life
    {
        get => _life;
        private set => _life = value;
    }

    private bool _targetPlayer;
    private bool _attack;
    private EnemySpawnPoint _enemySpawnPoint;
    public bool HaveSock => _enemySpawnPoint.SockAvailable;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<AnimationBehavior>();
        
        Life = maxLife;
        _anim.Attack += Attack;
        _anim.AttackEnd += AttackEnd;
        _anim.HitRestore += HitRestore;
    }

    private void Attack()
    {
        _player.Hit();
    }

    private void HitRestore()
    {
        _hitted = false;
    }

    private void AttackEnd()
    {
        SetNewPoint();
        _attack = false;
        _targetPlayer = false;
    }

    public void Ready(PlayerIg player)
    {
        _player = player;
        _isReady = true;
    }

    private void FixedUpdate()
    {
        if (!_isReady || _hitted || _attack)
        {
            return;
        }
        
        _anim.SetVelocity(_agent.velocity);

        Vector3 playerPos = _player.transform.position;

        if (Vector3.Distance(playerPos, transform.position) < detectRadius)
        {
            // Player is on reach
            _targetPlayer = true;
            
            _agent.destination = playerPos;
        }
        else
        {
            _targetPlayer = false;
        }
        
        if (_agent.isStopped)
        {
            SetNewPoint();
            _agent.isStopped = false;
            return;
        }

        if (!IsOnDestination)
        {
            return;
        }
        
        if (_targetPlayer)
        {
            _attack = true;
            _anim.SetVelocity(0);
            _anim.SetInteract();
        }
        else
        {
            SetNewPoint();
        }

    }

    private void SetNewPoint()
    {
        if (RandomPoint(transform.position, detectRadius, out Vector3 pos))
        {
            _agent.SetDestination(pos);
        }
    }
    
    private static bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            if (!NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                continue;
            }
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }

    public void Hit(int damage = 1)
    {
        if (_hitted)
        {
            return;
        }
        
        _attack = false;
        _agent.isStopped = true;
        Life -= damage;
        
        if (Life <= 0)
        {
            Dead();
        }
        
        _hitted = true;
        _anim.SetHit();
    }

    public void Dead()
    {
        Transform t = transform;
        Instantiate(deadEffect, t.position,t.rotation);
        _enemySpawnPoint.RemoveEnemy(true);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = transform.position;
        Gizmos.DrawWireSphere(pos,detectRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos,limitReach);
    }
#endif
    public void Init(EnemySpawnPoint enemySpawnPoint)
    {
        _enemySpawnPoint = enemySpawnPoint;
    }
}