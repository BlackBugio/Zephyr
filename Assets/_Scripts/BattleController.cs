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

    private void Start()
    {
        uc = GetComponent<UiController>();
    }
    public void DefineBattleSides() // Definine cada em character se é attacker ou defender (partyA ou partyB)
    {
        foreach (CharBase character in partyA.CharInParty)
        {
            //chama btn e add list btns;
            character.isAttacker = true;
        }
        foreach (CharBase character in partyB.CharInParty)
        {
            character.isAttacker = false;
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

        foreach (CharBase character in partyA.CharInParty)
        {
            int charInitiative = character.AttributesD[CharBase.Attributes.Dextery] + character.AttributesD[CharBase.Attributes.Intelligence];
            if (CanAct(character) && charInitiative > initTestA)
            {
                initTestA = charInitiative;
                nextA = character;
            }
        }

        foreach (CharBase character in partyB.CharInParty)
        {
            int charInitiative = character.AttributesD[CharBase.Attributes.Dextery] + character.AttributesD[CharBase.Attributes.Intelligence];
            if (CanAct(character) && charInitiative > initTestB)
            {
                initTestB = charInitiative;
                nextB = character;
            }
        }

        if (nextA == null) charTurn = nextB;
        else if (nextB == null) charTurn = nextA;
        else if (nextB == null && nextA == null) { EndTurn(); return null; }

        else if (initTestA > initTestB) {charTurn = nextA; Debug.Log("passo do end turn"); }
        else if (initTestA < initTestB) charTurn = nextB;
        else if (initTestA == initTestB) // implementar nega de dex ou int
        {
            float roll = UnityEngine.Random.Range(0, 2);
            if (roll > 1) charTurn = nextA;
            else charTurn = nextB;
        }
        
        Debug.Log(charTurn.charName + "  " + charTurn.mentalBehaviour + " " + charTurn.preferedAttack);
        return charTurn;
    }

    public IEnumerator DecideAction(CharBase character) // etapa de decisão de ação do char, se for nulo chama o endturn pois todos já jogaram
    {   // se for o cpu, decide a acao
        if (!playerTurn) 
        {
            Debug.Log("IA decidindo a acao");
            if (character != null) //se for nulo chama o endturn
            {
                switch (character.mentalBehaviour)
                {
                    case CharBase.MentalBehaviour.Aggressive:
                        {
                            Debug.Log("Chegou no caso certo agora");
                             character.attackBehaviour = BehaviuorPerRole(character, CharBase.MentalBehaviour.Aggressive);

                            TakeAction(character, FindTargetFromBehaviour(character, EnemyParty(character)), CharBase.Actions.Attack);
                            break;
                        }
                    case CharBase.MentalBehaviour.Tactical:
                        TakeAction(character, FindTargetFromBehaviour(character, EnemyParty(character)), CharBase.Actions.Attack);
                        break;
                    case CharBase.MentalBehaviour.Defensive:
                        TakeAction(character, FindTargetFromBehaviour(character, EnemyParty(character)), CharBase.Actions.Attack);
                        break;
                }

            }
            else { EndTurn(); }
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

            TakeAction(character, targetA, actionToTake);
        }
        yield return null;
    }

    public void TakeAction(CharBase character, CharBase target, CharBase.Actions action) // toma a ação definida e chama novamente a rolagem de iniciativa para o próximo char
    {
        Debug.Log("Taking action  " + target);
        if (action == CharBase.Actions.Attack)
        {
            switch (character.preferedAttack)
            {
                case CharBase.Attacks.Melee: AttackAct(character, target, CharBase.Attributes.Strength, CharBase.Attributes.Strength); break;
                case CharBase.Attacks.Range: AttackAct(character, target, CharBase.Attributes.Dextery, CharBase.Attributes.Dextery); break;
                case CharBase.Attacks.Ritual: AttackAct(character, target, CharBase.Attributes.Wisdom, CharBase.Attributes.Wisdom); break;
                case CharBase.Attacks.Magic: AttackAct(character, target, CharBase.Attributes.Intelligence, CharBase.Attributes.Intelligence); break;
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
        character.acted = true;
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
            else EndBattle(partyB);
        }

        foreach (CharBase character in partyB.CharInParty)
        {
            if (character != null)
            {
                character.acted = false; // limpa a lista de ações do turno
            }
            else EndBattle(partyA);
        }
        StartCoroutine(DecideAction(InitiativeRoll()));
    } 



    public bool CanAct(CharBase character) // testa se alguma ação foi tomada no turno
    {
        Debug.Log("char can act?" + character.charName + !character.acted);
        return !character.acted;
       
    }

    public void EndBattle(PartyBase winner) // finaliza batalha anuncia vencedor
    {
        Debug.Log(" End BAttle w diubUb Ss");
    } 

    public PartyBase EnemyParty(CharBase character) // verifica a outra party
    {
        if (character.isAttacker) return partyB;
        else return partyA;
    }

    // acha targets e seleciona attributos e acoes baseado no perfil de comportamento;
    #region IA
    public CharBase FindTargetFromBehaviour(CharBase character, PartyBase otherParty) // encontra o target de acordo com o behaviour
    {
        Debug.Log("finding in party" + character.attackBehaviour);
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
             if (enemy.AttributesD[attribute] > highAttrib) { highAttrib = enemy.AttributesD[attribute]; highChar = enemy;}
            else
                if (enemy.AttributesD[attribute] > lowAttrib) { lowAttrib = enemy.AttributesD[attribute]; lowChar = enemy;}
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
                if (enemy.HP > highAttrib) { highAttrib = enemy.HP; highChar = enemy; }
            
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

    public void AttackAct (CharBase attacker, CharBase defender, CharBase.Attributes atkAttrib, CharBase.Attributes defAttrib)
    {
        Debug.Log(attacker.charName + " atker   " + defender.charName + "   Defender");
        Debug.Log(defender.HP + "  hp antes");
        defender.HP = defender.HP - (attacker.AttributesD[atkAttrib] - (defender.AttributesD[defAttrib]));
        Debug.Log(defender.HP + "  hp depois");
    }

    public void SkillAct(CharBase attacker, CharBase defender, CharBase.Attributes atkAttrib, CharBase.Attributes defAttrib)
    {
        Debug.Log(attacker.charName + " atker   " + defender.charName + "   Defender");
        Debug.Log(defender.HP + "  hp antes");
        defender.HP = defender.HP - (attacker.AttributesD[atkAttrib] - (defender.AttributesD[defAttrib]));
        Debug.Log(defender.HP + "  hp depois");
    }

    public CharBase.AttackBehavior BehaviuorPerRole(CharBase character, CharBase.MentalBehaviour mental )
    {
        switch (character.charRole)
        {
            case CharBase.Roles.Tank:

                {
                    if (character.AttributesD[CharBase.Attributes.Dextery] > character.AttributesD[CharBase.Attributes.Strength])
                        character.preferedAttack = CharBase.Attacks.Range;
                    else character.preferedAttack = CharBase.Attacks.Melee;

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
