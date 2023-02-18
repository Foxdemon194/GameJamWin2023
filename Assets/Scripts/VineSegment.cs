using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineSegment : MonoBehaviour
{
    public int inputDirection = 0;
    public VineType vineType = VineType.Straight;

    [SerializeField] GameObject vineTPrefab;
    [SerializeField] GameObject vineXPrefab;
    
    public void ExtendVine(int newDirection, out GameObject newSegment)
    {
        if(vineType == VineType.T)
        {
            newSegment = Instantiate(vineXPrefab, transform.position, Quaternion.identity, transform.parent);
            Destroy(gameObject);
            return;
        }
        if(vineType == VineType.Straight)
        {
            if (newDirection == (inputDirection + 1) % 4)
                transform.Rotate(0, 0, 90);
            else
                transform.Rotate(0, 0, -90);
            newSegment = Instantiate(vineTPrefab, transform.position, transform.rotation, transform.parent);
            Destroy(gameObject);
            return;
        }
        if(vineType == VineType.LClock) 
        {
            if (newDirection == (inputDirection + 3) % 4)
                transform.Rotate(0, 0, 0);
            else
                transform.Rotate(0, 0, 90);
            newSegment = Instantiate(vineTPrefab, transform.position, transform.rotation, transform.parent);
            Destroy(gameObject);
            return;
        }
        if (vineType == VineType.LCounter)
        {
            if (newDirection == (inputDirection + 1) % 4)
                transform.Rotate(0, 0, 90);
            else
                transform.Rotate(0, 0, 0);
            newSegment = Instantiate(vineTPrefab, transform.position, transform.rotation, transform.parent);
            Destroy(gameObject);
            return;
        }
        newSegment = null;
    }
}

public enum VineType
{
    Straight,
    LClock,
    LCounter,
    T
}