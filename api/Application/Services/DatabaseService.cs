using api.Core.Entities;
using api.Core.Interfaces;
using api.Models;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;

namespace api.Application.Services
{
    /// <summary>
    /// Service for database installation operations
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private readonly ILogger<DatabaseService> _logger;
        private readonly IRepository<ConnectionConfig> _repository;
        private readonly string[] _supportedDatabaseTypes = { "MySQL", "Postgres", "MSSQL", "Oracle" };
        
        public DatabaseService(ILogger<DatabaseService> logger, IRepository<ConnectionConfig> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetSupportedDatabaseTypes()
        {
            return _supportedDatabaseTypes;
        }

        /// <inheritdoc />
        public async Task<bool> CheckDatabaseOrUserExistsAsync(InstallRequest request)
        {
            try
            {
                string rootConnectionString = CreateConnectionString(
                    request.DbType,
                    request.ServiceIP,
                    request.Port,
                    GetDefaultDatabaseName(request.DbType),
                    request.RootUser,
                    request.RootPassword
                );

                _logger.LogInformation("Checking if database {Database} or user {User} exists", 
                    request.DbName, request.AdminUser);

                return await CheckDatabaseAndUserExistAsync(rootConnectionString, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if database or user exists");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> CreateDatabaseAndUserAsync(InstallRequest request)
        {
            try
            {
                // Get connection string for root user
                string rootConnectionString = CreateConnectionString(
                    request.DbType,
                    request.ServiceIP,
                    request.Port,
                    GetDefaultDatabaseName(request.DbType),
                    request.RootUser,
                    request.RootPassword
                );

                _logger.LogInformation("Creating database {Database} and user {User}", 
                    request.DbName, request.AdminUser);

                // Check if database or user already exists
                if (await CheckDatabaseAndUserExistAsync(rootConnectionString, request))
                {
                    _logger.LogWarning("Database {Database} or user {User} already exists", 
                        request.DbName, request.AdminUser);
                    return false;
                }

                // Create database and user
                await CreateDatabaseAndUserAsync(rootConnectionString, request);

                // Save the connection configuration
                var config = new ConnectionConfig
                {
                    Name = $"{request.DbName} on {request.ServiceIP}",
                    DbType = request.DbType,
                    ServerAddress = request.ServiceIP,
                    Port = request.Port,
                    DatabaseName = request.DbName,
                    Username = request.AdminUser,
                    IsActive = true
                };

                await _repository.AddAsync(config);

                _logger.LogInformation("Successfully created database {Database} and user {User}", 
                    request.DbName, request.AdminUser);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating database and user");
                throw;
            }
        }

        #region Private Helper Methods

        private string GetDefaultDatabaseName(string dbType)
        {
            return dbType.ToLower() switch
            {
                "mysql" => "mysql",
                "postgres" => "postgres",
                "mssql" => "master",
                "oracle" => "ORCL",
                _ => throw new ArgumentException($"Unsupported database type: {dbType}")
            };
        }

        private string CreateConnectionString(string dbType, string server, string port, string database, string username, string password)
        {
            return dbType.ToLower() switch
            {
                "mysql" => $"Server={server};Port={port};Database={database};User Id={username};Password={password};AllowUserVariables=True;Allow User Variables=True;SslMode=None;AllowPublicKeyRetrieval=true;",
                "postgres" => $"Host={server};Port={port};Database={database};Username={username};Password={password};",
                "mssql" => $"Server={server},{port};Database={database};User Id={username};Password={password};TrustServerCertificate=True;",
                "oracle" => $"Data Source={server}:{port}/{database};User Id={username};Password={password};",
                _ => throw new ArgumentException($"Unsupported database type: {dbType}")
            };
        }

        private async Task<bool> CheckDatabaseAndUserExistAsync(string rootConnectionString, InstallRequest request)
        {
            return request.DbType.ToLower() switch
            {
                "mysql" => await CheckMySQLDatabaseAndUserExistAsync(rootConnectionString, request),
                "postgres" => await CheckPostgresDatabaseAndUserExistAsync(rootConnectionString, request),
                "mssql" => await CheckMSSQLDatabaseAndUserExistAsync(rootConnectionString, request),
                "oracle" => await CheckOracleDatabaseAndUserExistAsync(rootConnectionString, request),
                _ => throw new ArgumentException($"Unsupported database type: {request.DbType}")
            };
        }

        private async Task CreateDatabaseAndUserAsync(string rootConnectionString, InstallRequest request)
        {
            switch (request.DbType.ToLower())
            {
                case "mysql":
                    await CreateMySQLDatabaseAndUserAsync(rootConnectionString, request);
                    break;
                case "postgres":
                    await CreatePostgresDatabaseAndUserAsync(rootConnectionString, request);
                    break;
                case "mssql":
                    await CreateMSSQLDatabaseAndUserAsync(rootConnectionString, request);
                    break;
                case "oracle":
                    await CreateOracleDatabaseAndUserAsync(rootConnectionString, request);
                    break;
                default:
                    throw new ArgumentException($"Unsupported database type: {request.DbType}");
            }
        }

        #region MySQL Implementation

        private async Task<bool> CheckMySQLDatabaseAndUserExistAsync(string rootConnectionString, InstallRequest request)
        {
            try
            {
                _logger.LogInformation("Connecting to MySQL with connection string: {ConnectionString}", 
                    rootConnectionString.Replace(request.RootPassword, "******"));
                
                using var connection = new MySqlConnection(rootConnectionString);
                
                _logger.LogInformation("Opening MySQL connection to server: {Server}:{Port}", 
                    request.ServiceIP, request.Port);
                    
                await connection.OpenAsync();
                _logger.LogInformation("Successfully connected to MySQL server");

                // Check if database exists
                using var checkDbCommand = new MySqlCommand(
                    $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{request.DbName}';",
                    connection
                );
                var dbExists = await checkDbCommand.ExecuteScalarAsync() != null;
                _logger.LogInformation("Database {DbName} exists: {Exists}", request.DbName, dbExists);

                // Check if user exists
                using var checkUserCommand = new MySqlCommand(
                    $"SELECT User FROM mysql.user WHERE User = '{request.AdminUser}';",
                    connection
                );
                var userExists = await checkUserCommand.ExecuteScalarAsync() != null;
                _logger.LogInformation("User {User} exists: {Exists}", request.AdminUser, userExists);

                return dbExists || userExists;
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "MySQL connection error: {ErrorCode}, {Message}", ex.Number, ex.Message);
                
                // Provide more detailed troubleshooting information
                if (ex.Message.Contains("Access denied"))
                {
                    _logger.LogError("Authentication failed - ensure the MySQL username and password are correct. " +
                                   "For MySQL 8+, you may need to use 'ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY 'password';'");
                }
                else if (ex.Message.Contains("Unable to connect"))
                {
                    _logger.LogError("Connection failed - ensure MySQL server is running on {Server}:{Port}", 
                        request.ServiceIP, request.Port);
                }
                
                throw;
            }
        }

        private async Task CreateMySQLDatabaseAndUserAsync(string rootConnectionString, InstallRequest request)
        {
            try
            {
                _logger.LogInformation("Connecting to MySQL to create database and user");
                
                using var connection = new MySqlConnection(rootConnectionString);
                await connection.OpenAsync();
                _logger.LogInformation("Successfully connected to MySQL server for database creation");

                // Create database with UTF8MB4 character set
                _logger.LogInformation("Creating MySQL database: {DbName}", request.DbName);
                using var createDbCommand = new MySqlCommand(
                    $"CREATE DATABASE IF NOT EXISTS `{request.DbName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;",
                    connection
                );
                await createDbCommand.ExecuteNonQueryAsync();
                _logger.LogInformation("Successfully created database: {DbName}", request.DbName);

                // Attempt to determine MySQL version to adapt user creation syntax
                string mysqlVersion = "unknown";
                try
                {
                    using var versionCommand = new MySqlCommand("SELECT VERSION();", connection);
                    mysqlVersion = (await versionCommand.ExecuteScalarAsync())?.ToString() ?? "unknown";
                    _logger.LogInformation("MySQL server version: {Version}", mysqlVersion);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to determine MySQL version");
                }

                // Temporarily disable password validation policy
                _logger.LogInformation("Checking current password validation policy");
                using var checkPolicyCommand = new MySqlCommand(
                    "SHOW VARIABLES LIKE 'validate_password%';",
                    connection
                );
                
                bool passwordPolicyEnabled = false;
                using var reader = await checkPolicyCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    string varName = reader.GetString(0);
                    string varValue = reader.GetString(1);
                    _logger.LogInformation("MySQL password policy setting: {Name} = {Value}", varName, varValue);
                    
                    if (varName.Equals("validate_password.policy", StringComparison.OrdinalIgnoreCase) || 
                        varName.Equals("validate_password_policy", StringComparison.OrdinalIgnoreCase))
                    {
                        passwordPolicyEnabled = !varValue.Equals("0", StringComparison.OrdinalIgnoreCase) && 
                                               !varValue.Equals("LOW", StringComparison.OrdinalIgnoreCase);
                    }
                }
                await reader.CloseAsync();
                
                // If password policy is enabled, try to temporarily set it to LOW
                if (passwordPolicyEnabled)
                {
                    _logger.LogInformation("Attempting to temporarily set password policy to LOW");
                    try
                    {
                        // First try the MySQL 8+ syntax
                        using var setPolicyCommand = new MySqlCommand(
                            "SET GLOBAL validate_password.policy = LOW;",
                            connection
                        );
                        await setPolicyCommand.ExecuteNonQueryAsync();
                        _logger.LogInformation("Successfully set password policy to LOW (MySQL 8+ syntax)");
                    }
                    catch
                    {
                        try
                        {
                            // Then try the MySQL 5.7 syntax
                            using var setPolicyCommand = new MySqlCommand(
                                "SET GLOBAL validate_password_policy = LOW;",
                                connection
                            );
                            await setPolicyCommand.ExecuteNonQueryAsync();
                            _logger.LogInformation("Successfully set password policy to LOW (MySQL 5.7 syntax)");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Could not modify password policy, proceeding with default policy");
                        }
                    }
                }
                
                // Create user with stronger password if needed
                string passwordToUse = request.AdminPassword;
                if (passwordPolicyEnabled && passwordToUse.Length < 8)
                {
                    // Strengthen the password if it's too weak
                    _logger.LogWarning("Password may not meet policy requirements, strengthening it");
                    passwordToUse = EnsureStrongPassword(passwordToUse);
                    _logger.LogInformation("Password strengthened to meet policy requirements");
                }

                // Create user and grant privileges - adjust syntax based on MySQL version
                string createUserSql;
                if (mysqlVersion.StartsWith("8.") || mysqlVersion.StartsWith("9."))
                {
                    // MySQL 8+ or 9+ syntax
                    _logger.LogInformation("Using MySQL 8+/9+ syntax for user creation");
                    createUserSql = 
                        // Create user for remote connections (%) and localhost 
                        $"CREATE USER IF NOT EXISTS '{request.AdminUser}'@'%' IDENTIFIED BY '{passwordToUse}';" +
                        $"CREATE USER IF NOT EXISTS '{request.AdminUser}'@'localhost' IDENTIFIED BY '{passwordToUse}';" +
                        
                        // Grant all privileges on the specific database (including all tables)
                        $"GRANT ALL PRIVILEGES ON `{request.DbName}`.* TO '{request.AdminUser}'@'%';" +
                        $"GRANT ALL PRIVILEGES ON `{request.DbName}`.* TO '{request.AdminUser}'@'localhost';" +
                        
                        // Grant additional global privileges useful for administration
                        $"GRANT CREATE USER, RELOAD, PROCESS ON *.* TO '{request.AdminUser}'@'%';" +
                        $"GRANT CREATE USER, RELOAD, PROCESS ON *.* TO '{request.AdminUser}'@'localhost';" +
                        
                        // Apply the changes
                        "FLUSH PRIVILEGES;";
                }
                else
                {
                    // MySQL 5.x syntax (more compatible)
                    _logger.LogInformation("Using MySQL 5.x syntax for user creation");
                    createUserSql = 
                        // Create user for remote connections (%) and localhost
                        $"CREATE USER IF NOT EXISTS '{request.AdminUser}'@'%' IDENTIFIED BY '{passwordToUse}';" +
                        $"CREATE USER IF NOT EXISTS '{request.AdminUser}'@'localhost' IDENTIFIED BY '{passwordToUse}';" +
                        
                        // Grant all privileges on the specific database (including all tables)
                        $"GRANT ALL PRIVILEGES ON `{request.DbName}`.* TO '{request.AdminUser}'@'%';" +
                        $"GRANT ALL PRIVILEGES ON `{request.DbName}`.* TO '{request.AdminUser}'@'localhost';" +
                        
                        // Grant additional global privileges useful for administration
                        $"GRANT CREATE USER, RELOAD, PROCESS ON *.* TO '{request.AdminUser}'@'%';" +
                        $"GRANT CREATE USER, RELOAD, PROCESS ON *.* TO '{request.AdminUser}'@'localhost';" +
                        
                        // Apply the changes
                        "FLUSH PRIVILEGES;";
                }

                _logger.LogInformation("Creating MySQL user: {User}", request.AdminUser);
                using var createUserCommand = new MySqlCommand(createUserSql, connection);
                await createUserCommand.ExecuteNonQueryAsync();
                _logger.LogInformation("Successfully created user: {User}", request.AdminUser);
                
                // If we modified the password, log a message about it
                if (passwordToUse != request.AdminPassword)
                {
                    _logger.LogWarning("Note: The password was modified to comply with MySQL password policy: {Password}", passwordToUse);
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "MySQL error creating database and user: {ErrorCode}, {Message}", ex.Number, ex.Message);
                
                if (ex.Message.Contains("Access denied"))
                {
                    _logger.LogError("Insufficient privileges - ensure the MySQL root user has CREATE DATABASE and CREATE USER privileges");
                }
                
                throw;
            }
        }

        #endregion

        #region PostgreSQL Implementation

        private async Task<bool> CheckPostgresDatabaseAndUserExistAsync(string rootConnectionString, InstallRequest request)
        {
            using var connection = new NpgsqlConnection(rootConnectionString);
            await connection.OpenAsync();

            // Check if database exists
            using var checkDbCommand = new NpgsqlCommand(
                $"SELECT 1 FROM pg_database WHERE datname = '{request.DbName}';",
                connection
            );
            var dbExists = await checkDbCommand.ExecuteScalarAsync() != null;

            // Check if user exists
            using var checkUserCommand = new NpgsqlCommand(
                $"SELECT 1 FROM pg_roles WHERE rolname = '{request.AdminUser}';",
                connection
            );
            var userExists = await checkUserCommand.ExecuteScalarAsync() != null;

            return dbExists || userExists;
        }

        private async Task CreatePostgresDatabaseAndUserAsync(string rootConnectionString, InstallRequest request)
        {
            using var connection = new NpgsqlConnection(rootConnectionString);
            await connection.OpenAsync();

            // Create user first (PostgreSQL requires this order)
            using var createUserCommand = new NpgsqlCommand(
                $"DO $$ BEGIN " +
                $"  IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = '{request.AdminUser}') THEN " +
                $"    CREATE USER \"{request.AdminUser}\" WITH PASSWORD '{request.AdminPassword}'; " +
                $"  END IF; " +
                $"END $$;",
                connection
            );
            await createUserCommand.ExecuteNonQueryAsync();

            // Create database with UTF8 encoding
            using var createDbCommand = new NpgsqlCommand(
                $"CREATE DATABASE \"{request.DbName}\" WITH OWNER = \"{request.AdminUser}\" ENCODING = 'UTF8' LC_COLLATE = 'en_US.UTF-8' LC_CTYPE = 'en_US.UTF-8';",
                connection
            );
            await createDbCommand.ExecuteNonQueryAsync();

            // Grant privileges
            using var grantCommand = new NpgsqlCommand(
                $"GRANT ALL PRIVILEGES ON DATABASE \"{request.DbName}\" TO \"{request.AdminUser}\";",
                connection
            );
            await grantCommand.ExecuteNonQueryAsync();
        }

        #endregion

        #region SQL Server Implementation

        private async Task<bool> CheckMSSQLDatabaseAndUserExistAsync(string rootConnectionString, InstallRequest request)
        {
            using var connection = new SqlConnection(rootConnectionString);
            await connection.OpenAsync();

            // Check if database exists
            using var checkDbCommand = new SqlCommand(
                $"SELECT 1 FROM sys.databases WHERE name = '{request.DbName}';",
                connection
            );
            var dbExists = await checkDbCommand.ExecuteScalarAsync() != null;

            // Check if user exists
            using var checkUserCommand = new SqlCommand(
                $"SELECT 1 FROM sys.server_principals WHERE name = '{request.AdminUser}';",
                connection
            );
            var userExists = await checkUserCommand.ExecuteScalarAsync() != null;

            return dbExists || userExists;
        }

        private async Task CreateMSSQLDatabaseAndUserAsync(string rootConnectionString, InstallRequest request)
        {
            using var connection = new SqlConnection(rootConnectionString);
            await connection.OpenAsync();

            // Create database with sensible collation
            using var createDbCommand = new SqlCommand(
                $"IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = '{request.DbName}') " +
                $"BEGIN " +
                $"  CREATE DATABASE [{request.DbName}] COLLATE SQL_Latin1_General_CP1_CI_AS; " +
                $"END",
                connection
            );
            await createDbCommand.ExecuteNonQueryAsync();

            // Create login if it doesn't exist
            using var createLoginCommand = new SqlCommand(
                $"IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = '{request.AdminUser}') " +
                $"BEGIN " +
                $"  CREATE LOGIN [{request.AdminUser}] WITH PASSWORD = '{request.AdminPassword}'; " +
                $"END",
                connection
            );
            await createLoginCommand.ExecuteNonQueryAsync();

            // Create user and add to db_owner role
            using var createUserCommand = new SqlCommand(
                $"USE [{request.DbName}]; " +
                $"IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = '{request.AdminUser}') " +
                $"BEGIN " +
                $"  CREATE USER [{request.AdminUser}] FOR LOGIN [{request.AdminUser}]; " +
                $"  ALTER ROLE [db_owner] ADD MEMBER [{request.AdminUser}]; " +
                $"END",
                connection
            );
            await createUserCommand.ExecuteNonQueryAsync();
        }

        #endregion

        #region Oracle Implementation

        private async Task<bool> CheckOracleDatabaseAndUserExistAsync(string rootConnectionString, InstallRequest request)
        {
            using var connection = new OracleConnection(rootConnectionString);
            await connection.OpenAsync();

            // Check if user exists (in Oracle, users and schemas are the same)
            using var checkUserCommand = new OracleCommand(
                $"SELECT 1 FROM dba_users WHERE username = '{request.AdminUser.ToUpper()}';",
                connection
            );
            var userExists = await checkUserCommand.ExecuteScalarAsync() != null;

            return userExists;
        }

        private async Task CreateOracleDatabaseAndUserAsync(string rootConnectionString, InstallRequest request)
        {
            using var connection = new OracleConnection(rootConnectionString);
            await connection.OpenAsync();

            // Create tablespace if it doesn't exist
            using var createTablespaceCommand = new OracleCommand(
                $"DECLARE " +
                $"  v_count NUMBER; " +
                $"BEGIN " +
                $"  SELECT COUNT(*) INTO v_count FROM dba_tablespaces WHERE tablespace_name = '{request.DbName}_TS'; " +
                $"  IF v_count = 0 THEN " +
                $"    EXECUTE IMMEDIATE 'CREATE TABLESPACE {request.DbName}_TS DATAFILE ''{request.DbName}.dbf'' SIZE 100M AUTOEXTEND ON'; " +
                $"  END IF; " +
                $"END;",
                connection
            );
            await createTablespaceCommand.ExecuteNonQueryAsync();

            // Create user with appropriate privileges
            using var createUserCommand = new OracleCommand(
                $"DECLARE " +
                $"  v_count NUMBER; " +
                $"BEGIN " +
                $"  SELECT COUNT(*) INTO v_count FROM dba_users WHERE username = '{request.AdminUser.ToUpper()}'; " +
                $"  IF v_count = 0 THEN " +
                $"    EXECUTE IMMEDIATE 'CREATE USER {request.AdminUser} IDENTIFIED BY \"{request.AdminPassword}\" DEFAULT TABLESPACE {request.DbName}_TS TEMPORARY TABLESPACE TEMP'; " +
                $"    EXECUTE IMMEDIATE 'GRANT CONNECT, RESOURCE, DBA TO {request.AdminUser}'; " +
                $"    EXECUTE IMMEDIATE 'ALTER USER {request.AdminUser} QUOTA UNLIMITED ON {request.DbName}_TS'; " +
                $"  END IF; " +
                $"END;",
                connection
            );
            await createUserCommand.ExecuteNonQueryAsync();
        }

        #endregion

        #region Helper Methods
        
        /// <summary>
        /// Ensures a password is strong enough to meet MySQL password policy requirements
        /// </summary>
        private string EnsureStrongPassword(string originalPassword)
        {
            // If password is already strong, return it
            if (IsStrongPassword(originalPassword))
            {
                return originalPassword;
            }
            
            // Start with the original password
            string strongPassword = originalPassword;
            
            // Ensure minimum length of 8 characters
            if (strongPassword.Length < 8)
            {
                strongPassword = strongPassword.PadRight(8, 'A');
            }
            
            // Ensure it has at least one uppercase letter
            if (!strongPassword.Any(char.IsUpper))
            {
                strongPassword += "A";
            }
            
            // Ensure it has at least one lowercase letter
            if (!strongPassword.Any(char.IsLower))
            {
                strongPassword += "a";
            }
            
            // Ensure it has at least one digit
            if (!strongPassword.Any(char.IsDigit))
            {
                strongPassword += "1";
            }
            
            // Ensure it has at least one special character
            if (!strongPassword.Any(c => !char.IsLetterOrDigit(c)))
            {
                strongPassword += "!";
            }
            
            return strongPassword;
        }
        
        /// <summary>
        /// Checks if a password meets the requirements for a strong password
        /// </summary>
        private bool IsStrongPassword(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(c => !char.IsLetterOrDigit(c));
        }
        
        #endregion

        #endregion
    }
}