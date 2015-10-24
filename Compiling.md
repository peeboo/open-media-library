# Introduction #

A lot of people want to try out OML, but a public release isn't available yet so here you have the instructions on how to compile it yourself.

For a reference how to use the SVN look here:
http://internetducttape.com/2007/03/03/howto_google_code_hosting_subversion_tortoisesvn/


# Details #

Requirements
  * Visual Studio 2008 (the free version [Visual C# 2008 Express Edition](http://www.microsoft.com/express/vcsharp/) will also work)
  * Windows Installer 4.5 (http://www.microsoft.com/downloadS/details.aspx?familyid=5A58B56F-60B6-4412-95B9-54D056D6F9F4&displaylang=en). Required to install SQL Server in some cases.
  * Windows Powershell (required to install the SQL administration tools).
  * NUNIT (http://www.nunit.org/index.php?p=download)
  * WIX 3.0 (wix.sourceforge.net)
  * Media Center SDK 5.3 (http://www.microsoft.com/downloads/details.aspx?FamilyID=A43EA0B7-B85F-4612-AA08-3BF128C5873E&displaylang=en)
  * Dxperience (http://www.devexpress.com/go/DevExpressDownload_DXperience.aspx)
    * Go to this page, http://www.devexpress.com/Products/Free/WebRegistration/ to sign up and get the tools needed for free. This is not a must, but else you will get a popup.  If you continue to have the popup return, you can follow the steps in this link, http://www.devexpress.com/Support/Center/KB/p/A705.aspx to get rid of the popup.

SQLServer Requirements

There are many versions of SQL Server available, browse to http://www.microsoft.com/express/sql/download to download. If you are an advanced user or developer you should download the 'SQL Server 2008 Express with Tools' version and install it manually using the procedure described in http://code.google.com/p/open-media-library/wiki/InstallSQLServerManually. Otherwise if you are simply building the code to create an installer download the 'SQL Server 2008 Express (Runtime Only)' version and place in PostInstallerWizard\SQLInstallers. This allows creation of the installer with the SQL installer embedded within.



# Steps to setup your build environment: #

1) Install Visual Studio, Media Center SDK & NUNIT.

2) For SQL prerequisites, Install Windows Installer 4.5 & Windows Powershell



# Steps to build the code: #

1) Compile the project using Visual Studio 2008 . Build -> Rebuild Solution

Note: This has to be done on a pc with Vista Media Center and the Media Center SDK 5.3 installed.

2) using a cmd prompt go into the Library\Setup (or Setup64) dir and run Build.bat

Note: This will require wix 3.0 to be installed.

3) run the installer located in the Library\bin\Release folder.

4) Once you have the program installed you will have a post installation prompt appear. This will for server installs run through installing SQL and configure the databse or for client installs give the option to specify the server that your database is installed on.

5) The first time that you open up the DBEditor program after installing the program, it will create the database.  You may get an error about no user - if so close out and reenter.

At this point you should be good to go.  If your not seeing something similar to what has been posted please post the files located in \program data\openmedialibrary\logs and your \program files\openmedialibrary\oml.dat file on our forums and we'll take a look at whats going on.  (Note, it wont be a high priority, but we'll get to it).

# Troubleshooting #

**Q:** I get the following error compiling: Error 10 The command ""C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\..\..\..\Microsoft SDKs\Windows\v6.0\Bin\GacUtil.exe" /i "C:\OML\OMLEngine\bin\Release\OMLEngine.dll" /f" exited with code 3. OMLEngine

**A:** First try to run Visual Studio as an administrator. If that doesn't help, try to go to \program files\microsoft SDKs\Windows\ if the only folder there is 6.0A then open a command prompt and go to \program files\microsoft sdk\windows and input: mklink v6.0 v6.0a. Try compiling again, should help.

NOTE:  DVD Profiler plugin will not currently produce anything that can "play" movies.  If you want to play movies for now I'd stick with the mymovies export file or movie collectorz untill DVD Profiler is fixed. The Movie Collectorz imported was tested with version 5.5 build 2.