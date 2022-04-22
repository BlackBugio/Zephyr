using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Security;
using UnityEngine.UI;



public class UiController : MonoBehaviour
{
    public Transform AttribPanel;
    public Transform InfoPanel;
    public Transform DungeonPanel;
    public Transform BattleTabPanel;
    public Button MysteryBox3, AddToParty, RemoveFromParty, TestBattle, BackToMainScreenButton;
    public Button[] Dungeon;
    public Transform mainCharGrid;
    public Transform totalPartyGrid;
    public Transform marketAvailableGrid;
    public Transform[] PartySlotGrid;
    public GameObject charLinePrefab;
    public GameObject partyLinePrefab;
    public CharBase charOnScreen;
    public PartyBase selectedParty;
    public GameObject[] Screens;
    public int currentScreen;
    public Transform feedbackBattlePanel;
    public Transform endBattlePanel;
    public GameObject textAnimationFeedback;


    //testing variables
    public TextMeshProUGUI partyPower;
    public TextMeshProUGUI userName;
    public TextMeshProUGUI zCoins;


    public delegate void CharOnScreen(CharBase c);
    public static event CharOnScreen OnCharOnScreen;


    private void OnEnable()
    {
        OnCharOnScreen += ShowCharOnScreen;
        BattleController.OnFeedbackLine += EnterFeedBAckLine;
        ScreenActive(0); //ativa login
        BackToMainScreenButton.onClick.AddListener(delegate { ScreenActive(1); });
    }

    // UI LABEL DEVE TER O FINAL DA NOMENCLATURA CORRETA ---->  label + " (" + a + ")"
    public void FillAttribPanel(CharBase charActive)
    {
        int a = 0;
        foreach (CharBase.Attributes i in charActive.AttributesD.Keys)
        {
            string label = "Attrib (" + a + ")";
            TextMeshProUGUI attribLabel = AttribPanel.Find(label).GetComponent<TextMeshProUGUI>();
            attribLabel.text = i.ToString();
            TextMeshProUGUI attribValue = attribLabel.transform.Find("value").GetComponent<TextMeshProUGUI>();
            attribValue.text = charActive.AttributesD[i].ToString();
            a++;
        }
        a = 0;
        //popula os bonus
        foreach (CharBase.Attributes i in charActive.AttributeBonusD.Keys)
        {
            string label = "Attrib (" + a + ")";
            TextMeshProUGUI attribLabel = AttribPanel.Find(label).GetComponent<TextMeshProUGUI>();
            attribLabel.text = i.ToString();
            TextMeshProUGUI attribValue = attribLabel.transform.Find("bonus").GetComponent<TextMeshProUGUI>();
            attribValue.text = charActive.AttributeBonusD[i].ToString();
            a++;
        }
    }

    public void FillInfo(CharBase character, Transform parent)
    {
        TextMeshProUGUI textValue;

        /* foreach (Transform t in parent)
         {
             string infoLabel = t.gameObject.GetComponent<TextMeshProUGUI>().text.ToString();
             textValue = t.Find("value").GetComponent<TextMeshProUGUI>();
             if (textValue != null)
                 textValue.text = charActive.charName;

         }*/

        textValue = parent.Find("Name").Find("value").GetComponent<TextMeshProUGUI>();
        if (textValue != null)
            textValue.text = character.charName;

        textValue = parent.Find("Level").Find("value").GetComponent<TextMeshProUGUI>();
        if (textValue != null)
            textValue.text = character.charLevel.ToString();

        try 
        {
            textValue = parent.Find("Rarity").Find("value").GetComponent<TextMeshProUGUI>();
            if (textValue != null)
                textValue.text = character.charRarity.ToString();
        }
             
              catch (System.InvalidCastException e)
        {
            Debug.LogWarning("Exception : " + e.Message);
        }

        textValue = parent.Find("Class").Find("value").GetComponent<TextMeshProUGUI>();
        if (textValue != null)
            textValue.text = character.charClass.ToString();

        textValue = parent.Find("Role").Find("value").GetComponent<TextMeshProUGUI>();
        if (textValue != null)
            textValue.text = character.charRole.ToString();

       
        try
        {
                textValue = parent.Find("MP").Find("value").GetComponent<TextMeshProUGUI>();
            if (textValue != null)
                textValue.text = character.MP.ToString();
        }

        catch (System.InvalidCastException e)
        {
            if (textValue != null)
                textValue.text = character.MP.ToString();
            Debug.LogWarning("Exception : " + e.Message);
        }

        textValue = parent.Find("Fatigue").Find("value").GetComponent<TextMeshProUGUI>();
        if (textValue != null)
            textValue.text = character.charFatigue.ToString();

        textValue = parent.Find("HP").Find("value").GetComponent<TextMeshProUGUI>();
        if (textValue != null)
            textValue.text = character.HP.ToString();

        textValue = parent.Find("XP").Find("value").GetComponent<TextMeshProUGUI>();
        if (textValue != null)
            textValue.text = character.charXP.ToString();

        /*costValue = InfoPanel.Find("Cost").Find("Value").GetComponent<Text>();
        costValue.text = charActive.charCost.ToString();*/
    }

