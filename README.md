# Kaede

## 使い方

Examples(PowerShell)<br>
ID = 9300708 = ピンクビーン

### APNG 出力

AnimatedPNGs 下に出力されます。

```
.\Kaede.Batch.exe extract -i 8500002 -p C:\Nexon\MapleStory -t Mob -r 1
```

オプション

```
-i {Id of target} # 8500002
-p {Path of MapleStory's directory} # Mob.wz, etc...
-t {Name of {TARGET}.wz} # Mob
-r {Ratio of output images size} # default = 1
```

### ID から名前検索

```
.\Kaede.Batch.exe name -i 8500002 -p C:\Nexon\MapleStory
```

オプション

```
-i {Id of target}
-p {Path of MapleStory's directory}
```

### 名前から ID 検索(一部合致)

```
.\Kaede.Batch.exe id -n ピンクビーン -p C:\Nexon\MapleStory
```

オプション

```
-n {A part of target name}
-p {Path of MapleStory's directory}
```
