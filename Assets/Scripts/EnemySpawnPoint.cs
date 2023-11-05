using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))][DisallowMultipleComponent]
public class EnemySpawnPoint : MonoBehaviour, ILevelManager
{
    [SerializeField] private EnemyScriptable enemyScriptable;
    private Enemy _enemy;
    public bool haveSock;
    private bool _sockAvailable;
    public LevelManager LevelManager { get; set; }

    public bool SockAvailable
    {
        get => _sockAvailable;
        private set => _sockAvailable = value;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = haveSock ? Color.magenta : Color.yellow;
        Transform t = transform;
        Vector3 p = t.position;
        GizmosExtensions.Arrow(p,t.forward);
        
        Gizmos.DrawSphere(p,.2f);
    }
#endif

    private bool _isInit;

    private void Init()
    {
        if (_isInit)
        {
            return;
        }

        _isInit = true;
        
        SockAvailable = haveSock;
    }

    private void Awake()
    {
        Init();
    }

    public Enemy SpawnEnemy()
    {
        if (_enemy != null)
        {
            Debug.LogWarning("Enemy already exist!",gameObject);
            return null;
        }
        Init();

        Transform t = transform;
        _enemy = Instantiate(enemyScriptable.prefab, t.position, t.rotation);
        _enemy.Init(this);
        LevelManager.ui.AddEnemy(_enemy);
        return _enemy;
    }

    public void RemoveEnemy(bool dead)
    {
        if (SockAvailable && dead)
        {
            LevelManager.SockCollected++;
            SockAvailable = false;
        }
        
        if (_enemy == null)
        {
            return;
        }
        
        Destroy(_enemy.gameObject);
        _enemy = null;
    }
}
