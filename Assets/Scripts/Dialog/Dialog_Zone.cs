using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog_Zone : MonoBehaviour
{
    GameObject playerInDialoge;
    public Dialog_Manager_Puzzle dialog_Manager;
    [SerializeField]
    GameObject F_sign;

    bool _playerInDialog = false;

    private void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.tag == "Player") && !_playerInDialog)
        {

            var interactF = other.GetComponent<CharacterMovement>().interactPressed;
            

            F_sign.SetActive(true);

            
            if (interactF)
            {
                F_sign.SetActive(false);
                other.GetComponent<CharacterMovement>().interactPressed = false;
                playerInDialoge = other.gameObject;
                other.GetComponent<CharacterMovement>().InDialog = true;
                other.GetComponent<CharacterController>().enabled = false;
                dialog_Manager.StartDialog();
                _playerInDialog = true;
            }
        }
        else
        {
            F_sign.SetActive(false);
        }
    }

    public void OnExitDialog()
    {
       playerInDialoge.GetComponent<CharacterController>().enabled = true;
       playerInDialoge.GetComponent<CharacterMovement>().InDialog = false;
        _playerInDialog = false;
    }

    private void OnTriggerExit(Collider other)
    {
        F_sign.SetActive(false);
    }
}
