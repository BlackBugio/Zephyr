using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MysteryBox
{
    public BoxType boxType;
    public Dictionary<CharBase.Rarity, float> RarityProbability;  
    private int _ZDPrice;
    private int _DollarPrice;

    private float _dropCommom;
    private float _dropUncommom;
    private float _dropRare;
    private float _dropSuperRare;
    private float _dropLegendary;
    private float _dropEpic;

    public float ZDPrice
    {
        get { return _ZDPrice; }
        set
        {
            switch ((int)boxType)
            {
                case 0: _ZDPrice = 100; break;
                case 1: _ZDPrice = 200; break;
                case 2: _ZDPrice = 300; break;
                case 3: _ZDPrice = 500; break;               
            };
        }
    }

    public int DollarPrice
    {
        get { return _DollarPrice; }
        set
        {
            switch ((int)boxType)
            {
                case 0: _DollarPrice = 0; break;
                case 1: _DollarPrice = 80; break;
                case 2: _DollarPrice = 200; break;
                case 3: _DollarPrice = 300; break;
            };
        }
    }
    
    public float DropCommom
    {
        get { return _dropCommom; }
        set
        {
            switch ((int)boxType)
            {
                case 0: _dropCommom = 60; break;
                case 1: _dropCommom = 0; break;
                case 2: _dropCommom = 0; break;
                case 3: _dropCommom = 0; break;
            };
        }
    }
      
    public float DropUncommom
    {
        get { return _dropUncommom; }
        set
        {
            switch ((int)boxType)
            {
                case 0: _dropUncommom = 25; break;
                case 1: _dropUncommom = 25; break;
                case 2: _dropUncommom = 0; break;
                case 3: _dropUncommom = 0; break;
            };
        }
    }
    
    public float DropRare
    {
        get { return _dropRare; }
        set
        {
            switch ((int)boxType)
            {
                case 0: _dropRare = 10; break;
                case 1: _dropRare = 60; break;
                case 2: _dropRare = 25; break;
                case 3: _dropRare = 0; break;
            };
        }
    }
    
    public float DropSuperRare
    {
        get { return _dropSuperRare; }
        set
        {
            switch ((int)boxType)
            {
                case 0: _dropSuperRare = 4; break;
                case 1: _dropSuperRare = 10; break;
                case 2: _dropSuperRare = 60; break;
                case 3: _dropSuperRare = 30; break;
            };
        }
    }
    
    public float DropLegendary
    {
        get { return _dropLegendary; }
        set
        {
            switch ((int)boxType)
            {
                case 0: _dropLegendary = 0.9f; break;
                case 1: _dropLegendary = 4; break;
                case 2: _dropLegendary = 10; break;
                case 3: _dropLegendary = 60; break;
            };
        }
    }

    public float DropEpic
    {
        get { return _dropEpic; }
        set
        {
            switch ((int)boxType)
            {
                case 0: _dropEpic = 0.1f; break;
                case 1: _dropEpic = 1; break;
                case 2: _dropEpic = 5; break;
                case 3: _dropEpic = 10; break;
            };
        }
    }

    public enum BoxType
    {
        commum,
        rare,
        superRare,
        legendary
    } 

    public MysteryBox(int aboxType)
    {
        boxType = (BoxType)aboxType;
        ZDPrice = aboxType;
        DollarPrice = aboxType;
        DropCommom = aboxType;
        DropUncommom = aboxType;
        DropRare = aboxType;
        DropSuperRare = aboxType;
        DropLegendary = aboxType;
        DropEpic = aboxType;
        Debug.Log("Zprice -->" + ZDPrice);        
    }

}

