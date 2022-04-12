using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DungeonController : MonoBehaviour
{
    UiController uc;
    PartyController pc;
    DropsController dc;
    Dictionary<PartyBase, DungeonBase> InDungeon = new Dictionary<PartyBase, DungeonBase>(); // avaliar uso
    List<PartyBase> InDungeonParty = new List<PartyBase>();
    System.Random r;


    public delegate void PartyInDungeon(PartyBase party, bool enter);
    public static event PartyInDungeon OnPartyInDungeon;

    void Start()
    {
        uc = GetComponent<UiController>();
        dc = GetComponent<DropsController>();
        pc = GetComponent<PartyController>();
        r = new System.Random();
        SetUpDungeonButtons();

    }
    public void SetUpDungeonButtons()
    {
        for (int i = 0; i < uc.Dungeon.Length; i++)
        {
            DungeonBase iDungeon = new DungeonBase(RandomNames.GenerateName(4,r), i);
            TextMeshProUGUI label = uc.Dungeon[i].GetComponentInChildren<TextMeshProUGUI>();
            label.text = "level " + iDungeon.GetDifficulty;
            uc.Dungeon[i].onClick.AddListener(delegate { EnterDungeon(iDungeon); });
        }
    }

    public void EnterDungeon(DungeonBase dungeon)
    {
        PartyBase partyInDungeon =  pc.selectedParty;
        if (!partyInDungeon.inDungeon && partyInDungeon.HasMinParty())
        {
            InDungeonPanel(dungeon, partyInDungeon,true);
            Debug.Log(partyInDungeon.index);
            InDungeonParty.Add(partyInDungeon);
            OnPartyInDungeon(partyInDungeon, true);
            Debug.Log(partyInDungeon.index + " Entrou na dungeon " + dungeon.GetDifficulty);            
            StartCoroutine(ExitDungeon(dungeon, partyInDungeon, dungeon.Duration));
        }
        else Debug.Log(partyInDungeon.index + " JÁ está na dungeon" + dungeon.GetDifficulty);
    }

    public bool DungeonExplorationResult(DungeonBase dungeon, PartyBase party)
    {       
        float probs = party.PartyPower + dungeon.Probability;
        float roll = Random.Range(0, 100);
        if (probs > roll) return true;
        else return false;        
    }

    public IEnumerator ExitDungeon(DungeonBase dungeon, PartyBase party, float time)
    {
       yield return new WaitForSeconds(time);

        if (DungeonExplorationResult(dungeon,party))
        {
            dc.DropMysteryBox((int)dungeon._difficulty);
            GameController.ZDCOINS += dungeon.Prize;
        }
        Debug.Log(party.index + " SAIU da dungeon" + dungeon.GetDifficulty);        
        OnPartyInDungeon(party, false);
        InDungeon.Remove(party);
        InDungeonPanel(dungeon, party, false);
    }  

    public void InDungeonPanel (DungeonBase dungeon, PartyBase party, bool enter)
    {
        if (enter)
        {
            GameObject partyLine = Instantiate(uc.partyLinePrefab, uc.DungeonPanel);
            partyLine.name = party.index.ToString();
            uc.FillPartyPanel(party, dungeon, partyLine.transform);
            TextMeshProUGUI timeValue = partyLine.transform.Find("Time").GetComponent<TextMeshProUGUI>();
            timeValue.text = dungeon.Duration.ToString();
            StartCoroutine(DungeonTimer(timeValue, dungeon.Duration));
        }
        else
        {
            foreach (Transform line in uc.DungeonPanel)
            {
                if (line.name == party.index.ToString())  Destroy(line.gameObject,2); 
            }
        }
    }   

    public IEnumerator DungeonTimer(TextMeshProUGUI timerLabel,float timer)
    {
        float i = 0;
        float wait = 1;
        while (i != timer)
        {
            timerLabel.text = (timer - i).ToString();
            i++;
            yield return new WaitForSeconds(wait);            
        }        
    }
    /*
    void ClockGameplay()
    {
        gameTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(gameTime / 60F);
        int seconds = Mathf.FloorToInt(gameTime - minutes * 60);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        hud.labelTotalTime.TextMeshProUGUI = niceTime;
    }
    */
}
