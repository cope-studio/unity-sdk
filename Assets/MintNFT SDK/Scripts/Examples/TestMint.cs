using System;

using UnityEngine;
using MintNFTSDK.SingleMint;

public class TestMint : MonoBehaviour
{
    [Header("Request Body Fields")]
    [Space(5f)]
    // Define fields for the request body
    [Tooltip("Wallet address used for getting the API Key")]
    public string wallet;
    [Tooltip("The custom contract address")]
    public string contractAddress;
    [Tooltip("The Wallet address to which the NFT has to be Minted to")]
    public string to;
    [Tooltip("IPFS Uri that you got as a respose while uploading")]
    public string tokenUri;
    [Tooltip("Total supply for ERC1155 contract")]
    public int supply;

    // API Key from the platform
    private string APIKEY = ""; // Add your API key here

    // JWT from the platform
    private string JWT = ""; // Add your JWT here

    // Mint request body object
    private MintBody body = new MintBody();
    private string bodyJson;

    private void Awake()
    {
        if (APIKEY == "")
            Debug.LogError("No API key provided! Please provide a valid API key.");
    }

    // Start is called before the first frame update
    private void Start()
    {
        body.wallet = wallet;
        body.contractAddress = contractAddress;
        body.to = to;
        body.tokenUri = tokenUri;
        body.supply = supply; // Only relevant if the custom contract is 1155

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

    // Mint the NFT (async operation)
    public async void MintFile()
    {
        string result = await MintNFT.Mint(APIKEY, JWT, bodyJson);
        Debug.Log(result);
    }
}

[Serializable]
public class MintBody
{
    public string wallet;
    public string contractAddress;
    public string to;
    public string tokenUri;
    public int supply;
}

