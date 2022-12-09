using System;

using UnityEngine;
using MintNFTSDK.SingleMint;

public class TestRefreshJWT : MonoBehaviour
{
    [Header("Request Body Fields")]
    [Space(5f)]
    // Define fields for the request body
    [Tooltip("Wallet address used for getting the API Key")]
    public string wallet;

    // API Key from the platform
    private string APIKEY = ""; // Add your API key here

    // Refresh token from the platform
    private string REFRESH_TOKEN = ""; // Add your Refresh token here

    // Refresh request body object
    private RefreshBody body = new RefreshBody();
    private string bodyJson;

    // Start is called before the first frame update
    void Start()
    {
        body.wallet = wallet;

        bodyJson = JsonUtility.ToJson(body);

        if (wallet == "")
        {
            Debug.LogError("No wallet address provided! Please provide a valid wallet address.");
        }
        else
        {
            Debug.Log(bodyJson);
        }
    }

    // Refresh the JWT token
    public async void RefreshJWT()
    {
        string result = await MintNFT.RefreshJWT(APIKEY, REFRESH_TOKEN, bodyJson);
        Debug.Log(result);
    }
}

[Serializable]
public class RefreshBody
{
    public string wallet;
}
