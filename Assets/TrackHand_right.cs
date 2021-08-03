//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: For controlling in-game objects with tracked devices.
//
//=============================================================================

using UnityEngine;
using Valve.VR;

namespace Valve.VR
{
    public class TrackHand_right : MonoBehaviour
    {
        private void OnNewPoses(TrackedDevicePose_t[] poses)
        {
            var error = ETrackedPropertyError.TrackedProp_Success;
            var serialNumber = new System.Text.StringBuilder((int)64);
            for (int i = 0; i < 18; i++)
            {
                OpenVR.System.GetStringTrackedDeviceProperty((uint)i,ETrackedDeviceProperty.Prop_SerialNumber_String,serialNumber,64,ref error);
                if (serialNumber.ToString() == "LHR-5423DE85" || serialNumber.ToString() == "LHR-88A2CD57")
                {
                    if (poses[i].bPoseIsValid)
                    {
                        var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);
                        transform.localPosition = pose.pos;
                        transform.localRotation = pose.rot;
                    }
                }
            }
                        
        }

        SteamVR_Events.Action newPosesAction;

        TrackHand_right()
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
        }

    }
}