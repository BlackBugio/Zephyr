using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection;
using System.Security;
using System;

public class FillInfoLine : UiController
{
    // busca os fields dentro de uma classe e retonar um array testando se o transform existe e esta ativo
    private Array classFields;

    public static void FillCharLine(CharBase character, Transform parent)
    {
        
            TextMeshProUGUI nameValue = parent.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();
        if (nameValue != null)

                nameValue.text = character.charName;

        TextMeshProUGUI levelValue = parent.Find("Level").GetComponent<TextMeshProUGUI>();
        if (levelValue != null)
            levelValue.text = character.charLevel.ToString();

        TextMeshProUGUI rarityValue = parent.Find("Rarity").GetComponent<TextMeshProUGUI>();
        if (rarityValue != null)
            rarityValue.text = character.charRarity.ToString();

        TextMeshProUGUI classValue = parent.Find("Class").GetComponent<TextMeshProUGUI>();
        if (classValue != null)
            classValue.text = character.charClass.ToString();

        TextMeshProUGUI roleValue = parent.Find("Role").GetComponent<TextMeshProUGUI>();
        if (roleValue != null)
            roleValue.text = character.charRole.ToString();
        TextMeshProUGUI description = parent.Find("Role").GetComponent<TextMeshProUGUI>();
        if (roleValue != null)
            roleValue.text = character.charRole.ToString();

    }

    // procura os membros de uma deternimnada classe 
public static void GetMembersArray(Type typeToFindMembers)
    {
        try
        {           
            MemberInfo[] myMemberInfo;
            Type myType = typeToFindMembers;            
            // Get the information related to all public member's of 'MyClass'.
            myMemberInfo = myType.GetMembers();
            for (int i = 0; i < myMemberInfo.Length; i++)
            {
                // Display name and type of the concerned member.
                Debug.Log( myMemberInfo[i].Name +"  "+ myMemberInfo[i].MemberType);
            }
        }
        catch (SecurityException e)
        {
            Console.WriteLine("Exception : " + e.Message);
        }
        
    }
}