


--------------------------------------------

# About the 'framework':

### Which class is for what in general?

The original 'prototype' was very much built on top of the Placenote sample 'StickyNote', which is simple but not really extendable. As our UI, virtual content and related interaction would be much more complex later, we need an extenable frameork.


- **SocialBeeARMain** is the 'game manager'.

- **UIManager** is for managing all UI elemtns. All UI change should be done through UIManager instead of directly to the UI elements (e.g.panels, buttons, text). UIManager is the centrialized place where we can categorize UI elements and set the 'mode/state' of the whole UI. 

- **UIFacade** and many ***Facade** are for managing difference level of UI elements.
  
- **MessageManager** is for sending either notification messages(for end user) or debug messages(the 'console' in UI), or updating debug informations such as the feature-point number. All message update are through a queue(MainThreadTaskQueue) which is updated per-frame, so that it doesn't block the main thread.

- **AnchorManager** for managing all the anchor objects(one activity = one anchor) that created by the creator. This class mainly maintain the list of anchor but do not handle their status change or interaction. 

- AnchorManager should be managing the anchors only, if in future we allow users to create more virutal content in AR, we may need to create a **VirtualContentManager** seperately, and it may needs to be extended for various kinfdof activities, such as PoIContentManager, PhotoContentManager, VideoContentManager..., etc. 
  
- **AnchorController** is is extended from previous 'NoteController'(StickyNote sample), as we have a much more complex Anchor object now instead of the placenote 'note' object. This script is on every anchor obejct, responsible for controlling its status and handling the interaction. 

- **NavigationManager** to be implemented for navigation.



--------------------------------------------
# Naming convension

- Namespace: SocialBeeAR

- In case some code need to be modified from PlaceNote library or PlaceNote sample. please apply below naming comvension, so that it can be easily searched out, to be able to upgrade PlaceNote library easily in future.

```cs
//Commented off by Cliff Lin>>>>>>>>>>>>>>
//description...
//Session.start();
//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
```

```cs
//Added off by Cliff Lin>>>>>>>>>>>>>>
//description...
Session.start();
//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
```

```cs
//Modified by by Cliff Lin>>>>>>>>>>>>>>
//description...
Session.start();
//--------------original--------------
//Session.stop();
//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
```








--------------------------------------------
# AnchorConfig

**Note**: This is the configuration anchor data, which is currently stored in PlaceNote cloud, but can be easily switch to store in SocialBee server. 

This JSON-format configuration incldues the pose(position, rotation) of all the anchor objects(the 3D icon-shape object), as well as its context information(the related experience and activity collection). It also contains some activity specific data(**activitySpecific**), for example, the question/answer for Trivia activity.

Currently this configuration only include anchors. In case in future we will allow users to create more virutal content in the game, those content should have an related pose(position, rotation) to one of the anchors. That configuration can be extended or seperated from this configuration files.

###Sample


```cs
{ 
    "sbActivityCollection": 
    [
        {
            "experienceId": "<Generated Unique experience ID>",
            "activityCollectionId": "<Generated activity collection ID>",

            "activityId": "<Generated Unity Activity ID>",
            "activityName": "<Human-friendly activity name>",
            "activityType": "PoI",

            "pose":
            {
                "position":
                {
                    "x": "0",
                    "y": "0",
                    "z": "0"
                },
                "rotation":
                {
                    "x": "0",
                    "y": "0",
                    "z": "0",
                    "w": "0" 
                }
            },
                
            "activitySpecific": 
            {
                "Key": "PoIDescription", "Value": "This PoI is ..."
            }
        },

        { 
            "experienceId": "<Generated Unique experience ID>",
            "activityCollectionId": "<Generated activity collection ID>",

            "activityId": "<Generated Unity Activity ID>",
            "activityName": "<Human-friendly activity name>",
            "activityType": "Trivia",

            "pose":
            {
                "position":
                {
                    "x": "0",
                    "y": "0",
                    "z": "0"
                },
                "rotation":
                {
                    "x": "0",
                    "y": "0",
                    "z": "0",
                    "w": "0" 
                }
            },

            "activitySpecific": 
            {
                "Key": "TriviaQuestion", "Value": "Trivia question 1",
                "Key": "TriviaAnwser", "Value": "Trivia anwser 1"
            }
        }

    ]
}

```




--------------------------------------------
#TO-BE-INTEGRATED


###SBContextManager
The **SBContextManager.cs** is the class to be called by other classes for retrieving 'context' information, including the experience ID, experience name(if available), activity collection ID, and activity collection name(if available), which should be retrieved from iOS app methods.
```cs
private void InitSBContext()
{
   //TODO: to be replaced by retrieving from the native app, this hard-coding is only for testing!
   SetSBContext("EXP_001", "Seattle City Tour", "ACTG_001", "Seattle Art Museum");
}
```

###Activity ID 
Currently there is a method in **Utilities.cs** for  generating anchor ID(activity ID), and current implementation is just a draft. It should be aligned with the native app, it should call the native app to generate the activity ID.

```cs
public static string GenerateAnchorUniqueId(string experienceId, string activityGroupId)
{
    return string.Format("ACT_{0}-{1}-{2}",
        experienceId,
        activityGroupId,
        new System.DateTimeOffset(System.DateTime.UtcNow).ToUnixTimeSeconds());
}
```

### Trivia Form
Calling the native UI for inputting Trivia question info in this method:
```cd
public void EditTrivia()
```



--------------------------------------------

