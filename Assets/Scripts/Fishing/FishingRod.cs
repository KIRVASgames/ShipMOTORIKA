using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ShipMotorika
{
    /// <summary>
    /// Удочка игрока.
    /// </summary>
    public class FishingRod : MonoBehaviour
    {
        [SerializeField] private FishingRodAsset _asset;

        /// <summary>
        /// Название удочки (для магазина).
        /// </summary>
        [SerializeField] private string _name;
        public string Name => _name;

        /// <summary>
        /// Описание удочки для торговца.
        /// </summary>
        [SerializeField] private string _description;
        public string Description => _description;
        
        /// <summary>
        /// В соответствии с радиусом этого коллайдера меняет расстояние от FishingPlace, на котором можно ловить рыбу.
        /// </summary>
        [SerializeField] private CircleCollider2D _circleCollider;

        /// <summary>
        /// Для удобства радиус можно задавать/смотреть через инспектор.
        /// </summary>
        [SerializeField] private float _radius;
        public float Radius => _radius;

        /// <summary>
        /// Скорость ловли удочки. Чем выше, тем легче будет проходить мини-игру FishingChallenge.
        /// </summary>
        [SerializeField] private float _speed;
        public float Speed => _speed;

        /// <summary>
        /// Стоимость удочки у торговца.
        /// </summary>
        [SerializeField] private int _cost;
        public int Cost => _cost;

        /// <summary>
        /// Та FishingPoint, в которой игрок ловит рыбу в данный момент.
        /// </summary>
        private FishingPoint _activeFishingPoint;
        public FishingPoint FishingPoint => _activeFishingPoint;

        /// <summary>
        /// Пойманная рыба.
        /// </summary>
        private Fish _сaughtFish = null;
        public Fish CaughtFish => _сaughtFish;

        public event Action<bool> OnFishingPlaceNearby;
        public event Action OnFishAssigned;

        private bool _isTriggered = false;

        #region UnityEvents
        private void Start()
        {
            Initialize(_asset);
        }

        /// <summary>
        /// Показываем кнопку, по нажатию которой запустится мини-игра ловли рыбы.
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<FishingPoint>(out var fishingPoint)) 
            {
                if (!_isTriggered) // Защита от срабатывания нескольких FishingPoint при попадании в триггер.
                {
                    _isTriggered = true;
                    _activeFishingPoint = fishingPoint;
                    _activeFishingPoint.SetActive(true);
                    OnFishingPlaceNearby(true);
                }
            }
        }

        /// <summary>
        /// Перестаем показывать кнопку, по нажатию которой запустится мини-игра ловли рыбы.
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision) 
        {
            if (collision.TryGetComponent<FishingPoint>(out var fishingPoint))
            {
                _isTriggered = false;
                _activeFishingPoint = fishingPoint;
                _activeFishingPoint.SetActive(false);
                _activeFishingPoint = null;
                OnFishingPlaceNearby(false);

                FindFishNearby();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Для удобства Помогает отобразить радиус действия удочки на сцене.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
#endif
        #endregion

        /// <summary>
        /// Дополнительная проверка на рыбу вокруг. На случай, если в радиусе действия было несколько точек рыбы, и сработала защита OnTriggerEnter.
        /// </summary>
        private void FindFishNearby()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _radius);

            if (hits.Length > 0)
            {
                List<FishingPoint> fishingPoints = new ();
                
                foreach (var hit in hits)
                {
                    if (hit.gameObject.TryGetComponent<FishingPoint>(out var point))
                    {
                        fishingPoints.Add(point);
                    }
                }

                FishingPoint closestFishingPoint = null;
                float minDistance = Mathf.Infinity;

                foreach (var fishingPoint in fishingPoints)
                {                   
                    float distance = Vector2.Distance(transform.position, fishingPoint.transform.position);
                    
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestFishingPoint = fishingPoint;
                    }
                }

                if (closestFishingPoint != null)
                {
                    if (!_isTriggered)
                    {
                        _isTriggered = true;
                        _activeFishingPoint = closestFishingPoint;
                        _activeFishingPoint.SetActive(true);
                        OnFishingPlaceNearby(true);
                    }
                }
            }
        }

        /// <summary>
        /// В зависимости от заданного ScriptableObject задает параметры экземпляра класса.
        /// </summary>
        /// <param name="asset"></param>
        public void Initialize(FishingRodAsset asset)
        {
            _name = asset.Name; 
            _description = asset.Description;
            _speed = asset.Speed;
            _radius = asset.Radius;
            _cost = asset.Cost;

            _circleCollider.radius = _radius;
        }

        /// <summary>
        /// Сохраняет информацию о последней пойманной рыбе.
        /// </summary>
        /// <param name="fish"></param>
        public void AssignFish(Fish fish)
        {
            _сaughtFish = fish;

            OnFishAssigned?.Invoke();
        }

        /// <summary>
        /// Добавляет вес пойманной рыбы к текущему весу корабля.
        /// </summary>
        public void TryPutFishInShip()
        {
            if (_сaughtFish != null)
            {
                Player.Instance.Ship.FishContainer.ChangeWeightAmount(_сaughtFish.Weight);
                Player.Instance.Ship.FishContainer.ChangeCostAmount(_сaughtFish.Cost);
            }
        }
    }
}