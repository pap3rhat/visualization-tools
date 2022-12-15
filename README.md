ACHTUNG: Der build und das package sind aktuell nicht auf dem neusten Stand. Diese Read me ist auch noch nicht vollständig fertig (aber schon zu nem guten Teil). Die TODOs in der Read.me können ignoriert werde, die sind für mich als Erinnerungen an Überlegungen etc. gedacht.

[![Made with Unity](https://img.shields.io/badge/Made%20with-Unity-57b9d3.svg?style=for-the-badge&logo=unity)](https://unity3d.com) 
<br />

<a name="readme-top"></a>
<h1 align="center">visu-tools</h1>
<p align="center">
Unity package that allows you to apply full screen post-processing effects to your project in real time. Also includes the option to record and later replay your movement.
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
<li>     <a href="#visu-tools-package">Visu-tools package</a> 
<ul>
<li><a href="#visualization-effects-function">Visualization effects function</a></li>
<li><a href="#replay-function">Replay function</a></li>
<li><a href="#texture-grab-function">Texture grab function</a></li>
</ul></li>
</ul>
</li>
<li><a href="#technical-background">Technical background</a>
<ul>
<li><a href="#interlude">Interlude</a></li>
<li><a href="#linear-filters">Linear filters</a>
<ul>
<li><a href="#gaussian-blur">Gaussian blur</a></li>
<li><a href="#high-pass-filter">High-pass filter</a></li>
<li><a href="#sharpening">Sharpening</a></li>
<li><a href="#radial-blur">Radial blur</a></li>
<li><a href="#radial-blur-with-desaturation">Radial blur with desaturation</a></li>
<li><a href="#motion-blur">Motion blur</a></li>
<li><a href="#example-images">Example images</a></li>
</ul></li>
<li><a href="#motion-field">Motion field</a>
<ul>
<li><a href="#forward-rendering">Forward rendering</a></li>	
<li><a href="#motion-vectors">Motion vectors</a></li>	
</ul>	
</li>
</ul>
<li><a href="#license">License</a></li>
<li><a href="#acknowledgments">Acknowledgments</a></li>
</ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

This repository contains the code for my bachelors thesis at University of Bielefeld in the winter semester 22/23. <br />
The topic is: *"Entwicklung von Visualisierungswerkzeugen zur Auswertung von VR-Navigationsexperimenten beim Menschen"*(Development of visualization tools that can be used to evaluate vr-navigation-experiments performed by humans).

More precisely the task was to find ways of visualizing "meta-data" that humans might use when navigating through a VR-world. This "meta-data" includes things such as how the environment moves in respect to oneself or how sharp/blurry one perceives parts of the environment. <br />
The task also included creating a system to replay .CSV files that contain movement (position + orientation) data. In order to always have access to such files this project also contains recorder. <br />
Furthermore this project contains a function to save the motion vectors texture and the depth texture as images (.png). This is useful for analyzing the individual vectors for every frame.

*Why is it relevant to be able to have such tools?*
\TODO

<br />

The VR-experiments are designed and performed with Unity, thus all the evaluation tools are also made with Unity. 

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

This repository basically consist of three parts. 
* The first part is a *folder* called *visu-tools*. This folder can be opened via Unity and contains the whole unity project in which the task got implemented. It serves as an example showcasing the tools. From here on this part will be referenced by *visu-tools project*.
* The second part is a *.unitypackage* file called *visu-tools*. As the file ending suggest this file is a unity package and can be imported as such into a unity project. This package only contains the tools themselves, how they get incorporated into any given project is left to the user. As described above, *visu-tools project* serves as an example how one can incorporate them. From here on this part will be referenced by *visu-tools package*.
* The third part is a *folder* called *build-visu-tools*. This folder contains a *.exe* file called *visu-tools*. This file is the finished build of the *visu-tools project* and can be explored without the Unity editor and any knowledge about it (just double left-click it). As it is basically the same as the *visu-tools project* everything non Unity editor related that applies to *visu-tools project* also applies to this build. Thus it will not be mentioned individually from here on.

### Prerequisites
Everything got implemented and tested using the Unity editor version *2021.3.12f1* and a simple 3D core project base using the *built-in render pipeline*. <br />
XR support got tested using the Mock HMD Loader inside the Editor(!). Both render modes (multi pass, single pass instanced) were tested and functioning. The GUI elements in *visu-tools project* might not work with XR, however, as *visu-tools project* only shows an example usage, that is fine. <br />
If anything does not work for you, be sure to check if your set-up is any different. Especially other render pipelines might have problems!

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Installation

#### *Visu-tools project*
1. Clone or download this repository.
2. Open Unity hub on your computer.
3. Make sure you  have  Unity editor version *2021.3.12f1* installed.
4. Under *Projects* click *Open > Add project from disk* and select the folder named *visu-tools* contained in the just cloned/downloaded repository.
5. The Unity editor should now open up the project.

#### *Visu-tools package*
1. Clone or download this repository.
2. Open up the Unity project in which you want to incorporate the package. Make sure this project fulfills the <a href="#prerequisites">prerequisites</a>. You can try to use a different set-up, however, it is not guaranteed that the tools still work as expected. 
3. Open the folder containing the just cloned/downloaded repository.
4. Move the *.unitypackage* file in your Unity editor. An import dialogue will appear. Imported all the files.
5. You can see the files in your project tab under *Assets > visu-tools*.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- USAGE -->
## Usage

The usage differs between just using the example *visu-tools project* or incorporating the *visu-tools package* into another project. The following describes both cases. It is assumed that you completed the <a href="#installation">installation</a> and are familiar with the Unity editor (or are simply using the finished build).

### Visu-tools project
The project contains five different scenes: *Main Menu, Replay Menu, Test World, Record World, Replay World*. <br />
Open up the *Main Menu* scene and enter play mode. Now you can navigate between the different scenes. 

#### *Test World*
Here you can use your keyboard (W,A,S,D) and mouse to navigate your character in first person mode through an example environment. <br />
In order to spice things up you can either explore the *Options* menu, which is accessed via the *OPTIONS* button on the top-left, or do the following:
1. If you use play mode in full screen mode, don't.
2. In the hierarchy go to *Player > Main Camera*.
3. In the inspector go to *Control Shader (Script)*.
4. Under *Shader active* you can now select which visualization tool should be applied to your image. What the different effects do and how they work is explained here: <a href="#technical-background">Technical background</a>. However, you do not need this knowledge in order to just play around.
5. Go move around and see what happens!

#### *Record World*
Here you can use your keyboard (W,A,S,D) and mouse to navigate your character in first person mode through an example environment. <br />
By pressing the *START* button in the graphical interface you start recording your movement. By pressing the *STOP* button in the graphical interface you stop recording your movement. <br />
From the moment you press *START* to the moment you press *STOP* your movement information will be saved and can be later replayed in the *Replay World* scene. <br />

#### *Replay Menu*
Here you can simply select which file should be replayed. Select the file you want, then press *REPLAY*.

#### *Replay World*
Here you cannot move on your own. <br />
On the top right there are a few replay management buttons you can use (some might only appear if certain other buttons got pressed, but it's pretty self explanatory):
* *PLAY*: Starts the replay of your previously recorded movement. 
* *PAUSE*: Pauses the replay.
* *RESUME*: Resumes the replay.
* *STOP*: Stops the replay.
While the replay is playing you can try using different visualization effects as described here: <a href="#test-world">Test world</a>. 

<p align="right">(<a href="#readme-top">back to top</a>)</p>

/TODO: Bilder ?


### Visu-tools package
The package contains a view different folders filled with scripts, materials and other information in order to make everything work. Feel free to go through it and, if necessary, modify it for your needs.

#### *Visualization effects function*
In the following the different parts contributing to the visualization effects function will be listed. 
* *visu-tools > Editor > ControlShaderEditor* : Controls how the inspector for the ControlShader script looks. 
* *visu-tools > materials > Visualization Materials* : Contains three different materials that all use a different shader. 
* *visu-tools > Scripts > Visualization Scripts* : Contains four different scripts. Three of them, *Depth, ImageFilter, MotionField*, are classes with the necessary methods to use/apply the corresponding shader. The fourth file, _ControlShader_, is "the heart". It is derived from mono-behaviour and delegates the task of rendering the image with the correct post-processing effect on it. 
* *visu-tools > Shader > Visualization Shader* : Contains four files. Three of them are shader files and one of them is a shader include file. The shader files contain a shader with only one subshader, but multiple passes that use different fragment shaders.

The only part important for the **basic usage** is the *ControlShader* script. (The other parts require at least some knowledge about applying full screen post-processing shaders in Unity. Refer to the Unity manual for information on that topic.) **The visualization effects function is now added to your project as follows:**
1. In the hierarchy click on the camera object to which you want apply the post-processing visualization effects.
2. In the inspector go to *Add Component* and type *ControlShader*. Add the *ControlShader* C# script to your camera.
3. In the inspector open up the *Set-up* part of the *ControlShader* script. The materials should already be moved in there (if not, please do so); the camera should be empty. In this empty camera field move your camera object (the same object where you added the script as a component).
4. You're all set up! You can now control the applied effect using the *Shader Active* selection on the script in the inspector. Be sure to read the tool tips when it comes to the additional settings and check out the <a href="#technical-background">technical background</a> section. If you want the user to be able to control the effects themselves in the end, you have to implement a GUI for that.  

#### *Replay function*
In the following the different parts contributing to the replay function will be listed.
* *visu-tool > Scriptable Objects > Active File* : Scriptable object that holds information about the active file (i.e. the file that should be replayed). It stores the index of the file, the file name, as well as the positions and rotations for every frame (each stored in a list).
* *visu-tools > Scriptable Objects > File List* : Scriptable object that holds a list of all available file-paths, as well as the active file name and number.
* *visu-tools > Scripts > Replay Scripts*: Contains five different scripts. Two of which belong to the scriptable objects (*ActiveFile, FileList*). The other three are there to record, read and replay CSV files. 

For the **basic usage** the record, read and replay file have to be used as follows. <br />

*CSVRecorder*:
1. Attach the *CSVRecorder* file to any GameObject in the scene where the movement recording (of, for example, the player) should happen.
2. In the inspector move the GameObject, whose movement should be recorded, inside the *Recorde* slot of the script.
3. In the inspector you might write a _full_ path inside the *File Path* slot of the script. *File Path/LogFiles* is where the recorded CSV files will get saved to in this case. You can also leave this path empty. In that case the files will get saved to *Application.persistentDataPath/LogFiles*. 
4. In order to start the recording, call the *StartRecording* method of the script from anywhere you want.
5. In order to stop recording, call the *StopRecording* method of the script from anywhere you want.

**Important addition**: This recorder only saves the movement within *FixedUpdate* and **not** within *Update*. So it does not save the position and rotation of *every* frame. However, this makes it frame-rate independent, which is very important for replaying it at the same speed as recording it!

*CSVReader*:
1. In another script (eg one that controls the gui) create an Instance of the *CSVReader* script. Provide the constructor with the *Active File* and *File List* scriptable objects in the folder *visu-tools > Scriptable Objects*. You might additionally provide the constructor with a file path from where the CSV files should be read. If you don't, *Application.persistentDataPath/LogFiles* will be used as a default. In either case make sure only CSV files containing movement information are saved at that location.
2. In order to load all the CSV files that exist in the given location, call *LoadAllFiles*. This method fills the *File List* paths list with all the paths of avilable CSV files. As arguments this methods takes two boolean values. The first one (*setFirst*) determines whether the first found file will be set as the active file and the second one (*readFirst*) determines whether the information of the first file should be processed as well (and set in the *Active File* scriptable object). As a default both those values are true. Setting *readFirst* to true only makes sense if *setFirst* is set to true as well.
3. In order to set a file as the active file, call *SetActiveFile*. This method requires the index of the file within the paths list in the *File List* scriptable object. It also requires a bool (*read*) that determines whether or not the information of this file should be processed and saved as well.
4. In order to read the information contained in the file given by the *Active File* scriptable object, call *ReadActiveFile*. This method has no arguments. It reads in the position and rotation information and saves them to the corresponding lists of the *Active File* scriptable object. 

**Important addition**: In order for the information of the active file to be processed correctly, the CSV file **has to** have the following columns (that are also called that way!): *pos_x, pos_y, pos_z, quat_x, quat_y, quat_z, quat_w*. Their meaning is self-explanatory.

*CSVPlayer*:
1. Attach the *CSVPlayer* file to any GameObject in the scene that should be moved according to the CSV file, whose information is stored in the *Active File* scriptable object.
2. In the inspector move the *Active File* scriptable object from the *Scriptable Objects* folder inside the *Active File* slot of the script.
3. Calling the *StartReplay* method from anywhere causes the replay to start. The GameObject on which the script is attached will start to move according to the *Active File* scriptable object.
4. Calling the *PauseReplay* method from anywhere causes the replay to pause. The GameObject on which the script is attached will stop moving and stay at its current position.
5. Calling the *ResumeReplay* method from anywhere causes the replay to resume. The GameObject on which the script is attached will continue moving from where it paused.
6. Calling the *StopReplay* method from anywhere causes the replay to stop. The GameObject on which the script is attached will stop moving. If the replay gets started again it will start moving from the beginning (not the position where it is at at the moment of pausing).
7. Calling the *CheckRunning* method will return true if the replay has been started and not all movements have been replayed yet (it will also return true while the replay is paused!).

**Important addition**: This player updates the position and orientation of the GameObject within *FixedUpdate* and **not** within *Update*. So it does not change the position and rotation in *every* frame. However, this makes it frame-rate independent, which is very important for replaying it at the same speed as recording it! (The difference in 'missing frames' cannot be perceived). So if you are using your own files, that are not created using *CSVRecorder*, make sure that you record the movement within *FixedUpdate*!

<p align="right">(<a href="#readme-top">back to top</a>)</p>

#### *Texture grab function*
In the following the different parts contributing to the replay function will be listed.
* *visu-tools > materials > Texture Grab Materials* : Contains one material that uses the Texture Grab shader.
* *visu-tools > Scripts > Texture Grab Scripts*: Contains one script called *TextureGrabber* that can save the motion vector texture as well as the depth texture to a png file.
* *visu-tools > Shader > Visualization Shader* : Contains one shader file. That shader has one subshader and two passes. One pass returns the motion vector texture, the other pass returns the depth texture.

For the **basic usage** the *TextureGrabber* file has to be used as follows. <br />

1. In the hierarchy click on the camera object from which you want the textures.
2. In the inspector go to *Add Component* and type *TextureGrabber*. Add the *TextureGRabber* C# script to your camera.
3. In the inspector part for the script the material should already be moved in there (if not, please do so); the camera should be empty. In this empty camera field move your camera object (the same object where you added the script as a component).
4. You're all set up! You can now control which texture gets saved as an image (.png) by (un-)checking the corresponding box in the inspector part for the script.

**Important additions**: 
* The images get saved to *Application.persistentDataPath/DepthGrab* and *Application.persistentDataPath/MotionGrab* if no path is provided in the *File Path* slot in the inspector. Otherwise the get saved to *File Path/DepthGrab* and *File Path/MotionGrab*.
* For images the show the depth texture the nameing convention is as follows: *depth_cam.stereoActiveEye.ToString().ToLower()_Time.frameCount_Time.delta_Time*. For images the show the motion vectors texture the naming convention is as follows: *motion_cam.stereoActiveEye.ToString().ToLower()_Time.frameCount_Time.delta_Time*. <br /> In the case the xr is not active *cam.stereoActiveEye.ToString().ToLower()* resolves to *mono*. In the case that xr is active and multi pass is being used *cam.stereoActiveEye.ToString().ToLower()* resolves to *left* or *right*; so two images are saved for very frame (one for each eye). In the case that xr is active and single pass instanced is being used *cam.stereoActiveEye.ToString().ToLower()* resolves to *left*; so only the image for the left eye is saved for every frame (because single pass instanced rendering is weird...).
* In the motion vectors images the red channel contains the movement in x-direction and the green channel contains the movement in the y-direction. The original rendertexture values can range from $-1$ to $1$. Both values have been divided by *Time.delta_Time*, so they are not nearly $0$; for reconstructing purposes of the original values *Time.delta_Time* is contained in the file name. If the image contains another value range ($0-255$) and it is important for you to have the original range, you have to scale it back into the original range.
* In the depth texture images each color channel contains the same value. The original rendertexture values can range from $0$ to $1$ and are distributed linearly. $0$ represents the near plane, $1$ the far plane. If the image contains another value range ($0-255$) and it is important for you to have the original range, you have to scale it back into the original range.

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- TECHNICAL BACKGROUND -->
## Technical background

This section tries to (at least partially) explain how the different visualization tools work in theory. It will also cover some of the implementation details and things to be aware of when using the *visu-tool package* in your own project.
/TODO au bio stuff in read me? is eigentlich scho zu lang für ne read me und ghehört au ned wirklich zum technischne zeug dazu, also propbably ned

### Interlude
Before thinking about how an *individual* visualization tool works, one has to think about how they work *in general*. As explained here <a href="#about-the-project">About The Project</a>, the purpose of the visualization tools is to extract some kind of "meta data" from what a participant perceives during a VR-navigation-experiment. Broadly speaking, what they perceive is a rapid sequence of frames, where each frame itself can be interpreted as an image. This image shows part of the virtual world as seen through the virtual eyes/virtual camera of the participant. As the participant moves those eyes/this camera the image will change as it shows a different part of the virtual world. Because the changing of frames is happening at a very high speed (for example 400 frames per second) the participant cannot distinguish between the different frames and instead perceives it as a continuous stream of information that simulates movement. <br />
In order to now visualize the "meta data" in this stream, one starts by simply saving the movement information (position + rotation at a frame x) of the participant and replaying it with the same frame-rate as the participant experienced. This replay is what will now be manipulated. More precisely, each frame will be seen as an image on which an image-processing effect will be applied before it gets shown. In Unity this is done by using post-processing shaders that effect the final render image output of the camera by changing every pixel color within a fragment shader. <br />
So, essentially, one has to understand what an image is in this context and how it can be manipulated.

#### *What is an image*
In this context an image is one single frame that is contained within a $M \times N  \times 3$ dimensional matrix where $M$ is the height of the frame and $N$ is the width of the frame. The $3$ represents the three different RGB-color channels which contain a value form 0 to 255 (or 0 to 1 in Unity; keyword: normalization). So, for example, the value at $(j,k,0) \in M \times N \times 3$ represents the red color value of the pixel at height $j$ and width $k$. <br />
Applying an image processing effect now results in manipulating each RGB value for every pixel ( manipulating all $(j,k,i) \in M \times N \times 3$ ). This is simply done by using  matrix operations, one very important matrix operation in this case is *convolution* (more on that later).

/TODO hier wahrschienlich au frequenzen erwähnen, damit des unten mehr Sinn ergibt
/TODO Fourier??????????

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Linear filters
Linear filters are image pre-processing effects in which the final value of a pixel is given by a linear combination of the value of the pixel itself and of the values of the pixel in its neighborhood $W$. <br />
This calculation is realized by *convolution*. During convolution a small, square matrix with an odd height/width (e.g. $5 \times 5$, $9 \times 9$,...), a so called *kernel*, is  slid over the image matrix. Every entry of this kernel represents a weight *w_i* with which the value of the image pixel underneath the kernel gets multiplied. After every image pixel value underneath the kernel got multiplied with the corresponding weight, those values get summed up. This sum is now the final value of the image pixel that is positioned underneath the kernels middle. <br />
When dealing with colored images, this process is separately done for every color channel. So the final red/green/blue value of every pixel is determined independently of the other two colors. 

/TODO Bild für conv erklärung einfügen?


#### *Gaussian blur*
A Gaussian blur is a way of applying a low-pass filter to an image. A low-pass filter "smoothes" the image by keeping its low frequencies and discarding its high frequencies. Depending on how low/high a frequency needs to be in order to count as a low/high frequency, the image will appear more or less smooth/blurry; The more is discarded, the blurry the image will appear. <br />
This "discarding/keeping decision" can be realized by doing a convolution on each RGB-channel of the image. So the final red/green/blue value of a pixel $(j,k)$ is the result of the current red/green/blue pixel value added to the values of the pixel in its neighborhood $W$, where each of those values in weighted by a kernel. The bigger the kernel the bigger the neighborhood $W$. 

The important part now is how the kernel weights $w_i$ are designed. Getting rid of high frequencies can be seen as decreasing the disparity between the different pixel values; this can simply be realized by averaging over them. So a $3 \times 3$ kernel would look like this:
```math
\frac{1}{9} \cdot \begin{bmatrix}
	1 & 1 & 1 \\
	1 & 1 & 1 \\
	1 & 1 & 1 
\end{bmatrix} 
```
So $w_i=\frac{1}{9} \\ \forall i\in\[0..8\]$. <br />
As the size of such an averaging kernel increases, the neighborhood $W$ increases and the individual kernel weights decrease. So more pixel are involved in the averging step which makes the color transition smoother; hence the image appears more blurry. <br />
However, using such an averaging kernel lets the final image appear a bit "boxy"; hence why a low-pass filter that is implemented in such a fashion is referred to as *Box blur*. <br />
The effect of this filter can be seen here: <a href="#example-images">Example images</a> 

/TODO evtl doch Fourier ansprechen, damit box mittels transformation in frquenzbereich erklärt werden kann? Zu komplex?

In order to have less of a "boxy" look to the final image, the above kernel can slightly be adjusted. Instead of giving each pixel in $W$ the same weight, the weight will be dependent on the distance of each pixel $(j,k) \in W$ to the middle pixel in $W$. The further away a pixel, the smaller its weight. So a $3 \times 3$ kernel might look like this:
```math
\frac{1}{16} \cdot \begin{bmatrix}
	1 & 2 & 1 \\
	2 & 4 & 2 \\
	1 & 2 & 1 
\end{bmatrix} 
```
More elaborately, the weights $w_i$ are given by the product of two discrete Gaussian functions (hence Gaussian blur), one per image dimension:
```math
G(x,y) = \frac{1}{2 \pi \sigma ^2} \exp ^{-\frac{x^2 + y^2}{2 \sigma ^2}}
```
Here $x$ is the distance of the currently looked at pixel from the center pixel on the horizontal axis, $y$ is this distance on the vertical axis and $\sigma$ is the standard deviation of the Gaussian distribution. <br />

/TODO vllt mehr erklären warum Einfach produkt (seperierbarkeit!)

This can further be simplified, because the discrete Gaussian function can be approximated by the binomial distribution. So in order to get the correct kernel weights it is enough to simply look at Pascals triangle. <br />
Each row in Pascals triangle can be interpreted as a one-dimensional kernel that weights the pixel closer to the center more that those farther away. For example the second row (counting starts at $0$!) would yield the following kernel
```math
\begin{bmatrix}
	1 & 2 & 1 
\end{bmatrix} 
```
Normalizing these values yields
```math
\frac{1}{4} \cdot  \begin{bmatrix}
	1 & 2 & 1 
\end{bmatrix} 
```
So, in general, a one-dimensional Gaussian kernel can be obtained through 
```math
\frac{1}{2^n} \cdot  \begin{bmatrix}
	PascalRow(n,0) & PascalRow(n,1) & ... & PascalRow(n,n) 
\end{bmatrix} 
```
where $PascalRow(n,x)$ refers to the $x$-th value of the $n$-th row in Pascals triangle (counting starts at $0$ for both $x$ and $n$). <br />
In order to get a two-dimensional kernel, the kernel simply gets multiplied with its transpose; this works due to the separability of the two-dimensional Gaussian function. In the case of a $3 \times 3$ kernel that yields
```math
\frac{1}{4}  \cdot  \begin{bmatrix}
	1 & 2 & 1 
\end{bmatrix} \cdot \frac{1}{4}  \cdot  \begin{bmatrix}
	1 \\ 2 \\ 1 
\end{bmatrix}  = \frac{1}{16} \cdot \begin{bmatrix}
	1 & 2 & 1 \\
	2 & 4 & 2 \\
	1 & 2 & 1 
\end{bmatrix} 

```
which is exactly the kernel from above! <br />
The effect of this filter can be seen here: <a href="#example-images">Example images</a> 

#####  *Implementation remarks*
1. For efficiency reasons the separability of the kernel is being used. In the first shader pass a horizontal Gaussian blur is applied to the image, this gets saved as an intermediate result. On this intermediate result a vertical Gaussian blur is applied, resulting in a final output-image that is blurred horizontally and vertically.
2. For efficiency reasons the kernel weights are determined a little differently than described above. For example if the kernel is supposed to have dimensions $5 \times 5$, the $9$-th row of the Pascal triangle is being picked instead of the $5$-th row. Then the two most left and the two most rights values of this row are discarded, leaving the middle $5$ values of the $9$-th row. The reasoning behind this is that otherwise the outermost weights are quite small, so the pixel values underneath them do not contribute a lot to the final result; however, you still have to look up the image pixel value for each of them, which is the costly operation. So designing the kernel with a "higher" row and discarding values leads to bigger weights and thus more blur effect with comparably less pixel value look-up operations. <br />
Additionally Linear sampling is being used in order to reduce the look-up operations. 

/TODO maybe Linear sampling erklären

3. If you select the *Gaussian blur* shader option in the *ControlShader* script you can get the additional option to change the kernel size. The bigger this value, the stronger the effect. The minimum value is $5$, the maximum value is $127$ and the default value is $9$. Only odd values change the effect. Only integer values are possible.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

#### *High-pass filter*
A high-pass filter is basically the inverse of a low-pass filter. Instead of keeping the images low frequencies and discarding its high frequencies, a high-pass filter discards the images low frequencies and keeps its high frequencies.  This strengthens the disparity between the pixel values and erases area information. Same as for the low-pass filter: The more is discarded, the stronger this effect. <br />
This "discarding/keeping decision" can again be realized by doing a convolution on each RGB-channel of the image. 

The kernel weights $w_i$ are chosen quite simply. As the high-pass filter is the inverse operation of the low-pass filter, the kernels for the low-pass filter can just be inverted. <br />
For the $3 \times 3$ Box blur kernel this yields
```math
\begin{bmatrix}
	0 & 0 & 0 \\
	0 & 1 & 0 \\
	0 & 0 & 0 
\end{bmatrix} - \frac{1}{9} \cdot \begin{bmatrix}
	1 & 1 & 1 \\
	1 & 1 & 1 \\
	1 & 1 & 1 
\end{bmatrix}  = \frac{1}{9} \cdot  \begin{bmatrix}
	-1 & -1 & -1 \\
	-1 & 8 & -1 \\
	-1 & -1 & -1 
\end{bmatrix} 
```
For the $3 \times 3$ Gaussian blur kernel this yields
```math
\begin{bmatrix}
	0 & 0 & 0 \\
	0 & 1 & 0 \\
	0 & 0 & 0 
\end{bmatrix} - \frac{1}{16} \cdot \begin{bmatrix}
	1 & 2 & 1 \\
	2 & 4 & 2 \\
	1 & 2 & 1 
\end{bmatrix} 
= \frac{1}{16} \cdot  \begin{bmatrix}
	-1 & -2 & -1 \\
	-2 & 12 & -2 \\
	-1 & -2 & -1 
\end{bmatrix} 
```
The effect of this filter can be seen here: <a href="#example-images">Example images</a> 

#####  *Implementation remarks*
1. For efficiency reasons the high-pass filter is not implemented using convolution with a high-pass filter kernel. Instead of doing convolution with a high-pass filter kernel, convolution is done with a low-pass filter kernel (the implementation is the same efficient implementation as mentioned above in the <a href="#gaussian-blur">Gaussian blur</a> section). The result of this convolution is saved as an intermediate result. Afterwards, in the high-pass filter shader pass, for every pixel the final color is calculate. This is done be simply subtracting the color of the low-pass filtered intermediate result from the original color. Trivially, this is the same as doing a convolution with a high-pass filter kernel.
3. If you select the *High-pass* shader option in the *ControlShader* script you can get the additional option to change the kernel size. The bigger this value, the stronger the effect. The minimum value is $5$, the maximum value is $127$ and the default value is $9$. Only odd values change the effect. Only integer values are possible.

/TODO ggf bio stuff

<p align="right">(<a href="#readme-top">back to top</a>)</p>

#### *Sharpening*
Sharpening an image basically means to enhance the edges contained within an image. So the images high frequencies need to exaggerated while the images low frequencies stay the same. In contrast to a low-pass or high-pass filter no frequencies are discarded, only added. <br />
This adding of high frequencies can be realized by doing a convolution on each RGB-channel of the image. 

The kernel weights $w_i$ are again chosen quite simply. Since all that has to be done is adding $x$ times the high frequencies to the original image, a $3 \times 3$ Gaussian kernel looks as follows
```math
\begin{bmatrix}
	0 & 0 & 0 \\
	0 & 1 & 0 \\
	0 & 0 & 0 
\end{bmatrix}  + x \cdot \left(
\begin{bmatrix}
	0 & 0 & 0 \\
	0 & 1 & 0 \\
	0 & 0 & 0 
\end{bmatrix} - \frac{1}{16} \cdot \begin{bmatrix}
	1 & 2 & 1 \\
	2 & 4 & 2 \\
	1 & 2 & 1 
\end{bmatrix}  \right)
= \frac{1}{16} \cdot  \begin{bmatrix}
	-x & -2x & -x \\
	-2x & 13x & -2x \\
	-x & -2x & -x 
\end{bmatrix} 
```
where $x$ determines how much of the images high frequencies should be added to the original image. The higher $x$ the stronger the sharpening effect. <br />
The effect of this filter can be seen here: <a href="#example-images">Example images</a> 

#####  *Implementation remarks*
1. If you select the *Sharpening* shader option in the *ControlShader* script you can get the additional option to change the kernel size. The bigger this value, the stronger the effect. The minimum value is $5$, the maximum value is $127$ and the default value is $9$. Only odd values change the effect. Only integer values are possible.
2. The scaling factor $x$ is set to $0.85$ and cannot be controlled via the inspector. If you want to be able to control this value, you have to include it into the inspector yourself.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

#### *Radial blur*
As the name suggest a radial blur is a kind of a low-pass filter. So it "smoothes" the image by keeping its low frequencies and discarding its high frequencies. There are however some difference to a Box blur or a <a href="#gaussian-blur">Gaussian blur</a>. <br />
For a Box blur or a <a href="#gaussian-blur">Gaussian blur</a> the image gets smoothed along the horizontal axis by a kernel that for example looks like this ( $3 \times 3$ one-dimensional Gaussian kernel)
```math
\frac{1}{4} \cdot  \begin{bmatrix}
	1 & 2 & 1 
\end{bmatrix} 
```
and the image gets smoothed along the vertical axis by a kernel that for example looks like this ( $3 \times 3$ one-dimensional Gaussian kernel)
```math
\frac{1}{4} \cdot  \begin{bmatrix}
	1 \\ 2 \\ 1 
\end{bmatrix} 
```
For a radial blur the image gets smoothed along another axis/vector that is dependent on the currently looked at pixel and the *origin of the blur*. The *origin of the blur* refers to a point in the image that stays unaffected by the filter (i.e. it does not get blurred).  The farther away a pixel from this origin, the more it gets blurred. This results in an effect that can be seen in <a href="#example-images">Example images</a>; once with the origin being at the center of the image and once with the origin being off-center.

Mathematically the vector that determines the "smoothing-axis" is given by 
```math
((j,k) - (j_{origin}, k_{origin}))^T = \begin{bmatrix}
	j \\ k 
\end{bmatrix}  - \begin{bmatrix}
	j_{origin} \\ k_{origin}
\end{bmatrix}  =  \begin{bmatrix}
	j-j_{origin} \\ k-k_{origin}
\end{bmatrix}
```
where $(j,k)$ refers to a pixel (in on color layer) of image and $(j_{origin}, k_{origin})$ refers to the origin of the blur.

In order to get the final color of a pixel $(j,k)$, its neighbors along this axis are being looked at. More concrete, these neighbors are
```math
\pm \frac{n-1}{2} \cdot \begin{bmatrix}
	j-j_{origin} \\ k-k_{origin}
\end{bmatrix}
```
where $n$ determines how many pixel (including $(j,k)$ ) are being looked at; the size of the neighborhood $W$. <br />
As for the <a href="#gaussian-blur">Gaussian blur</a> or the Box blur, the value of each pixel in $W$ gets weighted by a weight $w_i$ and afterwards their sum is being set as the final color value of $(j,k)$. In the case of $|W|=3$, the weights could be given by the following Gaussian kernel
```math
\frac{1}{4} \cdot  \begin{bmatrix}
	1 & 2 & 1 
\end{bmatrix} 
```

The way how $W$ is defined clearly shows why pixel farther away from the origin of the blur are more effected by the blurring than pixel closer to the origin of the blur. <br />
The farther away a pixel from the origin, the bigger their in-between distance; hence the magnitude of 
```math  
\begin{bmatrix}
	j-j_{origin} \\ k-k_{origin}
\end{bmatrix}
```
gets bigger as well. <br />
Thus using this unnormalized(!) vector in determining $W$ leads to the individual pixel in $W$ being farther apart from each other as $(j,k)$ gets farther away from the origin of the blur. Resulting in more high image frequencies being discarded for farther away pixel and less high image frequencies being discarded for closer pixel. As the distance between the pixel in $W$ grows linearly with the distance of $(j,k)$ to the origin, the blur effect is linear as well.

#####  *Implementation remarks*
1. If you select the *Radial Blur* shader option in the *ControlShader* script you can get the additional option to change the kernel size. The bigger this value, the stronger the effect. The minimum value is $5$, the maximum value is $127$ and the default value is $9$. Only odd values change the effect. Only integer values are possible.
2. If you select the *Radial Blur* shader option in the *ControlShader* script you can get the additional option to change the origin of the blur on the horizontal and on the vertical axis. Both values range from $0$ to $1$, $(0,0)$ representing the bottom left corner and $(1,1)$ representing the top right corner. The default is $(0.5, 0.5)$. If you are using xr this origin can be determined for each eye independently.
3. If you select the *Radial Blur* shader option in the *ControlShader* script you can get the additional option to change the scale of the radial blur. This value scales the vector between each pixel and the origin of the blur; hence the bigger this value, the stronger the effect. The minimum value is $0$, the maximum value is $10$ and the default value is $5$.

/TODO: ggf bio stuff

<p align="right">(<a href="#readme-top">back to top</a>)</p>


#### *Radial blur with desaturation*
This filter is an extension of the <a href="#radial-blur">Radial blur</a> filter described above. As such the image gets blurred from an origin of blur towards the images edges; the farther away a pixel from the origin, the blurrier it appears. Additionally each pixel's color saturation is also dependent on the distance between $(j,k)$ and $(j_{origin},k_{origin})$; the farther away a pixel from the origin, the less it is saturated. This results in the image only containing gray values at the points furthest away from  $(j_{origin},k_{origin})$ and fully saturated values at $(j_{origin},k_{origin})$. An example of this filter can be seen here:  <a href="#example-images">Example images</a>.

The way the desaturation part is calculated is pretty straight forwards. <br />
First the maximal distance between the origin of the blur and any image pixel gets calculated. As the image is rectangular, no matter where the origin is positioned, the maximal distance will always be achieved with a corner pixel (i.e. $(0,0), (0,1), (1,0), (1,1) $). Thus
```math  
maxDist =  \left \lVert \max \left \{ 
\begin{bmatrix}
	0-j_{origin} \\ 0-k_{origin}
\end{bmatrix},
\begin{bmatrix}
	0-j_{origin} \\ 1-k_{origin}
\end{bmatrix},
\begin{bmatrix}
	1-j_{origin} \\ 0-k_{origin}
\end{bmatrix},
\begin{bmatrix}
	1-j_{origin} \\ 1-k_{origin}
\end{bmatrix}
\right \} \right \rVert
```
This value is obviously the same for every pixel. 

As a second step a scale value gets calculated for each individual pixel. This value determines how far a pixel is positioned from the origin of the blur relative to the maximal distance. 
```math  
scale = \frac{\left \lVert \begin{bmatrix}
		j-j_{origin} \\ k-k_{origin}
	\end{bmatrix} \right \rVert}{maxDist} 
```

Finally, the color for each pixel can be determined by a linear interpolation between the pixels intermediate color (i.e. after applying the radial blur on it) and its grayscale intermediate color. Thus
```math  
(j,k) = lerp[(j,k)', gray((j,k)'), scale] = (j,k)' + scale \cdot [gray((j,k)') - (j,k)']
```
where $(j,k)'$ refers to the pixels color after the radial blur and  $gray((j,k)')$ refers to the grayscale value of the color after the radial blur. More precisely, $gray((j,k)')$ is defined as
```math  
gray((j,k)') =  0.299 \cdot (j,k)'.r + 0.587 \cdot (j,k)'.g + 0.114 \cdot (j,k)'.b;
```
where $(j,k)'.r$ is the red channel value of $(j,k)'$ (analogous for the blue and green channel). <br />
If the final color of $(j,k)$ is determined separately for each color channel, in the $lerp$-function $(j,k)'$ only refers to that color channels value and $gray((j,k)')$ is the value defined above. If the final color of $(j,k)$ is determined for all color channels at once, in the $lerp$-function $(j,k)'$ refers to $[(j,k)'.r, (j,k)'.g, (j,k)'.b]$ and $gray((j,k)')$ is defined as $[gray((j,k)'), gray((j,k)'), gray((j,k)')]$ (so the same value for each color channel).

