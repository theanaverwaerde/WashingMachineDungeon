using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class UIInGame : MonoBehaviour
{
    private UIDocument _uiDocument;
    [SerializeField] private VisualTreeAsset enemyTemplate;
    
    private Dictionary<Enemy, TemplateContainer> _enemies = new();

    private const int MaxHeart = 3;
    private const int MaxEnemyHeart = 2;
    private const int MaxSock = 7;

    private const string HeartGrey = "heart-grey";
    private const string HeartRed = "heart-red";
    private const string HeartGreen = "heart-green";
    
    private const string Sock = "sock";
    private const string SockGrey = "sock-grey";
    private Camera _camera;
    
    private void Init()
    {
        if (_uiDocument != null)
        {
            return;
        }
        
        _uiDocument = GetComponent<UIDocument>();
        _camera = Camera.main;
    }

    public void InitPlayer(int maxLife)
    {
        VisualElement parent = _uiDocument.rootVisualElement.Q("Hearts");
    }
    
    public void PlayerHeart(int life)
    {
        VisualElement parent = _uiDocument.rootVisualElement.Q("Hearts");

        for (int i = 1; i <= MaxHeart; i++)
        {
            bool haveHeart = i <= life;
            VisualElement elt = parent.Q(i.ToString());

            if (haveHeart)
            {
                if (elt.ClassListContains(HeartRed))
                {
                    continue;
                }
                elt.RemoveFromClassList(HeartGrey);
                elt.AddToClassList(HeartRed);
                continue;
            }
            
            if (elt.ClassListContains(HeartGrey))
            {
                continue;
            }
            
            elt.RemoveFromClassList(HeartRed);
            elt.AddToClassList(HeartGrey);
        }
    }
    
    public void InitSock(int maxSock)
    {
        Init();
        
        VisualElement parent = _uiDocument.rootVisualElement.Q("Socks");
        
        for (int i = 1; i <= MaxSock; i++)
        {
            bool haveSock = i <= maxSock;
            VisualElement elt = parent.Q(i.ToString());

            if (haveSock)
            {
                if (elt.ClassListContains(Sock))
                {
                    elt.RemoveFromClassList(Sock);
                    elt.AddToClassList(SockGrey);
                }
                continue;
            }

            elt.style.visibility = Visibility.Hidden;
        }
    }
    
    public void PlayerSock(int sock)
    {
        VisualElement parent = _uiDocument.rootVisualElement.Q("Socks");

        for (int i = 1; i <= MaxSock; i++)
        {
            bool haveSock = i <= sock;
            VisualElement elt = parent.Q(i.ToString());

            if (haveSock)
            {
                if (elt.ClassListContains(Sock))
                {
                    continue;
                }
                elt.RemoveFromClassList(SockGrey);
                elt.AddToClassList(Sock);
                continue;
            }
            
            if (elt.ClassListContains(SockGrey))
            {
                continue;
            }
            
            elt.RemoveFromClassList(Sock);
            elt.AddToClassList(SockGrey);
        }
    }

    public void AddEnemy(Enemy enemy)
    {
        TemplateContainer template = enemyTemplate.CloneTree();
        _uiDocument.rootVisualElement.Add(template);
        
        _enemies.Add(enemy,template);
        
        template.style.position = Position.Absolute;
        template.style.width = 0;
        template.style.height = 0;

        int life = enemy.Life;

        if (!enemy.HaveSock)
        {
            template.Q("Sock").style.visibility = Visibility.Hidden;
        }
        
        VisualElement parent = template.Q("Life");

        int lastSize = 0;

        template.schedule.Execute(() =>
        {
            if (enemy == null)
            {
                _enemies.Remove(enemy);
                template.RemoveFromHierarchy();
                return;
            }
            
            var scaleFactor = 1920f / Screen.width;
            var screenPosition = _camera.WorldToScreenPoint(enemy.transform.position);
            screenPosition.x *= scaleFactor;
            screenPosition.y *= scaleFactor;

            template.style.left = screenPosition.x;
            template.style.bottom = screenPosition.y;

            lastSize = Screen.width;

            if (enemy.Life == life)
            {
                return;
            }

            for (int i = 1; i <= MaxEnemyHeart; i++)
            {
                bool haveHeart = i <= enemy.Life;
                VisualElement elt = parent.Q(i.ToString());

                if (haveHeart)
                {
                    if (elt.ClassListContains(HeartGreen))
                    {
                        continue;
                    }
                    elt.RemoveFromClassList(HeartGrey);
                    elt.AddToClassList(HeartGreen);
                    continue;
                }
            
                if (elt.ClassListContains(HeartGrey))
                {
                    continue;
                }
            
                elt.RemoveFromClassList(HeartGreen);
                elt.AddToClassList(HeartGrey);
            }

        }).StartingIn(0).Until(() => !_enemies.ContainsKey(enemy));
    }
}
