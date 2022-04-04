using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class NewMember : MonoBehaviour
    {
        [Header("Character Sprites")]
        [SerializeField]
        private Sprite _contortionistDefault;
        [SerializeField]
        private Sprite _mascotDefault;
        [SerializeField]
        private Sprite _mimeDefault;

        private List<Transform> _cards;

        private bool _selecting = false;

        private Transform _centerCard;
        private Transform _selectedCard;
        private Vector3 _originalPosition;
        private Vector3 _originalScale;

        public void Initialize(string name)
        {

            _centerCard = transform.Find("Card-View");
            _cards = new List<Transform>();

            transform.Find("Title").GetComponent<TextMeshProUGUI>().text = "The " + name + " has joined your team!";

            switch(name)
            {
                case "Contortionist":
                    transform.Find("Image").GetComponent<Image>().sprite = _contortionistDefault;
                    break;
                case "Mascot":
                    transform.Find("Image").GetComponent<Image>().sprite = _mascotDefault;
                    break;
                case "Mime":
                    transform.Find("Image").GetComponent<Image>().sprite = _mimeDefault;
                    break;
            }

            transform.Find("Image").GetComponent<Image>().SetNativeSize();

            List<CardScriptable> cards = new List<CardScriptable>();
            Object[] cardList = Resources.LoadAll("ScriptableObjects/" + name + "/Party/");
            foreach(Object card in cardList)
            {
                CardScriptable scriptable = (CardScriptable)card;
                if(scriptable._requiredLevel <= 1)
                {
                    cards.Add((CardScriptable)card);
                }
            }

            for(int i = 2; i <= 5; i++)
            {
                Transform cardPos = transform.Find("Card-" + i);
                Button cardButton = cardPos.GetComponent<Button>();
                cardButton.onClick.RemoveAllListeners();

                cardButton.enabled = true;
                Card cardScr = cardPos.GetComponent<Card>();
                cardScr.Initialize(cards[i - 2]);
                cardScr._cardImage.raycastTarget = true;

                int index = i - 2;
                cardButton.onClick.AddListener(delegate { Select(index); });

                _cards.Add(cardPos);
            }
        }

        public void Select(int index)
        {
            if(_selecting || _selectedCard != null)
            { return; }

            _selecting = true;
            _originalPosition = _cards[index].position;
            _originalScale = _cards[index].localScale;

            _cards[index].GetComponent<Card>()._backgroundImage.gameObject.SetActive(true);
            _cards[index].SetAsLastSibling();
            _cards[index].DOMove(_centerCard.position, .5f).SetUpdate(true).OnComplete(delegate { _selecting = false; });
            _cards[index].DOScale(_centerCard.localScale, .5f).SetUpdate(true);

            _selectedCard = _cards[index];
            return;
        }

        private void Deselect()
        {
            if(_selecting)
            { return; }

            _selecting = true;
            _selectedCard.GetComponent<Card>()._backgroundImage.gameObject.SetActive(false);
            _selectedCard.DOMove(_originalPosition, .5f).SetUpdate(true).OnComplete(delegate { _selecting = false; });
            _selectedCard.DOScale(_originalScale, .5f).SetUpdate(true);
            _selectedCard = null;
        }

        private void Update()
        {
            if(_selecting || _selectedCard == null)
            { return; }

            if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                Deselect();
            }

            if(Input.touchCount > 0)
            {
                if(Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Deselect();
                }
            }
        }
    }

}