From this definition follows that the pixels furthest away from the origin of the blur are completely desaturated.
```math  
scale = \frac{\left \lVert \begin{bmatrix}
		j-j_{origin} \\ k-k_{origin}
	\end{bmatrix} \right \rVert}{maxDist}  = \frac{maxDist}{maxDist} = 1
```
```math  
(j,k) = lerp[(j,k)', gray((j,k)'), 1] = (j,k)' + 1 \cdot [gray((j,k)') - (j,k)'] = (j,k)' - (j,k)' + gray((j,k)') = gray((j,k)')
```
It also follows that the origin of the blur is not desaturated at all
```math  
scale = \frac{\left \lVert \begin{bmatrix}
		j_{origin}-j_{origin} \\ k_{origin}-k_{origin}
	\end{bmatrix} \right \rVert}{maxDist}  = \frac{0}{maxDist} = 0
```
```math  
(j,k) = lerp[(j,k)', gray((j,k)'), 0] = (j,k)' + 0 \cdot [gray((j,k)') - (j,k)'] = (j,k)' 
```
As the distance between each image pixel $(j,k)$ and the origin of the blur grows linearly, the desaturation effect is linear as well.

#####  *Implementation remarks*
1. If you select the *Radial Blur Desat* shader option in the *ControlShader* script you can get the additional option to change the kernel size. The bigger this value, the stronger the effect. The minimum value is $5$, the maximum value is $127$ and the default value is $9$. Only odd values change the effect. Only integer values are possible.
2. If you select the *Radial Blur Desat* shader option in the *ControlShader* script you can get the additional option to change the origin of the blur on the horizontal and on the vertical axis. Both values range from $0$ to $1$, $(0,0)$ representing the bottom left corner and $(1,1)$ representing the top right corner. The default is $(0.5, 0.5)$. If you are using xr this origin can be determined for each eye independently.
3. If you select the *Radial Blur Desat* shader option in the *ControlShader* script you can get the additional option to change the scale of the radial blur. This value scales the vector between each pixel and the origin of the blur; hence the bigger this value, the stronger the effect. The minimum value is $0$, the maximum value is $10$ and the default value is $5$.

