using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IngameUI : MonoBehaviour
{
    public GameObject virtualController;
    public GameObject endScreen;

    public TextMeshProUGUI timeleftText;
    public TextMeshProUGUI endScreenResult;

    // Start is called before the first frame update
    void Start()
    {
        virtualController.gameObject.SetActive(true);
        endScreen.gameObject.SetActive(false);
    }

    public void UpdateTimeLeft(float val)
    {
        timeleftText.text = "Timeleft\n" + (int)val;
    }

    public void ShowEndGameUI(string resultGame)
    {
        virtualController.gameObject.SetActive(false);
        endScreen.gameObject.SetActive(true);
        endScreenResult.text = resultGame;
    }

    public void ResetGameUI()
    {
        virtualController.gameObject.SetActive(true);
        endScreen.gameObject.SetActive(false);
    }
}
