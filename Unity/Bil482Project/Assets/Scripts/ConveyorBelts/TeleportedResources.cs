using UnityEngine;

public class TeleportedResource : MonoBehaviour
{
    public bool isTeleportedFromConveyorBelt = false;

    void Start()
    {
        Debug.Log($" Resource {gameObject.name} marked as teleported from ConveyorBelt");
    }
}