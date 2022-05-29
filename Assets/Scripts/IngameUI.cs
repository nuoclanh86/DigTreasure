using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class IngameUI : MonoBehaviour
{
    public GameObject virtualController;
    public GameObject endScreen;
    public GameObject playAgainBtn;

    public TextMeshProUGUI timeleftText;
    public TextMeshProUGUI endScreenResult;

    public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        ResetGameUI();
    }

    public void UpdateTimeLeft(int val)
    {
        //Debug.Log("UpdateTimeLeft : " + val);
        timeleftText.text = "Timeleft\n" + val;
    }

    public void ShowEndGameUI(string resultGame)
    {
        Debug.Log("ShowEndGameUI");
        virtualController.gameObject.SetActive(false);
        endScreen.gameObject.SetActive(true);
        endScreenResult.text = resultGame;
        Debug.Log("PhotonView.ID: " + photonView.ViewID);
        Debug.Log("IsMasterClient: " + PhotonNetwork.IsMasterClient + " - photonView.IsMine: " + photonView.IsMine);
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
            playAgainBtn.gameObject.SetActive(true);
        else
            playAgainBtn.gameObject.SetActive(false);
    }

    public void ResetGameUI()
    {
        if (photonView.IsMine)
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
