@echo off
cd %CD%\..\..\..\..\..
copy /Y "%CD%\TestSettings\Nightly\Win7\x86\Integration.testsettings" "%CD%\Integration.testsettings"
"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\MSTest.exe" /testcontainer:"%CD%\TestBinaries\Dev2.IntegrationTests.dll" /testSettings:"Integration.testsettings"
pause