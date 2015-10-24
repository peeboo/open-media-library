# Introduction #

In many cases it is possible to use an existing installation of Microsoft SQL Server 2008 Service Pack 1 as the backend database of OML. There are a number of steps to be performed in order to set this up summarised below. This is aimed at advanced user so it doesn't go into the detail required for a novice user.

Steps

1. Create the OML database.

2. Create an OML Login in the database.

3. Assign login 'oml' to the database 'oml'.

4. Create the database schema.

5. Configure OML to use this database.


# Create the OML database #
1) Under 'Microsoft SQL Server 2008' in the start menu, click on 'SQL Server Management Studio'.

2) Click 'Connect' at the login prompt should sucessfully log you in to SQL.

3) Right Hand Click on 'Databases' and select 'New Database'.

4) Enter 'OML' as the database name. Click 'OK' to create database.

# Create an OML Login in the database #
1) Click the '+' symbol by 'Security' then again near 'Logins'.

2) Right Hand Click 'Logins' and select 'New Login'.

3) Enter 'oml' as the login name.

4) Select 'SQL Server authentication'.

5) Enter 'oml' in the 'Password' and 'Confirm password' boxes.

6) Untick 'Enforce password polocy', 'Enforce password expiration' and 'User must change password at next login'.

7) Change the default database to 'oml'.

8) Click 'OK'.

# Assign login 'oml' to the database 'oml' #

1) Double click 'oml' in the 'Logins' list.

2) Select 'User Mapping'.

3) Place a tick in the 'Map' column for the database 'oml'.

4) Tick the 'db\_owner' role.

5) Click 'OK'.

# Create the database schema #
1) In Windows Explorer, double click on 'Title Database.sql' under 'C:\Program Files \OpenMediaLibrary\SQLInstaller' in the OML source folder stucture.

2) SQL Server Management Studio should now be showing the SQL creation script. Look for a database selection drop down on a Management Studio Toolbar. Change this from 'master' to 'OML'.

3) Click 'Execute' in the tool bar. The database should now be created ready for use.

# Configure OML to use this database #

1) Open/create the OML settings.xml file (C:\ProgramData\OpenMediaLibrary\settings.xml)

2) Change settings to configure OML to use the database.

Example settings.xml file
```
<OML>
 <Settings>
  <SQLServerName>localhost</SQLServerName> 
  <SQLInstanceName>sqlexpress</SQLInstanceName> 
  <DatabaseName>oml</DatabaseName> 
  <SAPassword>omladmin</SAPassword> 
  <OMLUserAcct>oml</OMLUserAcct> 
  <OMLUserPassword>oml</OMLUserPassword> 
 </Settings>
</OML>
```