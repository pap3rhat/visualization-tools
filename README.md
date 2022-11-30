[![Made with Unity](https://img.shields.io/badge/Made%20with-Unity-57b9d3.svg?style=for-the-badge&logo=unity)](https://unity3d.com) 
<br />

<a name="readme-top"></a>
<h1 align="center">visu-tools</h1>
  <p align="center">
    Unity package that allows you to apply fullscreen post-processing effects to your project in real time.
  </p>
</div>


<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li>
    <a href="#usage">Usage</a>
    <ul> <li>
    <a href="#visu-tools-project">Visu-tools project</a> </li>
    <li>     <a href="#visu-tools-package">Visu-tools package</a> </li>
    </ul>
    </li>
    <li><a href="#technical-background">Technical background</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

This repository contains the code for my bachelors thesis at University of Bielefeld in the winter semester 22/23. <br />
The topic is: *"Entwicklung von Visualisierungswerkzeugen zur Auswertung von VR-Navigationsexperimenten beim Menschen"*(Development of visulazition tools that can be used to evlauate vr-navigation-experiments performed by humans).

More precisly the task was to find ways of visulaizing "meta-data" that humans might use when navigating through a vr-world. This "meta-data" includes things such as how the environment moves in respect to oneself or how sharp/blurry one percives parts of the environment. <br />
The task also included...\TODO

*Why is it relevant to be able to have such tools?*
\TODO

<br />

The vr-experiments are designed and performed with Unity, thus all the evaluation tools are also made with Unity. 

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

This repositiory basically consits of two parts. 
* The first part is a *folder* called *visu-tools*. This folder can be opend via Unity and contains the whole unity project in which the task got implemented. It serves as an example showcasing the tools. From here on this part will be referenced by *visu-tools project*.
* The second part is a *.unitypackage* file called *visu-tools*. As the file ending suggest this file is a unity package and can be imported as such into a unity project. This package only contains the tools themselves, how they get incorporated into any given project is left to the user. As described above, *visu-tools project* serves as an example how one can incorporate them. From here on this part will be referenced by *visu-tools package*.

### Prerequisites
Everything got implemented and tested using the Unity editor version *2021.3.12f1* and a simple 3D core project base using the *built-in render pipeline*. <br />
XR support got tested using the Mock HMD Loader. Both render modes (multi pass, single pass instanced) were tested and functioning. <br />
If anything does not work for you, be sure to check if your set-up is any different. Especially other render pipelines might have problems!

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Installation

#### Visu-tools project
1. Clone or download this repository.
2. Open Unity hub on your computer.
3. Make sure you  have  Unity editor version *2021.3.12f1* installed.
4. Under *Projects* click *Open > Add project from disk* and select the folder named *visu-tools* contained in the just cloned/downloaded repository.
5. The Unity editor should now open up the project.

#### Visu-tools package
1. Clone or download this repository.
2. Open up the Unity project in which you want to incorporate the package. Make sure this project fullfills the <a href="#prerequisites">prerequisites</a>. You can try to use a different set-up, however, it is not guaranted that the tools still work as expected. 
3. Open the folder containing the  just cloned/downloaded repository.
4. Move the *.unitypackage* file in your Unity Editor. An import dialogue will appear. Imported all the files.
5. You can see the files in your project tab under *Assets > visu-tools*.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- USAGE -->
## Usage

The usage differs between just using the example *visu-tools project* or incoporating the *visu-tools package* into another project. The following describes both cases. It is assumend that you completed the <a href="#installation">installation</a> and are familiar with the Unity editor.

### Visu-tools project
The project contains four different scenes: *Main Menu, Test World, Record World, Replay World*. <br />
Open up the *Main Menu* scene and enter play mode. Now you can navigate between the different scences. 

#### Test World
Here you can use your keyboard (W,A,S,D) and mouse to navigate your character in first person mode through an example environment. <br />
In order to spicy things up do the following:
1. If you use play mode in fullscreen mode, don't.
2. In the hierarchy go to *Player > Main Camera*.
3. In the inspector go to *Control Shader (Script)*.
4. Under *Shader acitve* you can now select which visualization tool should be applied to your image. What the different effects do and how they work is explained here: <a href="#technical-background">Technical background</a>. However, you do not need this knowledge in order to just play around.
5. Go move around and see what happens!

### Record World
Here you can use your keyboard (W,A,S,D) and mouse to navigate your character in first person mode through an example environment. <br />




/TODO: Bilder, sollte gui da sein des au no erklären/erwähnen.


### Visu-tools package



<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- TECHNICAL BACKBROUND -->
## Technical background

<p align="right">(<a href="#readme-top">back to top</a>)</p>




<!-- LICENSE -->
## License

<!-- Distributed under the MIT License. See `LICENSE.txt` for more information. -->

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* Supervisors:

<p align="right">(<a href="#readme-top">back to top</a>)</p>
