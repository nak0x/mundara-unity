using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Windows.Kinect;

public class DetectJoints : MonoBehaviour
{
    [SerializeField]
    public GameObject BodySrcManager;
    private BodySourceManager bodyManager;

    public GameObject DephSrcManager;

    public JointType TrackedJoint;
    private Body[] bodies;
    private DepthFrameReader reader;

    public Vector3 multiplier = new Vector3(1,1,1); // 14 is the perfect multiplicator for corresponde to the reel life where 10 unit = 1 meter
    public Vector3 offset = new Vector3(0,0,0);

    public float lerpValue = 2f;

    public float treadshold = 1f;

    public lookatcamera cameraFollower;

    public TMP_Text textDebugCamera;

    // Use this for initialization
    void Start()
    {

        if (BodySrcManager == null)
        {
            Debug.Log("Asign Game Object with Body Source Manager");
        }
        else
        {
            bodyManager = BodySrcManager.GetComponent<BodySourceManager>();
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (BodySrcManager == null)
        {
            updateTextDebug("No BodySrcManager", Color.red);
            return;
        }

        bodies = bodyManager.GetData();
        if (bodies == null)
        {
            updateTextDebug("no bodies detected", Color.red);
            return;
        }

        foreach (var body in bodies)
        {
            if (body == null)
            {
                updateTextDebug("no body detected", Color.red);
                continue;
            }
            if (body.IsTracked)
            {
                updateTextDebug("head detected", Color.green);
                var pos = body.Joints[TrackedJoint].Position;
                //gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(-pos.X * multiplier, pos.Y * multiplier, 107), lerpValue);
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3((-pos.X + offset.x) * multiplier.x, (pos.Y + offset.y) * multiplier.y, (pos.Z + offset.z) * multiplier.z), lerpValue);


                //cameraFollower.enabled = true; // enabled camera follower script after the first position set up because there is a bug lag if not
            }
            //else
            //{
            //    updateTextDebug("no body detected", Color.red);
            //}
        }

    }

    public void updateTextDebug(string text, Color color)
    {
        textDebugCamera.text = text;
        textDebugCamera.color = color;
    }
}
