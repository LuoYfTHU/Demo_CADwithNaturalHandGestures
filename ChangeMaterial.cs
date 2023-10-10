using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    public List<GameObject> object_list; 

    private OVRHand[] hands;
    private OVRSkeleton[] skeletons;
    private IList<OVRBone> righthand_bones;
    private bool ring_not_pinching;
    int IndexTip_id = (int) OVRSkeleton.BoneId.Hand_IndexTip;

    private GestureRecognizer GestureDetector;

    public string selected_object_name = "none";
    public GameObject selected_object;
    private GameObject SignalSphere;
    private List<Material> materials;
    public int material_option;
    bool selected;

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

        ring_not_pinching = true;
        selected = false;

        GestureDetector = GameObject.Find("GestureDetector").GetComponent<GestureRecognizer>();

        object_list = GameObject.Find("DesignOperation").GetComponent<DrawObject>().object_list;

        // Material signal sphere
        SignalSphere = GameObject.Find("SignalSphere");
        materials = SignalSphere.GetComponent<MaterialList>().materials;
        material_option = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if(righthand_bones.Count == 0)
        {
            righthand_bones = skeletons[(int)OVRHand.Hand.HandRight].Bones;
        }

        if(hands[0].GetFingerIsPinching(OVRHand.HandFinger.Ring))
        {
            selected_object_name = SelectObject();

            // If selected material sample, change material option

            // If selected edit object, change object material 
            if(selected_object_name != "none" && !selected)
            {
                selected = true;
                if(selected_object_name == "SignalSphere")
                {
                    change_material_option();
                }
                else
                {
                    GameObject ob = GameObject.Find(selected_object_name);
                    ob.GetComponent<MeshRenderer>().material = materials[material_option];
                } 
            }

            if(selected_object_name == "none" && selected)
            {
                selected = false;
            }
        }

    }

    // Select object or material sample
    string SelectObject()
    {
        selected_object_name = "none";   
        Vector3 IndexTip_pos = righthand_bones[IndexTip_id].Transform.position;    
        Vector3 material_sample_pos = GameObject.Find("SignalSphere").transform.position;     
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

        if(Vector3.Distance(IndexTip_pos, material_sample_pos) < 0.05f)
        {
            return "SignalSphere";
        }

        return "none";
    }

    void change_material_option()
    {
        material_option = (material_option + 1) % materials.Count;
        SignalSphere.GetComponent<MeshRenderer>().material = materials[material_option];

        return;
    }
}
