using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class Language : MonoBehaviour
{
    
    [DllImport("__Internal")]
    private static extern string GetLang();
    
    
    public string currentLanguage; // ru en
    
    public static Language Instance; 
    void Awake()
    {
#if !UNITY_EDITOR
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            currentLanguage = GetLang();
        }
        else
        {
            Destroy(gameObject);
            
        }
#elif UNITY_EDITOR
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            currentLanguage = "ru";
        }
        else
        {
            Destroy(gameObject);
            
        }
#endif            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
