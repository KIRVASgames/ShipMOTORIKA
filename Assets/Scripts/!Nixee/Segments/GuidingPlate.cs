using ShipMotorika;
using System;
using UnityEngine;

public class GuidingPlate : MonoBehaviour
{
    [Serializable]
    public class SegmentForPlate
    {
        [SerializeField] public MapSegment segment;
        [SerializeField] public int segmentID;
    }

    [SerializeField] private SegmentForPlate segmentForPlate;
    [SerializeField] private Collider2D Trigger;

    public void RevealSegment()
    {
        int realID = segmentForPlate.segmentID - 1;

        if (realID < 0)
            Debug.LogWarning("realID is less than zero in " + name);

        segmentForPlate.segment.OnSegmentRevealed?.Invoke(realID);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root.gameObject == PlayerController.Instance.gameObject)
        {
            RevealSegment();
            Trigger.enabled = false;
        }
    }
}