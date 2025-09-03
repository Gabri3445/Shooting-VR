using TMPro;
using UnityEngine;

public class AmmoCounter : MonoBehaviour
{
    public TMP_Text leftAmmoCounter;
    public TMP_Text rightAmmoCounter;

    public void UpdateAmmoCounter(int ammoCount)
    {
        leftAmmoCounter.text = ammoCount.ToString();
        rightAmmoCounter.text = ammoCount.ToString();
    }
}