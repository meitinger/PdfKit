Cultures=de

RcConfig=rc.config
OutputPath=..\bin\$(Configuration)\mui
IntermediateOutputPath=..\obj\$(Configuration)\mui
NeutralDll=$(OutputPath)\PdfKit.dll
!IFDEF Culture
OutputPath=$(OutputPath)\$(Culture)
IntermediateOutputPath=$(IntermediateOutputPath)\$(Culture)
OutputDll=$(OutputPath)\PdfKit.dll.mui
!UNDEF Cultures
!ELSE
OutputDll=$(NeutralDll)
!ENDIF
IntermediateDll=$(IntermediateOutputPath)\PdfKit.dll
NeutralRes=$(IntermediateOutputPath)\PdfKit.ln.res
SpecificRes=$(IntermediateOutputPath)\PdfKit.res

all: directories $(OutputDll) cultures

directories: $(OutputPath) $(IntermediateOutputPath)

$(OutputPath):
	MKDIR $(OutputPath)

$(IntermediateOutputPath):
	MKDIR $(IntermediateOutputPath)

!IFDEF Cultures
$(Cultures):
!ENDIF

cultures: $(Cultures)
!IFDEF Cultures
	!@NMAKE /NOLOGO Configuration=$(Configuration) Culture=$**
!ENDIF

clean: $(Cultures)
!IFDEF Cultures
	!@NMAKE /NOLOGO Configuration=$(Configuration) Culture=$** clean
!ENDIF
	DEL $(OutputDll) $(IntermediateDll) $(NeutralRes) $(SpecificRes)

$(NeutralDll): $(RcConfig) Verbs.rc
	RC /nologo /q $(RcConfig) /fm $(SpecificRes) /fo $(NeutralRes) Verbs.rc
	LINK /NOLOGO /DLL /NOENTRY /MACHINE:X86 /OUT:$(IntermediateDll) $(NeutralRes)
!IF "$(Configuration)" == "Release"
	SIGNTOOL sign /fd SHA256 /t http://timestamp.comodoca.com $(IntermediateDll)
!ENDIF
	MOVE /Y $(IntermediateDll) $(NeutralDll)

!IFDEF Culture
$(OutputDll): $(NeutralDll) $(RcConfig) Verbs.$(Culture).rc
	RC /nologo /q $(RcConfig) /fm $(SpecificRes) /fo $(NeutralRes) Verbs.$(Culture).rc
	LINK /NOLOGO /DLL /NOENTRY /MACHINE:X86 /OUT:$(IntermediateDll) $(SpecificRes)
	MUIRCT -c $(NeutralDll) -e $(IntermediateDll)
!IF "$(Configuration)" == "Release"
	SIGNTOOL sign /fd SHA256 /t http://timestamp.comodoca.com $(IntermediateDll)
!ENDIF
	MOVE /Y $(IntermediateDll) $(OutputDll)
!ENDIF
