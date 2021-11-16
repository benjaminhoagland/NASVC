
# $query = "select name,guid from script";
#Invoke-SqliteQuery -DataSource 'C:\NodeAlive\NASVC\NASVC\bin\Debug\NADB.sqlite' -Query $query | out-string

# $query = "select name, guid,alive  from node";
# Invoke-SqliteQuery -DataSource 'C:\NodeAlive\NASVC\NASVC\bin\Debug\NADB.sqlite' -Query $query | out-string

$query = "


select * from result
"; 
Invoke-SqliteQuery -DataSource 'C:\NodeAlive\NASVC\NASVC\bin\Debug\NADB.sqlite' -Query $query | out-string

exit;

$query = " 
Delete from location 
WHERE guid == '2211e516-209f-4510-8edf-81f9b3477506';
";
Invoke-SqliteQuery -DataSource 'C:\NodeAlive\NASVC\NASVC\bin\Debug\NADB.sqlite' -Query $query


