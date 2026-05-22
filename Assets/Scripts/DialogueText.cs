using System.Collections;
using UnityEngine;
using TMPro;
using Cinemachine;

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue Lines")]
    [TextArea] public string[] lines;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Settings")]
    public float interactRange = 3f;
    public KeyCode interactKey = KeyCode.R;

    [Header("Training Dummy")]
    public GameObject trainingDummy;
    public int dummyRevealLine = 6;

    [Header("Weapons")]
    public SwordPickup dualSwordsPickup;
    public int dualSwordsRevealLine = 7;
    public SwordPickup singleSwordPickup;
    public int singleSwordRevealLine = 8;

    [Header("Camera")]
    public CinemachineFreeLook freeLookCamera;

    private Transform player;
    private PlayerMovement playerMovement;
    private int currentLine = 0;
    private bool isOpen = false;
    private bool isTyping = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement>();
        dialoguePanel.SetActive(false);

        if (trainingDummy != null)
            trainingDummy.SetActive(false);
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (Input.GetKeyDown(interactKey))
        {
            if (!isOpen && dist <= interactRange)
            {
                StartDialogue();
            }
            else if (isOpen)
            {
                if (isTyping)
                    FinishLine();
                else
                    NextLine();
            }
        }

        if (isOpen && (Input.GetKeyDown(KeyCode.Escape) || dist > interactRange + 1f))
            CloseDialogue();
    }

    void StartDialogue()
    {
        isOpen = true;
        currentLine = 0;
        dialoguePanel.SetActive(true);
        LockPlayer(true);
        StartCoroutine(TypeLine(lines[currentLine]));
    }

    void NextLine()
    {
        currentLine++;

        if (currentLine >= lines.Length)
        {
            CloseDialogue();
            return;
        }

        if (currentLine == dummyRevealLine && trainingDummy != null)
            trainingDummy.SetActive(true);

        if (currentLine == dualSwordsRevealLine && dualSwordsPickup != null)
            dualSwordsPickup.RevealOnGround();

        if (currentLine == singleSwordRevealLine && singleSwordPickup != null)
            singleSwordPickup.RevealOnGround();

        StartCoroutine(TypeLine(lines[currentLine]));
    }

    void CloseDialogue()
    {
        isOpen = false;
        StopAllCoroutines();
        dialoguePanel.SetActive(false);
        LockPlayer(false);
    }

    void FinishLine()
    {
        StopAllCoroutines();
        dialogueText.text = lines[currentLine];
        isTyping = false;
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.03f);
        }
        isTyping = false;
    }

    void LockPlayer(bool locked)
    {
        if (freeLookCamera != null)
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = locked ? 0 : 350;
            freeLookCamera.m_YAxis.m_MaxSpeed = locked ? 0 : 3.5f;
        }

        if (playerMovement != null)
            playerMovement.enabled = !locked;
    }

    void OnGUI()
    {
        if (!isOpen && player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= interactRange)
            {
                GUIStyle style = new GUIStyle();
                style.fontSize = 20;
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 50, 300, 30),
                    "[R] Talk", style);
            }
        }
    }
}