using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace STLExamples
{
    public class ExportSTL : MonoBehaviour
    {
        public List<GameObject> object_list;
        private GameObject[] _objects;    

        private OVRHand[] hands;
        private OVRSkeleton[] skeletons;
        private IList<OVRBone> righthand_bones;
        int IndexTip_id = (int) OVRSkeleton.BoneId.Hand_IndexTip;

        private GestureRecognizer GestureDetector;

        public string selected_object_name = "none";
        public GameObject selected_object;
        bool exporting = false;
        Quaternion rotation_diff;

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

            _objects = object_list.ToArray();
	
		}

        void Update()
        {
            if(righthand_bones.Count == 0)
            {
                righthand_bones = skeletons[(int)OVRHand.Hand.HandRight].Bones;
            }

            if(exporting && !hands[1].GetFingerIsPinching(OVRHand.HandFinger.Pinky))
            {
                exporting = false;
            }
            
            if(hands[0].GetFingerIsPinching(OVRHand.HandFinger.Middle) && !exporting)
            {
                exporting = true;
                _objects = object_list.ToArray();
                ExportToBinarySTL();
			    ExportToTextSTL();
            }
        }
		
		public void ExportToBinarySTL()
		{
			string filePath = DefaultDirectory() + "/stl_example_binary.stl";
			bool success = STL.Export( _objects, filePath );
			if( success ){
				Debug.Log( "Exported " + " objects to binary STL file." + System.Environment.NewLine + filePath );
			}
		}


		public void ExportToTextSTL()
		{      
			string filePath = DefaultDirectory() + "/stl_example_text.stl";
			bool asASCII = true;
			bool success = STL.Export( _objects, filePath, asASCII );
			if( success ){
				Debug.Log( "Exported " + " objects to text based STL file." + System.Environment.NewLine + filePath );
			}
		}
		
		
		static string DefaultDirectory()
		{
			string defaultDirectory = "";
			if( Application.platform == RuntimePlatform.OSXEditor ){
				defaultDirectory = System.Environment.GetEnvironmentVariable( "UnityHome" ) + "/Export";
			} else {
				//defaultDirectory = System.Environment.GetFolderPath( System.Environment.SpecialFolder.Desktop );
				//defaultDirectory = System.Environment.GetEnvironmentVariable( "UnityHome" ) + "/Export";
				defaultDirectory = AssetDatabase.GetSubFolders("Assets/STL")[3];
			}
			return defaultDirectory;
		}
    }
}