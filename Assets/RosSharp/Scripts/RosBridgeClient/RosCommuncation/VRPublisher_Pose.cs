/*
© Siemens AG, 2017-2018
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;
using System;
using Valve.VR;

namespace RosSharp.RosBridgeClient
{
    public class VRPublisher_Pose : UnityPublisher<MessageTypes.Geometry.Pose>
    {
        public enum EIndex
        {
            None = -1,
            Hmd = (int)OpenVR.k_unTrackedDeviceIndex_Hmd,
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
        private MessageTypes.Geometry.Pose message;
        public TrackedDevicePose_t[] poses;
        private Vector3[] po;
        private Quaternion[] quat;
        public bool isValid { get; private set; }
        protected override void Start()
		{
			base.Start();
            InitializeMessage();
		}
        private void InitializeMessage()
        {
            message = new MessageTypes.Geometry.Pose
            {
                position = new MessageTypes.Geometry.Point(),
                orientation = new MessageTypes.Geometry.Quaternion()
            };
            poses = new TrackedDevicePose_t[17];
            po = new Vector3[17];
            quat = new Quaternion[17];
        }
        private void UpdateMessage(TrackedDevicePose_t[] poses)
        {
            if (index == EIndex.None)
                return;

            var i = (uint)index;

            isValid = false;
            if (poses.Length <= i)
                return;

            if (!poses[i].bDeviceIsConnected)
                return;

            if (!poses[i].bPoseIsValid)
                return;

            isValid = true;
            
            float fPredictedSecondsFromNow = GetPredictedSecondsFromNow();
            /*  
            for seated position     : ETrackingUniverseOrigin.TrackingUniverseSeated
            for standing position   : ETrackingUniverseOrigin.TrackingUniverseStanding
            */
            OpenVR.System.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseSeated,fPredictedSecondsFromNow ,poses);
            
            var pose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);
                     
            po[i] = pose.pos;
            quat[i] = pose.rot;

            // Write message (Geometry/Pose)            
            message = GetPose(po[i].Unity2Ros(), quat[i].Unity2Ros());

            // Check serial numbers for trackers
            var error = ETrackedPropertyError.TrackedProp_Success;
            var serialNumber = new System.Text.StringBuilder((int)64);
            OpenVR.System.GetStringTrackedDeviceProperty(i,ETrackedDeviceProperty.Prop_SerialNumber_String,serialNumber,64,ref error);

            if (serialNumber.ToString() == "LHR-B979AA9E" || serialNumber.ToString() == "LHR-5567029A")
            {                
                Topic = "/TRACKER0";
                Publish(message);
            }
            else if (serialNumber.ToString() == "LHR-3F2A7A7B" || serialNumber.ToString() == "LHR-D74F7D1A")
            {                
                Topic = "/TRACKER1";
                Publish(message);
            }
            else if (serialNumber.ToString() == "LHR-7330E069" || serialNumber.ToString() == "LHR-78CF9EE8")
            {                
                Topic = "/TRACKER2";
                Publish(message);
            }
            else if (serialNumber.ToString() == "LHR-8C0A4142" || serialNumber.ToString() == "LHR-CA171B68")
            {                
                Topic = "/TRACKER3";
                Publish(message);
            }
            else if (serialNumber.ToString() == "LHR-3C32FE4B" || serialNumber.ToString() == "LHR-172B3493")
            {                
                Topic = "/TRACKER4";
                Publish(message);
            }
            else if (serialNumber.ToString() == "LHR-5423DE85" || serialNumber.ToString() == "LHR-88A2CD57")
            {    
                Topic = "/TRACKER5";
                Publish(message);
            }
            else
            {
                Publish(message);
            }
        }
        private static MessageTypes.Geometry.Pose GetPose(Vector3 vector3,Quaternion quat)
        {
            MessageTypes.Geometry.Pose pose = new MessageTypes.Geometry.Pose();
            pose.position.x = vector3.x;
            pose.position.y = vector3.y;
            pose.position.z = vector3.z; 
            pose.orientation.x = quat.x;
            pose.orientation.y = quat.y;
            pose.orientation.z = quat.z;
            pose.orientation.w = quat.w;

            return pose;
        }
        private float GetPredictedSecondsFromNow()
        {
            float fSecondsSinceLastVsync = 0;
            ulong frameCounter = 0;
            OpenVR.System.GetTimeSinceLastVsync( ref fSecondsSinceLastVsync, ref frameCounter );
 
            ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;
            float fDisplayFrequency = OpenVR.System.GetFloatTrackedDeviceProperty(0, ETrackedDeviceProperty.Prop_DisplayFrequency_Float, ref error );
            float fFrameDuration = 1.0f / fDisplayFrequency;
            float fVsyncToPhotons = OpenVR.System.GetFloatTrackedDeviceProperty(0, ETrackedDeviceProperty.Prop_SecondsFromVsyncToPhotons_Float, ref error );
    
            float fPredictedSecondsFromNow = fFrameDuration - fSecondsSinceLastVsync + fVsyncToPhotons;
 
            return fPredictedSecondsFromNow;
        }
        SteamVR_Events.Action newPosesAction;

        VRPublisher_Pose()
        {
            newPosesAction = SteamVR_Events.NewPosesAction(UpdateMessage);
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
        public void SetDeviceIndex(int index)
        {
            if (System.Enum.IsDefined(typeof(EIndex), index))
                this.index = (EIndex)index;
        }
    }
}
