using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI TextComponent;
    public string[] lines;
    public float textSpeed;

    private int index;

    [SerializeField]
    CharacterMovement CharacterControl;

    [SerializeField]
    GameObject Anwser;

    [SerializeField]
    Dialog_Manager_Puzzle manager;

    // Start is called before the first frame update
    void Awake()
    {
        TextComponent.text = string.Empty;
        StartDialog();
    }

    // Update is called once per frame
    void Update()
    {
        if(CharacterControl.GetComponent<CharacterMovement>().LMBPressed)
        {
            CharacterControl.GetComponent<CharacterMovement>().LMBPressed = false;
            if (TextComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                TextComponent.text = lines[index];
            }
        }
    }

    void StartDialog()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            manager.TallkSoundPlay();
            TextComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length -1)
        {
            index++;
            TextComponent.text = string.Empty;
            StartCoroutine (TypeLine());
        }
        else
        {
            Anwser.SetActive(true);
            manager.TallkSoundStop();
        }
    }
}
