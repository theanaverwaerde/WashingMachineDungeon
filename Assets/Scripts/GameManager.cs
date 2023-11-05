using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake() 
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
            DontDestroyOnLoad(this);
        } 
        
        SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
    }

    private int _currentLevel = 0;
    public const int LastLevel = 4;

    public WashingLevel machine;
    public Vector3 machineCenter;

    private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name != "Game")
        {
            return;
        }
        
        var washingLevel = FindObjectsOfType<WashingLevel>().ToList();
        washingLevel.ForEach(x => x.OpenDoor(_currentLevel == x.order));
        machine = washingLevel.FirstOrDefault(x => _currentLevel == x.order);
        if (machine == null)
        {
            Debug.LogError($"No machine for {_currentLevel}");
            return;
        }
        machineCenter = machine!.GetComponent<BoxCollider>().bounds.center;
    }

    public void Win()
    {
        if (_currentLevel == LastLevel)
        {
            SceneManager.LoadScene("End");
        }
        else
        {
            _currentLevel++;
            SceneManager.LoadScene("Game");
        }
    }

    public void Lose()
    {
        SceneManager.LoadScene("Game");
    }

    public void Enter()
    {
        machine.Enter();
    }
}
