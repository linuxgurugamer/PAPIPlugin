PAPIPlugin Continued
==========
Forked from https://github.com/asarium/PAPIPlugin.

Origin work at 
https://forum.kerbalspaceprogram.com/index.php?/topic/38153-023-runway-papi-array-version-032/
 

This plugin adds working PAPI arrays to the runways of Kerbal Space Program.
The array will show two red and two white lights when following correct glide slope.

![UI](http://i.imgur.com/8C42Qjy.png)

Building
----------
First add an environment variable with the name *KSP_PATH*
which contains the path to your KSP install where the plugin should be deployed to.

Then open the visual studio solution.
You will need to add the assemblies
**UnityEngine**, **KSPUtil**, **Assembly-CSharp-firstpass** and **Assembly-CSharp**
from *<KSP_PATH>/KSP_x64_Data/Managed* to the references of the project.
After that either directly run or build the solition.

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
