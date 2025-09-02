using UnityEngine;

public class Pistol : MonoBehaviour
{
    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }
}
