using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterController : MonoBehaviour
{
    public TextMeshProUGUI mainText;
    public List<TextMeshProUGUI> alternativeTexts;
    public Color highlightText;
    public TextMeshProUGUI dayText;
    public GameObject drawStitching;
    public GameObject drawRT;
    public List<AudioClip> penStrokes;
    public List<AudioClip> pageTurns;
    public AudioClip penOpen;
    public AudioClip penClose;
    public AudioSource penCapSource;
    public AudioSource pageTurnSource;

    //awful
    public TextMeshProUGUI introTextField1;
    public TextMeshProUGUI introTextField2;
    public TextMeshProUGUI introTextField3;
    public TextMeshProUGUI introTextField4;

    //time pressure stuff
    public TextMeshProUGUI outroTextField;
    public TextMeshProUGUI enterText;

    private List<string> alternativeWords;
    private Color normalText;
    private StoryController storyController;
    private int? chosenWord = null;
    private string chosenWordBuffer;
    private bool pageDoneFlag;
    private bool dayDoneFlag;
    private bool movingPages;
    private bool drawingMode;

    private bool ending;
    private int endingPaperNum = 10;

    private bool intro;

    private bool capOff;
    private bool hasDrawn;
    private float originalFontSizeAlternatives;
    private float originalFontSizeMainText;
    private PageController pageController;
    private DrawController drawController;
    private Canvas canvas;
    private Animator canvasAnim;
    private AudioSource audioSource;
    private bool gameOver;

    private void Start()
    {
        normalText = alternativeTexts[0].color;
        storyController = FindObjectOfType<StoryController>();
        pageController = FindObjectOfType<PageController>();
        drawController = FindObjectOfType<DrawController>();
        canvas = GetComponentInChildren<Canvas>();
        audioSource = GetComponent<AudioSource>();
        canvasAnim = canvas.GetComponent<Animator>();
        originalFontSizeAlternatives = alternativeTexts[0].fontSize;
        originalFontSizeMainText = mainText.fontSize;

        introTextField1.enabled = true;
        introTextField2.enabled = true;
        introTextField3.enabled = false;
        introTextField4.enabled = false;

        alternativeTexts[0].text = "";
        alternativeTexts[1].text = "";
        alternativeTexts[2].text = "ENTER>";
        intro = true;

        capOff = false;
        pageDoneFlag = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameOver)
        {
            return;
        }

        if (ending)
        {
            if (Input.anyKeyDown)
            {
                if (endingPaperNum > 0)
                {
                    StartCoroutine(PageTurnHandling());
                    endingPaperNum--;
                }
                else
                {
                    StartCoroutine(NewDayHandling());
                    dayText.text = "Thank you for playing";
                    dayText.fontSize = 10;

                    outroTextField.enabled = true;
                    enterText.enabled = false;

                    alternativeTexts[0].text = "";
                    alternativeTexts[1].text = "";
                    alternativeTexts[2].text = "";

                    pageController.DrawEndLogo();

                    gameOver = true;
                    return;
                }

            }
        }

        if (intro)
        {
            if (Input.GetButtonDown("Submit"))
            {
                introTextField1.enabled = false;
                introTextField2.enabled = false;
                introTextField3.enabled = true;
                introTextField4.enabled = true;
                intro = false;
                dayDoneFlag = true;
            }
            return;
        }

        if (Input.inputString.Length == 0)
            return;

        if (dayDoneFlag && !movingPages)
        {
            if (Input.GetButtonDown("Submit"))
            {
                StartCoroutine(NewDayHandling());
            }
            return;
        }

        if (pageDoneFlag && !movingPages)
        {
            if (Input.GetButtonDown("Submit"))
            {
                StartCoroutine(PageTurnHandling());
            }
            return;
        }

        if (drawingMode)
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                drawController.EraseDrawing();
            }

            if (Input.GetButtonDown("Submit"))
            {
                EndDrawing();
            }
        }


        //In the case of no started word, check if player starts a word.
        if (chosenWord == null && alternativeWords != null)
        {
            for (int i = 0; i < alternativeWords.Count; i++)
            {
                if (alternativeWords[i].Length > 0 && Input.inputString.ToLower()[0] == alternativeWords[i].ToLower()[0])
                {
                    chosenWord = i;
                    chosenWordBuffer = alternativeWords[i];
                    alternativeTexts[i].color = highlightText;
                }
            }
        }

        //Word is started, check for next letter.
        if (chosenWord != null)
        {
            if (chosenWord.HasValue && Input.inputString.ToLower()[0] == alternativeWords[chosenWord.Value].ToLower()[0])
            {
                mainText.text += alternativeWords[chosenWord.Value][0];
                alternativeWords[chosenWord.Value] = alternativeWords[chosenWord.Value].Substring(1);
                alternativeTexts[chosenWord.Value].text = alternativeWords[chosenWord.Value];
                PlayPenStroke();
            }

            if (alternativeWords[chosenWord.Value].Length <= 0)
            {
                if (CheckForSpecial())
                    return;

                chosenWord = null;
                UpdateAlternatives();
                ResetAlternativesFormatting();
                return;
            }
        }
    }

    private bool CheckForSpecial()
    {
        alternativeWords = storyController.GetNextWords(chosenWordBuffer).ToList();

        if (alternativeWords[0] == "\\end")
        {
            alternativeWords[0] = "";
            ending = true;
        }

        //Idiotic hard coding
        if (alternativeWords[0] == "\\n")
        {
            mainText.text += "\n\n";
            alternativeWords = storyController.GetNextWords(chosenWordBuffer).ToList();
        }

        //More big brain hard coding
        if (alternativeWords[0] == "\\pg")
        {
            NewPage();
            return true;
        }

        if (alternativeWords[0] == "\\draw")
        {
            NewDrawing();
            return true;
        }

        //I give up
        if (alternativeWords[0].Contains("DAY"))
        {
            NewDay();
            pageController.Curse();
            return true;
        }

        return false;
    }

    private void PlayPenStroke()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = penStrokes[Random.Range(0, penStrokes.Count)];
            audioSource.Play();
        }
    }
    private void PlayPenOpen()
    {
        if (!penCapSource.isPlaying && !capOff)
        {
            penCapSource.clip = penOpen;
            penCapSource.Play();
            capOff = true;
        }
    }
    private void PlayPenClose()
    {
        if (!penCapSource.isPlaying && capOff)
        {
            penCapSource.clip = penClose;
            penCapSource.Play();
            capOff = false;
        }
    }

    private void PlayPageTurn()
    {
        if (!pageTurnSource.isPlaying)
        {
            pageTurnSource.clip = pageTurns[Random.Range(0, pageTurns.Count)];
            pageTurnSource.Play();
        }
    }


    private void NewDrawing()
    {
        ResetAlternativesFormatting();
        drawController.enableDrawing = true;
        drawingMode = true;
        drawStitching.SetActive(true);
        alternativeWords[0] = "<backspace";
        alternativeWords[1] = "draw";
        alternativeWords[2] = "ENTER>";

        alternativeTexts[1].color = highlightText;

        UpdateAlternatives();
    }

    private void EndDrawing()
    {
        drawController.Save();
        drawController.enableDrawing = false;
        drawingMode = false;
        drawStitching.SetActive(false);
        alternativeWords[0] = "";
        alternativeWords[1] = "";
        alternativeWords[2] = "";
        chosenWord = null;
        hasDrawn = true;
        CheckForSpecial();
        UpdateAlternatives();
        ResetAlternativesFormatting();
    }

    private void NewPage()
    {
        ResetAlternativesFormatting();
        pageDoneFlag = true;

        alternativeWords[0] = "";
        alternativeWords[2] = "ENTER>";

        alternativeTexts[1].color = highlightText;
        alternativeTexts[1].fontSize = alternativeTexts[1].fontSize * 1.35f;

        chosenWord = null;
        UpdateAlternatives();
    }

    private void NewDay()
    {
        dayText.text = alternativeWords[0];

        ResetAlternativesFormatting();
        dayDoneFlag = true;
        alternativeWords[0] = "";
        alternativeWords[2] = "ENTER>";

        alternativeTexts[1].color = highlightText;
        alternativeTexts[1].fontSize = alternativeTexts[1].fontSize * 1.35f;

        chosenWord = null;
        UpdateAlternatives();
    }

    IEnumerator NewDayHandling()
    {
        canvasAnim.Play("SendLetter");
        movingPages = true;
        PlayPageTurn();

        yield return new WaitForSeconds(2);

        PlayPenClose();
        dayDoneFlag = false;
        pageDoneFlag = true;
        movingPages = false;
    }

    IEnumerator PageTurnHandling()
    {
        PlayPageTurn();

        var sortingsOrderIndex = pageController.TurnPage();
        movingPages = true;

        yield return new WaitForSeconds(2);

        PlayPenOpen();
        alternativeWords = storyController.GetNextWords(chosenWordBuffer).ToList();
        UpdateAlternatives();
        ResetAlternativesFormatting();
        canvas.sortingOrder = sortingsOrderIndex - 1;

        for (int i = 0; i < alternativeTexts.Count; i++)
        {
            alternativeTexts[i].enabled = true;
        }

        if (hasDrawn)
        {
            drawRT.SetActive(false);
        }

        mainText.text = "";
        mainText.enabled = true;

        pageDoneFlag = false;
        movingPages = false;
    }

    void ResetAlternativesFormatting()
    {
        for (int i = 0; i < alternativeWords.Count; i++)
        {
            alternativeTexts[i].color = normalText;
            alternativeTexts[i].fontSize = originalFontSizeAlternatives;
        }

        mainText.fontSize = originalFontSizeMainText;
    }

    void UpdateAlternatives()
    {
        for (int i = 0; i < alternativeWords.Count; i++)
        {
            alternativeTexts[i].text = alternativeWords[i];
        }
    }

    public void DeletePages()
    {
        mainText.text = "";

        if (hasDrawn)
        {
            drawRT.SetActive(false);
        }

        for (int i = 0; i < alternativeTexts.Count; i++)
        {
            alternativeTexts[i].text = "";
        }
    }
}
