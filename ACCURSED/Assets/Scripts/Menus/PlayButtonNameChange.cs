using Esper.ESave;
using TMPro;
using UnityEngine;

public class PlayButtonNameChange : MonoBehaviour
{
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private string continueText = "Continue";
    [SerializeField] private string newGameText = "New Game";

    private void Start()
    {
        if (buttonText == null)
        {
            Debug.LogError("Play button text is not assigned on " + gameObject.name, this);
            return;
        }

        if (SaveStorage.instance == null)
        {
            Debug.LogError("SaveStorage is unavailable; cannot determine the play button label.", this);
            return;
        }

        buttonText.text = SaveMenuButton.HasAnySave ? continueText : newGameText;
    }
}
