using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PLayerSetup : NetworkBehaviour
{
   
    public Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    Camera sceneCamera;

    void Start()
    {
        if(!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }else{
            
            DisableSceneCamera();
        }
        GetComponent<Player>().Setup();
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
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer (transform.name);
    }
}