/TODO: ggf bio stuff

<p align="right">(<a href="#readme-top">back to top</a>)</p>


#### *Motion blur*
As the name implies this effect is also a kind of a low-pass filter. So it "smoothes" the image by keeping its low frequencies and discarding its high frequencies. There are however some difference to a Box blur or a <a href="#gaussian-blur">Gaussian blur</a>. <br />
Like the <a href="#radial-blur">radial blur</a> it smooths each pixel along an axis that is generally not the horizontal and/or vertical axis; the axis is dependent on each pixel. More precisely, the axis is given by the *motion vector* of each pixel. Broadly speaking a pixel's *motion vector* specifies the direction in which a pixel "moved" from the previous frame to the current frame; the bigger the magnitude of this vector, the more the pixel "moved". A more detailed description of what this motion vector exactly is, how to calculate it and an explanation on why "moved" is being used here, is given in /TODO hier einfügen wo es steht und ggf diese formulierung no ändern wenn doch ned so erklärt.

This definition of the "smoothing axis" leads to the following observation: The more a pixel "moved" from the previous frame to the current frame (i.e. the magnitude of the corresponding motion vector is big), the blurrier it will appear in the final image. Hence, if both frames look exactly the same (no pixel "moved"), there will be no blur at all. 

In a vr-navigation-experiment the direction of each motion vector is dependent on the direction in which the participant moves their virtual eyes/camera. For example, if the participant moves their virtual eyes/camera to the right, the motion vectors will point to the left. So for each pixel the blur would be done with the horizontal axis as the "smoothing axis". More on how the motion vectors depend on the participant's movement here: /TODO zu motion field verweisen

