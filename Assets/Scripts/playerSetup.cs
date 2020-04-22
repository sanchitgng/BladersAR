using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class playerSetup : MonoBehaviourPun
{
    public TextMeshProUGUI playerNameText;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            // the player is local player
            transform.GetComponent<Movement_Controller>().enabled = true;
            transform.GetComponent<Movement_Controller>().joystick.gameObject.SetActive(true);
        }
        else
        {
            // The player is remote player
            transform.GetComponent<Movement_Controller>().enabled = false;
            transform.GetComponent<Movement_Controller>().joystick.gameObject.SetActive(false);
        }
        SetPlayerName();
    }

    public void SetPlayerName()
    {
        if(playerNameText != null)
        {
            if (photonView.IsMine)
            {
                playerNameText.text = "You";
                playerNameText.color = Color.red;
            }
            else
            {
                playerNameText.text = photonView.Owner.NickName;
            }
        }
    }
}
