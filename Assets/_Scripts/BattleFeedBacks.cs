using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleFeedBacks : UiController
{
    public GameObject[] FeedbackToInstantiate;
    // Start is called before the first frame update
       
    public static void ChangeStatAnim(CharBase character, string StatToAffect, string value)
    {
        int valueToInt = System.Int32.Parse(value);
        Transform statToChangeValue = character.feedBackReference.Find(StatToAffect).transform.Find("value");
       
        TextMeshProUGUI statToChangeText  = statToChangeValue.GetComponent<TextMeshProUGUI>();       
        if (character.alive)
        { 
            statToChangeText.text = character.HP.ToString();
        }
        else
        {
            statToChangeText.text = "0";
            Image charRefBG = character.feedBackReference.GetComponent<Image>();
            charRefBG.color = Color.gray;

        }
        GameObject feedbackLive = Instantiate(Resources.Load<GameObject>("SpiningText"), statToChangeValue);        
        TextMeshProUGUI feedBackText = feedbackLive.GetComponent<TextMeshProUGUI>();
        feedbackLive.transform.position += Vector3.right * .013f;
        feedBackText.text = value;
        if (valueToInt > 0) feedBackText.color = Color.red;
        else feedBackText.color = Color.green;
        Destroy(feedbackLive, 2f);               
    }

    // Update is called once per frame
    public static void SelecedFeedback(CharBase character, bool active)
    {
        character.feedBackReference.transform.Find("SelectionFeedBack").GetComponent<Image>().enabled = active;
    }
    public static IEnumerator ParticleAttack(float time, Transform emmiter, Transform receiver)
    {
        
        GameObject particleObj = Instantiate(Resources.Load<GameObject>("ParticleAttack"), emmiter);

        Vector3 startingPos = particleObj.transform.position;
        Vector3 finalPos = receiver.position;
        float elapsedTime = 0;

        Destroy(particleObj, time+.01f);
        while (elapsedTime < time)
        {
            particleObj.transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

}
