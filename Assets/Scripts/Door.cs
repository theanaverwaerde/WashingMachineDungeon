using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Door : MonoBehaviour, ILevelManager
{
    public LevelManager LevelManager { get; set; }
    
    [SerializeField] private Room room;
    [SerializeField] private Transform destination;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        room.EnterRoom();
        LevelManager.MovePlayerAndCamera(room, destination);
    }
}