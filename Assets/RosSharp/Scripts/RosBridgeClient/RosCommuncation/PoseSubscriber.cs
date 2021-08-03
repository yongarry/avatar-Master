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

namespace RosSharp.RosBridgeClient
{
    public class PoseSubscriber : UnitySubscriber<MessageTypes.Geometry.Pose>
    {
        public Transform SubscribedTransform;

        private Vector3 position;
        private Quaternion rotation;
        private bool isMessageReceived;
        public enum TrackingType
        {
            /// <summary>
            /// With this setting, both the pose's rotation and position will be applied to the parent transform
            /// </summary>
            RotationAndPosition,
            /// <summary>
            /// With this setting, only the pose's rotation will be applied to the parent transform
            /// </summary>
            RotationOnly,
            /// <summary>
            /// With this setting, only the pose's position will be applied to the parent transform
            /// </summary>
            PositionOnly
        }

        [SerializeField]
        TrackingType m_TrackingType;
        /// <summary>
        /// The tracking type being used by the tracked pose driver
        /// </summary>
        public TrackingType trackingType
        {
            get { return m_TrackingType; }
            set { m_TrackingType = value; }
        }
        protected override void Start()
		{
			base.Start();
		}
		
        private void Update()
        {
            if (isMessageReceived)
                ProcessMessage();
        }
        protected override void ReceiveMessage(MessageTypes.Geometry.Pose message)
        {
            position = GetPosition(message).Ros2Unity();
            rotation = GetRotation(message).Ros2Unity();
            isMessageReceived = true;
        }
        private void ProcessMessage()
        {
            // SubscribedTransform.position = position;
            // SubscribedTransform.rotation = rotation;
            SetLocalTransform(position,rotation);
        }

        private Vector3 GetPosition(MessageTypes.Geometry.Pose message)
        {
            return new Vector3(
                (float)message.position.x,
                (float)message.position.y,
                (float)message.position.z);
        }

        private Quaternion GetRotation(MessageTypes.Geometry.Pose message)
        {
            return new Quaternion(
                (float)message.orientation.x,
                (float)message.orientation.y,
                (float)message.orientation.z,
                (float)message.orientation.w);
        }

        protected virtual void SetLocalTransform(Vector3 newPosition, Quaternion newRotation)
        {
            if ((m_TrackingType == TrackingType.RotationAndPosition ||
                m_TrackingType == TrackingType.RotationOnly))
            {
                SubscribedTransform.rotation = newRotation;
            }

            if ((m_TrackingType == TrackingType.RotationAndPosition ||
                m_TrackingType == TrackingType.PositionOnly))
            {
                SubscribedTransform.position = newPosition;
            }
        }
    }
}   