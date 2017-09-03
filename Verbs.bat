DEL bin\Verbs.dll bin\Verbs.*.dll && ^
RC /q Verbs.rcconfig /fm obj\Verbs.en.res /fo obj\Verbs.res Verbs.rc && ^
LINK /DLL /NOENTRY /MACHINE:X86 /OUT:bin\Verbs.dll obj\Verbs.res && (
FOR %%R IN (Verbs.*.rc) DO ^
RC /q Verbs.rcconfig /fm obj\%%~nR.res /fo obj\Verbs.res %%R && ^
LINK /DLL /NOENTRY /MACHINE:X86 /OUT:obj\%%~nR.dll obj\%%~nR.res && ^
MUIRCT /c bin\Verbs.dll /e obj\%%~nR.dll && ^
MOVE obj\%%~nR.dll bin\%%~nR.dll
) && ^
DEL obj\Verbs.res obj\Verbs.*.res obj\Verbs.*.dll && ^
SIGNTOOL sign /t http://timestamp.comodoca.com bin\Verbs.dll bin\Verbs.*.dll
