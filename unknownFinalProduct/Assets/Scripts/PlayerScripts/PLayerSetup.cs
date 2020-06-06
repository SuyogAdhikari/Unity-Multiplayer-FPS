using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PLayerSetup : NetworkBehaviour
{
   
    public Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";

    [SerializeField]
    GameObject playerGraphics;
   
    [SerializeField]
    GameObject playerUIPrefab;
    private GameObject playerUIInstance;

    Camera sceneCamera;

    void Start()
    {
        if(!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }else{
            
            DisableSceneCamera();
            
            //Disable Player graphics for local player
            SetLayerRecursively (playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //Create PlayerUI
            playerUIInstance = Instantiate (playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
        }
        GetComponent<Player>().Setup();
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public override void OnStartClient(){
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        GameManager.RegisterPlayer (_netID, _player);
    }

    //Assigns RemoteLayer to player other than self
    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    //Disable Scene Camera view when player is connected to lobby or game
    void DisableSceneCamera()
    {
        sceneCamera = Camera.main;
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(false);
        }
    }

    //Disable the components so that player can only control his model
    void DisableComponents()
    {
        for(int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    //Switches to scene camera if player is not connected to any lobby
    //Destroy the player if disconnected
    void OnDisable ()
    {
        Destroy (playerUIInstance);
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer (transform.name);
    }
}
