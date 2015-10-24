# Install SQL Server and create the database Manually: #

The OML 'server' installers should automatically perform these steps.

1) Install SQL Server Express. To assist with the correct setup, there are 4 configuration scripts below the 'SQL Scripts' folder in the trunk source repository. Choose the most suitable for your requiments and 32/64 bit OS.

For example to install the x32 version with the admin tools, enter into a command prompt -

SQLEXPRWT\_x86\_ENU.exe /CONFIGURATIONFILE="c:\OML\OML\SQL Scripts\SQLConfigWithTools\_x32.ini"

NOTE - The exact command will depend on the installation file downloaded from microsoft and the location of the ini files.

2) Under 'Microsoft SQL Server 2008' in the start menu, click on 'SQL Server Management Studio'.

3) Click 'Connect' at the login prompt should sucessfully log you in to SQL.

4) Right Hand Click on 'Databases' and select 'New Database'.

5) Enter 'OML' as the database name. Click 'OK' to create database.

6) In Windows Explorer, double click on 'Title Database.sql' under 'SQL Scripts' in the OML source folder stucture.

7) SQL Server Management Studio should now be showing the SQL creation script. Look for a database selection drop down on a Management Studio Toolbar. Change this from 'master' to 'OML'.

8) Click 'Execute' in the tool bar. The database should now be created ready for use.