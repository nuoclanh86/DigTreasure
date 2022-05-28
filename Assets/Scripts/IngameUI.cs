using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class IngameUI : MonoBehaviour
{
    public GameObject virtualController;
    public GameObject endScreen;

    public TextMeshProUGUI timeleftText;
    public TextMeshProUGUI endScreenResult;

    public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        ResetGameUI();
    }

    public void UpdateTimeLeft(float val)
    {
        timeleftText.text = "Timeleft\n" + (int)val;
    }

    public void ShowEndGameUI(string resultGame)
    {
        if (PhotonNetwork.InRoom && photonView.IsMine)
        {
            virtualController.gameObject.SetActive(false);
            endScreen.gameObject.SetActive(true);
            endScreenResult.text = resultGame;
        }
    }

    public void ResetGameUI()
    {
        if (PhotonNetwork.InRoom && photonView.IsMine)
        {
            virtualController.gameObject.SetActive(true);
            endScreen.gameObject.SetActive(false);
        }
        else
        {
            virtualController.gameObject.SetActive(false);
            endScreen.gameObject.SetActive(false);
        }
    }
}
