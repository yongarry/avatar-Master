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

namespace RosSharp.RosBridgeClient
{
    public class PosePublisher : UnityPublisher<MessageTypes.Geometry.Pose>
    {
        private MessageTypes.Geometry.Pose message;
        public Transform PublishedTransform;
        private Vector3 position;
        private Quaternion rotation;

        protected override void Start()
		{
			base.Start();
            InitializeMessage();
		}
		
        private void FixedUpdate()
        {
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Geometry.Pose
            {
                position = new MessageTypes.Geometry.Point(),
                orientation = new MessageTypes.Geometry.Quaternion()
            };
        }

        private void UpdateMessage()
        {
            Vector3 position = PublishedTransform.localPosition;
            Quaternion rotation = PublishedTransform.localRotation;
            
            message = GetPose(position.Unity2Ros(), rotation.Unity2Ros());
            
            Publish(message);
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

    }
}