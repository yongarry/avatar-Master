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
    public class VRPublisher : UnityPublisher<MessageTypes.VR.matrix_3_4>
    {
        private MessageTypes.VR.matrix_3_4 message;
        public Transform PublishedTransform;
        private Vector3 position;
        private Vector3 rotation;
        private Quaternion quat;
        public TrackedDevicePose_t[] poses;
        protected override void Start()
		{
			base.Start();
            InitializeMessage();
		}
		
        private void FixedUpdate()
        {
            UpdateMessage();
            Debug.Log("matrix:"+poses[0].mDeviceToAbsoluteTracking.m0);
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.VR.matrix_3_4()
            {
                firstRow = new double[4],
                secondRow = new double[4],
                thirdRow = new double[4],
            };
        }

        private void UpdateMessage()
        {
            Vector3 position = PublishedTransform.localPosition;
            Quaternion quat = PublishedTransform.localRotation;
            Vector3 rotation = PublishedTransform.localRotation.eulerAngles;
                    
            UdatePosef(position.Unity2Ros(), rotation.Unity2Ros());
            UdatePoses(position.Unity2Ros(), rotation.Unity2Ros());
            UdatePoset(position.Unity2Ros(), rotation.Unity2Ros());

            Publish(message);
        }
        
        private void UdatePosef (Vector3 pos, Vector3 rot)
        {
            double rotx=(double)((rot.x)*(Math.PI)) / 180;
            double roty=(double)((rot.y)*(Math.PI)) / 180;
            double rotz=(double)((rot.z)*(Math.PI)) / 180;

            message.firstRow[0]=(double)(Math.Cos(rotz)*Math.Cos(roty));
            message.firstRow[1]=(double)(Math.Cos(rotz)*Math.Sin(roty)*Math.Sin(rotx) - Math.Sin(rotz)*Math.Cos(rotx));
            message.firstRow[2]=(double)(Math.Cos(rotz)*Math.Sin(roty)*Math.Cos(rotx) + Math.Sin(rotz)*Math.Sin(rotx));
            message.firstRow[3]=(double)pos.x;
        }

        private void UdatePoses (Vector3 pos, Vector3 rot)
        {
            double rotx=(double)((rot.x)*(Math.PI)) / 180;
            double roty=(double)((rot.y)*(Math.PI)) / 180;
            double rotz=(double)((rot.z)*(Math.PI)) / 180;

            message.secondRow[0]=(double)(Math.Sin(rotz)*Math.Cos(roty));
            message.secondRow[1]=(double)(Math.Sin(rotz)*Math.Sin(roty)*Math.Sin(rotx) + Math.Cos(rotz)*Math.Cos(rotx));
            message.secondRow[2]=(Math.Sin(rotz)*Math.Sin(roty)*Math.Cos(rotx) - Math.Cos(rotz)*Math.Sin(rotx));
            message.secondRow[3]=(double)pos.y;
        }

        private void UdatePoset (Vector3 pos, Vector3 rot)
        {
            double rotx=(double)((rot.x)*(Math.PI)) / 180;
            double roty=(double)((rot.y)*(Math.PI)) / 180;
            double rotz=(double)((rot.z)*(Math.PI)) / 180;

            message.thirdRow[0]=(double)(-Math.Sin(roty));
            message.thirdRow[1]=(double)(Math.Cos(roty)*Math.Sin(rotx));
            message.thirdRow[2]=(double)(Math.Cos(roty)*Math.Cos(rotx));
            message.thirdRow[3]=(double)pos.z;
        }

    }
    
}
