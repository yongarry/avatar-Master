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
    public class VRPublisher_3x4matrix : UnityPublisher<MessageTypes.VR.matrix_3_4>
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
            Device11,
            Device12,
            Device13,
            Device14,
            Device15,
            Device16
        }
        public EIndex index;
        private MessageTypes.VR.matrix_3_4 message;
        private TrackedDevicePose_t[] poses;
        public bool isValid { get; private set; }
        //public string[] serialNumber = new string[17];
        protected override void Start()
		{
			base.Start();
            InitializeMessage();
		}
        private void InitializeMessage()
        {
            message = new MessageTypes.VR.matrix_3_4()
            {
                firstRow = new double[4],
                secondRow = new double[4],
                thirdRow = new double[4],
            };
            
            poses = new TrackedDevicePose_t[17];
            //poses[0].mDeviceToAbsoluteTracking = new HmdMatrix34_t();
            //string[] serialNumber = new string[17];
            
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
            OpenVR.System.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseSeated,fPredictedSecondsFromNow ,poses);
            //var pose = new SteamVR_Utils.RigidTransform(poses[0].mDeviceToAbsoluteTracking);
            message.firstRow[0]=(double)poses[i].mDeviceToAbsoluteTracking.m10;
            message.firstRow[1]=(double)poses[i].mDeviceToAbsoluteTracking.m8;
            message.firstRow[2]=-(double)poses[i].mDeviceToAbsoluteTracking.m9;
            message.firstRow[3]=-(double)poses[i].mDeviceToAbsoluteTracking.m11;

            message.secondRow[0]=(double)poses[i].mDeviceToAbsoluteTracking.m2;
            message.secondRow[1]=(double)poses[i].mDeviceToAbsoluteTracking.m0;
            message.secondRow[2]=-(double)poses[i].mDeviceToAbsoluteTracking.m1;
            message.secondRow[3]=-(double)poses[i].mDeviceToAbsoluteTracking.m3;

            message.thirdRow[0]=-(double)poses[i].mDeviceToAbsoluteTracking.m6;
            message.thirdRow[1]=-(double)poses[i].mDeviceToAbsoluteTracking.m4;
            message.thirdRow[2]=(double)poses[i].mDeviceToAbsoluteTracking.m5;
            message.thirdRow[3]=(double)poses[i].mDeviceToAbsoluteTracking.m7;

            var error = ETrackedPropertyError.TrackedProp_Success;
            var serialNumber = new System.Text.StringBuilder((int)64);
            OpenVR.System.GetStringTrackedDeviceProperty(i,ETrackedDeviceProperty.Prop_SerialNumber_String,serialNumber,64,ref error);

            if (serialNumber.ToString() == "LHR-B979AA9E" || serialNumber.ToString() == "LHR-5567029A")
            {
                //Debug.Log(serialNumber.ToString());
                Topic = "/TRACKER0";
                Publish(message);
            }
            else if (serialNumber.ToString() == "LHR-3F2A7A7B" || serialNumber.ToString() == "LHR-D74F7D1A")
            {
                //Debug.Log(serialNumber.ToString());
                Topic = "/TRACKER1";
                Publish(message);
            }
            else if (serialNumber.ToString() == "LHR-7330E069" || serialNumber.ToString() == "LHR-78CF9EE8")
            {
                //Debug.Log(serialNumber.ToString());
                Topic = "/TRACKER2";
                Publish(message);
            }
            else if (serialNumber.ToString() == "LHR-8C0A4142" || serialNumber.ToString() == "LHR-CA171B68")
            {
                //Debug.Log(serialNumber.ToString());
                Topic = "/TRACKER3";
                Publish(message);
            }
            else if (serialNumber.ToString() == "LHR-3C32FE4B" || serialNumber.ToString() == "LHR-172B3493")
            {
                //Debug.Log(serialNumber.ToString());
                Topic = "/TRACKER4";
                Publish(message);
            }
            else if (serialNumber.ToString() == "LHR-5423DE85" || serialNumber.ToString() == "LHR-88A2CD57")
            {
                //Debug.Log(serialNumber.ToString());
                Topic = "/TRACKER5";
                Publish(message);
            }
            else
            {
                //Debug.Log(serialNumber.ToString());
                Publish(message);
            }
            
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

        VRPublisher_3x4matrix()
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
