using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace ShipMotorika
{
    /// <summary>
    /// Открывает информацию о пойманной рыбе в интерфейсе при успешном прохождении мини-игры FishingChallenge.
    /// </summary>
    public class CaughtFishUI : MonoBehaviour
    {
        [SerializeField] private GameObject _canvasPanel;
        [SerializeField] private Image _image;
        [SerializeField] private Button _accept;
        [SerializeField] private Button _decline;

        #region UnityEvents
        private void Start()
        {
            _canvasPanel.SetActive(false);

            _accept.onClick.AddListener(DoOnAccept);
            _decline.onClick.AddListener(DoOnDecline);
            FishingChallenge.Instance.OnTryCatchFish += ActivatePanel;
            Player.Instance.FishingRod.OnFishAssigned += SetFishImage;
        }

        private void OnDestroy()
        {
            _accept.onClick.RemoveListener(DoOnAccept);
            _decline.onClick.RemoveListener(DoOnDecline);
            FishingChallenge.Instance.OnTryCatchFish -= ActivatePanel;
            Player.Instance.FishingRod.OnFishAssigned -= SetFishImage;
        }
        #endregion

        /// <summary>
        /// Взять рыбу.
        /// </summary>
        private void DoOnAccept()
        {
            _canvasPanel.SetActive(false);

            Player.Instance.FishingRod.TryPutFishInShip();
            FishingChallenge.Instance.Deactivate();
        }

        /// <summary>
        /// Отпустить рыбу.
        /// </summary>
        private void DoOnDecline()
        {
            _canvasPanel.SetActive(false);

            Player.Instance.FishingRod.AssignFish(null);
            FishingChallenge.Instance.Deactivate();
        }

        private void ActivatePanel(bool success)
        {
            if (success)
            {
                _canvasPanel.SetActive(true);
            }
        }

        private void SetFishImage()
        {
            if (Player.Instance.FishingRod.CaughtFish != null)
            {
                _image.sprite = Player.Instance.FishingRod.CaughtFish.Sprite.sprite;
                _image.SetNativeSize(); // Attention! Only for Debug!
            }
        }
    }
}