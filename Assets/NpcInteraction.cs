using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NpcInteraction : MonoBehaviour
{
    public GameObject dialogueUI;
    public Text dialogueText;
    public GameObject talkToNpc;
    public string[] dialogueLines;
    private int currentLine = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    public float typingSpeed = 0.05f;
    private bool isPlayerInRange = false;
    public Animator npcAnimator; // Reference to the NPC's Animator

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.T))
        {
            if (dialogueUI.activeSelf)
            {
                if (isTyping)
                    FinishTypingCurrentLine();
                else
                    ShowNextLine();
            }
            else
            {
                talkToNpc.SetActive(false);
                dialogueUI.SetActive(true);
                ShowNextLine();

                // Trigger talking animation
                if (npcAnimator != null)
                    npcAnimator.SetBool("IsTalking", true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            talkToNpc.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            talkToNpc.SetActive(false);
            dialogueUI.SetActive(false);
            ResetDialogue();

            // Stop talking animation
            if (npcAnimator != null)
                npcAnimator.SetBool("IsTalking", false);
        }
    }

    private void ShowNextLine()
    {
        if (dialogueLines == null || dialogueLines.Length == 0) return;

        if (currentLine < dialogueLines.Length)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLine]));
            currentLine++;
        }
        else
        {
            dialogueUI.SetActive(false);
            ResetDialogue();

            // Stop talking animation
            if (npcAnimator != null)
                npcAnimator.SetBool("IsTalking", false);
        }
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void FinishTypingCurrentLine()
    {
        if (currentLine > 0)
        {
            dialogueText.text = dialogueLines[currentLine - 1];
            isTyping = false;

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
        }
    }

    private void ResetDialogue()
    {
        currentLine = 0;
        isTyping = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }
}
