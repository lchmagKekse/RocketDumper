# Rocket League Dumper
---
Remake of [Rocket League - Dumper (x64)](https://www.unknowncheats.me/forum/other-fps-games/402519-rocket-league-dumper-x64.html) by [MultiHackerCoun](https://www.unknowncheats.me/forum/members/1269372.html) to get some practice with C#.

## Preview
![preview](https://cdn.discordapp.com/attachments/1104154655518376021/1138192227307368608/image.png)

## Output
```
GObjects.txt

[0x1E8A4463C00] Class Core.Config_ORS
[0x1E8A31ABC00] Class Core.Object
[0x1E8A445C400] Class Core.ClassTupleCollection_ORS
[0x1E8A445C800] Class Core.ClassTuple_ORS
[0x1E8A445CC00] Class Core.SubscriptionCollection_ORS
...
```

```
GNames.txt

[000000] None
[000001] ByteProperty
[000002] IntProperty
[000003] BoolProperty
[000004] FloatProperty
[000005] ObjectProperty
[000006] NameProperty
...
```

### Install

```
dotnet pack
dotnet tool install --global --add-source ./nupkg RocketDumper
```

### Usage

```
rocketdumper
```
(Creates GObjects.txt & GNames.txt in your current directory)

### Uninstall

```
dotnet tool uninstall -g RocketDumper
```
