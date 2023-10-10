using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
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
            }
            else
            {

            }
            
        }
    }

}