/TODO: irgendwie nen bspw Bild dafür machen, vermutlich aus Programm rausscreenshoten, weil von 2 bildern kann ja ned einfach präzise berechnet werden (könnte nur mit irgendwie nem OF algo, aber des is ja ned des gleiche)

#####  *Implementation remarks*
1. The motion vectors used in this effect are gotten from the *_CameraMotionVectorsTexture* that is provided by Unity (in order for Unity to provide this texture *DepthTextureMode.MotionVectors* has to be activated on the camera where the effect should be applied to). Each pixel in the texture contains the motion vector corresponding to each pixel in *_MainTex* (i.e. here: the texture that contains the color for each pixel in the current frame). The movement in the x-direction is given by the value of the red channel, the movement in the y-direction is given by the value of the green channel. The values range from $-1$ to $1$. Because the values tend to be very close to $0$ (not a lot of movement for each pixel), they are scaled by $\frac{1}{Time.deltaTime}$.
2. If you select the *Motion blur* shader option in the *ControlShader* script you can get the additional option to change the kernel size. The bigger this value, the stronger the effect. The minimum value is $5$, the maximum value is $127$ and the default value is $9$. Only odd values change the effect. Only integer values are possible.
3. If you select the *Motion blur* shader option in the *ControlShader* script you can get the additional option to change the scale of the motion blur. This value scales each motion; hence the bigger this value, the stronger the effect. The minimum value is $0$, the maximum value is $10$ and the default value is $5$.

