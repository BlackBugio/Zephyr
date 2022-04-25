using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Security;
using UnityEngine.UI;



public class UiController : MonoBehaviour
{
    public Transform AttribPanel;
    public Transform AdventureSelectionPanel;
    public Transform InfoPanel;
    public Transform InfoMainImage;
    public Transform DungeonPanel;
    public Transform BattleTabPanel;
    public Button MysteryBox3, AddToParty, RemoveFromParty, TestBattle, BackToMainScreenButton;
    public Button[] buttonsToScreen;
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
        ScreenActive(4); //ativa login
        BackToMainScreenButton.onClick.AddListener(delegate { ScreenActive(0); }); // gambi

        foreach (Button bt in buttonsToScreen)
        {
            bt.onClick.AddListener(delegate { ScreenActive(bt.transform.GetSiblingIndex()); });
        }
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

    public void FillInfoMainImage(CharBase character)
    {         
        RawImage newCharImge = InfoMainImage.Find("CharImage").GetComponent<RawImage>();
        newCharImge.texture = Resources.Load<Texture>(character.charImagePath.ToString());      
    }

    public void FillInfo(CharBase character, Transform parent)
    {
        TextMeshProUGUI textValue;

        foreach (Transform t in parent)
        {
            switch (t.name)
            {
                case "Name" :
                    textValue = t.Find("value").GetComponent<TextMeshProUGUI>(); textValue.text = character.charName;
                    break;
                case "Level":
                    textValue = t.Find("value").GetComponent<TextMeshProUGUI>();textValue.text = character.charLevel.ToString();
                    break;
                case "Rarity":
                    textValue = t.Find("value").GetComponent<TextMeshProUGUI>(); textValue.text = character.charRarity.ToString();
                    break;
                case "Class":
                    textValue = t.Find("value").GetComponent<TextMeshProUGUI>(); textValue.text = character.charClass.ToString();
                    break;
                case "Role":
                    textValue = t.Find("value").GetComponent<TextMeshProUGUI>(); textValue.text = character.charRole.ToString();
                    break;
                case "MP":
                    textValue = t.Find("value").GetComponent<TextMeshProUGUI>(); textValue.text = character.MP.ToString();
                    break;
                case "MPMax":
                    textValue = t.Find("value").GetComponent<TextMeshProUGUI>(); textValue.text = character.Max_MP.ToString();
                    break;
                case "HP":
                    textValue = t.Find("value").GetComponent<TextMeshProUGUI>(); textValue.text = character.HP.ToString();
                    break;
                case "HPMax":
                    textValue = t.Find("value").GetComponent<TextMeshProUGUI>(); textValue.text = character.Max_HP.ToString();
                    break;
                case "Fatigue":
                    textValue = t.Find("value").GetComponent<TextMeshProUGUI>(); textValue.text = character.charFatigue.ToString();
                    break;
                case "Aligment":
                    textValue = t.Find("value").GetComponent<TextMeshProUGUI>(); textValue.text = character.charAligment.ToString();
                    break;
                case "Mentality":
                    textValue = t.Find("value").GetComponent<TextMeshProUGUI>(); textValue.text = character.mentalBehaviour.ToString();
                    break;
                case "XP":
                    textValue = t.Find("value").GetComponent<TextMeshProUGUI>(); textValue.text = character.charXP.ToString();
                    break;
                case "Description":
                    textValue = t.Find("value").GetComponent<TextMeshProUGUI>(); textValue.text = character.charDescripton.ToString();
                    break;
                case "RawImageViewPort":
                    RawImage newCharImge = t.Find("CharImage").GetComponent<RawImage>(); newCharImge.texture = Resources.Load<Texture>(character.charImagePath.ToString());
                    break;
            }
        }
    }

    public void ShowCharOnScreen(CharBase character)
    {
        charOnScreen = character;
        if (character.party != null) selectedParty = character.party;
        FillAttribPanel(character);
        FillInfo(character, InfoPanel);
        FillInfoMainImage(character);


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
        GameObject newCharLine = (GameObject)Instantiate(Resources.Load("CharBar"), grid);
        newCharLine.name = character.charName;
        Button btn1 = newCharLine.GetComponent<Button>();
        btn1.onClick.AddListener(delegate { OnCharOnScreen(character); });
        FillInfo(character, newCharLine.transform);
       
    }

    public void ChangeToOtherGrid(CharBase character, Transform gridAdd, List<CharBase> ladd, Transform gridRem, List<CharBase> lrem)
    {
        try
        {
            GameObject charLine = gridRem.transform.Find(character.charName).gameObject;
            if (lrem != null) lrem.Remove(character);
            if (ladd != null) ladd.Add(character);
            charLine.transform.SetParent(gridAdd.transform);
        }
        catch
        {
            Debug.Log("not selected");
        }

    }

    // extensao que permite mudar o prefab de destino pelo Resources.load
    public void ChangeToOtherGrid(CharBase character, Transform gridAdd, List<CharBase> ladd, Transform gridRem, List<CharBase> lrem, string prefab)
    {
            GameObject charLine = gridRem.transform.Find(character.charName).gameObject;
            Destroy(charLine);           
            GameObject newTypeInGrid = Instantiate(Resources.Load<GameObject>(prefab), gridAdd);
            newTypeInGrid.transform.SetAsFirstSibling();
            newTypeInGrid.name = character.charName;            
            Button btn1 = newTypeInGrid.GetComponent<Button>();
            btn1.onClick.AddListener(delegate { OnCharOnScreen(character); });
            if (lrem != null) lrem.Remove(character);
            if (ladd != null) ladd.Add(character);
            FillInfo(character, newTypeInGrid.transform);
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

