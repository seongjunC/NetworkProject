using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject tank;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject missile;

    [SerializeField] Transform missileStartPos;
    [SerializeField] Transform missileTargetPos;

    IEnumerator Start()
    {
        StartCoroutine(TextChange());

        missile.transform.position = missileStartPos.position;
        missile.gameObject.SetActive(true);
        explosion.gameObject.SetActive(false);

        float duration = 0.4f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            missile.transform.position = Vector3.Lerp(missileStartPos.position, missileTargetPos.position, t);
            yield return null;
        }

        explosion.gameObject.SetActive(true);
        missile.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.9f);

        gameObject.SetActive(false);
    }

    IEnumerator TextChange()
    {
        int count = 3;
        while (true)
        {
            if (count == 3)
            {
                text.text = "Loading";
                count = 0;
            }
            else
            {
                text.text += ".";
                count++;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
