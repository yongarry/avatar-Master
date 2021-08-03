/*
© Siemens AG, 2017-2019
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
using System.Collections.Generic;

namespace RosSharp.RosBridgeClient
{
    public class JointStateSubscriber_modified : UnitySubscriber<MessageTypes.Sensor.JointState>
    {
        public Transform SubscribedTransform;
        private Vector3 rotation;
        private bool isMessageReceived;

        protected override void Start()
		{
			base.Start();
		}
		
        private void Update()
        {
            if (isMessageReceived)
                ProcessMessage();
        }
        protected override void ReceiveMessage(MessageTypes.Sensor.JointState message)
        {
            rotation = GetRotation(message);
            isMessageReceived = true;
        }
        private void ProcessMessage()
        {
            SubscribedTransform.eulerAngles = rotation;
        }

        private Vector3 GetRotation(MessageTypes.Sensor.JointState message)
        {
            return new Vector3(
                (float)(message.position[24]/Math.PI*180),
                (float)(-message.position[23]/Math.PI*180),
                0);
        }
    }
}

