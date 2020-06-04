using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;

    public bool isDead{
        get{ return _isDead; }
        protected set {_isDead = value; }
    }


    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth; 

    [SerializeField]
    private Behaviour[]  disableOnDeath;
    private bool[] wasEnabled;

    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for(int i = 0; i<wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        } 
        SetDefaults();
    }

    [ClientRpc]
    public void RpcTakeDamage (int _amount)
    {
        if(isDead)
            return;

        currentHealth -= _amount;

        Debug.Log(transform.name + " boii lost few juices and how has "+ currentHealth + "health.");
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;

        for(int i=0; i<disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        Debug.Log(transform.name + " Got rekt!!!");
         Collider _col = GetComponent<Collider>();
        
        if(_col != null)
            _col.enabled = false;
        //Respawning
        StartCoroutine(Respawn());

    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);
        SetDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        Debug.Log(transform.name +  " boiii's back");
    } 
    public void SetDefaults()
    {
        _isDead = false;
        currentHealth = maxHealth;

        for(int i = 0; i<disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider _col = GetComponent<Collider>();
        if(_col != null)
            _col.enabled = true;
    }
}
