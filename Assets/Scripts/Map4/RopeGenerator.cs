using UnityEngine;

public class RopeGenerator : MonoBehaviour
{
    public GameObject ropeSegmentPrefab;
    public int segmentCount = 20;
    public float segmentSpacing = 0.5f;

    private GameObject lastSegment;

    void Start()
    {
        Vector2 spawnPosition = transform.position;

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject newSegment = Instantiate(ropeSegmentPrefab, spawnPosition, Quaternion.identity);
            newSegment.transform.rotation = Quaternion.identity; // giữ thẳng

            if (i == 0)
            {
                newSegment.GetComponent<HingeJoint2D>().connectedBody = null;
            }
            else
            {
                newSegment.GetComponent<HingeJoint2D>().connectedBody = lastSegment.GetComponent<Rigidbody2D>();
            }

            lastSegment = newSegment;
            spawnPosition.y -= segmentSpacing;
        }
    }
}
