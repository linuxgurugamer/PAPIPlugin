PAPIPlugin Continued
==========
Forked from https://github.com/asarium/PAPIPlugin.

Origin work at 
https://forum.kerbalspaceprogram.com/index.php?/topic/38153-023-runway-papi-array-version-032/
 

This plugin adds working PAPI arrays to the runways of Kerbal Space Program.
The array will show two red and two white lights when following correct glide slope.

![UI](http://i.imgur.com/8C42Qjy.png)

======================================================================================================
The following is an easy way to create new settings:
    First, install Vessel Mover,  Waypoint Manager, HyperEdit and NavHud
    Launch a plane or rover (small rover is more accurate), and move it to the desired runway.  Depending 
		on what mods you have installed, you might be able to launch it at the new location, otherwise you can 
		use HyperEdit or any other method to move the vessel to the new location
    Use Vessel Mover to position the vessel where you want the lights to be.  The lights are normally at 
		the left side of the runway (as you approach)
    Use Waypoint Manager to create a waypoint where the vessel is.
    Create a new file in the PAPIPlugin directory called PAPI.config.  Add the following to the file. For 
		the second section, move to the other end of the runway, turn around, and repeat
    
LightGroup
{
	Name = (replace with location name)
	Body = (replace with planetary body name)
	LightArray 
	{
		Type = PAPIArray
		Namespace = PAPIPlugin.Arrays
		Latitude = (replace with Latitude of array location)
		Longitude = (replace with Longitude of array location)
		Heading = (direction of runway MINUS 90)
		GlideslopeTolerance = 1.5
		TargetGlideslope = 3
		HeightAboveTerrain = 7
		PartCount = 4
		LightRadius = 8
		LightDistance = 6
	}
	LightArray 
	{
		Type = PAPIArray
		Namespace = PAPIPlugin.Arrays
		Latitude = (replace with Latitude of array location)
		Longitude = (replace with Longitude of array location)
		Heading = (direction of runway PLUS 90)
		GlideslopeTolerance = 1.5
		TargetGlideslope = 3
		HeightAboveTerrain = 7
		PartCount = 4
		LightRadius = 8
		LightDistance = 6
	}
}

======================================================================================================
Config file documentation:

Version 0.2 added the possibility to add light arrays within a config file. This is done by the LightGroup config node type. 
Every node represents an object which groups multiple lights together.

You can find an example of that config file here: https://github.com/asarium/PAPIPlugin/blob/master/assets/GameData/PAPIPlugin/lights.cfg

The options on a LightGroup are:

Name: The name of the group (optional)
Body: The name of the body the group is located on (e.g. Kerbin) (required)
To add actual light arrays you need to add LightArray entries inside the group node. Each entry needs a Type value and may have a Namespace value. These are used to identify the class which is used for the light array.

Possible values are:

Type: PAPIArray
Namespace: PAPIPlugin.Arrays
Options:
Latitude: The latitude of the array location (required)
Longitude: The longitude of the array location (required)
Heading: The heading the array points to. This is used to aligned the array and also to make the array fade out when viewed from behind.(required)
TargetGlideslope: The target path where the array will be half white half red, default is 6. (optional)
GlideslopeTolerance: The difference from the target glidepath where the array will not be entirely red or white, default is 1.5.(optional)
Height: The actual height of the array above the terrain below (optional)
PartCount: The number of lights in the array (optional)
LightRadius: The radius of the lights (optional)
LightDistance: The distance between two lights, excluding the actual light radius (optional)

Credits
----------
- TaranisElsu - Author of the TacLib library

- asarium - Author of Runway PAPI array

- angavrilov - Contributor of Runway PAPI array

- blizzy78 - Author of Toolbar

- cybutek - Author of KSP-AVC

- Authors of CKAN

License
----------
MIT
