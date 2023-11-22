using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateEffectedUI : MonoBehaviour
{
    public Image image;
    public IEnumerator SetDuration_co(float duration)
    {
        //지속시간 갱신되면 앞으로 이동시켜줘
        float time = 0;

        float startNum = 1;
        float endNum = 0;

        while (time <= duration)
        {
            startNum = Mathf.Lerp(startNum, endNum, Time.deltaTime / duration*2.5f); 
            image.color = new Color(image.color.r, image.color.g, image.color.b, startNum);

            time += Time.deltaTime;
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);

        gameObject.SetActive(false);

        /*while(Time.deltaTime >= duration)
        {
            yield return null;
        }*/

    }
}
