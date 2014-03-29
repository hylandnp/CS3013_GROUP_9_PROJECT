CS3013 Group 9 Project
======================


CS3013 - Software Engineering Group Project Repository for Group 9: "Making Kinections"

======================


REQUIREMENTS/DEPENDENCIES:

Visual Studio 2013 (for C# and .NET framework) - 

Kinect For Windows SDK 1.8 - http://www.microsoft.com/en-us/kinectforwindowsdev/downloads.aspx

XNA Game Studio 4.0 - http://www.microsoft.com/en-ie/download/details.aspx?id=23714

Fizbin Gesture Library (included) - https://github.com/EvilClosetMonkey/Fizbin.Kinect.Gestures

======================


CHANGELOG:

Neil (24/02/2014)

- Added Kinect sensor selector class based on Microsoft XNA examples.


Neil (25/02/2014)

- Modified selector class, moved logo rendering to main class, added switch statement & game state control.

- Also started adding Kinect stream managers/renderers for debugging use.

- Re-named selector class to KinectManager, and incorporated the colour stream manager (with debug video output if desired).


Neil (04/03/2014)

- Added depth stream manager class.

- Added some more empty/todo classes for the game.

- Slight modifications to the KinectManager's debug messages & video output.

- Added basic skeleton stream manager class (still WIP - rendering not working!).

- Skeleton rendering now fixed, added boolean "was_drawn" flag to the manager classes (used to check when to update the texture to save on function calls).


Richard (05-12/03/2014)

- Research for Kinect integration & XNA programming.


Neil (09/03/2014)

- Minor bugfixes.

- Finished most of the SkeletonStreamManager class.

- Added simple basic hand position tracking (relative to the game screen) - needs more work on scaling & offsets.


Patrick (10/03/2014)

- Added Cursor class.

- Basic Cursor rendering & gestures.


Neil (11/03/2014)

- Minor bug & formatting fixes.

- Added the Fizbin Gesture Library for recognising gestures (replacing attempted home-brew code).

- Incorporated Fizbin Gesture Library tracking.


Gavan (17-21/03/2014)

- Basic button functionality.

- Started button timing code.


Patrick (19-23/03/2014)

- Started colour-swapping & picture-painting game.


Patrick (23/03/2014)

- Added colouring-in for textures.


Neil (23/03/2014)

- Moved colouring-in to the PaintingGame class.


Gavan (25-28/03/2014)

- Implemented button timing.

- Finished Button & ColourButton implementation.


Neil (29/03/2014)

- Setup menus & colour selection UI (unfinished).




