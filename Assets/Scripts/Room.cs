using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class Room : MonoBehaviour, ILevelManager
{
    public LevelManager LevelManager { get; set; }
    private EnemySpawnPoint[] _spawnPoints;
    private Enemy[] _enemies;
    public Transform cameraSpot;
    private bool _needClearRoom;
    private int _length;
    
    public void Initialize()
    {
        Transform t = transform;
        Collider[] colliders = Physics.OverlapBox(t.position, t.lossyScale*.5f, t.rotation, 64, QueryTriggerInteraction.Collide);
        _length = colliders.Length;

        _spawnPoints = new EnemySpawnPoint[_length];
        _enemies = new Enemy[_length];

        for (int i = 0; i < _length; i++)
        {
            _spawnPoints[i] = colliders[i].GetComponent<EnemySpawnPoint>();
        }
    }

    public void EnterRoom()
    {
        LevelManager.RoomReady += RoomReady;
        
        for (int i = 0; i < _length; i++)
        {
            _enemies[i] = _spawnPoints[i].SpawnEnemy();
        }
    }

    private void RoomReady(PlayerIg player)
    {
        if (_needClearRoom)
        {
            ClearRoom();
            LevelManager.RoomReady -= RoomReady;
        }
        else
        {
            ReadyEnemies(player);
        }
    }

    private void ClearRoom()
    {
        for (int i = 0; i < _length; i++)
        {
            _spawnPoints[i].RemoveEnemy(false);
            _enemies[i] = null;
        }
        
        _needClearRoom = false;
    }

    private void ReadyEnemies(PlayerIg player)
    {
        _needClearRoom = true;
        
        for (int i = 0; i < _length; i++)
        {
            _enemies[i].Ready(player);
        }
    }

    #if UNITY_EDITOR

    private readonly Collider[] _colliders = new Collider[1];
    private void OnDrawGizmos()
    {
        Transform t = transform;
        Vector3 position = t.position;
        Vector3 lossyScale = t.lossyScale;
        Vector3 lossyScaleHalf = lossyScale*.5f;
        Quaternion rotation = t.rotation;
        int sizeEnemy = Physics.OverlapBoxNonAlloc(position, lossyScaleHalf, _colliders, rotation, 64, QueryTriggerInteraction.Collide);
        int sizeSock = Physics.OverlapBoxNonAlloc(position, lossyScaleHalf, _colliders, rotation, 512, QueryTriggerInteraction.Collide);

        
        Gizmos.color = sizeEnemy > 0 ? sizeSock > 0 ? Color.green : Color.yellow : Color.red;

        Matrix4x4 oldMat = Gizmos.matrix;
        
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(position, rotation, lossyScale);
        Gizmos.matrix = rotationMatrix;
        
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        
        Gizmos.matrix = oldMat;
    }
    #endif
    public int GetSockCount()
    {
        Transform t = transform;
        var c = Physics.OverlapBox(t.position, t.lossyScale * .5f, t.rotation, 512,
            QueryTriggerInteraction.Collide);
        int sockGround = c.Length;
        return _spawnPoints.Count(x => x.haveSock) + sockGround;
    }
}
