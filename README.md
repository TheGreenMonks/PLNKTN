# PLNKTN

ECOLOGICAL FOOTPRINT

The user’s Ecological Footprint (EF) is calculated by the total amount of “items” the user has logged (consumed) within each category that day.
For example: miles/km driven + Amount of a specific food item etc. 
Currently we store only EF on the server, which is calculated through a basic algorithm on the front end including CO2 and other variables like landmass, but in the future, we might want to store more data like CO2 on the server as well.
EF is stored daily. 


CATEGORIES

Currently we have 6 main CATEGORIES and each has several items under them and together they add up to 27 “items” or data point per day.
We currently only need to store “Amount” for items under TRANSPORT. This way we can measure increase or decrease and use it to complete a challenge for driving less etc. 
We don’t need this for Categories like Diet, I’ve just put it on the list to show how it’s presented on the front end. With that said, we want to add Amount to items like Clothing so that people can log how many pairs of a certain item they bought. We don’t have that built in the front end yet.
The amount of categories and items are more than likely to change in the future. 

Basic Items
Livings space (Amount)
people in household (Amount)
Car MPG - liter/mile (Amount)

Diet
Plant based (Amount)
Beef (Amount)
Pork (Amount)
Poultry (Amount)
Egg (Amount)
Diary (Amount)
Seafood (Amount)

Transport
Subway (Amount)
Bus (Amount)
Car (Amount)
Bicycle
Flight (Amount)

Electronics
S
M
L
Used

Clothing
S
M
L
USED

Footwear
S
M
L
USED


Collective EF
We calculate the average user EF, daily. Currently we look at the number of downloads of the app and output an average EF. This represent our collective, meaning all of PLNKTN user’s impact, on earth (EF).
This feature is not only great to see the average score of all users but we plan to use this to create competitive Teams and groups based on location and affiliation.

USER ID
Every user is given a unique User ID when they launch the app for the first time. All their data is associated with this user ID. The ID is hidden to the user.
In the future we want to be able to attach an email address to this user ID so that they can retrieve and restore their data without the need of a password or creating an actual account.


GAME LOGIC:

CHALLENGES
The server needs to see if a user has met certain type criteria to complete a challenge. Example: Did the user skip beef for 1 week. 
If they did, it needs to send a response to change a state of a challenge to “Completed”.
Since challenges will be added and will be changing in the future their images and text need to live on the server.
Each challenge has a unique ID with unique criterias and data assigned to it. 
They also have an image assigned but this image is shared amongst the challenges that involve the same type of “Item” 
ie: Beef challenges share same image, Car challenges share same image, used medium sized electronic items same image etc. (based on current items making it a total of 24 images.)
We will need to know the time of a when a challenge was completed in order to know if the user was notified

REWARDS
The server needs to see if a user has met certain type criteria to unlock a reward.
The criteria for unlocking a reward is if the surrounding 6 challenges of that reward is completed. (See image)
If they did, it needs to send a response and change the status of that reward to Unlocked.
Each reward needs to know which 6 challenges needs to be completed in order to Unlock that reward.
Each reward has a unique ID. with unique criterias and data assigned to it. 
We are still working and designing the data and information that needs to be stored for each reward (location, region, fun facts etc.)
Since we currently only have one reward, Trees, they all share same image.
We will need to know the time of a when a reward was unlocked in order to know if the user was notified.
Since rewards will be added and will be changing in the future their images and text need to live on the server.


This sketch shows 2 Rewards (trees) unlocked based on 6 “surrounding” challenges completed  (the second reward is as you see out of frame in this image but hopefully you get the idea :)


USING REWARDS
The server needs to see if a user has used or acted on a reward. 
(in the case of trees, did the user choose a country/region to plant that tree? If not it will remain UNLOCKED but not used)
If they did, it needs to send a response that the reward was used or acted upon.

THE EDITOR
We have created an editor where we design the “Map” of our game and challenges and rewards. When we EXPORT this data the server will be able to see each Challenge, Reward, ID and the data and image assigned to them. 
It will also show which 6 surrounding challenges are needed for each REWARD in order to be unlocked. We are working on making this even easier for you guys.

NOTIFICATIONS:

There are several different events for notification. NOTE! A notification has a different purpose than sending a response in the event of meeting a criteria.
Their purpose is to notify the user that criterias have been met whereas GAME LOGICS are for unlocking and progressing the game logics in the app.
Currently these are the notifications:

Challenge notification
(If the user complete a challenge and if so, how many?)
Reward notification
(If the user unlocked a reward)

NOTE: Notifications will be handled by the front end. 
To do so, it will check with the DB if the user has a new challenge completed or rewards unlocked, and then schedule a notification.

EVENTS

1 - We currently log events on the server, check game logic and store EF data on the server only once per user session; when closing the app or when the app goes in the background.
During this event, the game logic needs to see if there are any criterias met that would potentially Complete a Challenge or Unlock Reward.
If so, a notification will be sent to the user the FOLLOWING day at a specific time. (notifications will be scheduled by the front end, back end doesn't need to “send” notifications)

Example: Today at 5pm user inputs that they took public transportation and closes the app. This is the last time he/she will open the app today. Next day at say 9am he/she will get a notification that certain challenge has been completed and, if applicable, a reward has been unlocked. When he/she opens the app he/she will see the new rewards.

2 - We also need to check the Last time a user opened the app.
This is for two reasons;

A - To check what date user completed a challenge or unlocked a reward, and compare it to last day they opened the app so that we know if they need to see the "challenge completed/reward unlocked" pop up when they open the app.

B - To keep track of user retention to find out how we are doing with the concept and design of the app overall.
