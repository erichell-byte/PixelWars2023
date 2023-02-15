using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Yandex : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private RawImage _photo;
    [SerializeField] private GameObject loginButtonGO;
    [SerializeField] private Button rateGameButton;
     
    [DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    private static extern void GiveMePlayerData();

    [DllImport("__Internal")]
    private static extern void RateGame();
    
    [DllImport("__Internal")]
    private static extern void Auth();

    
    public void RateToYandex()
    {
        RateGame();
    }
    public void LoginToYandex()
    {
        Auth();
        GiveMePlayerData();
    }

    public void SetName(string name)
    {
        _nameText.text = name;
    }

    public void SetPhoto(string url)
    {
        StartCoroutine(DownloadImage(url));
    }

    IEnumerator DownloadImage(string mediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            Debug.Log(request.error);
        else
            _photo.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

    public void NotAuthorizedUI()
    {
        loginButtonGO.SetActive(true);
        _nameText.gameObject.SetActive(false);
        _photo.gameObject.SetActive(false);
        rateGameButton.gameObject.SetActive(false);
        
    }

    public void AuthorizedUI()
    {
        loginButtonGO.SetActive(false);
        _nameText.gameObject.SetActive(true);
        _photo.gameObject.SetActive(true);
        GiveMePlayerData();
        rateGameButton.gameObject.SetActive(true);
        
    }

    public void GameWasRated()
    {
        rateGameButton.interactable = false;
    }
    


}