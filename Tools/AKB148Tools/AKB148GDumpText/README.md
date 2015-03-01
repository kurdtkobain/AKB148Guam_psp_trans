# AKB148GDumpText
Script Dump/Insert for AKB 1/48: Idol to Guam de Koishitara PSP.


##Usage:

Dump: AKB148GDumpText.exe -d FILE.asb Script.txt

Dump (Only Event Dialog): AKB148GDumpText.exe -de FILE.asb Script.txt

Insert: AKB148GDumpText.exe -i Script.txt FILE.asb

Text dumped by tool is as follows:

|Offset|Size of text in bytes|Text (UTF8)|
|:------:|:---------------------:|:-------------:|
|15804|149|すみません、今なんて言ったか聞き逃しちゃって……。&lt;LINEEND&gt;申し訳ありませんが、もう一度言ってくれませんか？&lt;END&gt;|

&lt;LINEEND&gt; is equivalent to 0x0A.

&lt;END&gt; is equivalent to 0x00.

Note: if text is smaller than size it will pad with spaces, if larger it will stop at size.