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
    public class TrackerStatus : UnityPublisher<MessageTypes.Std.Bool>
    {
        private MessageTypes.Std.Bool message;
        private TrackedDevicePose_t[] poses;
        private int trackerNum;
        protected override void Start()
		{
			base.Start();
            InitializeMessage();
		}
        private void InitializeMessage()
        {
            message = new MessageTypes.Std.Bool();
            
            poses = new TrackedDevicePose_t[18];

            trackerNum = 0;
        }
        private void UpdateMessage(TrackedDevicePose_t[] poses)
        {
            trackerNum = 0;
            var error = ETrackedPropertyError.TrackedProp_Success;
            var serialNumber = new System.Text.StringBuilder((int)64);
            for (int i = 0; i < 18; i++)
            {
                var index = (uint)i;
                OpenVR.System.GetStringTrackedDeviceProperty(index,ETrackedDeviceProperty.Prop_SerialNumber_String,serialNumber,64,ref error);

                if (serialNumber.ToString() == "LHR-B979AA9E" || serialNumber.ToString() == "LHR-5567029A")
                {
                    if (poses[i].bPoseIsValid)
                    {
                        trackerNum++;
                    }
                }
                else if (serialNumber.ToString() == "LHR-3F2A7A7B" || serialNumber.ToString() == "LHR-D74F7D1A")
                {
                    if (poses[i].bPoseIsValid)
                    {
                        trackerNum++;
                    }
                }
                else if (serialNumber.ToString() == "LHR-7330E069" || serialNumber.ToString() == "LHR-78CF9EE8")
                {
                    if (poses[i].bPoseIsValid)
                    {
                        trackerNum++;
                    }
                }
                else if (serialNumber.ToString() == "LHR-8C0A4142" || serialNumber.ToString() == "LHR-CA171B68")
                {
                    if (poses[i].bPoseIsValid)
                    {
                        trackerNum++;
                    }
                }
                else if (serialNumber.ToString() == "LHR-3C32FE4B" || serialNumber.ToString() == "LHR-172B3493")
                {
                    if (poses[i].bPoseIsValid)
                    {
                        trackerNum++;
                    }
                }
                else if (serialNumber.ToString() == "LHR-5423DE85" || serialNumber.ToString() == "LHR-88A2CD57")
                {
                    if (poses[i].bPoseIsValid)
                    {
                        trackerNum++;
                    }
                }
                
            }
            if (trackerNum == 6)
            {   
                message.data = true;
                Publish(message);
                trackerNum = 0;
            }
            else
            {
                message.data = false;
                Publish(message);
            }   
        }
        SteamVR_Events.Action newPosesAction;

        TrackerStatus()
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
    }
}
