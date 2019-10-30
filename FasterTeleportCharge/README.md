# FasterTeleporterCharge

This mod increases the charge rate of the teleporter at a configurable rate after you have defeated the boss.

The time saved from the increased rate is then added to the run timer to preserve difficulty time scale.

## How it works

`ChargeMulti` multiplies the rate at which the teleporter charges. `vanillaRate * ChargeMulti`

If `AddToRunTime` is enabled, the increased rate is then logged and added to the run timer when the teleporter finishes charging.

For example, if you saved 20 seconds because of the increased charge rate, 20 seconds is now added on to your run time.

This means you saved 20 seconds of real time but the game thinks you sat there for the full duration!

This ever so slightly a nerf to the player because you have less time to farm exp during the teleporter event.

## Configuration
1. **Make sure you run the game with the mod installed to generate the config file**
2. Navigate to `\Risk of Rain 2\BepInEx\config\`
3. Open `com.Elysium.FasterTeleportCharge.cfg` in any text editor
4. Edit the values for the setting as you see fit!

You can also set the setting in-game with the commands listed below.

### Settings
| Setting                    | Default Value |                    Command |
| :------------------------- | :-----------: | -------------------------: |
| AddToRunTime               |          true |           ftp_addtoruntime |
| ChargeMulti                |             3 |            ftp_chargemulti |
| ShowMsg                    |          true |                ftp_chatmsg |
| InstantCharge              |         false |          ftp_instantcharge |

- AddToRunTime: Adds the time saved from the teleporter charge to the run timer to preserve difficulty time scale.

- ChargeMulti: Multiplier applied to charge interval after the teleporter boss is defeated. (1 = Vanilla)

- ShowMsg: Display a chat message with the amount of time saved when the teleporter is fully charged.

- InstantCharge: Charge the teleporter instantly after the telepoert boss is defeated.


## Installation Guide

- Copy the `FasterTeleportCharge.dll` file to your BepInEx plugins folder.


## FAQ

**I want to play this with my friends. Do they also need to install this mod?**

*No, only the host requires this mod to function. It will do nothing if you are not the host.*

---

**How do I configure the mod while the game is running?**

*Open up the console window (``ctrl + alt + ` ``). All commands starts with `ftp_` and will autocomplete.*


## Bug Reports, Suggestions & Feedback

Please feel free to contact me for suggestions/feedback/bug reports on discord *`Elysium#5804`*.

## Changelog

`1.0.0` - Initial release.
