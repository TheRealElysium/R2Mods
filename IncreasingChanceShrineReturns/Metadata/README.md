# IncreasingChanceShrineReturns
#### By Elysium

This mod aims to resolve the losing streaks that sometimes occurs while using chance shrines.

After an unsuccessful attempt on a chance shrine, the next attempt will be more likely to succeed.

Formula used is `vanilla rate * (0.93^failures)`  Example shown below.  Rate is configurable if you do not agree with the default.

Instead of this happening to you:

![Close up](https://i.imgur.com/92DUBNC.png)

This could happen instead!

![Close up](https://i.imgur.com/OawdV2s.png)

---

## Features

- Does not change the default failure rate, so you are not more likely to win two in a row.

- Fully configurable to allow you to make the game play the way you want. Make each attempt harder instead of easier!

- Toggleable custom chat message that displays the failure rate.

- Toggleable default shrine message.

- Toggleable chance shrine cooldown.  No need to wait 2 seconds if you don't want to.

---

### Installation Guide

- Copy the `IncreasingChanceShrineReturns.dll` file to your BepInEx plugins folder.

---

### Configuration
1. **Make sure you run the game with the mod installed to generate the config file**
2. Navigate to `\Risk of Rain 2\BepInEx\config\`
3. Open `Elysium.IncChanceShrineReturns.cfg` in any text editor
4. Edit the values for settings as you see fit!

You can also set settings in-game with the commands listed below.

### Settings
| Setting                    | Default Value |                    Command |
| :------------------------- | :-----------: | -------------------------: |
| Percentage                 |            93 |               icsr_percent |
| Cooldown                   |          true |              icsr_cooldown |
| Custom Message             |          true |               icsr_message |

---

### FAQ
---

`I want to play this with my friends. Do they also need to install this mod?`

*No, only the host requires this mod to function. It will do nothing if you are not the host.*

*If you encounter version mismatch you can use my other mod:*[SetBuildID](https://thunderstore.io/package/TheRealElysium/SetBuildID/)

---

`How do I configure the mod while the game is running?`

*Open up the console window (``ctrl + alt + ` ``). All commands starts with `icsr_` and will autocomplete.*

---

`How do I use this mod with only the cooldown or message removed?`

*Simply set the Percentage to 100 and failure changes will be disabled.*

---

`Does this mod work with InfiniteChance?`

*Yes, it has been developed with InfiniteChance in mind. Supporting decreasing success rates and a custom formula:* `vanilla rate * (percentage^uses)`

*You should probably set the percentage to something higher than 100 while using InfiniteChance if you don't want easy items.*

*Example with percentage set to 107:*

![Close up](https://i.imgur.com/qDkEqO3.png)

---

### Bug Reports, Suggestions & Feedback

Please feel free to contact us for suggestions/feedback/bug reports on discord *`Elysium#5804`*.

---

### Tested Compatibility

- [InfiniteChance](https://thunderstore.io/package/HimeTwyla/InfiniteChance/)

---

### Changelog

`1.0.0` - Initial release.
