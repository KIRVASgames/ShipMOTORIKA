using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ShipMotorika
{
    public sealed class FishingChallenge : MonoBehaviour
    {
        [SerializeField] private Image _fishCircleImage;
        [SerializeField] private Image _playerCircleImage;   
        [SerializeField] private float _waitBeforeDestroyTime = 1f;

        public static event Action<bool> OnTryCatchFish;
        public event Action OnDestroy;

        private Vector3 _defaultPlayerScale;

        private Color _defaultFishColor;
        private Color _defaultPlayerColor;

        private readonly float _minScale = 1f;
        private readonly float _maxScale = 6f;

        private readonly float _passScaleMin = 3f;
        private readonly float _passScaleMax = 4f;

        private float _scale;
        private float _speed = 1f;

        private bool _isLooped;

        private void Start()
        {
            _speed = Player.Instance.GetComponent<FishingRod>().Speed;

            SaveParametrs();
        }

        private void Update()
        {
            DoCircleAnimation();

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                enabled = false;

                TryCatchFish();
            }
        }

        private void SaveParametrs()
        {
            _defaultFishColor = _fishCircleImage.color;
            _defaultPlayerColor = _playerCircleImage.color;

            _defaultPlayerScale = _playerCircleImage.rectTransform.localScale;
            _scale = _minScale;

            _isLooped = false;
        }

        private void RestoreParametrs()
        {
            _fishCircleImage.color = _defaultFishColor;
            _playerCircleImage.color = _defaultPlayerColor;

            _playerCircleImage.rectTransform.localScale = _defaultPlayerScale;
            _scale = _minScale;

            _isLooped = false;
        }

        private void DoCircleAnimation()
        {
            if (_isLooped)
            {
                _scale -= _speed * Time.deltaTime;

                if (_scale <= _minScale)
                {
                    _isLooped = false;
                }
            }
            else
            {
                _scale += _speed * Time.deltaTime;

                if (_scale >= _maxScale)
                {
                    _isLooped = true;
                }
            }

            _playerCircleImage.rectTransform.localScale = new Vector3(_scale, _scale, _scale);
        }

        private void TryCatchFish()
        {
            if (_scale >= _passScaleMin && _scale <= _passScaleMax)
            {
                _fishCircleImage.color = Color.green;
                OnTryCatchFish?.Invoke(true);

                Debug.Log("Success!");
            }
            else
            {
                _fishCircleImage.color = Color.red;
                OnTryCatchFish?.Invoke(false);

                Debug.Log("Failure!");
            }

            StartCoroutine(DestroyItself());
        }

        private IEnumerator DestroyItself()
        {
            yield return new WaitForSeconds(_waitBeforeDestroyTime);
            RestoreParametrs();
            Destroy(gameObject);
            OnDestroy?.Invoke();
        }
    }
}