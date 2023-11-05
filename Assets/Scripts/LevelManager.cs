using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Transform spawn;
    [SerializeField] private Room firstRoom;
    [SerializeField] private float timeTransition = 2;
    public UIInGame ui;
    private PlayerIg _player;
    private Transform _cameraParent;
    public Action<PlayerIg> RoomReady { get; set; }

    public int SockCollected
    {
        get => _sockCollected;
        set
        {
            _sockCollected = value;
            ui.PlayerSock(_sockCollected);
            if (_sockCount == _sockCollected)
            {
                Win();
            }
        }
    }

    private void Win()
    {
        GameManager.Instance.Win();
    }

    private int _sockCount;
    private int _sockCollected;


    private void Awake()
    {
        ui = FindObjectOfType<UIInGame>();
        _player = FindObjectOfType<PlayerIg>();
        _cameraParent = Camera.main!.transform.parent;
        
        var allObject = FindObjectsOfType<MonoBehaviour>();
        allObject.OfType<ILevelManager>().ToList().ForEach(x => x.LevelManager = this);
        var rooms = allObject.OfType<Room>().ToList();
        rooms.ForEach(x => x.Initialize());
        _sockCount += rooms.Sum(x=> x.GetSockCount());
        
        ui.InitSock(_sockCount);
        
        firstRoom.EnterRoom();
        
        MovePlayerAndCamera(firstRoom, spawn, true);
    }

    public void MovePlayerAndCamera(Room nextRoom, Transform position, bool instant = false)
    {
        _player.OnSwitchRoom = true;
        if (instant)
        {
            _cameraParent.position = nextRoom.cameraSpot.position;
            _player.transform.position = position.position;
            
            RoomReady.Invoke(_player); 
            _player.OnSwitchRoom = false;
            return;
        }

        LeanTween.move(_cameraParent.gameObject, nextRoom.cameraSpot, timeTransition);
        LeanTween.move(_player.gameObject, position, timeTransition);
        
        LeanTween.delayedCall(timeTransition, _ =>
        {
            RoomReady.Invoke(_player);
            _player.OnSwitchRoom = false;
        });
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (spawn == null)
        {
            return;
        }
        
        Gizmos.color = Color.green;
        Vector3 p = spawn.position;
        GizmosExtensions.Arrow(p,spawn.forward);
        
        Gizmos.DrawSphere(p,.2f);
    }
#endif
}