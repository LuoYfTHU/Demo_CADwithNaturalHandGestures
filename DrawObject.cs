using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawObject : MonoBehaviour
{
    bool DeskTriggered = false;
    bool drawing = false;
    bool vor_drawing = false;
    // Hand gesture code
    int HandGesture_id = 0;
    private OVRHand[] hands;
    private OVRSkeleton[] skeletons;
    public IList<OVRBone> righthand_bones;
    private GestureRecognizer GestureDetector;
    int IndexTip_id = (int)OVRSkeleton.BoneId.Hand_IndexTip;
    string draw_option = "cube";

    // Object serial number
    int object_id = 0;
    int current_object_id = 0;

    public List<GameObject> object_list = new List<GameObject>();

    Vector3 start_position = new Vector3(0,0,0);
    Vector3 defaut_size = new Vector3((float)0.05, (float)0.05, (float)0.05);
    float defaut_length = 0.05f;

    GameObject signal_cube;
    GameObject signal_cylinder;
    Renderer signal_cube_renderer;
    Renderer signal_cylinder_renderer;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        // Wait for update: break the if session
        if (hands[1].IsTracked) {
        righthand_bones = skeletons[(int)OVRHand.Hand.HandRight].Bones;
        // Get hand gesture and audio direction
       // HandGesture_id = GestureDetector.RecognizeGesture();
        //GetAudioMes();

        // Get draw option
        if(hands[0].GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            draw_option = "Cylinder";
            signal_cube = GameObject.Find("SignalCube");
            signal_cube.GetComponent<Renderer>().material.color = Color.white;

            signal_cylinder = GameObject.Find("SignalCylinder");
            signal_cylinder.GetComponent<Renderer>().material.color = Color.red;
        }
        if(hands[0].GetFingerIsPinching(OVRHand.HandFinger.Middle))
        {
            draw_option = "cube";
            signal_cube = GameObject.Find("SignalCube");
            signal_cube.GetComponent<Renderer>().material.color = Color.red;

            signal_cylinder = GameObject.Find("SignalCylinder");
            signal_cylinder.GetComponent<Renderer>().material.color = Color.white;
        }

        // Detect indication of drawing
        Vector3 IndexTip_pos = righthand_bones[IndexTip_id].Transform.position;

        // (Wait for updating) change to switch
        // Start drawing
        if (DeskTouched(IndexTip_pos) && !drawing) // (Wait for updating) && pointer hand gesture
        {
            drawing = true;
            start_position = TouchPosition();
            Drawing(draw_option, start_position, start_position + defaut_size);
            object_id += 1;
        }
        else
        {
            bool inprocess_draw = DeskTouched(IndexTip_pos) && drawing;
            if (inprocess_draw)
            {
                Vector3 update_position = TouchPosition();
               // if(Vector3.Distance(start_position, update_position) > 0.005)
                {
                    ChangeScale(current_object_id.ToString(), start_position, update_position);
                }  
            }
        }

        // Detect confirmation of completion of drawing
        if (!DeskTouched(IndexTip_pos) && drawing)
        {
            drawing = false;
            current_object_id += 1;
        }
        }
    }

    // When finger touched the desk, return true
    bool DeskTouched(Vector3 FingertipPosition)
    {
        bool desk_touched = false;
        float x = FingertipPosition[0];
        float y = FingertipPosition[1];
        float z = FingertipPosition[2];

        desk_touched = y >= 0.7254 && y <= 0.7719 && x >= -2 && x <= 0 && z >= -0.8 && z <= 0.2;

        return desk_touched;
    }

    // Return finger touched position 
    Vector3 TouchPosition()
    {
        return righthand_bones[IndexTip_id].Transform.position;
    }

    // Draw a primitive object
    // Parameters:
    void Drawing(string draw_option, Vector3 start_pos, Vector3 end_pos)
    {
        if (draw_option == "cube")
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3((start_pos.x + end_pos.x) / 2, 0.7533f + defaut_length / 2, (start_pos.z + end_pos.z) / 2);
            cube.transform.localScale = defaut_size;
            cube.transform.localScale = new Vector3(Vector3.Distance(start_pos, end_pos), defaut_length, Vector3.Distance(start_pos, end_pos));           

            cube.name = object_id.ToString();
            cube.SetActive(true);
            cube.AddComponent<Rigidbody>().isKinematic = true;
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.enabled = true;
            cubeRenderer.material.SetColor("_Color", Color.grey);

            object_list.Add(cube);
        }

        if (draw_option == "Cylinder")
        {
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.transform.position = new Vector3((start_pos.x + end_pos.x) / 2, 0.7533f + defaut_length, (start_pos.z + end_pos.z) / 2);
            cylinder.transform.position = (start_pos + end_pos) / 2;
            cylinder.transform.localScale = new Vector3(Vector3.Distance(start_pos, end_pos), defaut_length / 2, Vector3.Distance(start_pos, end_pos));

            cylinder.name = object_id.ToString();
            cylinder.SetActive(true);
            cylinder.AddComponent<Rigidbody>().isKinematic = true;
            //cylinder.AddComponent<Collider>().enabled = true;     
            var cylinderRenderer = cylinder.GetComponent<Renderer>();
            cylinderRenderer.enabled = true;
            cylinderRenderer.material.SetColor("_Color", Color.grey);

            object_list.Add(cylinder);
        }

        return;
    }

    void ChangeScale(string name, Vector3 start_position, Vector3 end_position)
    {
        GameObject operate_object = GameObject.Find(name);
        operate_object.transform.localScale = new Vector3(Vector3.Distance(start_position, end_position), defaut_length, Vector3.Distance(start_position, end_position));

        return;
    }
}
