using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    public class DeleteSave : MonoBehaviour
    {
        [SerializeField]
        private GameObject _partyButton;

        private GameObject _deleteText;
        private GameObject _cancelButton;
        private GameObject _confirmButton;
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(FirstClick);

            _deleteText = transform.parent.Find("DeleteText").gameObject;
            _cancelButton = transform.parent.Find("CancelDelete").gameObject;
            _confirmButton = transform.parent.Find("ConfirmDelete").gameObject;

            _cancelButton.GetComponent<Button>().onClick.AddListener(ResetButtons);
            _confirmButton.GetComponent<Button>().onClick.AddListener(Delete);
        }

        public void FirstClick()
        {
            this.gameObject.SetActive(false);
            if(_partyButton != null)
            {
                _partyButton.SetActive(false);
            }

            _deleteText.SetActive(true);
            _cancelButton.SetActive(true);
            _confirmButton.SetActive(true);
        }

        public void Delete()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameAudio.Instance.Button_Forward);

            ProgressionHandler.Instance.DeleteCurrentSave();

            _deleteText.SetActive(false);
            _cancelButton.SetActive(false);
            _confirmButton.SetActive(false);
        }

        public void ResetButtons()
        {
            this.gameObject.SetActive(true);
            if(_partyButton != null)
            {
                _partyButton.SetActive(true);
            }

            _deleteText.SetActive(false);
            _cancelButton.SetActive(false);
            _confirmButton.SetActive(false);
        }
    }

}
