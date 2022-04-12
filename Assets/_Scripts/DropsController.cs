using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class DropsController : MonoBehaviour
{
    MysteryBox mbox;
    UiController uc;
    public System.Random r;
    private float randomRoll = 0;

    public delegate void CharDrop(CharBase.Rarity rarity);
    public static event CharDrop OnChardroped;

    private void OnEnable()
    {
        uc = GetComponent<UiController>();
        r = new System.Random();
       // uc.MysteryBox0.onClick.AddListener ( delegate { DropMysteryBox(0); });
       // uc.MysteryBox1.onClick.AddListener (delegate { DropMysteryBox(1); });
       // uc.MysteryBox2.onClick.AddListener (delegate { DropMysteryBox(2); });
        uc.MysteryBox3.onClick.AddListener (delegate { DropMysteryBox(3); });
    }
    public void DropMysteryBox(int aboxType)
    {
        randomRoll = Random.Range(0f, 100f);
        Debug.Log("RandoRoll " + randomRoll);
        mbox = new MysteryBox(aboxType);

        SortedDictionary<CharBase.Rarity, float> RarityProbability = new SortedDictionary<CharBase.Rarity, float>
        {
            { CharBase.Rarity.commun, mbox.DropCommom },
            { CharBase.Rarity.uncommun, mbox.DropUncommom },
            { CharBase.Rarity.rare, mbox.DropRare },
            { CharBase.Rarity.superRare, mbox.DropSuperRare },
            { CharBase.Rarity.legendary, mbox.DropLegendary },
            { CharBase.Rarity.epic, mbox.DropEpic }
        };

        var items = from pair in RarityProbability
                    orderby pair.Value ascending
                    select pair;

        float accumulateChance = 0;

        foreach (var pair in items)
        {
            accumulateChance += pair.Value;
           
            if (randomRoll <= accumulateChance)
            {               
                OnChardroped(pair.Key);
                break;
            }
        }
    }
}
