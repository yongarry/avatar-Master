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
    public class OdometryPublisher : UnityPublisher<MessageTypes.Nav.Odometry>
    {
        private MessageTypes.Nav.Odometry message;
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
            message = new MessageTypes.Nav.Odometry();
            message.pose = new MessageTypes.Geometry.PoseWithCovariance();
            // message.pose.pose.orientation = new MessageTypes.Nav.Odometry.Quaternion();
        }

        private void UpdateMessage()
        {
            Vector3 position = PublishedTransform.localPosition;
            Quaternion rotation = PublishedTransform.localRotation;
            
            message.pose = GetPose(position.Unity2Ros(), rotation.Unity2Ros());
            
            Publish(message);
        }
        private static MessageTypes.Geometry.PoseWithCovariance GetPose(Vector3 vector3,Quaternion quat)
        {
            MessageTypes.Geometry.PoseWithCovariance pose = new MessageTypes.Geometry.PoseWithCovariance();
            pose.pose.position.x = vector3.x;
            pose.pose.position.y = vector3.y;
            pose.pose.position.z = vector3.z; 
            pose.pose.orientation.x = quat.x;
            pose.pose.orientation.y = quat.y;
            pose.pose.orientation.z = quat.z;
            pose.pose.orientation.w = quat.w;

            return pose;
        }

    }
}