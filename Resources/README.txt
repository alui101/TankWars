Authors : Ali Hassoun & Emiliano Plaza, Spring 2021

03/28/2021
	Setup PS8 by creating a TankWars solution. Included multiple projects consisting of : GameController, Model, Resources and View. 
	Included NetworkController.dll in a Libraries folder located in Resources.

03/29/2021
	Added program class to be able to make the View project a windows application. Fixed white space with helpMenu.

04/01/2021:
	Included Vector2D project and Vector2D class. Managed to make a connection to the server by including network functionality in the GameController. 
	Default PlayerName set to "player". Implemented asynchronous receive methods.

04/02/2021:
	Created a class for Beams and Powerups in the Model. Implemented fields for all of the Model classes. Provided an instance of World to the controller, 
	included basic Json properties to the model class. Included a drawing panel. Implemented getters/setters and adders. Began using Json deserialization 
	in order to introduce objects into the world.

04/04/2021:
	Started the process of drawing in the drawingpanel.

04/05/2021:
	Finally managed to draw the tanks, although not perfectly.

04/06/2021:
	Added the images for the tanks. Improved the tank's drawings, also resolved an issue where after the tank ties its view was immediately changing. Fixed bugs
	with the way tanks were being added and removed in the controller.

04/07/2021:
	Added images for turrets, then implemented action handlers for user input imperfectly.

04/08/2021:
	Resolved an issue with the drawing panel's location. Also made sure movement was smooth. Added power ups and explosions.  Implemented drawing walls 
	and now the aimer for the tank works properly.

04/09/2021:
	Added the score and health bar for the tanks. Also implemented the help menu. Also fixed a bug where when a tank picks up two power ups it was shooting
	two beams consecutively but now it's working. Commented code and implemented the beam. Also changed the list of objects in the World from hash sets to 
	dictionaries.

Polish: 
	1-Remembers the initial direction if two directions are entered consecutively. For example, if the user presses 'up,' and presses 'right' while 
	holding down 'up,' and then releases 'right,' the tank will continue moving upward. 
	2-Changes the color of the HP bar as it gets low.
	3-HelpMenu.

	--------------------------------------------------PS9: Server Documentation--------------------------------------------------

04/12/2021:
	Adding projects for the server consisting of : Server, ServerController. Have not started implementation.

04/14/2021:
	Initiated handshake, now multiple clients can connect. Included settings.XML in the resources folder. Started to keep track of clients with a dictionary.
	Implemented the delegate callback passed to the networking class to handle a new client connecting. Completed Receive player name, the delegate that implements the server's part of the initial handshake.
	Implemented a delegate for processing client direction commands. Implemented a removeClient method which informs the console that a client disconnected.
	Began working on a XML parser class that is going to read an XML settings file to determine the rules and settings of the game.

04/15/2021:
	Partially implemented XmlParser. Properly determined relative file path. Wrote a switch case system that can Convert any int value provided by settings.xml.
	Included an instance of the world in ServerController. Continued implementing XMLParser which can now capture the provided walls.
	Fully implemented XMLParser so now it can retrieve all of the provided information from settings.xml. Managed to send a tank to a client.
	Included getters/setters in XMLParser to retrieve all of its information, provided fields in ServerController to store this information.

04/16/2021:
	Partially implemented commands. Constructed a busy loop that infinitely loops and updates the world every time a new frame is computed. 
	The update method now sends tank movement information to every client.

04/17/2021:
	Included more code in process movements so now it can process up,down,left,right and detects direction of turrent.Fixed a bug where all tanks in the server were moving the same way.
	Started implementing sending projectiles to the clients. Made a constructor to use when creating projectiles in the server. Possible timer needed to delay projectiles.

04/18/2021:
	Included functionality to spawn tanks randomly around the world, although collisions are not yet considered. Projectiles are now processed
	and their motion determined, which are then sent to the client. Made it so that tanks disappear when disconnected. Insured projectiles can be fired at a reasonable rate with a timer.

04/19/2021:
	Implemented Helper method that checks if a tank collides with a wall.Implemented collision detection for projectiles and walls.

04/20/2021:
	Implemented collision detection for projectiles and tanks. Improved software practice of ServerController.cs.

04/21/2021:
	Implemented respawning and fixed bugs with tank-proj collision. Included a string builder that allows Json to be sent simultaneously.
	Fixed projectile bug, where it would get quicker when other clients join.

04/22/2021:
	Fixed collection modified issue.Fixed bugs with explosion, improved the design of the projectile delay and respawn delay. 
	Fixed issue where projectiles would disappear before colliding with anything. Also fixed a bug where only one tank was able to shoot at a time. Added locks to resolve a collection modified issue.
	Provided cleaner functionality with a voided method. Implemented beams, unresolved bug where beam is only fired after multiple right clicks when there is more than 1 client. Implemented powerups.

04/23/2021:
	Began groundwork for extras. Implemented team gamemode, added wraparound, added cleanup for projectiles, fixed beam bug, added control commands to the model project. 
	Made it so that our server handles malformed data by disconnecting the client. Finished full game logic.

	Extras: There is a bonus game mode that allows for teams; projectiles don't harm teammates. 
			To enable this mode go to the settings.xml file and change the setting labeled 'GameMode' to true.
			Teammates are identified with a team name appended to their own user name.