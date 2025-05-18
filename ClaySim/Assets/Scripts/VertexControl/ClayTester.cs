using UnityEngine;

public class ClayTester : MonoBehaviour
{
    public AdaptiveClay clay;
    public Transform testPoint;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            clay.DeformAtPoint(testPoint.position);
        }
    }
}
