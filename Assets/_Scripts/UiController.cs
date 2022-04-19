using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



public class UiController : MonoBehaviour
{
    public Transform AttribPanel;
    public Transform InfoPanel;
    public Transform DungeonPanel;
    public Transform BattleTabPanel;
    public Button MysteryBox3, AddToParty, RemoveFromParty, TestBattle;
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


    //testing variables
    public TextMeshProUGUI partyPower;
    public TextMeshProUGUI userName;
    public TextMeshProUGUI zCoins;


    public delegate void CharOnScreen(CharBase c);
    public static event CharOnScreen OnCharOnScreen;


    private void OnEnable()
    {
        OnCharOnScreen += ShowCharOnScreen;
        ScreenActive(0); //ativa login

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

    public void FillInfo(CharBase charActive, Transform parent)
    {

        TextMeshProUGUI nameValue = parent.Find("Name").Find("value").GetComponent<TextMeshProUGUI>();
        nameValue.text = charActive.charName;

        TextMeshProUGUI levelValue = parent.Find("Level").Find("value").GetComponent<TextMeshProUGUI>();
        levelValue.text = charActive.charLevel.ToString();

        TextMeshProUGUI rarityValue = parent.Find("Rarity").Find("value").GetComponent<TextMeshProUGUI>();
        rarityValue.text = charActive.charRarity.ToString();

        TextMeshProUGUI classValue = parent.Find("Class").Find("value").GetComponent<TextMeshProUGUI>();
        classValue.text = charActive.charClass.ToString();

        TextMeshProUGUI roleValue = parent.Find("Role").Find("value").GetComponent<TextMeshProUGUI>();
        roleValue.text = charActive.charRole.ToString();

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

  /*  public void FillCharLine(CharBase character, Transform parent)
    {
        TextMeshProUGUI nameValue = parent.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();
        if(nameValue !=null)
        nameValue.text = character.charName;

        TextMeshProUGUI levelValue = parent.Find("Level").GetComponent<TextMeshProUGUI>();
        if (nameValue != null)
            levelValue.text = character.charLevel.ToString();

        TextMeshProUGUI rarityValue = parent.Find("Rarity").GetComponent<TextMeshProUGUI>();
        if (nameValue != null)
            rarityValue.text = character.charRarity.ToString();

        TextMeshProUGUI classValue = parent.Find("Class").GetComponent<TextMeshProUGUI>();
        if (nameValue != null)
            classValue.text = character.charClass.ToString();

        TextMeshProUGUI roleValue = parent.Find("Role").GetComponent<TextMeshProUGUI>();
        if (nameValue != null)
            roleValue.text = character.charRole.ToString();

    }*/

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
            if (i == screen) Screens[i].SetActive(true);
            else Screens[i].SetActive(false);
        }
    }

}

