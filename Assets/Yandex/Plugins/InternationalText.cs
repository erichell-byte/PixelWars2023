using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InternationalText : MonoBehaviour
{
    [SerializeField] private string en;
    [SerializeField] private string ru;
    [SerializeField] private string tr;

    private void Start()
    {
        if (Language.Instance.currentLanguage == "en")
        {
            GetComponent<TextMeshProUGUI>().text = en;
        }
        else if (Language.Instance.currentLanguage == "ru")
        {
            GetComponent<TextMeshProUGUI>().text = ru;
        }
        else if (Language.Instance.currentLanguage == "tr")
        {
            GetComponent<TextMeshProUGUI>().text = tr;
        }
        else
        {
            GetComponent<TextMeshProUGUI>().text = en;
        }
    }
}
