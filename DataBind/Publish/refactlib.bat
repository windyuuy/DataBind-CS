SET Source=%1
SET Bin=%1\publish\DataBind
IF NOT EXIST %Bin%\DataBind MKDIR %Bin%\DataBind
IF NOT EXIST %Bin%\Editor MKDIR %Bin%\Editor
IF NOT EXIST %Bin%\Editor\Entry MKDIR %Bin%\Editor\Entry
IF NOT EXIST %Bin%\Editor\Service MKDIR %Bin%\Editor\Service

XCOPY %Source%\CiLin.dll %Bin%\Editor\Service /y
XCOPY %Source%\CiLin.pdb %Bin%\Editor\Service /y
XCOPY %Source%\DataBindService.dll %Bin%\Editor\Service /y
XCOPY %Source%\DataBindService.pdb %Bin%\Editor\Service /y
XCOPY %Source%\Mono.Cecil.dll %Bin%\Editor\Service /y

XCOPY %Source%\UnityDataBindService.dll %Bin%\Editor\Entry /y
XCOPY %Source%\UnityDataBindService.pdb %Bin%\Editor\Entry /y

XCOPY %Source%\DataBind.dll %Bin%\DataBind /y
XCOPY %Source%\DataBind.xml %Bin%\DataBind /y
XCOPY %Source%\EngineAdapter.dll %Bin%\DataBind /y
XCOPY %Source%\EngineAdapter.pdb %Bin%\DataBind /y