/TODO: ggf bio stuff

<p align="right">(<a href="#readme-top">back to top</a>)</p>

#### *Example images*
The pictures below show the effects of the above described linear filters using a $5 \times 5$ kernel. 
![result](https://user-images.githubusercontent.com/61543847/206854573-f7dc2db4-837a-4239-852f-6a13b434007b.png)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

/TODO bei motion filed "moved" erklären
### Motion Field
The *motion field* of a frame is a vector field where each vector captures the per-pixel, screen-space motion of the scenes' objects from one frame to the next. Here scene refers to the virtual world which the participant can explore. <br />
In order to have a better understanding about what this exactly means and how the motion field can be calculated, it is important to understand some of the basics on how Unity renders a 3D scene on a 2D screen; so how (parts of) a *computer graphics pipeline* (aka *rendering pipeline*) works. 

/TODO: erklären was rendern heißt?

Unity's built-in render pipeline (which this project uses) uses *forward rendering*. This means that the 3D objects contained within a scene get projected onto the 2D image plane. Doing so needs a few transformations and projections. /TODO ray tracing? 

#### *Forward rendering*
A scene (here: the virtual world) contains multiple objects (most of the time), for example a tree, a house or a treehouse. Each of these objects is defined by a set of *vertices*, which get connected via edges into *triangles*. Those triangles can have a color or a texture on them, which makes the objects look like they are supposed to (e.g. a green tree or a brick house). <br />
The definition of those vertices happens within a *local coordinate system* (aka *object coordinate system*, aka *model coordinate system*). So each object is defined within its own coordinate system. <br />
The scene itself is ankered within a *world coordinate system*.  

The first step of forward rendering is to transform each objects vertices local coordinates into coordinates in the world coordinate system; this is called *model transformation*. In order to to transform a single vertex's local coordinates into the world coordinates they need to be scaled, rotated and translated appropriately.
/TODO: example Bild? <br />
For this to be efficient, matrices are being used to perform those operations. Because the scene is three-dimensional the homogeneous matrices are four-dimensional and the vertices local coordinates get adjusted to be four-dimensional as well. /TODO: diese koordinaten definitv erklären, da später für prjoektion sehr wichtig, vllt abe rne dhier sondenr nur in arbeit<br />
The four-dimensional matrix used for scaling a vertex is defined as
```math 
S_{x,y,z} = \begin{bmatrix}
	s_x & 0 & 0 & 0\\
	0 & s_y & 0 & 0\\
	0 & 0 & s_z & 0\\
	0 & 0 & 0 & 1
\end{bmatrix} 
```
where $s_i$ determines how much the vertex should be scaled in $i$-direction. <br />
The four-dimensional matrix used for translating a vertex is defined as
```math 
T_{x,y,z} = \begin{bmatrix}
	1 & 0 & 0 & t_x\\
	0 & 1 & 0 & t_y\\
	0 & 0 & 1 & t_z\\
	0 & 0 & 0 & 1
\end{bmatrix} 
```
where $t_i$ determines how much the vertex should be translated in $i$-direction. <br />
The four-dimensional matrices used for rotating a vertex are defined as /TODO: erklären warum drei? Zusammen mit homogne koordinaten?
```math 
R_x = \begin{bmatrix}
	1 & 0 & 0 & 0\\
	0 & \cos(\alpha_x) & \sin(\alpha_x) & 0\\
	0 & -\sin(\alpha_x) & \cos(\alpha_x) & 0\\
	0 & 0 & 0 & 1
\end{bmatrix} \\ \\
R_y = \begin{bmatrix}
	\cos(\alpha_y) & 0 & -\sin(\alpha_y) & 0\\
	0 & 1 & 0 & 0\\
	\sin(\alpha_y) & 0 & \cos(\alpha_y) & 0\\
	0 & 0 & 0 & 1
\end{bmatrix} \\ \\
R_z = \begin{bmatrix}
	\cos(\alpha_z) &  \sin(\alpha_z) & 0 & 0\\
	-\sin(\alpha_z) &  \cos(\alpha_z) & 0 & 0\\
	0 & 0 &1 & 0\\
	0 & 0 & 0 & 1
\end{bmatrix} 
```
where $R_i$ determines around which axis the vertex is being rotated and $\alpha_i$ determines with what angle the vertex is being rotated.

/TODO: bspw hier einfügen, dann macht das darunter auch mehr Sinn

In order to be more efficient a vertex is not first transformed with one matrix, then this result is being transformed with another matrix and so on, until the vertex is in world coordinates. Instead the needed matrices are first multiplied together (sequence matters!), the resulting matrix is called *world matrix M*. This *world matrix* is not unique to each vertex, but to each object (so all vertices that make up one object are multiplied with the same world matrix).

/TODO: bspw von oben dann hier weiter eklären, auf reihenfolge eingehen

After every vertex's coordinates got transformed from their corresponding local coordinate system into the world coordinate system, they get transformed in the *camera coordinate system*; this transformation is called *view transformation*. <br />
The camera coordinate system is a coordinate system where the participants virtual camera/eyes is position at the coordinate systems origin and is looking down the negative z-axis. The participants virtual camera is defined by
1. The camera's location in world coordinates $e$
2. The camera's viewing direction $v$
3. The camera's up direction $u$
4. The camera's right direction $r$
5. $v$, $u$, $r$ are orthonormal

A *standard camera* (i.e. a camera that is positioned at the world coordinate system origin and looks down the negative z-axis; which is what we want) is defined as
1. $e = (0,0,0)$
2. $v = (0,0,-1)$
3. $u = (0,1,0)$
4. $r = (1,0,0)$

Converting the standard camera's coordinate system into the participants camera's coordinate system  can be realized by the following matrix
```math 
\begin{bmatrix}
	r_x & u_x & -v_x & e_x\\
	r_y & u_y & -v_y & e_y\\
	r_z & u_z & -v_z & e_z\\
	0 & 0 & 0 & 1
\end{bmatrix} 
```
It is easy to see that this matrix does nothing more than projecting the unit vector in x-direction onto the $r$-vector of the participants camera, the unit vector in y_direction onto the $u$-vector of the participants camera, the unit vector in z_direction onto the $v$-vector of the participants camera and moves the standard camera's origin to the origin of the participants camera $e$. <br />
This matrix therefore does the opposite of what is needed, it transforms the standard camera's coordinate system into the participants camera's coordinate system. Thus the opposite operation (transforming the participants camera's coordinate system into the standard camera's coordinate system) can be realized by
```math 
\begin{bmatrix}
	r_x & u_x & -v_x & e_x\\
	r_y & u_y & -v_y & e_y\\
	r_z & u_z & -v_z & e_z\\
	0 & 0 & 0 & 1
\end{bmatrix} ^{-1} = 
\begin{bmatrix}
	r_x & r_y & r_z & 0\\
	u_x & u_y & u_z & 0\\
	-v_x & -v_y & -v_z & 0\\
	0 & 0 & 0 & 1
\end{bmatrix} 
\cdot
\begin{bmatrix}
	1 & 0 & 0 & -e_x\\
	0 & 1 & 0 & -e_y\\
	0 & 0 & 1 & -e_z\\
	0 & 0 & 0 & 1
\end{bmatrix} 
```
This matrix is called *view matrix V*. Every vertex $v$ gets multiplied by this matrix after it has been multiplied by the world matrix *M*; so $v'=  V \cdot M \cdot v$ (where $v$ and $v'$ are $4 \times 1$ column vectors). Now every vertex is given in camera/eye coordinates.  This is the simplest form of coordinates before they get projected from 3D space to 2D space.

