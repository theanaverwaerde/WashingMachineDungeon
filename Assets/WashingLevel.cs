using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class WashingLevel : MonoBehaviour
{
    private Transform _doorTransform;
    private ParticleSystem _particleSystem;
    [Range(0,GameManager.LastLevel)] public int order;
    [SerializeField] private string sceneName;
    [SerializeField] private float angle = 120;
    [SerializeField] private float time = 2;
    
    [SerializeField] private VisualTreeAsset enemyTemplate;

    private void Awake()
    {
        _doorTransform = transform.GetChild(0);
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    public void OpenDoor(bool open)
    {
        if (!open)
        {
            return;
        }
        
        _particleSystem.Play();
        LeanTween.rotateAround(_doorTransform.gameObject,Vector3.up, angle, time);
        
        TemplateContainer template = enemyTemplate.CloneTree();
        
        FindObjectOfType<UIDocument>().rootVisualElement.Add(template);
        var t = FindObjectOfType<PlayerHub>().transform;
        
        template.style.position = Position.Absolute;
        template.style.width = 0;
        template.style.height = 0;
        int lastSize = 0;

        var text = template.Q("Text");

        template.schedule.Execute(() =>
        {
            var dist = Vector3.Distance(GameManager.Instance.machineCenter, t.position);
            text.style.visibility = dist < PlayerHub.InteractDistance ? Visibility.Visible : Visibility.Hidden;

            if (lastSize == Screen.width)
            {
                return;
            }
            
            var scaleFactor = 1920f / Screen.width;
            var screenPosition = Camera.main!.WorldToScreenPoint(transform.position);
            screenPosition.x *= scaleFactor;
            screenPosition.y *= scaleFactor;

            template.style.left = screenPosition.x;
            template.style.bottom = screenPosition.y;

            lastSize = Screen.width;

        }).StartingIn(0).Every(10);
    }

    public void Enter()
    {
        SceneManager.LoadScene(sceneName);
    }
}
