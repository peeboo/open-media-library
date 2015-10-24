# Introduction #

To use a client / server installation of Open Media Library certain firwall ports need opening.


# Details #

Firewall settings on the server for SQL Server

1. Open the 'Windows Firewall' setting and click 'Change settings'.

2. Select the 'Exceptions' tab.

3. Click 'Add Port' and enter the name 'SQL Server Browser', Port 1434 - Protocol 'UDP'. Click OK.

4. Click 'Add Program'. Click, browse to 'C:\Program Files\Microsoft SQL Server\MSSQL10.OML\MSSQL\Binn\sqlservr.exe'. Click 'Open' then clicl 'OK'.

NOTE: For an advanced dive into firewall issues please read this thread http://www.ornskov.dk/forum/index.php?topic=1342.0