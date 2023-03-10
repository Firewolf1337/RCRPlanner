# Ravencreekracing Planner (RCRPlanner) for iRacing
I have developed this planner to make decisions easier which tracks I should purchase for the season.
This got a bit out of hand and this planner is now a bit more, than originally planned. 
Now you can see all series, cars, tracks and upcoming races for in iRacing.
You can automatically start applications which you need while racing and close them together with the planner.
In addition, it is possible to create some interesting statistics.


If you are interested, try it! No installation necessary. Just extract the zip file in an empty folder of your choice.


## Handling and Troubleshooting 
* **There is no auto refresh of the data.**
    * You need to manually refresh the data when the new season or "Week 13" starts or you purchase something.

* **Please try to start with a fresh copy in an empty folder, if you have any problems after some time with the planner.**

## Overview
#### Start and Login
At first run, you need to login with your iRacing credentials to download all necessary files and data from the iRacing API.
If you want, you can save the credentials so updating the data can be done without entering it again later. (Credentials are stored locally in your windows profile on your PC)
Login data can be changed on the top right through clicking on Login.
![Login](/doc/images/login.png)

#### Profile 
Next to the Login, you can find the profile button. The button includes your helmet color and your initials.
There you can see your ingame name and iRacing ID and your license class and iRating. Also you can reload the data from the API.
![Profile](/doc/images/profile.png)

#### Main Menu
On the upper part, you can see the different views for the different contents in iRacing. At the bottom, you can update or reset the Planner or set it to start minimized. Also you can see the actual running version of your planner.

![Main Menu](/doc/images/main_menu.png)

#### Races view
The default view is the races view, which gives you a list of upcoming races, which can be filtered as you like by selecting some filters by clicking Filter on the top left. 
In this view you also can set alarms for your preferred race by selecting the time at top (Alarm offset:) and clicking on the Clock symbol in the race list.
By clicking on the row of interest, it will show some more information (all weeks and cars) about the series.
![Races](/doc/images/races.png)

#### Series view
Here you can see all existing series in the season. By clicking on the ✰ you can mark the series as favorite and then can be filtered in races.
Also by clicking a series, you can see some more details (weeks and cars).
![Series](/doc/images/series.png)

#### Cars view
Like in series view, you can see all cars in iRacing which also can be selected as favorite. Moreover, by clicking on a car you get some more information about that.
![Cars](/doc/images/cars.png)

#### Tracks view 
This view as nearly the same like the car view only for the tracks. By clicking a track, you can see additionally all layouts of the track.
![Tracks](/doc/images/tracks.png)

#### Purchasing guide
This view will help you to decide which tracks you should buy. For this view, you have to select your favorite series. On this information, it will select the tracks, which are in this series and show you the count how often this track is in your favorite series (Participation). You can filter out all series where you are over 8 races in the season so that you maybe don't need to purchase.
![Purchasing Guide](/doc/images/purchase_guide.png)

#### Autostart Programs
Here you can add Programs which you want to start either automatically or by clicking on the "⟳" in the main menu next to "Autostart Programs"
* Auto start programs on launch - will start the programs when the planner is started.
* Start programs minimized - will try to launch them minimized.
* Kill programs on close - will try to close these programs by its initial process id. (some programs starts more programs which prevent it to be closed by this feature)
* Kill programs by Name - will stop all programs with the same name like the .exe. 
    - !!! This can cause data loss if manually started the program with the same name. For example if you would start "Word" with the planner and by hand this would kill all instances of Word without asking for saving !!!

![Autostart Programs](/doc/images/autostart.png)

#### Statistics
![Participation](/doc/images/part_stats.png)
![iRating](/doc/images/irating_stats.png)

## Building information

If you want to build it on your own, please modify RCRPlanner/salt.txt. With this String you password will be encrypted and stored on your PC.
Also change the repo in the App.config file to prevent downloading another version from this repo.

