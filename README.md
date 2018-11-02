# ED-ShieldsBasic
A mod for the Game Rimworld


This mod allows you to place shield generators. They are expensive and power hungry but can really strengthen your defences. The standard shields will stop projectiles that try to enter it, but allow weapons to be fired out.

#Specifications

There are a number of different statistics that govern how Shield work.
Strength: How much Damage a shield can Take
Radius: How large the covered area is.
BlockIndirect: Can is Block Bullets and other directly fired Projectiles.
BlockDirect: Can it block Mortars and other Indirect Fire Weapons.
FireSupression: Can is extinguish fires.
DropPods: Can is target Drop Pods, frying their navigation and causing them to crash.


	Standard Shield:
Strength:4000
Radius:8
BlockIndirect:Yes
BlockDirect:Yes
FireSupression:Yes
DropPods:No

	Small shield:
Strength:200
Radius:2
BlockIndirect:Yes
BlockDirect:Yes
FireSupression:Yes
DropPods:No
	
	Fortress shield:
Strength:16000
Radius:20
BlockIndirect:Yes
BlockDirect:Yes
FireSupression:No
DropPods:Yes
	
	SIF shield:
Strength:6000
Radius:8
BlockIndirect:No
BlockDirect:Yes
FireSupression:Yes
DropPods:No

This is a Special type of Shield that specifically covers certain building instead of creating a large bubble.
	
	
#Change Log

01.00.00
*Initial Release -A14

01.00.01
*Fix for potentially not loading Graphical Resources on loading a saved game.

02.00.00
*Update to Alpha 15
*Change to using Comps instead of Custom C# thingDefs, partially due to https://ludeon.com/forums/index.php?topic=25249

02.00.01
* Modifying Building_Shield.CurrentStatus, IsActive and CheckPowerOn.

03.00.00
*Update to Alpha 16

03.00.01
*Converting WorkToMake to WorkToBuild

03.00.02
* Shields now Require Components

0.18.0.0
* Update to Beta 18

0.18.0.1
* Increased Documentation.

0.19.0.0
 * Update to B19

0.19.0.1
 * Fix for not covering Embrasures
 
1.0.0.0
 * Update to 1.0
 * Some UI images from "Operative85" on the ludeon Forums