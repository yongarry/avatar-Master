//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: For controlling in-game objects with tracked devices.
//
//=============================================================================

using UnityEngine;
using Valve.VR;

namespace Valve.VR
{
    public class TrackedObject_target_modified : MonoBehaviour
    {
        public Transform targetTransformation;
        public enum EIndex
        {
                        None = -1,
            Hmd = (int)OpenVR.k_unTrackedDeviceIndex_Hmd,
            UNDEFINED,
            BASESTATION1,
            BASESTATION2,
            BASESTATION3,
            BASESTATION4,
            TRACKER0,
            TRACKER1,
            TRACKER2,
            TRACKER3,
            TRACKER4,
            TRACKER5,
            TRACKER00,
            TRACKER11,
            TRACKER22,
            TRACKER33,
            TRACKER44,
            TRACKER55
        }

        public EIndex index;

        [Tooltip("If not set, relative to parent")]
        public Transform origin;
        public bool isValid { get; private set; }

        private void OnNewPoses(TrackedDevicePose_t[] poses)
        {
            if (index == EIndex.None)
                return;

            var i = (int)index;

            isValid = false;
            if (poses.Length <= i)
            {
                Debug.Log("length");
                return;
            }
            if (!poses[i].bDeviceIsConnected)
            {
                Debug.Log("not connected");
                return;
            }
            if (!poses[i].bPoseIsValid)
            {
                var error = ETrackedPropertyError.TrackedProp_Success;
                var serialNumber = new System.Text.StringBuilder((int)64);
                OpenVR.System.GetStringTrackedDeviceProperty((uint)i,ETrackedDeviceProperty.Prop_SerialNumber_String,serialNumber,64,ref error);
                Debug.Log("pose not valid" + serialNumber);
                return;
            }
            isValid = true;

            var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);

            if (origin != null)
            {
                // transform.position = origin.transform.TransformPoint(pose.pos);
                // transform.rotation = origin.rotation * pose.rot;

                targetTransformation.position = origin.transform.TransformPoint(pose.pos);
                targetTransformation.rotation = origin.rotation * pose.rot;
            }
            else
            {
                // transform.localPosition = pose.pos;
                // transform.localRotation = pose.rot;
                var rotat = pose.rot.eulerAngles;
                // Debug.Log(rotat.x);
                if(rotat.x > 45 || rotat.x <-45)
                {
                    targetTransformation.localPosition = pose.pos;
                    targetTransformation.localRotation = pose.rot;
                }
                else
                {
                    pose.rot.x = 0; pose.rot.y = 0; pose.rot.z = 0; pose.rot.w = 0;
                    targetTransformation.localPosition = pose.pos;
                    targetTransformation.localRotation = pose.rot;
                }
            }
        }

        SteamVR_Events.Action newPosesAction;

        TrackedObject_target_modified()
        {
            newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
        }

        private void Awake()
        {
            OnEnable();
        }

        void OnEnable()
        {
            var render = SteamVR_Render.instance;
            if (render == null)
            {
                enabled = false;
                return;
            }

            newPosesAction.enabled = true;
        }

        void OnDisable()
        {
            newPosesAction.enabled = false;
            isValid = false;
        }

        public void SetDeviceIndex(int index)
        {
            if (System.Enum.IsDefined(typeof(EIndex), index))
                this.index = (EIndex)index;
        }
    }
}