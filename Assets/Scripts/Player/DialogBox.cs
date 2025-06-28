using UnityEngine;
using TMPro;
using System.Collections;

public class DialogBox : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    private int index;
    private bool isTyping = false;

    void Start()
    {
        textComponent.text = string.Empty;
        StartDialog();
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    StopAllCoroutines();
        //    gameObject.SetActive(false);
        //    return;
        //}
        if (isTyping) return;

       
            if (Input.GetKeyDown(KeyCode.F))
            {
                NextLine();
              //  Debug.Log("@@@");
            }
        
        
        
           
        
    }

    void StartDialog()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        textComponent.text = string.Empty;

        foreach (char letter in lines[index])
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;

        if (index == 0 || index == 1)
        {
            StartCoroutine(DelayedNextLine());
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
           StartCoroutine(DelayedTypeLine());
        }
        else
        {

            gameObject.SetActive(false);
            //DestroyObject(gameObject);
        }
    }

    IEnumerator DelayedTypeLine()
    {
        yield return new WaitForSeconds(0.07f); 
        StartCoroutine(TypeLine());
    }

    IEnumerator DelayedNextLine()
    {
        yield return new WaitForSeconds(0f); 
        NextLine();
    }
}