/TODO: Bidler für camera stuuf? Siehe botsch VL

The first part of this projection is called *projection transformation*. Here the coordinates (given as *camera/eye coordinates*) get transformed into *normalized device coordinates*. Those coordinates encompass the unit cube $[-1,1]^3$, so the x-, y- and z-coordinates can range between $-1$ and $1$. This is, obviously, still a volume and not 2D. However, the z-coordinates will just be left out in the final image and are not being stored in the image (making it 2D). /TODO: hier vllt cube anders definierne , weil 4d und ned 3d; vllt aber au in text zu homogenen koordinaten sagen,d ass die letzte immer dazu gedacht wird wenn notwenidg oder so <br />
In order to transform the camera coordinates into normalized device coordinates, the camera's viewing volume (the *frustum*) first has to be mapped into the unit cube. This *frustum* is defined by
1. the camera's *near* and *far* clipping planes
2. how far the camera sees to the *left* and the *right* sides
3. how far the camera sees to the *top* and the *bottom*

/TODO: Bild?

Projecting the frustum (*frustum mapping*) is now done with the following matrix
```math 
\begin{bmatrix}
	\frac{2n}{r-l} & 0 & \frac{l+r}{r-l} & 0\\
	0 & \frac{2n}{t-b} & \frac{b+t}{t-b} & 0\\
	0 & 0 & -\frac{n+f}{f-n} & -\frac{2nf}{f-n}\\
	0 & 0 & -1 & 0
\end{bmatrix} 
```
where $n, f, l, r, t, b$ refer to the definition above. <br />
This matrix is called the *projection matrix P*. For every vertex and each frame the view matrix and the projection matrix are the same; only the world matrix differs. Hence, in order to be efficient $P \cdot V$ only gets calculated once each frame and is being used for every vertex. 

