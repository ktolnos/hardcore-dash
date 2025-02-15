using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAddons : MonoBehaviour
{
    public TextMeshProUGUI ammoCountTMP;
    public TextMeshProUGUI healthTMP;
    private Gun gun;
    private PlayerMovement playerMovement;
    private Damagable damagable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        damagable = gameObject.GetComponent<Damagable>();
    }

    // Update is called once per frame
    void Update()
    {
        gun = playerMovement.gun;
        if (gun != null)
        {
            ammoCountTMP.text = gun.ammo.ToString();
        }
        else
        {
            ammoCountTMP.text = "";
        }
        healthTMP.text = damagable.health.ToString();
    }
}
