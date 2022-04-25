using System;
using UnityEngine;
using System.Reflection;
using Newtonsoft.Json;
using System.Collections.Generic;

[System.Serializable]
public class CharBase
{
    public int charID;
    public string charName;
    public string charDescripton;
    public string charImagePath;
    public int charLevel;
    public int charXP;
    public int charFatigue;
    public Aligment charAligment;
    public int price;
    public int HP;
    public int MP;
    public bool alive
    {
        get
        {
            if (HP > 0) return true;
            else return false;
        }
    }
    public Roles _charRole;
    public int Health;
    public Rarity charRarity;
    public StandartClass charClass;

    public Roles charRole
    {
        get { return _charRole; }
        private set
        {
            switch (charClass)
            {
                case StandartClass.Fighter: _charRole = Roles.Tank; break;
                case StandartClass.Rogue: _charRole = Roles.DPS; break;
                case StandartClass.Priest: _charRole = Roles.Healer; break;
                case StandartClass.Summoner: _charRole = Roles.Controller; break;
            };
        }
    }
    public Actions takingAction;
    public Attacks preferedAttack;
    public AttackBehavior attackBehaviour;
    public MentalBehaviour mentalBehaviour;
    public PartyBase party = null;
    public Dictionary<Attributes, int> AttributesD = new Dictionary<Attributes, int>();
    public Dictionary<Attributes, int> AttributeBonusD = new Dictionary<Attributes, int>();
    public Dictionary<Actions, bool> ActionsD = new Dictionary<Actions, bool>();
    public Dictionary<Attacks, bool> AttacksD = new Dictionary<Attacks, bool>();
    public Dictionary<CharSkill, bool> SkillsD = new Dictionary<CharSkill, bool>();
    public bool isAttacker = false;
    public bool acted;
    public Transform feedBackReference { get; set;}

    private int _Max_HP;
    private int _Max_MP;
    public int Max_HP 
    { 
        get { return _Max_HP;}
        set {
            
            if (charClass == StandartClass.Fighter)
            {
                _Max_HP = AttributesD[Attributes.Constitution] + 12 + 17 * charLevel;
            }
            if (charClass == StandartClass.Priest)
            {
                _Max_HP = AttributesD[Attributes.Constitution] + 8 + 5 * charLevel;
            }
            if (charClass == StandartClass.Rogue)
            {
                _Max_HP = AttributesD[Attributes.Constitution] + 6 + 4 * charLevel;
            }
            if (charClass == StandartClass.Summoner)
            {
                _Max_HP = AttributesD[Attributes.Constitution] + 8 + 5 * charLevel;
            }
        }
    }
    public int Max_MP
    { 
        get { return _Max_MP; }
        set
        {
            if (charClass == StandartClass.Fighter)
            {
                _Max_MP = 0;
            }
            if (charClass == StandartClass.Priest)
            {
                _Max_MP = AttributesD[Attributes.Wisdom] + 1 + 1 * charLevel;
            }
            if (charClass == StandartClass.Rogue)
            {
                _Max_MP = 0;
            }
            if (charClass == StandartClass.Summoner)
            {
                _Max_MP = AttributesD[Attributes.Wisdom] + 1 + 1 * charLevel;
            }
        }
    }

    #region Enums
    public enum AttackBehavior { LowHP, HighHP, LowStrength, HighStrength, LowDextery, HighDextery, RoleT, RoleD, RoleH, RoleC } // comportamentos predefinidos de ataque
    public enum MentalBehaviour { Aggressive, Tactical, Defensive }
    public enum Aligment 
    {   
        ChaoticGood, ChaoticNeutral, ChaoticEvil,
        NeutralGood, NeutralNeutral, NeutralEvil,
        LawfullGood, LawfullNeutral, LawfullEvil, 
    }
    public enum Actions
    {
        Attack,
        Skill,
        Item,
        Cast,
        Escape,
    }
    public enum Attacks
    {
        Melee,
        Range,
        Magic,
        Ritual
    }
    public enum Rarity
    {
        commun,
        uncommun,
        rare,
        superRare,
        legendary,
        epic
    }
    public enum Roles
    {
        Tank,
        Healer,
        DPS,
        Controller,
    }
    public enum StandartClass
    {
        Fighter,
        Priest,
        Rogue,
        Summoner,
    }
    public enum SpecialClass
    {
        Summoner,
        Paladin,
        Druid,
        Bard,
        Shaman,
        Necromancer,
        Barbarian,
        Warlock
    }
    public enum Attributes
    {
        Strength,
        Constitution,
        Dextery,
        Intelligence,
        Wisdom,
        Charisma,
    }
    #endregion

    public CharBase()
    {

    }
    public CharBase(string cname, string cdescripton,int cmentalBehaviour, int clevel, int crarity, int cclass)
    {
        charName = cname;
        charDescripton = cdescripton;
        charLevel = clevel;
        mentalBehaviour = (MentalBehaviour)cmentalBehaviour;
        charRarity = (Rarity)crarity;
        charClass = (StandartClass)cclass;
        SetRole(charClass);
    }
    //Construtor do DropsController
    public CharBase(CharBase.Rarity rarity)
    {
        charName = RandomNames.GenerateNameR(6);
        charLevel = 0;
        charRarity = (Rarity)rarity;
        charClass = (StandartClass)UnityEngine.Random.Range(0,4);
        SetRole(charClass);
    }

    [JsonConstructor]
    public CharBase(string cname, int cID, int crarity, int cclass, int personality, int aligment, string chardescription, string image)
    {
        charName = cname;
        charID = cID;
        charAligment = (Aligment)aligment;
        mentalBehaviour = (MentalBehaviour)personality;
        charRarity = (Rarity)crarity;
        charClass = (StandartClass)cclass;
        charDescripton = chardescription;
        charImagePath = image;
    }
    public void SetRole(StandartClass c)
    {
        switch (c)
        {      
            case StandartClass.Fighter: _charRole = Roles.Tank;break;
            case StandartClass.Rogue: _charRole = Roles.DPS; break;
            case StandartClass.Priest: _charRole = Roles.Healer; break;
            case StandartClass.Summoner: _charRole = Roles.Controller; break;
        }
    }
    public void FillAttributesAndActions(string jsonAttribs, bool update)
    {
        if (!update)
        {

            AttributesD = JsonConvert.DeserializeObject<Dictionary<Attributes, int>>(jsonAttribs);
            foreach (string i in Enum.GetNames(typeof(CharBase.Attributes)))
            {
                AttributeBonusD.Add((Attributes)Enum.Parse(typeof(Attributes), i), 0);
            }
        }
           
        foreach (string i in Enum.GetNames(typeof(CharBase.Actions)))
            ActionsD.Add((Actions)Enum.Parse(typeof(Actions), i), false);
        foreach (string i in Enum.GetNames(typeof(CharBase.Attacks)))
            AttacksD.Add((Attacks)Enum.Parse(typeof(Attacks), i), false);
    }
     

}
