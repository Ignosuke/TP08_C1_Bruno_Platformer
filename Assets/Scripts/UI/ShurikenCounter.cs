using TMPro;
using UnityEngine;

// Los shurikens del jugador en el HUD, como un numero
public class ShurikenCounter : MonoBehaviour
{
    [SerializeField] private PlayerThrower thrower;
    [SerializeField] private TMP_Text text;

    private void OnEnable()
    {
        thrower.OnAmmoChanged += Refresh;
    }

    private void OnDisable()
    {
        thrower.OnAmmoChanged -= Refresh;
    }

    private void Start()
    {
        Refresh(thrower.GetAmmo());
    }

    private void Refresh(int ammo)
    {
        text.text = ammo.ToString();
    }
}
