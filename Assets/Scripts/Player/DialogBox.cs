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
        if (isTyping) return;

        if (index == 2)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                NextLine();
            }
        }
        else if (index == 3)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                NextLine();
            }
        }
        else if (index == 4)
        {
            if (Input.GetMouseButtonDown(0))
            {
                NextLine();
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
        }
    }

    IEnumerator DelayedTypeLine()
    {
        yield return new WaitForSeconds(1f); 
        StartCoroutine(TypeLine());
    }

    IEnumerator DelayedNextLine()
    {
        yield return new WaitForSeconds(0.7f); 
        NextLine();
    }
}
