using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CharMarket : MonoBehaviour
{
    public UiController uc;
    public PartyController pc;
    public int autoGenerateAtStart = 10;
    public bool refilAvailableChar = true;
    public List<CharBase> MarketAvailableChars = new List<CharBase>();
    public System.Random r;

    public delegate void CharHired();
    public static event CharHired OnCharHired;


    private void OnEnable()
    {
        uc = GetComponent<UiController>();
        pc = GetComponent<PartyController>();
        r = new System.Random();
        for (int i = 0; i < autoGenerateAtStart; i++) GenerateChar();
        uc.charOnScreen = MarketAvailableChars[0];
    }


    // Hire Char moving it to TotalParty List
    public void HireChar()
    {
        uc.ChangeToOtherGrid(uc.charOnScreen, uc.totalPartyGrid, pc.TotalParty, uc.marketAvailableGrid, MarketAvailableChars);
        if (refilAvailableChar)
            GenerateChar();
    }

   


    public void GenerateChar()
    {
       // CharBase charGen = new CharBase(RandomNames.GenerateName(6, r), 0, 0, Random.Range(0, 4));
       // uc.AddGrid(charGen, uc.marketAvailableGrid.transform, MarketAvailableChars, true);

    }

    public void ButtonHire()
    {
        HireChar();
    }

    //obsoleto
    public void RemoveMarketGrid(CharBase c)
    {
        foreach (Transform t in uc.marketAvailableGrid.transform)
        {
            if (t.name == c.charName)
            {

                Destroy(t.gameObject);
            }
        }
    }
}


