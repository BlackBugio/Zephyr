using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public PartyBase partyA;
    public PartyBase partyB;
    public bool isSurpriseAttack;
    public int defenseModifier = 3;
    public bool actionsToTake = true;

    public void DefineBattleSides() // Definine cada em character se � attacker ou defender (partyA ou partyB)
    {
        foreach (CharBase character in partyA.CharInParty) character.isAttacker = true;
        foreach (CharBase character in partyB.CharInParty) character.isAttacker = false;
    }

    public void StartBattle(PartyBase attacker, PartyBase defender) // m�todo publico de chamada de batalha
    {
        partyA = attacker;
        partyB = defender;
        DefineBattleSides();
        StartTurn();
    }

    public void StartTurn() // inicia um novo turno
    {
        DecideAction(InitiativeRoll());
    }

    public CharBase InitiativeRoll()  //Testa a deztreza e int de todos os char de cada party e retorna o char da vez;  
    {
        CharBase charTurn = null;
        CharBase nextA = null;
        CharBase nextB = null;
        int initTestA = 0;
        int initTestB = 0;

        if (isSurpriseAttack)
        {
            foreach (CharBase character in partyA.CharInParty)
            {
                int charDextery = character.AttributesD[CharBase.Attributes.Dextery] + character.AttributesD[CharBase.Attributes.Intelligence];
                if (charDextery > initTestA && CanAct(character))
                {
                    initTestA = character.AttributesD[CharBase.Attributes.Dextery];
                    charTurn = character;
                }
            }

            foreach (CharBase character in partyB.CharInParty) // defenders perdem o turno
            {
                character.ActionsD[CharBase.Actions.Attack] = true;
            }
        }
        else
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
            else if (nextB == null && nextA == null) return null;
            else if (initTestA > initTestB) charTurn = nextA;
            else if (initTestA < initTestB) charTurn = nextB;
            else if (initTestA == initTestB) // implementar nega de dex ou int
            {
                float roll = Random.Range(0, 2);
                if (roll > 1) charTurn = nextA;
                else charTurn = nextB;
            }
        }

        return charTurn;
    }

    public void DecideAction(CharBase character) // etapa de decis�o de a��o do char, se for nulo chama o endturn pois todos j� jogaram
    {
        if (character != null) //se for nulo chama o endturn
        {
            switch (character.mentalBehaviour)
            {
                case CharBase.MentalBehaviour.Aggressive:
                    {
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
        else { EndTurn();}
    }

    public void TakeAction(CharBase character, CharBase target, CharBase.Actions action) // toma a a��o definida e chama novamente a rolagem de iniciativa para o pr�ximo char
    {
        if (action == CharBase.Actions.Attack)
        {
            switch (character.preferedAttack)
            {
                case CharBase.Attacks.Melee: AttackAct(character, target, CharBase.Attributes.Strenght, CharBase.Attributes.Strenght); break;
                case CharBase.Attacks.Range: AttackAct(character, target, CharBase.Attributes.Dextery, CharBase.Attributes.Dextery); break;
                case CharBase.Attacks.Ritual: AttackAct(character, target, CharBase.Attributes.Wisdom, CharBase.Attributes.Wisdom); break;
                case CharBase.Attacks.Magic: AttackAct(character, target, CharBase.Attributes.Intelligence, CharBase.Attributes.Intelligence); break;
            }
        }

        else if (action == CharBase.Actions.UseSkill)
        {
            //implementar Skill
        }

        else if (action == CharBase.Actions.ConsumeItem)
        {
            //implementar Itens
        }

        DecideAction(InitiativeRoll());
    }

    public void EndTurn() // acaba o turno, limpa lista de a��es tomadas
    {
        foreach (CharBase character in partyA.CharInParty)
        {
            if (character != null)
            {
                character.ClearActions(); // limpa a lista de a��es do turno
            }
            else EndBattle(partyB);
        }

        foreach (CharBase character in partyB.CharInParty)
        {
            if (character != null)
            {
                character.ClearActions(); // limpa a lista de a��es do turno
            }
            else EndBattle(partyA);
        }
    } 



    public bool CanAct(CharBase character) // testa se alguma a��o foi tomada no turno
    {
        foreach (var pair in character.ActionsD)
        {
            if (pair.Value == true) return false;
        }
        return true;
    }

    public void EndBattle(PartyBase winner) // finaliza batalha anuncia vencedor
    {
        //implementar end battle
    } 

    public PartyBase EnemyParty(CharBase character) // verifica a outra party
    {
        if (character.isAttacker) return partyB;
        else return partyA;
    } 

    public CharBase FindTargetFromBehaviour(CharBase character, PartyBase otherParty) // encontra o target de acordo com o behaviour
    {      
        return character.attackBehaviour switch
        {
            CharBase.AttackBehavior.HighDextery => FindAttribInPArty(otherParty, CharBase.Attributes.Dextery, true),
            CharBase.AttackBehavior.LowDextery => FindAttribInPArty(otherParty, CharBase.Attributes.Dextery, false),
            CharBase.AttackBehavior.HighStrength => FindAttribInPArty(otherParty, CharBase.Attributes.Strenght, true),
            CharBase.AttackBehavior.LowStrength => FindAttribInPArty(otherParty, CharBase.Attributes.Strenght, false),
            CharBase.AttackBehavior.HighHP => FindHPInPArty(otherParty, true),
            CharBase.AttackBehavior.LowHP => FindHPInPArty(otherParty, false),
            CharBase.AttackBehavior.RoleD => FindRoleInPArty(otherParty, CharBase.Roles.DPS),
            CharBase.AttackBehavior.RoleH => FindRoleInPArty(otherParty, CharBase.Roles.Healer),
            CharBase.AttackBehavior.RoleT => FindRoleInPArty(otherParty, CharBase.Roles.Tank),
            CharBase.AttackBehavior.RoleC => FindRoleInPArty(otherParty, CharBase.Roles.Controller),
            _ => null,
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
                   if (enemy.HP > lowAttrib) { lowAttrib = enemy.HP; lowChar = enemy; }
        }
        if (high) return highChar;
        else return lowChar;
    }

    public CharBase FindRoleInPArty(PartyBase enemyParty, CharBase.Roles role) // pesquisa role espec�fico conforme argumento
    {
        foreach (CharBase enemy in enemyParty.CharInParty) if (enemy.role == role) return enemy;         
        return null;
    } 

    public void AttackAct (CharBase attacker, CharBase defender, CharBase.Attributes atkAttrib, CharBase.Attributes defAttrib)
    {
        defender.HP =- attacker.AttributesD[atkAttrib] - defender.AttributesD[defAttrib] / defenseModifier;
    }

    public CharBase.AttackBehavior BehaviuorPerRole(CharBase character, CharBase.MentalBehaviour mental )
    {
        switch (character.role)
        {
            case CharBase.Roles.Tank:

                {
                    if (character.AttributesD[CharBase.Attributes.Dextery] > character.AttributesD[CharBase.Attributes.Strenght])
                        character.preferedAttack = CharBase.Attacks.Range;
                    else character.preferedAttack = CharBase.Attacks.Melee;

                    if (Random.Range(0, 2) < 1) return CharBase.AttackBehavior.HighHP;
                    else return CharBase.AttackBehavior.HighStrength;
                }
            case CharBase.Roles.DPS:
                {
                    if (character.AttributesD[CharBase.Attributes.Dextery] > character.AttributesD[CharBase.Attributes.Strenght])
                        character.preferedAttack = CharBase.Attacks.Range;
                    else character.preferedAttack = CharBase.Attacks.Melee;

                    if (Random.Range(0, 2) < 1) return CharBase.AttackBehavior.LowHP;
                    else return CharBase.AttackBehavior.HighHP;
                }

            case CharBase.Roles.Healer:
                {
                    if (character.AttributesD[CharBase.Attributes.Dextery] > character.AttributesD[CharBase.Attributes.Strenght])
                        character.preferedAttack = CharBase.Attacks.Range;
                    else character.preferedAttack = CharBase.Attacks.Melee;

                    if (Random.Range(0, 2) < 1) return CharBase.AttackBehavior.LowDextery;
                    else return CharBase.AttackBehavior.LowStrength;
                }

            case CharBase.Roles.Controller:
                {
                    if (character.AttributesD[CharBase.Attributes.Dextery] > character.AttributesD[CharBase.Attributes.Strenght])
                        character.preferedAttack = CharBase.Attacks.Range;
                    else character.preferedAttack = CharBase.Attacks.Melee;

                    if (Random.Range(0, 2) < 1) return CharBase.AttackBehavior.LowHP;
                    else return CharBase.AttackBehavior.LowStrength;
                }
        }
        return character.attackBehaviour;
    }
}
