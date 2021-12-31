# Kaede

## 使い方

### 準備

Examples(on PowerShell)<br>
ID = 9421558 = ピンクビーン

### APNG 出力

「AnimatedPNGs」下に出力されます。

```
.\Kaede.Console.exe extract -i 9300708
```

オプション

```
-i {id of target}
-w {name of wz file} #Mob.wz
-b {name of monster book csv file}
-r {rate of output images size} #初期値1 = サイズ1倍(1～256までの整数)
```

### ID から名前検索

```
.\Kaede.Console.exe search_name -i 9300708 -b {IDブックのパス}
```

オプション

```
-i {id of target}
-b {Name of monster book csv file}
```

### 名前から ID 検索(一部合致)

```
.\Kaede.Console.exe search_ids -n ピンクビーン -b {IDブックのパス}
.\Kaede.Console.exe search_ids -n キノコ -b {IDブックのパス}
```

オプション

```
-n {A part of target name}
-b {Name of monster book csv file}
```
