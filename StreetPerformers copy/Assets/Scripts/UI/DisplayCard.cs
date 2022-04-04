using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class DisplayCard : MonoBehaviour
    {
        [Header("Icons")]
        public Sprite _attackImage;
        public Sprite _defenseImage;
        public Sprite _supportImage;

        [Header("Backgrounds")]
        [SerializeField]
        private Sprite _attackBackground;
        [SerializeField]
        private Sprite _defenseBackground;
        [SerializeField]
        private Sprite _supportBackground;

        [Header("Suits")]
        [SerializeField]
        private Sprite _normal;
        [SerializeField]
        private Sprite _spades;
        [SerializeField]
        private Sprite _clubs;
        [SerializeField]
        private Sprite _diamonds;
        [SerializeField]
        private Sprite _hearts;

        [Header("Backsides")]
        [SerializeField]
        private Sprite _aceBack;
        [SerializeField]
        private Sprite _contortionistBack;
        [SerializeField]
        private Sprite _mascotBack;
        [SerializeField]
        private Sprite _mimeBack;

        private CardScriptable _scriptable;

        public bool _visible = false;

        public void Initialize(CardScriptable scriptable)
        {
            _scriptable = scriptable;
            _visible = true;

            transform.Find("Name").GetComponent<TextMeshProUGUI>().text = _scriptable._cardName;
            string manaText = "";
            switch(_scriptable._manaType)
            {
                case CardScriptable.ManaType.VALUE:
                    manaText = "" + _scriptable._manaCost;
                    break;
                case CardScriptable.ManaType.MAX:
                    manaText = "MAX";
                    break;
                case CardScriptable.ManaType.X:
                    manaText = "X";
                    break;
                default:
                    break;
            }
            transform.Find("Mana").gameObject.SetActive(true);
            transform.Find("Mana").Find("ManaCost").GetComponent<TextMeshProUGUI>().text = manaText;
            transform.Find("Description").GetComponent<TextMeshProUGUI>().text = _scriptable._cardDesc;

            //Sets the color of the card background based off the card class
            switch(_scriptable._class)
            {
                case CardScriptable.CardClass.ATTACK:
                    transform.Find("Class").GetComponent<Image>().sprite = _attackImage;
                    transform.Find("Background").GetComponent<Image>().sprite = _attackBackground;
                    break;
                case CardScriptable.CardClass.DEFENSE:
                    transform.Find("Class").GetComponent<Image>().sprite = _defenseImage;
                    transform.Find("Background").GetComponent<Image>().sprite = _defenseBackground;
                    break;
                case CardScriptable.CardClass.SUPPORT:
                    transform.Find("Class").GetComponent<Image>().sprite = _supportImage;
                    transform.Find("Background").GetComponent<Image>().sprite = _supportBackground;
                    break;
                default:
                    break;
            }

            Image cardImage = transform.Find("CardImage").GetComponent<Image>();
            Image cardBack = transform.Find("CardBackside").GetComponent<Image>();
            cardBack.gameObject.SetActive(false);
            switch(_scriptable._id.Split('-')[0])
            {
                case "Ace":
                    cardImage.sprite = _spades;
                    cardBack.sprite = _aceBack;
                    break;
                case "Contortionist":
                    cardImage.sprite = _diamonds;
                    cardBack.sprite = _contortionistBack;
                    break;
                case "Mascot":
                    cardImage.sprite = _hearts;
                    cardBack.sprite = _mascotBack;
                    break;
                case "Mime":
                    cardImage.sprite = _clubs;
                    cardBack.sprite = _mimeBack;
                    break;
                default:
                    cardImage.sprite = _normal;
                    cardBack.sprite = _aceBack;
                    break;
            }
        }

        public void FlipToBack(string character)
        {
            _visible = false;
            Image cardBack = transform.Find("CardBackside").GetComponent<Image>();
            switch(character)
            {
                case "Ace":
                    cardBack.sprite = _aceBack;
                    break;
                case "Contortionist":
                    cardBack.sprite = _contortionistBack;
                    break;
                case "Mascot":
                    cardBack.sprite = _mascotBack;
                    break;
                case "Mime":
                    cardBack.sprite = _mimeBack;
                    break;
                default:
                    cardBack.sprite = _aceBack;
                    break;
            }

            cardBack.gameObject.SetActive(true);
            transform.Find("Mana").gameObject.SetActive(false);
        }
    }

}