/TODO: erklären warum des funktioniert? Auwand für was des iege egal is für den Rest, aber so is es halt einfach nur ne matrix mit stuff die da is weil sie da is
Wahrscheinclich schon erklären, dann mach auch perspective divison sinn -> man sieht halt das z in w drin is und dadurhc perspektive rein kommt

After this projection the coordinates are, however, not yet in normalized device coordinates. Instead they are given in *clip space coordinates*; hence why this coordinate space is referred to as *clip space*. <br />
Given the coordinates of a vertex $v'$, its clip space coordinates range from $-w'$ to $w'$ where $w'$ refers to the fourth coordinate of $v'$. More precisely $v'$ is given as
```math 
v' = \begin{bmatrix}
	w' \cdot x' \\
	w' \cdot y' \\
	w' \cdot z' \\
	w'
\end{bmatrix} 
```
where $w' \not= 0$, because $v'$ is a point and not a vector. <br />
In order to transform those coordinates into normalized device coordinates, there is not more to do than dividing every coordinate by $w'$ (this is referred to as *perspective division* or *homogenization*). Doing this with every vertex then truly maps the frustum into the unit cube. It should be noted that there will be (a lot of) vertices for which it does not hold that their coordinates are between $-w'$ and $w'$ or $-1$ to $1$ after perceptive division. Those vertices are the ones outside the frustum given by the virtual camera; the ones the virtual camera cannot see. The implications of this will be acknowledged later.

