using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PartyController : MonoBehaviour
{
    private protected UiController uc;
    public PartyBase selectedParty;
    public TabGroup tabGroup;

    public List<CharBase> TotalParty = new List<CharBase>();
    public List<PartyBase> PartySlots = new List<PartyBase>();

    private const int maxPartySlots = 4;
    private const int maxPartyQuant = 5;

    //public delegate void PartyUpdate(float partyPower);
    //public static event PartyUpdate OnPartyUpdate;

    void Start()
    {
        DungeonController.OnPartyInDungeon += PartyEnteredDungeon;
        uc = GetComponent<UiController>();
        uc.AddToParty.onClick.AddListener(AddActivePartyGrid);
        uc.RemoveFromParty.onClick.AddListener(RemoveActivePartyGrid);
        //uc.CloseParty.onClick.AddListener(delegate { CloseParty(activeParty); });
        //uc.EditParty.onClick.AddListener(delegate { EditParty(uc.selectedParty); });

        TabGroup.OnTabActive += GetPartySlotChange;

        for (int i = 0; i < maxPartySlots; i++)
        {
            PartySlots.Add(new PartyBase(RandomNames.GenerateNameR(4), i));
        }
        selectedParty = PartySlots[0];
        DropsController.OnChardroped += GenerateMysteryBoxChar;
    }

    public void AddActivePartyGrid()
    {
        if (selectedParty.CharInParty.Count < maxPartyQuant && !selectedParty.inDungeon)
        {
            uc.ChangeToOtherGrid(uc.charOnScreen, uc.PartySlotGrid[selectedParty.index], selectedParty.CharInParty, uc.totalPartyGrid, TotalParty);
            selectedParty.partyQuant++;
            UpdatePartyPower();
        }
        else Debug.Log("Max Party Reached");
    }

    public void RemoveActivePartyGrid()
    {
        if (!selectedParty.inDungeon)
        {
            uc.ChangeToOtherGrid(uc.charOnScreen, uc.totalPartyGrid, TotalParty, uc.PartySlotGrid[selectedParty.index], selectedParty.CharInParty);
            selectedParty.partyQuant--;
            UpdatePartyPower();
        }
    }

    public void GenerateMysteryBoxChar(CharBase.Rarity rarity)
    {
        CharBase charGen = new CharBase(rarity);
        uc.AddGrid(charGen, uc.mainCharGrid, TotalParty, true);
        uc.ShowCharOnScreen(charGen);
    }

    public void GetPartySlotChange(int tabIndex)
    {
        selectedParty = PartySlots[tabIndex];
        UpdatePartyPower();
        Debug.Log(selectedParty.index);
    }

    public void UpdatePartyPower()
    {
        if(selectedParty.HasMinParty()) uc.partyPower.text = selectedParty.PartyPowerCalculation().ToString();
        else uc.partyPower.text = "";
    }

    public void PartyEnteredDungeon (PartyBase party, bool entered)
    {
        if (entered)
        {
            party.inDungeon = true;
            tabGroup.OnTabOccupied(tabGroup.tabButtons[party.index]);
        }
        else
        {
            party.inDungeon = false;
            tabGroup.OnTabDeOccupied(tabGroup.tabButtons[party.index]);
        }       
    }
    // old code with transition slot (edit, close slot)
    /*public void EditParty(PartyBase partyBase)
    {
        if (!partyBase.inDungeon)
        {
            foreach (CharBase c in ActiveParty)
            {
                uc.ChangeToOtherGrid(c, uc.totalPartyGrid, TotalParty, uc.ActivePartyGrid, null);
            }
            ActiveParty.Clear();
            foreach (CharBase c in partyBase.CharInParty)
            {
                uc.ChangeToOtherGrid(c, uc.ActivePartyGrid, ActiveParty, uc.PartySlotGrid[partyBase.partySlotIndex], null);
                c.party = null;
            }
            partyBase.slotAloccated = false;
            PartySlots.Remove(partyBase);
            partyBase.CharInParty.Clear();
            activeParty = partyBase;
            uc.selectedParty = partyBase;
        }
        else Debug.Log("party in dungeon");
    }*/

    /*public void CloseParty(PartyBase partyBase)
    {
        if(ActiveParty.Count >= minPartyQuant)
        {
            if (!partyBase.slotAloccated)
            {
                activeParty = new PartyBase(RandomNames.GenerateNameR(4), PartySlots.Count);
                partyBase.slotAloccated = true;
                PartySlots.Add(partyBase);
                
            }

            foreach (CharBase c in ActiveParty)
            {
                c.party = partyBase;
                uc.ChangeToOtherGrid(c, uc.PartySlotGrid[partyBase.partySlotIndex], partyBase.CharInParty, uc.ActivePartyGrid, null);
                
            }            
            partyBase.PartyPowerCalculation();
            partyBonus = uc.PartySlotGrid[partyBase.partySlotIndex].transform.parent.Find("PartyBonus").gameObject.GetComponent<Text>();
            partyBonus.text = partyBase.PartyPower.ToString();
            ActiveParty.Clear();
            Debug.Log(partyBase.partyName);
        }                     
    }*/

}
