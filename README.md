## Welcome to Sumo2Unity Tool

## Introduction
We observed traffic safety researchers invested considerable time, effort, and money in developing a co-simulation tool that integrated Traffic Simulation SUMO (Simulation of Urban MObility) and Unity Game Engine. Each study ‘re-invented the wheel’ and therefore sacrificed resources that could have been used to focus on the ultimate goal, i.e., improving safety. Sumo2Unity tool integrates Traffic Simulation SUMO (Simulation of Urban MObility) with Unity Game Engine. SUMO2Unity is an open-source project contains the following items: 
1.	It automatically import complex road network from SUMO to Unity;
2.	Integrate SUMO and Unity. This included programming the data exchange between the trajectory coordinates (X, Y, Z) of the vehicles and the signal timing (phase and duration) every 0.10 seconds; and 
3.	Develop performance functions to evaluate the performance of the integration.

Check out the quick 2 minute demo

[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/9nSCKIz6lQI/0.jpg)](https://www.youtube.com/watch?v=9nSCKIz6lQI)

## Pre-Requisites
- Download latest version of [Sumo](https://eclipse.dev/sumo/). We recommend version 1.18 or 1.19.
- Download [Unity Hub](https://unity.com/download).
- Once you install Unity Hub, download and install Unity Editor. We recommend version 2022.3.16f1.

## Getting Started
-SUMO - Step by Step Tutorial [Youtube Tutorial](https://www.youtube.com/playlist?list=PLAk8GOoajG6tKI74YID0hwjXVg8KBxNAD)
1. Install SUMO (Version 1.18 or 1.19)
2. Set Up SUMO Environment Variables
3. Install Notepad ++
4. Watch SUMO Tutorial
   
-Unity - Step by Step Tutorial [Youtube Tutorial](https://www.youtube.com/playlist?list=PLAk8GOoajG6veF2wQ_CJJpQXNDufEQuoA)
1. Install Unity HUB
2. Install Unity Editor. We recommend version 2022.3.16f1.
3. Install Visual Studio
4. Install Visual Studio Dependencies
5. Watch Unity VR Tutorial  
   
-SUMO2Unity - Step by Step Tutorial [Youtube Tutorial](https://www.youtube.com/playlist?list=PLAk8GOoajG6uIKpzW0R9aQ9mWXRY77azk)
1. Download this repository as a zip file.
2. Extract the zip file.
3. Add the extract file in Unity Hub and open it with version 2022.3.16f1.
4. In the editor, go to `_Project/Scenes` and open SUMO2Unity scene.
5. Watch SUMO2Unity Tutorial

## Usage
- To change the simulation settings, go to `Assets/_Project/Sumo_Data`.
- Open Sumo2Unity file to see the existing simulation.
- To change simulation, click on `Edit > Open network in netedit` or press `ctrl + T`.
- You can add or modify vehicles and traffic signals in the Simulation.

## Additional Help
- If you need more help or have any questions, feel free to create a new issue at the [Issues](https://github.com/SUMO2Unity/SUMO2Unity/issues) section. 

## Paper
If you use SUMO2Unity, please cite our paper.

Mohammadi, A., Park, P. Y., Nourinejad, M., Cherakkatil, M. S. B., & Park, H. S. (2024, June). SUMO2Unity: An Open-Source Traffic Co-Simulation Tool to Improve Road Safety. In 2024 IEEE Intelligent Vehicles Symposium (IV) (pp. 2523-2528). IEEE.

## License
- SUMO2Unity codes are distributed under MIT License.
- SUMO2Unity assets are distributed under CC-BY License.

The following assets are developed by Unity and are free to use for commercial and non-commercial use. The assets are licensed by the standard Unity Asset Store EULA (https://unity.com/legal/as-terms).
1. Automotive HMI Template
2. TerrainDemoScene_URP
3. UnityStandardAssets
4. Viking Village

