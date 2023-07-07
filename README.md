# Ravencreekracing Planner (RCRPlanner) for iRacing
I have developed this planner to make decisions easier which tracks I should purchase for the season.
This got a bit out of hand and this planner is now a bit more, than originally planned. 
Now you can see all series, cars, tracks and upcoming races for in iRacing.
You can automatically start applications which you need while racing and close them together with the planner.
In addition, it is possible to create some interesting statistics.

***Get an overview [here](/../../wiki/Overview)***

**If you are interested, try it! -> [Latest Release](/../../releases/latest)** <br />
No installation necessary. Just extract the zip file in an empty folder of your choice.
<br />
<br />
___ 
*If you like this planner, we would appreciate any tips.*
<br /><a href="https://www.buymeacoffee.com/RCRacing" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-green.png" alt="Buy Me A Coffee" style="height: 40px !important;width: 162px !important;" ></a>

## Handling and Troubleshooting 

* **Most of the table rows are clickable and will show more information.**
   * To close the opend row again press left mouse button.

* **There is no auto refresh of the data. In case of missing cars, tracks, series, races or the new season starts reload the data from iRacing.**
    * Click on the Profile -> "Reload data"

* **Please try to start with a fresh copy in an empty folder, if you have any problems while starting the planner.**
    * If there is still a problem additionaly delete the folders in %LocalAppData%\RCRPlanner\

* **For transparent backgrounds of the car and track pictures.** 
    * Start the Planner and wait for the Pictures to download.
    * Install [ImageMagick](https://imagemagick.org/)
    * Run the TransparentPics.ps1 file with powershell.
        * has to be run again when new cars or tracks will be released.

## Building information

If you want to build it on your own, please modify RCRPlanner/salt.txt. With this String you password will be encrypted and stored on your PC.
Also change the repo in the App.config file to prevent downloading another version from this repo.
