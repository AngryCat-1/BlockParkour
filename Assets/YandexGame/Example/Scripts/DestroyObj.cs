using UnityEngine;

public class DestroyObj : MonoBehaviour
{
    [SerializeField] GameObject destroyObj;

    public void Destroy()
    {
        Destroy(destroyObj);
    }
}
