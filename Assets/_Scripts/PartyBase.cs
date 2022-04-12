using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyBase 
{
    public string name;
    public int partyQuant;
    public int index;
    public bool slotAloccated = false;
    public bool inDungeon = false;
    public int fullPartyBonus = 10;
    public int missinCharPenalty = 5;
    private const int minPartyQuant = 3;
    private float partyPower;

    public bool HasMinParty()
    {
        if (partyQuant >= minPartyQuant) return true;
        else return false;
    }
    public float PartyPower {
        get { return partyPower; }
        private set { partyPower = PartyPowerCalculation(); } }
    public float partyXP = 0;
    public List<CharBase> CharInParty = new List<CharBase>();


    public PartyBase(string name, int index)
    {
        this.name = name;
        this.index = index;
    }

    public float PartyPowerCalculation()
    {
        int powerBonus = 0;
        int partyLevel = 0;
        int partyRarity = 0;
        bool bonusTank = false;
        bool bonusController = false;
        bool bonusHealer = false;
        bool bonusDPS = false;

        foreach (CharBase c in CharInParty)
        {
            partyLevel += c.level;
            partyRarity += (int)c.rarity;
            switch (c.role)
            {
                case CharBase.Roles.Controller: bonusController = true; continue;
                case CharBase.Roles.DPS: bonusDPS = true; continue;
                case CharBase.Roles.Tank: bonusTank = true; continue;
                case CharBase.Roles.Healer: bonusHealer = true; continue;
            }
        }

        if (bonusController && bonusTank && bonusDPS && bonusHealer) powerBonus += fullPartyBonus;
        if (partyQuant == 3) powerBonus -= missinCharPenalty * 2;
        if (partyQuant == 4) powerBonus -= missinCharPenalty;

        partyPower = partyLevel + partyRarity + powerBonus;

        return partyPower;
    }
}
