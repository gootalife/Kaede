# Kaede

## 使い方
### 準備
ゲームフォルダにあるMob.wzをResources下にコピー。
Examples(PowerShell)<br>
ID = 9300708 = ピンクビーン

### APNG出力
AnimatedPNGs下に出力されます。
```
.\Kaede.Console.exe extract -i 9300708
```
オプション
```
-i {id of target}
-m {magnification of output images size}
-w {name of wz file} # 省略可
-b {name of monster book csv file} # 省略可
```

### IDから名前検索
```
.\Kaede.Console.exe search_name -i 9300708
```
オプション
```
-i {id of target}
-b {Name of monster book csv file} # 省略可
```

### 名前からID検索(一部合致)
```
.\Kaede.Console.exe search_ids -n ピンクビーン
```
オプション
```
-n {A part of target name}
-b {Name of monster book csv file} # 省略可
```