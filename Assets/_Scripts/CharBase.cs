using System;
using UnityEngine;
using System.Collections.Generic;

public class CharBase
{
    public string name;
    public int level;
    public int cost;
    private int _HP;
    private int _MP;
    public int Health { get; set; }
    public Rarity rarity;
    public StandartClass classChar;
    public Roles role;
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

    public int HP
    {
        get { return _HP;}
        set { if (classChar == StandartClass.Fighter)
            {
                _HP = AttributesD[Attributes.Constitution] + 12 + 17 * level;
            }
            if (classChar == StandartClass.Priest)
            {
                _HP = AttributesD[Attributes.Constitution] + 8 + 5 * level;
            }
            if (classChar == StandartClass.Rogue)
            {
                _HP = AttributesD[Attributes.Constitution] + 6 + 4 * level;
            }
            if (classChar == StandartClass.Summoner)
            {
                _HP = AttributesD[Attributes.Constitution] + 8 + 5 * level;
            }
        }
    }
    public int MP
    {
        get { return _MP; }
        set
        {
            if (classChar == StandartClass.Fighter)
            {
                _MP = 0;
            }
            if (classChar == StandartClass.Priest)
            {
                _MP = AttributesD[Attributes.Wisdom] + 1 + 1 * level;
            }
            if (classChar == StandartClass.Rogue)
            {
                _MP = 0;
            }
            if (classChar == StandartClass.Summoner)
            {
                _MP = AttributesD[Attributes.Wisdom] + 1 + 1 * level;
            }
        }
    }
    public enum AttackBehavior { LowHP, HighHP, LowStrength, HighStrength, LowDextery, HighDextery, RoleT, RoleD, RoleH, RoleC } // comportamentos predefinidos de ataque
    public enum MentalBehaviour { Aggressive, Tactical, Defensive }
    public enum Actions
    {
        Attack,
        UseSkill,
        ConsumeItem,
        CastSpell,
        Run,
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
        Strenght,
        Constitution,
        Dextery,
        Intelligence,
        Wisdom,
        Charisma,
    }
    public CharBase(string cname, int clevel, int crarity, int cclass)
    {
        name = cname;
        level = clevel;
        rarity = (Rarity)crarity;
        classChar = (StandartClass)cclass;
        SetRole(classChar);
        FillAttributesAndActions();
    }
    //Construtor do DropsController
    public CharBase(CharBase.Rarity rarity)
    {
        name = RandomNames.GenerateNameR(6);
        level = 0;
        this.rarity = (Rarity)rarity;
        classChar = (StandartClass)UnityEngine.Random.Range(0,4);
        SetRole(classChar);
        FillAttributesAndActions();
    }
    private void SetRole(StandartClass c)
    {
        if (c == StandartClass.Fighter) role = Roles.Tank;
        if (c == StandartClass.Rogue) role = Roles.DPS;
        if (c == StandartClass.Priest) role = Roles.Healer;
        if (c == StandartClass.Summoner) role = Roles.Controller;
    }
    private void FillAttributesAndActions()
    {
        foreach (string i in Enum.GetNames(typeof(CharBase.Attributes)))
            AttributesD.Add((Attributes)Enum.Parse(typeof(Attributes), i), UnityEngine.Random.Range(1, 10));
        foreach (string i in Enum.GetNames(typeof(CharBase.Actions)))
            ActionsD.Add((Actions)Enum.Parse(typeof(Actions), i), false);
        foreach (string i in Enum.GetNames(typeof(CharBase.Attacks)))
            AttacksD.Add((Attacks)Enum.Parse(typeof(Attacks), i), false);
    }
    public void ClearActions()
    {
        foreach (string i in Enum.GetNames(typeof(CharBase.Actions)))
            ActionsD[(Actions)Enum.Parse(typeof(Actions), i)] = false;
    } 

}
