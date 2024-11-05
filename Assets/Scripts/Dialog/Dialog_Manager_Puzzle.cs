using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog_Manager_Puzzle : MonoBehaviour
{
    public GameObject first_replica;
    public GameObject number_replica;

    public GameObject higher_replica;
    public GameObject lower_replica;

    public GameObject out_replica;
    public GameObject back_in_replica;

    public GameObject win_replica;
    public GameObject lose_replica;

    public Dialog_Zone dialogZone;

    [SerializeField]
    int anwserNumber;

    public AudioSource Talk;

    int numberOfTry;

    bool firstTimeDialog = true;

    [SerializeField]
    CharacterMovement CharacterControl;

    public void StartDialog()
    {
        if (firstTimeDialog)
        {
            firstTimeDialog = false;
            first_replica.SetActive(true);
            
        }
        else if (!firstTimeDialog) 
        {
            BackInDialog();
        }
    }

    void GenerateAnwserNumber()
    {
        anwserNumber = Random.Range(1, 11);
    }

    public void CompareTheNumber(int num)
    {
        if ((num == anwserNumber) && (numberOfTry < 3))
        {
            WinDialog();
        }
        else if ((num < anwserNumber) && (numberOfTry < 3))
        {
            HigherDialog();
        }
        else if ((num > anwserNumber) && (numberOfTry < 3))
        {
            LowerDialog();
        }
        else
        {
            LoseDialog();
        }
    }

    public void OutDialog()
    {
        out_replica.SetActive(true);
    }

    public void BackInDialog()
    {
        back_in_replica.SetActive(true);
    }

    public void HigherDialog()
    {
        higher_replica.SetActive(true);
    }

    public void LowerDialog()
    {
        lower_replica.SetActive(true);
    }

    public void WinDialog()
    {
        win_replica.SetActive(true);
    }

    public void LoseDialog()
    {
        lose_replica.SetActive(true);
    }
    public void NumberDialog()
    {
        GenerateAnwserNumber();
        number_replica.SetActive(true);
    }
    public void addTryScore()
    {
        numberOfTry++;
    }

    public void TallkSoundPlay()
    {
        Talk.Play();
    }
    public void TallkSoundStop()
    {
        Talk.Stop();
    }




}
