# MintNFT Unity SDK

Minting your NFTs over the web and plugging APIs to mint it through your own DApps are all cool. What about being able to mint in-game NFTs directly from Unity? We have got you covered! Introducing our Unity SDK

## Implementation Steps

1. Download the contents from the repo
2. Open the project on Unity, preferably Unity 2021.3
3. There are 3 scene files namely "TestUpload", "TestMint" and "TestRefreshJWT" which you can use to test
4. The "TestUpload.cs", "TestMint.cs" and "TestRefreshJWT.cs" scripts can help you get started to write your own scripts
5. Main 3 methods in the "MintNFT.cs" class are "Upload", "Mint" and "RefreshJWT"

### Upload
```
string result = MintNFT.Upload(string APIKEY, List<IMultipartFormSection> formData);
```
### Mint
```
string result = MintNFT.Mint(string APIKEY, string JWT, string stringifiedJsonBody);
```
### Refresh JWT
```
string result = MintNFT.RefreshJWT(string APIKEY, string REFRESH_TOKEN, string stringifiedJsonBody);
```

To get the APIKEY, JWT and REFRESH_TOKEN a please go to API dashboard - https://app.0xmint.io/create/api
To check the formats of the formdata, json body, etc. please check our documentation here - https://docs.0xmint.io/

## IMPORTANT
In v2 of the APIs, creating a custom contract from the API dashboard is mandatory.
WE ARE DEPRECATING V1 APIs AND SUGGEST THE USAGE OF ONLY V2 APIs!