    public void ShowCharOnScreen(CharBase character)
    {
        charOnScreen = character;
        FillAttribPanel(character);
        FillInfo(character, InfoPanel);
        if (character.party != null) selectedParty = character.party;
    }


    public void FillPartyPanel(PartyBase party, DungeonBase dungeon, Transform parent)
    {
        TextMeshProUGUI nameValue = parent.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();
        nameValue.text = party.name;

        TextMeshProUGUI powerValue = parent.Find("Power").GetComponent<TextMeshProUGUI>();
        powerValue.text = party.PartyPower.ToString();

        TextMeshProUGUI dungeonValue = parent.Find("Dungeon").GetComponent<TextMeshProUGUI>();
        dungeonValue.text = dungeon.name.ToString();

        TextMeshProUGUI difficultyValue = parent.Find("Difficulty").GetComponent<TextMeshProUGUI>();
        difficultyValue.text = dungeon.GetDifficulty.ToString();

        TextMeshProUGUI timeValue = parent.Find("Time").GetComponent<TextMeshProUGUI>();
        timeValue.text = dungeon.Duration.ToString();

        TextMeshProUGUI probabilityValue = parent.Find("Probability").GetComponent<TextMeshProUGUI>();
        probabilityValue.text = dungeon.Probability.ToString();
    }



    public void AddGrid(CharBase character, Transform grid, List<CharBase> l, bool add)
    {
        l.Add(character);
        GameObject newCharLine = (GameObject)Instantiate(charLinePrefab, grid);
        newCharLine.name = character.charName;
        FillInfoLine.FillCharLine(character, newCharLine.transform);
        Button btn1 = newCharLine.GetComponent<Button>();
        btn1.onClick.AddListener(delegate { OnCharOnScreen(character); });
    }

    public void ChangeToOtherGrid(CharBase character, Transform gridAdd, List<CharBase> ladd, Transform gridRem, List<CharBase> lrem)
    {
        try
        {
            charLinePrefab = gridRem.transform.Find(character.charName).gameObject;
            if (lrem != null) lrem.Remove(character);
            if (ladd != null) ladd.Add(character);
            charLinePrefab.transform.SetParent(gridAdd.transform);
        }
        catch
        {
            Debug.Log("not selected");
        }

    }

    public void ScreenActive(int screen)
    {
        for(int i = 0; i < Screens.Length; i++)
        {
            if (i == screen)
            {
                currentScreen = i; Screens[i].SetActive(true);
            }
            else Screens[i].SetActive(false);
        }
    }

    public void EnterFeedBAckLine (string message)
    {      
        GameObject newFeedbackMsg = Instantiate(Resources.Load("FeedBackText (0)"), feedbackBattlePanel) as GameObject;
        newFeedbackMsg.transform.SetAsFirstSibling();
        newFeedbackMsg.GetComponent<TextMeshProUGUI>().text = message;        
    }

}

