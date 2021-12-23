# avatar-Master
For direct access in unity, click Scenes/MASTER.unity in unity 
## Used assests
### 1. KLAK NDI PLUGIN
* [specific tutorial](https://github.com/keijiro/KlakNDI)
* located [folder](https://github.com/DyrosLab/avatar-Master/tree/main/Assets/Klak/NDI)
* Used script: NDI Receiver
 ![reciever](https://user-images.githubusercontent.com/71639336/143839001-5fde2376-b642-4783-9bde-511c0e040d2f.PNG)
* copies the received frames into render texture asset(Right_texture & Left_texture)

### 2. Curved plane 
* [Non-free asset](https://assetstore.unity.com/packages/3d/curved-plane-87846)
* located [folder](https://github.com/DyrosLab/avatar-Master/tree/main/Assets/Modules/CurvedPlane/Scripts)
* Used script: CurvedPlane.cs
 <p>
 <img src="https://user-images.githubusercontent.com/71639336/143839365-d5858bba-2084-4e87-b9d2-460243ba0df2.PNG" width="20%" height="20%"/>
 <img src="https://user-images.githubusercontent.com/71639336/143839710-8543b3ce-6a0c-4a78-8bb7-e59fc55c1482.PNG" width="30%" height="30%"/>
 <img src="https://user-images.githubusercontent.com/71639336/143839345-47bcb7fc-2e22-40c2-959c-fb79961500b8.PNG" width="40%" height="40%"/>
 </p>
 
* located in screen component
* created new material(Lef_mat & Right_mat) -> for these materials, select texture(Right_texture & Left_texture)
* for curvedplane.cs, select custom material to (Lef_mat & Right_mat)

### 3. SteamVR asset
* asset [link](https://assetstore.unity.com/packages/tools/integration/steamvr-plugin-32647)
* modified script: [PY_eventbased.cs](https://github.com/DyrosLab/avatar-Master/blob/main/Assets/PY_eventbased.cs)
  * original script: [SteamVR_TrackedObject.cs](https://github.com/DyrosLab/avatar-Master/blob/main/Assets/SteamVR/Scripts/SteamVR_TrackedObject.cs)
  * For reducing VR sickness, this script used.
  * if more than 30 degrees of orientation is moved, screen's orientation will be updated in VR.
### 4. ROS# (beta)
* [specific tutorial](https://github.com/siemens/ros-sharp)
* Rosconnector [Tutorial](https://github.com/siemens/ros-sharp/wiki/User_App_Fibonacci_Action_Client)
* used Features
  1. publish tracker orientation data msgs
      * tutorial [link](https://github.com/siemens/ros-sharp/wiki/Dev_NewMessageTypes)
      * modified script: [VRPublisher_.cs](https://github.com/DyrosLab/avatar-Master/tree/main/Assets/RosSharp/Scripts/RosBridgeClient/RosCommuncation) (3x4matrix msg type geometry_msgs/pose available)
  3. subscribe head orientation data msgs + update to screen orientation
      * [PoseSubscriber.cs](https://github.com/DyrosLab/avatar-Master/blob/main/Assets/RosSharp/Scripts/RosBridgeClient/RosCommuncation/PoseSubscriber.cs) (This occurs too much VR sickness because of msgs latency, incorrect data transfer, and individual differences)
  5. subscribe jointstates + update to URDF
      * tocabi urdf is already imported in package
      * enable the tocabi component in the scene
![tocabi_urdf](https://user-images.githubusercontent.com/71639336/142817641-7d7cacb2-cd56-44ec-82dc-ae17b3e52264.jpg)

## How to run
### Video streaming sender part
1. Method #1: BIRDDOG FLEX 4K IN (device that current system using)

  <img src="https://user-images.githubusercontent.com/71639336/142955000-b3b9e4ed-8f10-4c04-8a1d-28c20f6a299e.PNG" width="30%" height="30%" align="center"/>

2. Method #2: OBS - NDI plugin
* you can use this method for testing the scene
* [link](https://github.com/Palakis/obs-ndi/releases) for obs - ndi plugin install
* if you installed obs-ndi plugin, you might see the ouput settings on obs studio tools tab.
![obs](https://user-images.githubusercontent.com/71639336/142955737-51662c23-2e94-4f13-b379-f388cbf1a291.PNG)
* you can manage the source name + find the source name at the unity project on NDI_left or NDI_left component.
![recieve](https://user-images.githubusercontent.com/71639336/142957214-ca936cce-d78d-461b-b056-f5bcd9cbb8bb.PNG)

### Ros communication
Need two computers to transfer ros msgs.
1. Ubuntu computer ros part
* Run rosbridge + need rosbridge package
```
sudo apt install ros-<rosdistro>-rosbridge-suite
source /opt/ros/<rosdistro>/setup.bash
roslaunch rosbridge_server rosbridge_websocket.launch address:=<your com's ip address>
```
* To subscribe geometry msg, need to install geometry/pose ros msgs
2. Unity ros part 
![1](https://user-images.githubusercontent.com/71639336/147193806-08a96d71-ca99-4fd9-bea7-b26b4c897a48.png)
* Edit ros bridge server url to ubuntu ros com ip address(what you wrote for the roslaunch previously)
* If you only have HMD, just select one script of VR Publisher_pose which index is Hmd.
![2](https://user-images.githubusercontent.com/71639336/147193820-a846dc52-185b-4b69-8b2e-23747a9935af.PNG)
* If you have trackers, edit the script to publish msgs for trackers.
* Trackers have individual IDs, you can find it from the box you have purchased.
* edit the script like the upper image.
