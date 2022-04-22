using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleController : MonoBehaviour
{
    UiController uc;
    public bool playerTurn;
    public PartyBase partyA;
    public PartyBase partyB;
    public bool isSurpriseAttack;
    public int defenseModifier = 3;
    public bool actionsToTake = true;
    public int turnCount;
    public float timeStep = 0.5f;
    public int battleID;

    public delegate void FeedbackLine(string feedbackMessage);
    public static event FeedbackLine OnFeedbackLine;

    private void Start()
    {
        uc = GetComponent<UiController>();        
    }
    public void DefineBattleSides() // Definine cada em character se é attacker ou defender (partyA ou partyB)
    {
        Transform partyAPanel =  uc.BattleTabPanel.Find("PartyA Panel").transform;
        Transform partyBPanel = uc.BattleTabPanel.Find("PartyB Panel").transform;
        int jvcounter = 0;
        foreach (CharBase character in partyA.CharInParty)
        {
            //chama btn e add list btns;
            character.isAttacker = true;
            Transform charBattleBtn = partyAPanel.Find("charBattleStat ("+jvcounter+")").transform;
            charBattleBtn.gameObject.SetActive(true);
            uc.FillInfo(character, charBattleBtn);
            character.feedBackReference = charBattleBtn;
            jvcounter++;
        }
        jvcounter = 0;
        foreach (CharBase character in partyB.CharInParty)
        {
            character.isAttacker = false;
            Transform charBattleBtn = partyBPanel.Find("charBattleStat (" + jvcounter + ")").transform;
            charBattleBtn.gameObject.SetActive(true);
            uc.FillInfo(character, charBattleBtn);
            character.feedBackReference = charBattleBtn;
            jvcounter++;
        }
    }

    public void StartBattle(PartyBase attacker, PartyBase defender) // método publico de chamada de batalha
    {
        partyA = attacker;
        partyB = defender;
        DefineBattleSides();
        StartTurn();
    }

    public void StartTurn() // inicia um novo turno
    {
        StartCoroutine(DecideAction(InitiativeRoll())); // initiative retorna o personagem da vez que decide acao
    }

    public CharBase InitiativeRoll()  //Testa a deztreza e int de todos os char de cada party e retorna o char da vez;  
    {
        CharBase charTurn = null;
        CharBase nextA = null;
        CharBase nextB = null;
        int initTestA = 0;
        int initTestB = 0;

        if (CheckLiveParty(partyA))
        {
            foreach (CharBase character in partyA.CharInParty)
            {
                int charInitiative = character.AttributesD[CharBase.Attributes.Dextery] + character.AttributesD[CharBase.Attributes.Intelligence];
                if (CanAct(character) && charInitiative > initTestA)
                {
                    initTestA = charInitiative;
                    nextA = character;
                }
            }
        }
        else { EndBattle(partyB); return null; }

        if (CheckLiveParty(partyB))
        {
            foreach (CharBase character in partyB.CharInParty)
            {
                int charInitiative = character.AttributesD[CharBase.Attributes.Dextery] + character.AttributesD[CharBase.Attributes.Intelligence];
                if (CanAct(character) && charInitiative > initTestB)
                {
                    initTestB = charInitiative;
                    nextB = character;
                }
            }
        }
        else { EndBattle(partyA); return null;}

        if (initTestA > initTestB) { charTurn = nextA; }
        else if (initTestA < initTestB) charTurn = nextB;
        else if (initTestA == initTestB)
        {
            float roll = UnityEngine.Random.Range(0, 2);
            if (roll > 1) charTurn = nextA;
            else charTurn = nextB;
        }
        else if (initTestA == 0 || initTestB == 0) { Debug.Log("passo do end turn"); return null; }        
       
        if(charTurn !=null)
            return charTurn;
        else { return null; }
    }

    public IEnumerator DecideAction(CharBase character) // etapa de decisão de ação do char, se for nulo chama o endturn pois todos já jogaram
    {   // se for o cpu, decide a acao
        
        if (!playerTurn)
        {
            if (character != null) //se for nulo chama o endturn
            {
                BattleFeedBacks.SelecedFeedback(character, true);
                OnFeedbackLine("it`s " + character.charName + " turn, he is taking a decision based on " + character.mentalBehaviour + " behaviour");
                yield return new WaitForSeconds(timeStep);

                switch (character.mentalBehaviour)
                {
                    case CharBase.MentalBehaviour.Aggressive:
                        {
                            StartCoroutine(TakeAction(character, FindTargetFromBehaviour(character, EnemyParty(character)), CharBase.Actions.Attack));
                            break;
                        }
                    case CharBase.MentalBehaviour.Tactical:
                        StartCoroutine(TakeAction(character, FindTargetFromBehaviour(character, EnemyParty(character)), CharBase.Actions.Attack));
                        break;
                    case CharBase.MentalBehaviour.Defensive:
                        StartCoroutine(TakeAction(character, FindTargetFromBehaviour(character, EnemyParty(character)), CharBase.Actions.Attack));
                        break;
                }
            }
            else if(CheckLiveParty(partyA) &&CheckLiveParty(partyB)){ EndTurn(); }
        }
        else 
        {
            bool decidedAction = false;
            string msgCallback = "";
            System.Action<string> getActionCallBack = (btnCallback) => { decidedAction = true; msgCallback = btnCallback; };
            CharBase.Actions actionToTake = new CharBase.Actions();

            TabGroup battleTab = uc.BattleTabPanel.GetComponent<TabGroup>();
            int jvcounter = 0;
            // abre o menu de opcoes de acao do char
            foreach (var key in character.ActionsD)
            {               
                if (key.Value)
                {
                     battleTab.tabButtons[jvcounter].gameObject.SetActive(true);
                    TabButton bt = battleTab.tabButtons[jvcounter];
                    bt.tabButtonText = key.ToString();
                    GameObject panelSlot = battleTab.objectsToSwap[jvcounter].gameObject;
                    
                    jvcounter++;

                }
            }

            yield return new WaitUntil(() => decidedAction == true);

            Enum.TryParse("msgCallback", out actionToTake);
            // recebe o callback da decisao do jogador

            CharBase targetA = new CharBase();
            //toma a acao
           
            StartCoroutine(TakeAction(character, targetA, actionToTake));
        }
        yield return null;
    }

    public IEnumerator TakeAction(CharBase character, CharBase target, CharBase.Actions action) // toma a ação definida e chama novamente a rolagem de iniciativa para o próximo char
    {
        //Debug.Log("Taking action  " + target);
        if (action == CharBase.Actions.Attack)
        {
            switch (character.preferedAttack)
            {
                case CharBase.Attacks.Melee: StartCoroutine (AttackAct(character, target, CharBase.Attributes.Strength, CharBase.Attributes.Strength)); break;
                case CharBase.Attacks.Range: StartCoroutine (AttackAct(character, target, CharBase.Attributes.Dextery, CharBase.Attributes.Dextery)); break;
                case CharBase.Attacks.Ritual: StartCoroutine (AttackAct(character, target, CharBase.Attributes.Wisdom, CharBase.Attributes.Wisdom)); break;
                case CharBase.Attacks.Magic: StartCoroutine (AttackAct(character, target, CharBase.Attributes.Intelligence, CharBase.Attributes.Intelligence)); break;
            }
        }

        else if (action == CharBase.Actions.Skill)
        {
            //implementar Skill
        }

        else if (action == CharBase.Actions.Item)
        {
            //implementar Itens
        }

        yield return new WaitUntil(() => character.acted == true);
        StartCoroutine(TookAction(character));
    }

    public IEnumerator TookAction(CharBase character)
    {
        BattleFeedBacks.SelecedFeedback(character, false);
        yield return new WaitForSeconds(timeStep);
        StartCoroutine(DecideAction(InitiativeRoll()));
        
    }

    public void EndTurn() // acaba o turno, limpa lista de ações tomadas
    {
        foreach (CharBase character in partyA.CharInParty)
        {
            if (character != null)
            {
                character.acted = false; // limpa a lista de ações do turno
            }
        }

        foreach (CharBase character in partyB.CharInParty)
        {
            if (character != null)
            {
                character.acted = false; // limpa a lista de ações do turno
            }
        }
        turnCount++;
        StartCoroutine(DecideAction(InitiativeRoll()));
    }

    public void EndBattle(PartyBase winner) // finaliza batalha anuncia vencedor
    {
        StopAllCoroutines();
        if (winner.name == partyA.name)
            OnFeedbackLine(" End Battle, winner Party A em " + turnCount + " Turns");
        else
            OnFeedbackLine(" End Battle, winner Party B em " + turnCount + " Turns");

        uc.endBattlePanel.gameObject.SetActive(true);
    }
    
    #region Finders ferramentas para se achar
    public bool CanAct(CharBase character) // testa se alguma ação foi tomada no turno
    {
        if (!character.acted && character.alive)
            return true;

        else return false;
    }

    public bool CheckLiveParty(PartyBase party)
    {
        bool atLeastoneAlive = false;
        foreach (CharBase character in party.CharInParty)
        {
            if (character != null && character.alive)
            {
                atLeastoneAlive = true;
            }
        }
        return atLeastoneAlive;
    }

    public PartyBase EnemyParty(CharBase character) // verifica a outra party
    {
        if (character.isAttacker) return partyB;
        else return partyA;
    }
    #endregion

    #region IA acha targets e seleciona attributos e acoes baseado no perfil de comportamento;
    public CharBase FindTargetFromBehaviour(CharBase character, PartyBase otherParty) // encontra o target de acordo com o behaviour
    {
        OnFeedbackLine(character.charName +" is choosing target, it is an " + character.attackBehaviour);
        return character.attackBehaviour switch
        {
            CharBase.AttackBehavior.HighDextery => FindAttribInPArty(otherParty, CharBase.Attributes.Dextery, true),
            CharBase.AttackBehavior.LowDextery => FindAttribInPArty(otherParty, CharBase.Attributes.Dextery, false),
            CharBase.AttackBehavior.HighStrength => FindAttribInPArty(otherParty, CharBase.Attributes.Strength, true),
            CharBase.AttackBehavior.LowStrength => FindAttribInPArty(otherParty, CharBase.Attributes.Strength, false),
            CharBase.AttackBehavior.HighHP => FindHPInPArty(otherParty, true),
            CharBase.AttackBehavior.LowHP => FindHPInPArty(otherParty, false),
            CharBase.AttackBehavior.RoleD => FindRoleInPArty(otherParty, CharBase.Roles.DPS),
            CharBase.AttackBehavior.RoleH => FindRoleInPArty(otherParty, CharBase.Roles.Healer),
            CharBase.AttackBehavior.RoleT => FindRoleInPArty(otherParty, CharBase.Roles.Tank),
            CharBase.AttackBehavior.RoleC => FindRoleInPArty(otherParty, CharBase.Roles.Controller),
            _ => default,
        };
    } 

    public CharBase FindAttribInPArty(PartyBase enemyParty, CharBase.Attributes attribute, bool high) // pesquisa attributos maior ou menor
    { 
        int highAttrib = 0;
        int lowAttrib = 1000;
        CharBase highChar = null;
        CharBase lowChar = null;
        foreach (CharBase enemy in enemyParty.CharInParty)
        {
            if (high)
            {
                if (enemy.AttributesD[attribute] > highAttrib) { highAttrib = enemy.AttributesD[attribute]; highChar = enemy; }
            }            
            else
                if (enemy.AttributesD[attribute] < lowAttrib) { lowAttrib = enemy.AttributesD[attribute]; lowChar = enemy;}
        }
        if (high) return highChar;
        else return lowChar;
    }

    public CharBase FindHPInPArty(PartyBase enemyParty, bool high) // pesquisa hp maior ou menor
    {
        int highAttrib = 0;
        int lowAttrib = 10000;
        CharBase highChar = null;
        CharBase lowChar = null;
        foreach (CharBase enemy in enemyParty.CharInParty)
        {
            
            if (high)
            {
                if (enemy.HP > highAttrib) { highAttrib = enemy.HP; highChar = enemy; }
            }

            else
                if (enemy.HP < lowAttrib) { lowAttrib = enemy.HP; lowChar = enemy; }
        }
        if (high) return highChar;
        else return lowChar;
    }

    public CharBase FindRoleInPArty(PartyBase enemyParty, CharBase.Roles role) // pesquisa role específico conforme argumento
    {
        foreach (CharBase enemy in enemyParty.CharInParty) if (enemy.charRole == role) return enemy;         
        return null;
    }
    #endregion

    #region Acts as it says
    public IEnumerator AttackAct (CharBase attacker, CharBase defender, CharBase.Attributes atkAttrib, CharBase.Attributes defAttrib)
    {
       StartCoroutine( BattleFeedBacks.ParticleAttack(.5f,attacker.feedBackReference, defender.feedBackReference));
        OnFeedbackLine(attacker.charName + " is attacking  " + defender.charName + " on " + atkAttrib + " versus " + defAttrib);
        int damage = (attacker.AttributesD[atkAttrib] - ((defender.AttributesD[defAttrib]) / defenseModifier));
        defender.HP  -= damage;
        OnFeedbackLine(defender.charName +" recebeu dano de " + damage + ", hp = "+ defender.HP);
        BattleFeedBacks.ChangeStatAnim(defender, "HP", damage.ToString());

        if (!defender.alive)
        {
            OnFeedbackLine(defender.charName + " foi morto por " + attacker.charName);
            if (defender.isAttacker) partyA.CharInParty.Remove(defender);
            else partyB.CharInParty.Remove(defender);

            if (partyA.CharInParty.Count == 0){ EndBattle(partyB); yield return null;}
            else if (partyB.CharInParty.Count == 0){ EndBattle(partyA); yield return null; }
        }

        
        yield return new WaitForSeconds(timeStep);

        attacker.acted = true;
    }

    public void SkillAct(CharBase attacker, CharBase defender, CharBase.Attributes atkAttrib, CharBase.Attributes defAttrib)
    {
        Debug.Log(attacker.charName + " atker   " + defender.charName + "   Defender");
        Debug.Log(defender.HP + "  hp antes");
        defender.HP = defender.HP - (attacker.AttributesD[atkAttrib] - (defender.AttributesD[defAttrib]));
        Debug.Log(defender.HP + "  hp depois");
    }

    #endregion
    public CharBase.AttackBehavior BehaviuorPerRole(CharBase character, CharBase.MentalBehaviour mental )
    {
        switch (character.charRole)
        {
            case CharBase.Roles.Tank:
                {
                    character.preferedAttack = CharBase.Attacks.Melee;

                    if (UnityEngine.Random.Range(0, 2) < 1) return CharBase.AttackBehavior.HighHP;
                    else return CharBase.AttackBehavior.HighStrength;
                }
            case CharBase.Roles.DPS:
                {
                    if (character.AttributesD[CharBase.Attributes.Dextery] > character.AttributesD[CharBase.Attributes.Strength])
                        character.preferedAttack = CharBase.Attacks.Range;
                    else character.preferedAttack = CharBase.Attacks.Melee;

                    if (UnityEngine.Random.Range(0, 2) < 1) return CharBase.AttackBehavior.LowHP;
                    else return CharBase.AttackBehavior.HighHP;
                }

            case CharBase.Roles.Healer:
                {
                    if (character.AttributesD[CharBase.Attributes.Dextery] > character.AttributesD[CharBase.Attributes.Strength])
                        character.preferedAttack = CharBase.Attacks.Range;
                    else character.preferedAttack = CharBase.Attacks.Melee;

                    if (UnityEngine.Random.Range(0, 2) < 1) return CharBase.AttackBehavior.LowDextery;
                    else return CharBase.AttackBehavior.LowStrength;
                }

            case CharBase.Roles.Controller:
                {
                    if (character.AttributesD[CharBase.Attributes.Dextery] > character.AttributesD[CharBase.Attributes.Strength])
                        character.preferedAttack = CharBase.Attacks.Range;
                    else character.preferedAttack = CharBase.Attacks.Melee;

                    if (UnityEngine.Random.Range(0, 2) < 1) return CharBase.AttackBehavior.LowHP;
                    else return CharBase.AttackBehavior.LowStrength;
                }
        }
        return character.attackBehaviour;
    }
}
