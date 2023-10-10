using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObject : MonoBehaviour
{
    public List<GameObject> object_list; 

    private OVRHand[] hands;
    private OVRSkeleton[] skeletons;
    private IList<OVRBone> righthand_bones;
    int IndexTip_id = (int) OVRSkeleton.BoneId.Hand_IndexTip;

    private GestureRecognizer GestureDetector;

    public string selected_object_name = "none";
    public GameObject selected_object;
    bool selected = false;
    Quaternion rotation_diff;

    // Start is called before the first frame update
    void Start()
    {
        hands = new OVRHand[]
        {
            GameObject.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/OVRHandPrefab").GetComponent<OVRHand>(),
            GameObject.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/OVRHandPrefab").GetComponent<OVRHand>(),
        };

        skeletons = new OVRSkeleton[]
        {
            GameObject.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/OVRHandPrefab").GetComponent<OVRSkeleton>(),
            GameObject.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/OVRHandPrefab").GetComponent<OVRSkeleton>(),
        };

        righthand_bones = skeletons[(int)OVRHand.Hand.HandRight].Bones;

        GestureDetector = GameObject.Find("GestureDetector").GetComponent<GestureRecognizer>();

        object_list = GameObject.Find("DesignOperation").GetComponent<DrawObject>().object_list;
    }

    // Update is called once per frame
    void Update()
    {
        if(righthand_bones.Count == 0)
        {
            righthand_bones = skeletons[(int)OVRHand.Hand.HandRight].Bones;
        }
        
        if(hands[1].GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            if(selected)
            {
                selected_object.transform.position = righthand_bones[IndexTip_id].Transform.position;
                //selected_object.transform.rotation = righthand_bones[IndexTip_id].Transform.rotation;
            }
            else
            {
                // Find object closest to index tip
                selected_object_name = FindObject();
                if (selected_object_name != "none")
                {
                    // Move object with index tip
                    selected_object = GameObject.Find(selected_object_name);
                    selected = true; 
                }
            }
            
        }
        else
        {
            selected = false;
        }
        
    }

    string FindObject()
    {
        selected_object_name = "none";   
        print(IndexTip_id);
        print(righthand_bones.Count);
        Vector3 IndexTip_pos = righthand_bones[IndexTip_id].Transform.position;     
        float MinDistance = 10.0F;      
        float FingerObjectDistance = 10;

        // Iterate object list
        foreach (GameObject ob in object_list)
        {
            FingerObjectDistance = Vector3.Distance(IndexTip_pos, ob.transform.position);
            if (MinDistance > FingerObjectDistance)
            {
                MinDistance = FingerObjectDistance;
                selected_object_name = ob.name;
            }
        }

        // Only select object within distance limit
        if(MinDistance < 0.05f)
        {
            return selected_object_name;
        }

        return "none";
    }

    void UnDrawing(string name)
    {
        Debug.Log("Undrawing");
        GameObject operate_object = GameObject.Find(name);
        Destroy(operate_object);

        return;
    }
}
