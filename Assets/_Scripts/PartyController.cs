using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PartyController : MonoBehaviour
{
    private protected UiController uc;
    private protected WebTest wc;
    public PartyBase selectedParty;
    public TabGroup tabGroup;

    public List<CharBase> TotalParty = new List<CharBase>();
    public List<PartyBase> PartySlots = new List<PartyBase>();

    private const int maxPartySlots = 4;
    private const int maxPartyQuant = 5;

    public delegate void GotCharID(int characterID);
    public static event GotCharID OnGotCharID;

    private class ArrayID
    {
       public int characterID;
    }

    private void Start()
    {
        DungeonController.OnPartyInDungeon += PartyEnteredDungeon;
        uc = GetComponent<UiController>();
        wc = GetComponent<WebTest>();
        uc.AddToParty.onClick.AddListener(AddActivePartyGrid);
        uc.RemoveFromParty.onClick.AddListener(RemoveActivePartyGrid);      
        TabGroup.OnTabActive += GetPartySlotChange;
        WebTest.OnTryGetChar += GetServerChar;


        for (int i = 0; i < maxPartySlots; i++)
        {
            PartySlots.Add(new PartyBase(RandomNames.GenerateNameR(4), i));
        }
        selectedParty = PartySlots[0];
        DropsController.OnChardroped += GenerateMysteryBoxChar;
    }

    private void GetServerChar(string message)
    {
        StartCoroutine(ReadServerChar(message));
    }

    //le a linha de json enviada pelo server e converte em um personagem na lista total
    public IEnumerator ReadServerChar(string message)
    {
        bool isDone = false;
        string stringJson = "";
        System.Action<string> getCharCallBack = (wwwcalback) => { isDone = true; stringJson = wwwcalback; };
        JArray a = JArray.Parse(message);
        for (int i = 0; i < a.Count; i++)
        {
            isDone = false;
            string cD = a[i].ToString();
            ArrayID aID = JsonConvert.DeserializeObject<ArrayID>(cD);            
            //callback da coroutine getcharacter            
            StartCoroutine(wc.GetCharacter(aID.characterID, getCharCallBack));            
            yield return new WaitUntil(() => isDone == true); //espera carregar chars
            string charString = stringJson;// Debug.Log(charString);         
            // getattributes
            isDone = false;            
            StartCoroutine(wc.GetAttributes(aID.characterID, getCharCallBack));
            yield return new WaitUntil(() => isDone == true);  //espera carregar attribs
            string attribs = stringJson;// Debug.Log(attribs);
            // getstats
            isDone = false;
            StartCoroutine(wc.GetStats(aID.characterID, getCharCallBack));
            yield return new WaitUntil(() => isDone == true); //espera carregar stats
            string statsString = stringJson;// Debug.Log(statsString);
            CharBase newchar = JsonConvert.DeserializeObject<CharBase>(charString) as CharBase;
            JObject rss = JObject.Parse(statsString);
            newchar.HP = (int)rss["hp"];
            newchar.MP = (int)rss["mp"];
            newchar.charLevel = (int)rss["charlevel"];
            newchar.charXP = (int)rss["xp"];
            newchar.charFatigue = (int)rss["fatigue"];
            newchar.FillAttributesAndActions(attribs, false);
            newchar.SetRole(newchar.charClass);
            uc.AddGrid(newchar, uc.mainCharGrid, TotalParty, true);
            stringJson = "";
        }

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
        //Debug.Log(selectedParty.index);
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