So, for every vertex $v$ to be transformed from the corresponding model coordinates into the normalized device coordinates, it simply has to be multiplied from the left by $P \cdot V \cdot M$; thus $v' = P \cdot V \cdot M \cdot v$. And afterwards has to be normalized by $w'$.

Now every vertex is in normalized screen coordinates that range from $-1$ to $1$ (like mentioned above this range is not true for every vertex, but what happens here with those vertices shall not matter as they will be ignored later anyway). As a final step those coordinates are projected into *window/pixel coordinates*; this is called *viewport transformation*. Those window/pixel coordinates are defined in $[l,l+w] \times [b,b+h] \times [0,1]$, where
* $l$ determines where on the left side of the monitor the image should start (i.e. how much room should be on the left side of the monitor screen); e.g. $l=0$ means the image starts at left border of the monitor screen
* $b$ determines where on the bottom side of the monitor the image should start (i.e. how much room should be on the bottom side of the monitor screen); e.g. $b=0$ means the image starts at bootm border of the monitor screen
* $w$ determine sthe width of the image
* $h$ determines the height of the image

This transformation is simply done by using the following matrix
```math 
\begin{bmatrix}
	\frac{w}{2} & 0 & 0 & \frac{w}{2} + l\\
	0 & \frac{h}{2} & 0 & \frac{h}{2} + b\\
	0 & 0 & \frac{1}{2} & \frac{1}{2}\\
	0 & 0 & 0 & 1
\end{bmatrix} 
```
/TODO: matrix erklären? Den Teil überhaupt erwähnen? Für motion vectoren eigentlich egal, aber ohne is es irgendwie unvollständig

After this last transformation every vertex's coordinates are in the final 2D window/pixel coordinates. If the vertices get connected back into triangles (*primitive assembly*), it is the same as transforming and projecting every point within that triangle themselves into the window/pixel coordinates. Thus what is now left is a set of *primitives* (here: triangles) in window/pixel coordinates. <br />
Not all those primitives are, however, within the coordinate range of the screen. Some might not be in there at all, some might only partially be in there. This is, as mentioned earlier, due to not every vertex being in the view field of the virtual camera. All the parts that are not within the viewport range are not being *clipped*; hence why this step is called *clipping*. If only parts of a primitive got clipped in this step, the remains will get connected back into new triangles/new primitives. 

/TODO bild für clipping?

/TODO rasterization hier erwähnen/erklären -> notwendig um fragment shader zu erklären

/TODO schauen, ob das hier drunter no sinn ergibt oder ned

What has been explained until now is just part of the forward rendering pipeline. There are still a few steps to follow in between and/or afterwards, for example lightning and shading steps. However, those parts are not necessary to understand how the motion vectors are being calculated and thus are not explained here. /TODO: schauen was davon doch vllt noch interesant/notwendig, bspw. rasterization, wobei eig nix davon

<p align="right">(<a href="#readme-top">back to top</a>)</p>

#### *Motion vectors*
With the knowledge about how every vertex of a scene object is transformed and projected from 3D to 2D space, it is quite easy to understand how to calculate a motion vector for every pixel on the screen.

As mentioned above, a motion vector is a vector that captures the per-pixel, screen-space motion of the scenes' objects from one frame to the next. In order to obtain such a vector for every pixel of the screen, the following things have to be know for each vertex within the scene
1. The vertexes model-view-projection matrix of the current frame; short $MVP$ matrix (note: it might be called $MVP$ matrix, however, if a vertex if given as a column vector it might be more correct to call it $PVM$ matrix, because that is the calculation order)
2. The vertexes model-view-projection matrix of the previous frame; short $MVP_{prev}$ matrix (note: it might be called $MVP_{prev}$ matrix, however, if a vertex if given as a column vector it might be more correct to call it $PVM_{prev}$ matrix, because that is the calculation order)
3. If a vertex has moved within the object it is part of (so within its local coordinate system), that it moved and its old position (in its local coordinates)

/TODO gugn was gfenau iin fragment shader kommt, wie rasterization funktioniert, ob vertex und fragment shader eklärt werden müssen (zu welchem ausmas) 

AHHHHHHHHHHHH

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* Supervisors:

<p align="right">(<a href="#readme-top">back to top</a>)</p>
