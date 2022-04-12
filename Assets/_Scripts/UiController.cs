using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



public class UiController : MonoBehaviour
{
    public Transform AttribPanel;
    public Transform InfoPanel;
    public Transform DungeonPanel;
    public Button MysteryBox3, AddToParty, RemoveFromParty;
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
    public TextMeshProUGUI dungeonTimer;


    public delegate void CharOnScreen(CharBase c);
    public static event CharOnScreen OnCharOnScreen;


    private void OnEnable()
    {
        OnCharOnScreen += ShowCharOnScreen;
    }

    // UI LABEL DEVE TER O FINAL DA NOMENCLATURA CORRETA ---->  label + " (" + a + ")"
    public void FillAttribPanel(CharBase charActive)
    {
        int a = 0;
        Text attribLabel;
        Text attribValue;
        foreach (CharBase.Attributes i in charActive.AttributesD.Keys)
        {
            string label = "Attrib (" + a + ")";
            attribLabel = AttribPanel.Find(label).GetComponent<Text>();
            attribLabel.text = i.ToString();
            attribValue = attribLabel.transform.Find("value").GetComponent<Text>();
            attribValue.text = charActive.AttributesD[i].ToString();
            a++;
        }
    }

    public void FillInfo(CharBase charActive, Transform parent)
    {
        Text nameValue;
        Text levelValue;
        Text rarityValue;
        Text classValue;
        Text roleValue;
        //Text costValue;

        nameValue = parent.Find("Name").Find("value").GetComponent<Text>();
        nameValue.text = charActive.name;

        levelValue = parent.Find("Level").Find("value").GetComponent<Text>();
        levelValue.text = charActive.level.ToString();

        rarityValue = parent.Find("Rarity").Find("value").GetComponent<Text>();
        rarityValue.text = charActive.rarity.ToString();

        classValue = parent.Find("Class").Find("value").GetComponent<Text>();
        classValue.text = charActive.classChar.ToString();

        roleValue = parent.Find("Role").Find("value").GetComponent<Text>();
        roleValue.text = charActive.role.ToString();

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

    public void FillCharLine(CharBase character, Transform parent)
    {
        TextMeshProUGUI nameValue = parent.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();
        nameValue.text = character.name;

        TextMeshProUGUI levelValue = parent.Find("Level").GetComponent<TextMeshProUGUI>();
        levelValue.text = character.level.ToString();

        TextMeshProUGUI rarityValue = parent.Find("Rarity").GetComponent<TextMeshProUGUI>();
        rarityValue.text = character.rarity.ToString();

        TextMeshProUGUI classValue = parent.Find("Class").GetComponent<TextMeshProUGUI>();
        classValue.text = character.classChar.ToString();

        TextMeshProUGUI roleValue = parent.Find("Role").GetComponent<TextMeshProUGUI>();
        roleValue.text = character.role.ToString();

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
        newCharLine.name = character.name;
        FillCharLine(character, newCharLine.transform);        
        Button btn1 = newCharLine.GetComponent<Button>();
        btn1.onClick.AddListener( delegate { OnCharOnScreen(character); });
    }

    public void ChangeToOtherGrid(CharBase character, Transform gridAdd, List<CharBase> ladd, Transform gridRem, List<CharBase> lrem)
    {
        try
        {
            charLinePrefab = gridRem.transform.Find(character.name).gameObject;
            if(lrem != null) lrem.Remove(character);
            if(ladd != null) ladd.Add(character);
            charLinePrefab.transform.SetParent(gridAdd.transform);          
        }
        catch
        {
            Debug.Log("not selected");
        }

    }

}

