using api.Core.Entities.SaaS;
using api.Models;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;

namespace api.Application.Services
{
    /// <summary>
    /// Service for setting up SAAS tables in client databases
    /// </summary>
    public class SaasSetupService
    {
        private readonly ILogger<SaasSetupService> _logger;
        
        public SaasSetupService(ILogger<SaasSetupService> logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Sets up the SAAS tables in the specified database
        /// </summary>
        /// <param name="request">The installation request</param>
        /// <returns>True if setup was successful</returns>
        public async Task<bool> SetupSaasTables(InstallRequest request)
        {
            try
            {
                _logger.LogInformation("Setting up SAAS tables for database {Database}", request.DbName);
                
                // Create connection string for the newly created database
                string connectionString = CreateConnectionString(
                    request.DbType,
                    request.ServiceIP,
                    request.Port,
                    request.DbName,
                    request.AdminUser,
                    request.AdminPassword
                );
                
                // Create tables based on the database type
                bool success = await CreateSaasTables(request.DbType, connectionString);
                
                if (success)
                {
                    _logger.LogInformation("Successfully set up SAAS tables in database {Database}", request.DbName);
                }
                else
                {
                    _logger.LogError("Failed to set up SAAS tables in database {Database}", request.DbName);
                }
                
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up SAAS tables for database {Database}", request.DbName);
                throw;
            }
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
        
        private async Task<bool> CreateSaasTables(string dbType, string connectionString)
        {
            return dbType.ToLower() switch
            {
                "mysql" => await CreateMySqlTables(connectionString),
                "postgres" => await CreatePostgresTables(connectionString),
                "mssql" => await CreateMsSqlTables(connectionString),
                "oracle" => await CreateOracleTables(connectionString),
                _ => throw new ArgumentException($"Unsupported database type: {dbType}")
            };
        }
        
        private async Task<bool> CreateMySqlTables(string connectionString)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            
            try
            {
                // Create clients table
                using (var command = new MySqlCommand(GetMySqlClientsTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_clients table");
                }
                
                // Create users_ref table
                using (var command = new MySqlCommand(GetMySqlUsersRefTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_users_ref table");
                }
                
                // Create plans table
                using (var command = new MySqlCommand(GetMySqlPlansTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_plans table");
                }
                
                // Create client_plans table
                using (var command = new MySqlCommand(GetMySqlClientPlansTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_client_plans table");
                }
                
                // Create client_renewals table
                using (var command = new MySqlCommand(GetMySqlClientRenewalsTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_client_renewals table");
                }
                
                // Create payment_methods table
                using (var command = new MySqlCommand(GetMySqlPaymentMethodsTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_payment_methods table");
                }
                
                // Create billing_records table
                using (var command = new MySqlCommand(GetMySqlBillingRecordsTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_billing_records table");
                }
                
                // Insert default plan
                using (var command = new MySqlCommand(GetMySqlDefaultPlanSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Inserted default plans");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating MySQL tables");
                return false;
            }
        }
        
        private async Task<bool> CreatePostgresTables(string connectionString)
        {
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            
            try
            {
                // Create clients table
                using (var command = new NpgsqlCommand(GetPostgresClientsTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_clients table");
                }
                
                // Create users_ref table
                using (var command = new NpgsqlCommand(GetPostgresUsersRefTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_users_ref table");
                }
                
                // Create plans table
                using (var command = new NpgsqlCommand(GetPostgresPlansTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_plans table");
                }
                
                // Create client_plans table
                using (var command = new NpgsqlCommand(GetPostgresClientPlansTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_client_plans table");
                }
                
                // Create client_renewals table
                using (var command = new NpgsqlCommand(GetPostgresClientRenewalsTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_client_renewals table");
                }
                
                // Create payment_methods table
                using (var command = new NpgsqlCommand(GetPostgresPaymentMethodsTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_payment_methods table");
                }
                
                // Insert default plan
                using (var command = new NpgsqlCommand(GetPostgresDefaultPlanSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Inserted default plans");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PostgreSQL tables");
                return false;
            }
        }
        
        private async Task<bool> CreateMsSqlTables(string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            
            try
            {
                // Create clients table
                using (var command = new SqlCommand(GetMsSqlClientsTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_clients table");
                }
                
                // Create users_ref table
                using (var command = new SqlCommand(GetMsSqlUsersRefTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_users_ref table");
                }
                
                // Create plans table
                using (var command = new SqlCommand(GetMsSqlPlansTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_plans table");
                }
                
                // Create client_plans table
                using (var command = new SqlCommand(GetMsSqlClientPlansTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_client_plans table");
                }
                
                // Create client_renewals table
                using (var command = new SqlCommand(GetMsSqlClientRenewalsTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_client_renewals table");
                }
                
                // Create payment_methods table
                using (var command = new SqlCommand(GetMsSqlPaymentMethodsTableSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_payment_methods table");
                }
                
                // Insert default plan
                using (var command = new SqlCommand(GetMsSqlDefaultPlanSql(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Inserted default plans");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating MS SQL tables");
                return false;
            }
        }
        
        private async Task<bool> CreateOracleTables(string connectionString)
        {
            using var connection = new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString);
            await connection.OpenAsync();
            try
            {
                // Create clients table
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetOracleClientsTableSql();
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_clients table (Oracle)");
                }
                
                // Create users_ref table
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetOracleUsersRefTableSql();
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_users_ref table (Oracle)");
                }
                
                // Create plans table
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetOraclePlansTableSql();
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_plans table (Oracle)");
                }
                
                // Create client_plans table
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetOracleClientPlansTableSql();
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_client_plans table (Oracle)");
                }
                
                // Create client_renewals table
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetOracleClientRenewalsTableSql();
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_client_renewals table (Oracle)");
                }
                
                // Create payment_methods table
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetOraclePaymentMethodsTableSql();
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_payment_methods table (Oracle)");
                }
                
                // Create billing_records table
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetOracleBillingRecordsTableSql();
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Created cor_billing_records table (Oracle)");
                }
                
                // Insert default plan
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = GetOracleDefaultPlanSql();
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Inserted default plans (Oracle)");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Oracle tables");
                return false;
            }
        }
        
        #region SQL Generation Methods
        
        private string GetMySqlClientsTableSql()
        {
            return @"
                CREATE TABLE IF NOT EXISTS `cor_clients` (
                    `Id` CHAR(36) NOT NULL,
                    `CompanyName` VARCHAR(100) NOT NULL,
                    `VatNumber` VARCHAR(50) NULL,
                    `CountryCode` VARCHAR(2) NOT NULL,
                    `BillingAddressLine1` VARCHAR(100) NULL,
                    `BillingAddressLine2` VARCHAR(100) NULL,
                    `City` VARCHAR(50) NULL,
                    `State` VARCHAR(50) NULL,
                    `PostalCode` VARCHAR(20) NULL,
                    `Email` VARCHAR(100) NOT NULL,
                    `Phone` VARCHAR(20) NULL,
                    `Website` VARCHAR(100) NULL,
                    `Status` INT NOT NULL DEFAULT 0,
                    `DomainUrl` VARCHAR(100) NULL,
                    `DatabaseName` VARCHAR(50) NULL,
                    `Notes` VARCHAR(1000) NULL,
                    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (`Id`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
            ";
        }
        
        private string GetMySqlUsersRefTableSql()
        {
            return @"
                CREATE TABLE IF NOT EXISTS `cor_users_ref` (
                    `Id` CHAR(36) NOT NULL,
                    `ClientId` CHAR(36) NOT NULL,
                    `FirstName` VARCHAR(50) NOT NULL,
                    `LastName` VARCHAR(50) NOT NULL,
                    `Email` VARCHAR(100) NOT NULL,
                    `Phone` VARCHAR(20) NULL,
                    `Role` INT NOT NULL DEFAULT 0,
                    `PasswordHash` VARCHAR(100) NOT NULL,
                    `PasswordSalt` VARCHAR(50) NOT NULL,
                    `IsActive` BIT NOT NULL DEFAULT 1,
                    `LastLoginAt` DATETIME NULL,
                    `PreferredLanguage` VARCHAR(10) NOT NULL DEFAULT 'en-US',
                    `PreferredTheme` VARCHAR(20) NOT NULL DEFAULT 'light',
                    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (`Id`),
                    UNIQUE INDEX `IX_cor_users_ref_Email` (`Email`),
                    INDEX `IX_cor_users_ref_ClientId` (`ClientId`),
                    CONSTRAINT `FK_cor_users_ref_cor_clients_ClientId`
                        FOREIGN KEY (`ClientId`)
                        REFERENCES `cor_clients` (`Id`)
                        ON DELETE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
            ";
        }
        
        private string GetMySqlPlansTableSql()
        {
            return @"
                CREATE TABLE IF NOT EXISTS `cor_plans` (
                    `Id` CHAR(36) NOT NULL,
                    `Name` VARCHAR(50) NOT NULL,
                    `Description` VARCHAR(500) NULL,
                    `MonthlyPrice` DECIMAL(18,2) NOT NULL,
                    `AnnualPrice` DECIMAL(18,2) NOT NULL,
                    `MaxUsers` INT NOT NULL DEFAULT 0,
                    `MaxStorageGB` INT NOT NULL DEFAULT 0,
                    `Features` VARCHAR(1000) NULL,
                    `IsActive` BIT NOT NULL DEFAULT 1,
                    `DisplayOrder` INT NOT NULL DEFAULT 0,
                    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (`Id`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
            ";
        }
        
        private string GetMySqlClientPlansTableSql()
        {
            return @"
                CREATE TABLE IF NOT EXISTS `cor_client_plans` (
                    `Id` CHAR(36) NOT NULL,
                    `ClientId` CHAR(36) NOT NULL,
                    `PlanId` CHAR(36) NOT NULL,
                    `StartDate` DATETIME NOT NULL,
                    `EndDate` DATETIME NOT NULL,
                    `BillingCycle` INT NOT NULL DEFAULT 0,
                    `Price` DECIMAL(18,2) NOT NULL,
                    `IsActive` BIT NOT NULL DEFAULT 1,
                    `AutoRenew` BIT NOT NULL DEFAULT 1,
                    `Notes` VARCHAR(500) NULL,
                    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (`Id`),
                    INDEX `IX_cor_client_plans_ClientId` (`ClientId`),
                    INDEX `IX_cor_client_plans_PlanId` (`PlanId`),
                    CONSTRAINT `FK_cor_client_plans_cor_clients_ClientId`
                        FOREIGN KEY (`ClientId`)
                        REFERENCES `cor_clients` (`Id`)
                        ON DELETE CASCADE,
                    CONSTRAINT `FK_cor_client_plans_cor_plans_PlanId`
                        FOREIGN KEY (`PlanId`)
                        REFERENCES `cor_plans` (`Id`)
                        ON DELETE RESTRICT
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
            ";
        }
        
        private string GetMySqlClientRenewalsTableSql()
        {
            return @"
                CREATE TABLE IF NOT EXISTS `cor_client_renewals` (
                    `Id` CHAR(36) NOT NULL,
                    `ClientId` CHAR(36) NOT NULL,
                    `ClientPlanId` CHAR(36) NOT NULL,
                    `PreviousEndDate` DATETIME NOT NULL,
                    `NewStartDate` DATETIME NOT NULL,
                    `NewEndDate` DATETIME NOT NULL,
                    `Amount` DECIMAL(18,2) NOT NULL,
                    `PaymentDate` DATETIME NULL,
                    `PaymentMethod` VARCHAR(50) NULL,
                    `TransactionReference` VARCHAR(100) NULL,
                    `Status` INT NOT NULL DEFAULT 0,
                    `Notes` VARCHAR(500) NULL,
                    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (`Id`),
                    INDEX `IX_cor_client_renewals_ClientId` (`ClientId`),
                    INDEX `IX_cor_client_renewals_ClientPlanId` (`ClientPlanId`),
                    CONSTRAINT `FK_cor_client_renewals_cor_clients_ClientId`
                        FOREIGN KEY (`ClientId`)
                        REFERENCES `cor_clients` (`Id`)
                        ON DELETE CASCADE,
                    CONSTRAINT `FK_cor_client_renewals_cor_client_plans_ClientPlanId`
                        FOREIGN KEY (`ClientPlanId`)
                        REFERENCES `cor_client_plans` (`Id`)
                        ON DELETE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
            ";
        }
        
        private string GetMySqlPaymentMethodsTableSql()
        {
            return @"
                CREATE TABLE IF NOT EXISTS `cor_payment_methods` (
                    `Id` CHAR(36) NOT NULL,
                    `ClientId` CHAR(36) NOT NULL,
                    `Type` INT NOT NULL DEFAULT 0,
                    `Last4` VARCHAR(4) NULL,
                    `CardBrand` VARCHAR(20) NULL,
                    `ExpiryMonth` INT NULL,
                    `ExpiryYear` INT NULL,
                    `CardholderName` VARCHAR(100) NULL,
                    `Token` VARCHAR(500) NULL,
                    `IsDefault` BIT NOT NULL DEFAULT 0,
                    `IsActive` BIT NOT NULL DEFAULT 1,
                    `Notes` VARCHAR(500) NULL,
                    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (`Id`),
                    INDEX `IX_cor_payment_methods_ClientId` (`ClientId`),
                    CONSTRAINT `FK_cor_payment_methods_cor_clients_ClientId`
                        FOREIGN KEY (`ClientId`)
                        REFERENCES `cor_clients` (`Id`)
                        ON DELETE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
            ";
        }
        
        private string GetMySqlBillingRecordsTableSql()
        {
            return @"
                CREATE TABLE IF NOT EXISTS cor_billing_records (
                    Id CHAR(36) NOT NULL PRIMARY KEY,
                    ClientId CHAR(36) NOT NULL,
                    InvoiceNumber VARCHAR(50) NOT NULL UNIQUE,
                    Amount DECIMAL(18,2) NOT NULL,
                    Currency VARCHAR(10) NOT NULL,
                    BillingPeriodStart DATETIME NOT NULL,
                    BillingPeriodEnd DATETIME NOT NULL,
                    Status INT NOT NULL,
                    Description VARCHAR(255),
                    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PaidAt DATETIME,
                    PaymentMethod VARCHAR(50),
                    Notes VARCHAR(255),
                    CONSTRAINT FK_Billing_Client FOREIGN KEY (ClientId) REFERENCES cor_clients(Id) ON DELETE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
            ";
        }
        
        private string GetMySqlDefaultPlanSql()
        {
            Guid basicPlanId = Guid.NewGuid();
            Guid standardPlanId = Guid.NewGuid();
            Guid premiumPlanId = Guid.NewGuid();
            
            string basicFeatures = "{\"feature1\": true, \"feature2\": true, \"feature3\": false, \"feature4\": false}";
            string standardFeatures = "{\"feature1\": true, \"feature2\": true, \"feature3\": true, \"feature4\": false}";
            string premiumFeatures = "{\"feature1\": true, \"feature2\": true, \"feature3\": true, \"feature4\": true}";
            
            return $@"
                INSERT INTO cor_plans (Id, Name, Description, MonthlyPrice, AnnualPrice, MaxUsers, MaxStorageGB, Features, IsActive, DisplayOrder)
                VALUES 
                ('{basicPlanId}', 'Basic', 'Essential features for small businesses', 29.99, 299.99, 10, 5, '{basicFeatures}', 1, 1),
                ('{standardPlanId}', 'Standard', 'Advanced features for growing businesses', 79.99, 799.99, 50, 25, '{standardFeatures}', 1, 2),
                ('{premiumPlanId}', 'Premium', 'Full features for enterprises', 149.99, 1499.99, 100, 100, '{premiumFeatures}', 1, 3);
            ";
        }
        
        // PostgreSQL SQL generation methods
        
        private string GetPostgresClientsTableSql()
        {
            return @"
                CREATE TABLE IF NOT EXISTS cor_clients (
                    ""Id"" UUID NOT NULL,
                    ""CompanyName"" VARCHAR(100) NOT NULL,
                    ""VatNumber"" VARCHAR(50) NULL,
                    ""CountryCode"" VARCHAR(2) NOT NULL,
                    ""BillingAddressLine1"" VARCHAR(100) NULL,
                    ""BillingAddressLine2"" VARCHAR(100) NULL,
                    ""City"" VARCHAR(50) NULL,
                    ""State"" VARCHAR(50) NULL,
                    ""PostalCode"" VARCHAR(20) NULL,
                    ""Email"" VARCHAR(100) NOT NULL,
                    ""Phone"" VARCHAR(20) NULL,
                    ""Website"" VARCHAR(100) NULL,
                    ""Status"" INTEGER NOT NULL DEFAULT 0,
                    ""DomainUrl"" VARCHAR(100) NULL,
                    ""DatabaseName"" VARCHAR(50) NULL,
                    ""Notes"" VARCHAR(1000) NULL,
                    ""CreatedAt"" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    CONSTRAINT ""PK_cor_clients"" PRIMARY KEY (""Id"")
                );
            ";
        }
        
        private string GetPostgresUsersRefTableSql()
        {
            return @"
                CREATE TABLE IF NOT EXISTS cor_users_ref (
                    ""Id"" UUID NOT NULL,
                    ""ClientId"" UUID NOT NULL,
                    ""FirstName"" VARCHAR(50) NOT NULL,
                    ""LastName"" VARCHAR(50) NOT NULL,
                    ""Email"" VARCHAR(100) NOT NULL,
                    ""Phone"" VARCHAR(20) NULL,
                    ""Role"" INTEGER NOT NULL DEFAULT 0,
                    ""PasswordHash"" VARCHAR(100) NOT NULL,
                    ""PasswordSalt"" VARCHAR(50) NOT NULL,
                    ""IsActive"" BOOLEAN NOT NULL DEFAULT TRUE,
                    ""LastLoginAt"" TIMESTAMP NULL,
                    ""PreferredLanguage"" VARCHAR(10) NOT NULL DEFAULT 'en-US',
                    ""PreferredTheme"" VARCHAR(20) NOT NULL DEFAULT 'light',
                    ""CreatedAt"" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    CONSTRAINT ""PK_cor_users_ref"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_cor_users_ref_cor_clients_ClientId"" FOREIGN KEY (""ClientId"") REFERENCES cor_clients (""Id"") ON DELETE CASCADE
                );
                
                CREATE UNIQUE INDEX IF NOT EXISTS ""IX_cor_users_ref_Email"" ON cor_users_ref (""Email"");
                CREATE INDEX IF NOT EXISTS ""IX_cor_users_ref_ClientId"" ON cor_users_ref (""ClientId"");
            ";
        }
        
        private string GetPostgresPlansTableSql()
        {
            return @"
                CREATE TABLE IF NOT EXISTS cor_plans (
                    ""Id"" UUID NOT NULL,
                    ""Name"" VARCHAR(50) NOT NULL,
                    ""Description"" VARCHAR(500) NULL,
                    ""MonthlyPrice"" DECIMAL(18,2) NOT NULL,
                    ""AnnualPrice"" DECIMAL(18,2) NOT NULL,
                    ""MaxUsers"" INTEGER NOT NULL DEFAULT 0,
                    ""MaxStorageGB"" INTEGER NOT NULL DEFAULT 0,
                    ""Features"" VARCHAR(1000) NULL,
                    ""IsActive"" BOOLEAN NOT NULL DEFAULT TRUE,
                    ""DisplayOrder"" INTEGER NOT NULL DEFAULT 0,
                    ""CreatedAt"" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    CONSTRAINT ""PK_cor_plans"" PRIMARY KEY (""Id"")
                );
            ";
        }
        
        private string GetPostgresClientPlansTableSql()
        {
            return @"
                CREATE TABLE IF NOT EXISTS cor_client_plans (
                    ""Id"" UUID NOT NULL,
                    ""ClientId"" UUID NOT NULL,
                    ""PlanId"" UUID NOT NULL,
                    ""StartDate"" TIMESTAMP NOT NULL,
                    ""EndDate"" TIMESTAMP NOT NULL,
                    ""BillingCycle"" INTEGER NOT NULL DEFAULT 0,
                    ""Price"" DECIMAL(18,2) NOT NULL,
                    ""IsActive"" BOOLEAN NOT NULL DEFAULT TRUE,
                    ""AutoRenew"" BOOLEAN NOT NULL DEFAULT TRUE,
                    ""Notes"" VARCHAR(500) NULL,
                    ""CreatedAt"" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    CONSTRAINT ""PK_cor_client_plans"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_cor_client_plans_cor_clients_ClientId"" FOREIGN KEY (""ClientId"") REFERENCES cor_clients (""Id"") ON DELETE CASCADE,
                    CONSTRAINT ""FK_cor_client_plans_cor_plans_PlanId"" FOREIGN KEY (""PlanId"") REFERENCES cor_plans (""Id"") ON DELETE RESTRICT
                );
                
                CREATE INDEX IF NOT EXISTS ""IX_cor_client_plans_ClientId"" ON cor_client_plans (""ClientId"");
                CREATE INDEX IF NOT EXISTS ""IX_cor_client_plans_PlanId"" ON cor_client_plans (""PlanId"");
            ";
        }
        
        private string GetPostgresClientRenewalsTableSql()
        {
            return @"
                CREATE TABLE IF NOT EXISTS cor_client_renewals (
                    ""Id"" UUID NOT NULL,
                    ""ClientId"" UUID NOT NULL,
                    ""ClientPlanId"" UUID NOT NULL,
                    ""PreviousEndDate"" TIMESTAMP NOT NULL,
                    ""NewStartDate"" TIMESTAMP NOT NULL,
                    ""NewEndDate"" TIMESTAMP NOT NULL,
                    ""Amount"" DECIMAL(18,2) NOT NULL,
                    ""PaymentDate"" TIMESTAMP NULL,
                    ""PaymentMethod"" VARCHAR(50) NULL,
                    ""TransactionReference"" VARCHAR(100) NULL,
                    ""Status"" INTEGER NOT NULL DEFAULT 0,
                    ""Notes"" VARCHAR(500) NULL,
                    ""CreatedAt"" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    CONSTRAINT ""PK_cor_client_renewals"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_cor_client_renewals_cor_clients_ClientId"" FOREIGN KEY (""ClientId"") REFERENCES cor_clients (""Id"") ON DELETE CASCADE,
                    CONSTRAINT ""FK_cor_client_renewals_cor_client_plans_ClientPlanId"" FOREIGN KEY (""ClientPlanId"") REFERENCES cor_client_plans (""Id"") ON DELETE CASCADE
                );
                
                CREATE INDEX IF NOT EXISTS ""IX_cor_client_renewals_ClientId"" ON cor_client_renewals (""ClientId"");
                CREATE INDEX IF NOT EXISTS ""IX_cor_client_renewals_ClientPlanId"" ON cor_client_renewals (""ClientPlanId"");
            ";
        }
        
        private string GetPostgresPaymentMethodsTableSql()
        {
            return @"
                CREATE TABLE IF NOT EXISTS cor_payment_methods (
                    ""Id"" UUID NOT NULL,
                    ""ClientId"" UUID NOT NULL,
                    ""Type"" INTEGER NOT NULL DEFAULT 0,
                    ""Last4"" VARCHAR(4) NULL,
                    ""CardBrand"" VARCHAR(20) NULL,
                    ""ExpiryMonth"" INTEGER NULL,
                    ""ExpiryYear"" INTEGER NULL,
                    ""CardholderName"" VARCHAR(100) NULL,
                    ""Token"" VARCHAR(500) NULL,
                    ""IsDefault"" BOOLEAN NOT NULL DEFAULT FALSE,
                    ""IsActive"" BOOLEAN NOT NULL DEFAULT TRUE,
                    ""Notes"" VARCHAR(500) NULL,
                    ""CreatedAt"" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    CONSTRAINT ""PK_cor_payment_methods"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_cor_payment_methods_cor_clients_ClientId"" FOREIGN KEY (""ClientId"") REFERENCES cor_clients (""Id"") ON DELETE CASCADE
                );
                
                CREATE INDEX IF NOT EXISTS ""IX_cor_payment_methods_ClientId"" ON cor_payment_methods (""ClientId"");
            ";
        }
        
        private string GetPostgresDefaultPlanSql()
        {
            Guid basicPlanId = Guid.NewGuid();
            Guid standardPlanId = Guid.NewGuid();
            Guid premiumPlanId = Guid.NewGuid();
            
            string basicFeatures = "{\"feature1\": true, \"feature2\": true, \"feature3\": false, \"feature4\": false}";
            string standardFeatures = "{\"feature1\": true, \"feature2\": true, \"feature3\": true, \"feature4\": false}";
            string premiumFeatures = "{\"feature1\": true, \"feature2\": true, \"feature3\": true, \"feature4\": true}";
            
            return $@"
                INSERT INTO cor_plans (""Id"", ""Name"", ""Description"", ""MonthlyPrice"", ""AnnualPrice"", ""MaxUsers"", ""MaxStorageGB"", ""Features"", ""IsActive"", ""DisplayOrder"")
                VALUES 
                ('{basicPlanId}', 'Basic', 'Essential features for small businesses', 29.99, 299.99, 10, 5, '{basicFeatures}', TRUE, 1),
                ('{standardPlanId}', 'Standard', 'Advanced features for growing businesses', 79.99, 799.99, 50, 25, '{standardFeatures}', TRUE, 2),
                ('{premiumPlanId}', 'Premium', 'Full features for enterprises', 149.99, 1499.99, 100, 100, '{premiumFeatures}', TRUE, 3);
            ";
        }
        
        // MS SQL Server SQL generation methods
        
        private string GetMsSqlClientsTableSql()
        {
            return @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cor_clients')
                BEGIN
                    CREATE TABLE cor_clients (
                        Id UNIQUEIDENTIFIER NOT NULL,
                        CompanyName NVARCHAR(100) NOT NULL,
                        VatNumber NVARCHAR(50) NULL,
                        CountryCode NVARCHAR(2) NOT NULL,
                        BillingAddressLine1 NVARCHAR(100) NULL,
                        BillingAddressLine2 NVARCHAR(100) NULL,
                        City NVARCHAR(50) NULL,
                        State NVARCHAR(50) NULL,
                        PostalCode NVARCHAR(20) NULL,
                        Email NVARCHAR(100) NOT NULL,
                        Phone NVARCHAR(20) NULL,
                        Website NVARCHAR(100) NULL,
                        Status INT NOT NULL DEFAULT 0,
                        DomainUrl NVARCHAR(100) NULL,
                        DatabaseName NVARCHAR(50) NULL,
                        Notes NVARCHAR(1000) NULL,
                        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT PK_cor_clients PRIMARY KEY (Id)
                    );
                END
            ";
        }
        
        private string GetMsSqlUsersRefTableSql()
        {
            return @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cor_users_ref')
                BEGIN
                    CREATE TABLE cor_users_ref (
                        Id UNIQUEIDENTIFIER NOT NULL,
                        ClientId UNIQUEIDENTIFIER NOT NULL,
                        FirstName NVARCHAR(50) NOT NULL,
                        LastName NVARCHAR(50) NOT NULL,
                        Email NVARCHAR(100) NOT NULL,
                        Phone NVARCHAR(20) NULL,
                        Role INT NOT NULL DEFAULT 0,
                        PasswordHash NVARCHAR(100) NOT NULL,
                        PasswordSalt NVARCHAR(50) NOT NULL,
                        IsActive BIT NOT NULL DEFAULT 1,
                        LastLoginAt DATETIME2 NULL,
                        PreferredLanguage NVARCHAR(10) NOT NULL DEFAULT 'en-US',
                        PreferredTheme NVARCHAR(20) NOT NULL DEFAULT 'light',
                        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT PK_cor_users_ref PRIMARY KEY (Id),
                        CONSTRAINT FK_cor_users_ref_cor_clients_ClientId FOREIGN KEY (ClientId) REFERENCES cor_clients (Id) ON DELETE CASCADE
                    );
                    
                    CREATE UNIQUE INDEX IX_cor_users_ref_Email ON cor_users_ref (Email);
                    CREATE INDEX IX_cor_users_ref_ClientId ON cor_users_ref (ClientId);
                END
            ";
        }
        
        private string GetMsSqlPlansTableSql()
        {
            return @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cor_plans')
                BEGIN
                    CREATE TABLE cor_plans (
                        Id UNIQUEIDENTIFIER NOT NULL,
                        Name NVARCHAR(50) NOT NULL,
                        Description NVARCHAR(500) NULL,
                        MonthlyPrice DECIMAL(18,2) NOT NULL,
                        AnnualPrice DECIMAL(18,2) NOT NULL,
                        MaxUsers INT NOT NULL DEFAULT 0,
                        MaxStorageGB INT NOT NULL DEFAULT 0,
                        Features NVARCHAR(1000) NULL,
                        IsActive BIT NOT NULL DEFAULT 1,
                        DisplayOrder INT NOT NULL DEFAULT 0,
                        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT PK_cor_plans PRIMARY KEY (Id)
                    );
                END
            ";
        }
        
        private string GetMsSqlClientPlansTableSql()
        {
            return @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cor_client_plans')
                BEGIN
                    CREATE TABLE cor_client_plans (
                        Id UNIQUEIDENTIFIER NOT NULL,
                        ClientId UNIQUEIDENTIFIER NOT NULL,
                        PlanId UNIQUEIDENTIFIER NOT NULL,
                        StartDate DATETIME2 NOT NULL,
                        EndDate DATETIME2 NOT NULL,
                        BillingCycle INT NOT NULL DEFAULT 0,
                        Price DECIMAL(18,2) NOT NULL,
                        IsActive BIT NOT NULL DEFAULT 1,
                        AutoRenew BIT NOT NULL DEFAULT 1,
                        Notes NVARCHAR(500) NULL,
                        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT PK_cor_client_plans PRIMARY KEY (Id),
                        CONSTRAINT FK_cor_client_plans_cor_clients_ClientId FOREIGN KEY (ClientId) REFERENCES cor_clients (Id) ON DELETE CASCADE,
                        CONSTRAINT FK_cor_client_plans_cor_plans_PlanId FOREIGN KEY (PlanId) REFERENCES cor_plans (Id)
                    );
                    
                    CREATE INDEX IX_cor_client_plans_ClientId ON cor_client_plans (ClientId);
                    CREATE INDEX IX_cor_client_plans_PlanId ON cor_client_plans (PlanId);
                END
            ";
        }
        
        private string GetMsSqlClientRenewalsTableSql()
        {
            return @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cor_client_renewals')
                BEGIN
                    CREATE TABLE cor_client_renewals (
                        Id UNIQUEIDENTIFIER NOT NULL,
                        ClientId UNIQUEIDENTIFIER NOT NULL,
                        ClientPlanId UNIQUEIDENTIFIER NOT NULL,
                        PreviousEndDate DATETIME2 NOT NULL,
                        NewStartDate DATETIME2 NOT NULL,
                        NewEndDate DATETIME2 NOT NULL,
                        Amount DECIMAL(18,2) NOT NULL,
                        PaymentDate DATETIME2 NULL,
                        PaymentMethod NVARCHAR(50) NULL,
                        TransactionReference NVARCHAR(100) NULL,
                        Status INT NOT NULL DEFAULT 0,
                        Notes NVARCHAR(500) NULL,
                        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT PK_cor_client_renewals PRIMARY KEY (Id),
                        CONSTRAINT FK_cor_client_renewals_cor_clients_ClientId FOREIGN KEY (ClientId) REFERENCES cor_clients (Id) ON DELETE CASCADE,
                        CONSTRAINT FK_cor_client_renewals_cor_client_plans_ClientPlanId FOREIGN KEY (ClientPlanId) REFERENCES cor_client_plans (Id) ON DELETE CASCADE
                    );
                    
                    CREATE INDEX IX_cor_client_renewals_ClientId ON cor_client_renewals (ClientId);
                    CREATE INDEX IX_cor_client_renewals_ClientPlanId ON cor_client_renewals (ClientPlanId);
                END
            ";
        }
        
        private string GetMsSqlPaymentMethodsTableSql()
        {
            return @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cor_payment_methods')
                BEGIN
                    CREATE TABLE cor_payment_methods (
                        Id UNIQUEIDENTIFIER NOT NULL,
                        ClientId UNIQUEIDENTIFIER NOT NULL,
                        Type INT NOT NULL DEFAULT 0,
                        Last4 NVARCHAR(4) NULL,
                        CardBrand NVARCHAR(20) NULL,
                        ExpiryMonth INT NULL,
                        ExpiryYear INT NULL,
                        CardholderName NVARCHAR(100) NULL,
                        Token NVARCHAR(500) NULL,
                        IsDefault BIT NOT NULL DEFAULT 0,
                        IsActive BIT NOT NULL DEFAULT 1,
                        Notes NVARCHAR(500) NULL,
                        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT PK_cor_payment_methods PRIMARY KEY (Id),
                        CONSTRAINT FK_cor_payment_methods_cor_clients_ClientId FOREIGN KEY (ClientId) REFERENCES cor_clients (Id) ON DELETE CASCADE
                    );
                    
                    CREATE INDEX IX_cor_payment_methods_ClientId ON cor_payment_methods (ClientId);
                END
            ";
        }
        
        private string GetMsSqlDefaultPlanSql()
        {
            Guid basicPlanId = Guid.NewGuid();
            Guid standardPlanId = Guid.NewGuid();
            Guid premiumPlanId = Guid.NewGuid();
            
            string basicFeatures = "{\"feature1\": true, \"feature2\": true, \"feature3\": false, \"feature4\": false}";
            string standardFeatures = "{\"feature1\": true, \"feature2\": true, \"feature3\": true, \"feature4\": false}";
            string premiumFeatures = "{\"feature1\": true, \"feature2\": true, \"feature3\": true, \"feature4\": true}";
            
            return $@"
                IF NOT EXISTS (SELECT * FROM cor_plans)
                BEGIN
                    INSERT INTO cor_plans (Id, Name, Description, MonthlyPrice, AnnualPrice, MaxUsers, MaxStorageGB, Features, IsActive, DisplayOrder)
                    VALUES 
                    ('{basicPlanId}', 'Basic', 'Essential features for small businesses', 29.99, 299.99, 10, 5, N'{basicFeatures}', 1, 1),
                    ('{standardPlanId}', 'Standard', 'Advanced features for growing businesses', 79.99, 799.99, 50, 25, N'{standardFeatures}', 1, 2),
                    ('{premiumPlanId}', 'Premium', 'Full features for enterprises', 149.99, 1499.99, 100, 100, N'{premiumFeatures}', 1, 3);
                END
            ";
        }
        
        private string GetOracleClientsTableSql()
        {
            return @"
                BEGIN
                    EXECUTE IMMEDIATE 'CREATE TABLE cor_clients (
                        Id VARCHAR2(36) NOT NULL PRIMARY KEY,
                        CompanyName VARCHAR2(100) NOT NULL,
                        VatNumber VARCHAR2(50) NULL,
                        CountryCode VARCHAR2(2) NOT NULL,
                        BillingAddressLine1 VARCHAR2(100) NULL,
                        BillingAddressLine2 VARCHAR2(100) NULL,
                        City VARCHAR2(50) NULL,
                        State VARCHAR2(50) NULL,
                        PostalCode VARCHAR2(20) NULL,
                        Email VARCHAR2(100) NOT NULL,
                        Phone VARCHAR2(20) NULL,
                        Website VARCHAR2(100) NULL,
                        Status NUMBER(10) NOT NULL DEFAULT 0,
                        DomainUrl VARCHAR2(100) NULL,
                        DatabaseName VARCHAR2(50) NULL,
                        Notes VARCHAR2(1000) NULL,
                        CreatedAt DATE DEFAULT SYSDATE NOT NULL
                    )';
                EXCEPTION
                    WHEN OTHERS THEN
                        IF SQLCODE != -955 THEN -- ORA-00955: name is already used by an existing object
                            RAISE;
                        END IF;
                END;
            ";
        }
        
        private string GetOracleUsersRefTableSql()
        {
            return @"
                BEGIN
                    EXECUTE IMMEDIATE 'CREATE TABLE cor_users_ref (
                        Id VARCHAR2(36) NOT NULL PRIMARY KEY,
                        ClientId VARCHAR2(36) NOT NULL,
                        FirstName VARCHAR2(50) NOT NULL,
                        LastName VARCHAR2(50) NOT NULL,
                        Email VARCHAR2(100) NOT NULL,
                        Phone VARCHAR2(20) NULL,
                        Role NUMBER(10) NOT NULL DEFAULT 0,
                        PasswordHash VARCHAR2(100) NOT NULL,
                        PasswordSalt VARCHAR2(50) NOT NULL,
                        IsActive NUMBER(1) NOT NULL DEFAULT 1,
                        LastLoginAt DATE NULL,
                        PreferredLanguage VARCHAR2(10) NOT NULL DEFAULT ''en-US'',
                        PreferredTheme VARCHAR2(20) NOT NULL DEFAULT ''light'',
                        CreatedAt DATE DEFAULT SYSDATE NOT NULL,
                        CONSTRAINT FK_cor_users_ref_cor_clients_ClientId FOREIGN KEY (ClientId) REFERENCES cor_clients(Id) ON DELETE CASCADE
                    )';
                    EXECUTE IMMEDIATE 'CREATE UNIQUE INDEX IX_cor_users_ref_Email ON cor_users_ref (Email)';
                    EXECUTE IMMEDIATE 'CREATE INDEX IX_cor_users_ref_ClientId ON cor_users_ref (ClientId)';
                EXCEPTION
                    WHEN OTHERS THEN
                        IF SQLCODE != -955 THEN -- ORA-00955: name is already used by an existing object
                            RAISE;
                        END IF;
                END;
            ";
        }
        
        private string GetOraclePlansTableSql()
        {
            return @"
                BEGIN
                    EXECUTE IMMEDIATE 'CREATE TABLE cor_plans (
                        Id VARCHAR2(36) NOT NULL PRIMARY KEY,
                        Name VARCHAR2(50) NOT NULL,
                        Description VARCHAR2(500) NULL,
                        MonthlyPrice NUMBER(18,2) NOT NULL,
                        AnnualPrice NUMBER(18,2) NOT NULL,
                        MaxUsers NUMBER(10) NOT NULL DEFAULT 0,
                        MaxStorageGB NUMBER(10) NOT NULL DEFAULT 0,
                        Features VARCHAR2(1000) NULL,
                        IsActive NUMBER(1) NOT NULL DEFAULT 1,
                        DisplayOrder NUMBER(10) NOT NULL DEFAULT 0,
                        CreatedAt DATE DEFAULT SYSDATE NOT NULL
                    )';
                EXCEPTION
                    WHEN OTHERS THEN
                        IF SQLCODE != -955 THEN -- ORA-00955: name is already used by an existing object
                            RAISE;
                        END IF;
                END;
            ";
        }
        
        private string GetOracleClientPlansTableSql()
        {
            return @"
                BEGIN
                    EXECUTE IMMEDIATE 'CREATE TABLE cor_client_plans (
                        Id VARCHAR2(36) NOT NULL PRIMARY KEY,
                        ClientId VARCHAR2(36) NOT NULL,
                        PlanId VARCHAR2(36) NOT NULL,
                        StartDate DATE NOT NULL,
                        EndDate DATE NOT NULL,
                        BillingCycle NUMBER(10) NOT NULL DEFAULT 0,
                        Price NUMBER(18,2) NOT NULL,
                        IsActive NUMBER(1) NOT NULL DEFAULT 1,
                        AutoRenew NUMBER(1) NOT NULL DEFAULT 1,
                        Notes VARCHAR2(500) NULL,
                        CreatedAt DATE DEFAULT SYSDATE NOT NULL,
                        CONSTRAINT FK_cor_client_plans_cor_clients_ClientId FOREIGN KEY (ClientId) REFERENCES cor_clients(Id) ON DELETE CASCADE,
                        CONSTRAINT FK_cor_client_plans_cor_plans_PlanId FOREIGN KEY (PlanId) REFERENCES cor_plans(Id) ON DELETE RESTRICT
                    )';
                    EXECUTE IMMEDIATE 'CREATE INDEX IX_cor_client_plans_ClientId ON cor_client_plans (ClientId)';
                    EXECUTE IMMEDIATE 'CREATE INDEX IX_cor_client_plans_PlanId ON cor_client_plans (PlanId)';
                EXCEPTION
                    WHEN OTHERS THEN
                        IF SQLCODE != -955 THEN -- ORA-00955: name is already used by an existing object
                            RAISE;
                        END IF;
                END;
            ";
        }
        
        private string GetOracleClientRenewalsTableSql()
        {
            return @"
                BEGIN
                    EXECUTE IMMEDIATE 'CREATE TABLE cor_client_renewals (
                        Id VARCHAR2(36) NOT NULL PRIMARY KEY,
                        ClientId VARCHAR2(36) NOT NULL,
                        ClientPlanId VARCHAR2(36) NOT NULL,
                        PreviousEndDate DATE NOT NULL,
                        NewStartDate DATE NOT NULL,
                        NewEndDate DATE NOT NULL,
                        Amount NUMBER(18,2) NOT NULL,
                        PaymentDate DATE NULL,
                        PaymentMethod VARCHAR2(50) NULL,
                        TransactionReference VARCHAR2(100) NULL,
                        Status NUMBER(10) NOT NULL DEFAULT 0,
                        Notes VARCHAR2(500) NULL,
                        CreatedAt DATE DEFAULT SYSDATE NOT NULL,
                        CONSTRAINT FK_cor_client_renewals_cor_clients_ClientId FOREIGN KEY (ClientId) REFERENCES cor_clients(Id) ON DELETE CASCADE,
                        CONSTRAINT FK_cor_client_renewals_cor_client_plans_ClientPlanId FOREIGN KEY (ClientPlanId) REFERENCES cor_client_plans(Id) ON DELETE CASCADE
                    )';
                    EXECUTE IMMEDIATE 'CREATE INDEX IX_cor_client_renewals_ClientId ON cor_client_renewals (ClientId)';
                    EXECUTE IMMEDIATE 'CREATE INDEX IX_cor_client_renewals_ClientPlanId ON cor_client_renewals (ClientPlanId)';
                EXCEPTION
                    WHEN OTHERS THEN
                        IF SQLCODE != -955 THEN -- ORA-00955: name is already used by an existing object
                            RAISE;
                        END IF;
                END;
            ";
        }
        
        private string GetOraclePaymentMethodsTableSql()
        {
            return @"
                BEGIN
                    EXECUTE IMMEDIATE 'CREATE TABLE cor_payment_methods (
                        Id VARCHAR2(36) NOT NULL PRIMARY KEY,
                        ClientId VARCHAR2(36) NOT NULL,
                        Type NUMBER(10) NOT NULL DEFAULT 0,
                        Last4 VARCHAR2(4) NULL,
                        CardBrand VARCHAR2(20) NULL,
                        ExpiryMonth NUMBER(10) NULL,
                        ExpiryYear NUMBER(10) NULL,
                        CardholderName VARCHAR2(100) NULL,
                        Token VARCHAR2(500) NULL,
                        IsDefault NUMBER(1) NOT NULL DEFAULT 0,
                        IsActive NUMBER(1) NOT NULL DEFAULT 1,
                        Notes VARCHAR2(500) NULL,
                        CreatedAt DATE DEFAULT SYSDATE NOT NULL,
                        CONSTRAINT FK_cor_payment_methods_cor_clients_ClientId FOREIGN KEY (ClientId) REFERENCES cor_clients(Id) ON DELETE CASCADE
                    )';
                    EXECUTE IMMEDIATE 'CREATE INDEX IX_cor_payment_methods_ClientId ON cor_payment_methods (ClientId)';
                EXCEPTION
                    WHEN OTHERS THEN
                        IF SQLCODE != -955 THEN -- ORA-00955: name is already used by an existing object
                            RAISE;
                        END IF;
                END;
            ";
        }
        
        private string GetOracleBillingRecordsTableSql()
        {
            return @"
                BEGIN
                    EXECUTE IMMEDIATE 'CREATE TABLE cor_billing_records (
                        Id VARCHAR2(36) NOT NULL PRIMARY KEY,
                        ClientId VARCHAR2(36) NOT NULL,
                        InvoiceNumber VARCHAR2(50) NOT NULL UNIQUE,
                        Amount NUMBER(18,2) NOT NULL,
                        Currency VARCHAR2(10) NOT NULL,
                        BillingPeriodStart DATE NOT NULL,
                        BillingPeriodEnd DATE NOT NULL,
                        Status NUMBER(10) NOT NULL,
                        Description VARCHAR2(255),
                        CreatedAt DATE DEFAULT SYSDATE NOT NULL,
                        PaidAt DATE,
                        PaymentMethod VARCHAR2(50),
                        Notes VARCHAR2(255),
                        CONSTRAINT FK_Billing_Client FOREIGN KEY (ClientId) REFERENCES cor_clients(Id) ON DELETE CASCADE
                    )';
                EXCEPTION
                    WHEN OTHERS THEN
                        IF SQLCODE != -955 THEN -- ORA-00955: name is already used by an existing object
                            RAISE;
                        END IF;
                END;
            ";
        }
        
        private string GetOracleDefaultPlanSql()
        {
            Guid basicPlanId = Guid.NewGuid();
            Guid standardPlanId = Guid.NewGuid();
            Guid premiumPlanId = Guid.NewGuid();
            
            string basicFeatures = "{\"feature1\": true, \"feature2\": true, \"feature3\": false, \"feature4\": false}";
            string standardFeatures = "{\"feature1\": true, \"feature2\": true, \"feature3\": true, \"feature4\": false}";
            string premiumFeatures = "{\"feature1\": true, \"feature2\": true, \"feature3\": true, \"feature4\": true}";
            
            return $@"
                BEGIN
                    EXECUTE IMMEDIATE 'INSERT INTO cor_plans (Id, Name, Description, MonthlyPrice, AnnualPrice, MaxUsers, MaxStorageGB, Features, IsActive, DisplayOrder)
                    VALUES 
                    (''{basicPlanId}'', ''Basic'', ''Essential features for small businesses'', 29.99, 299.99, 10, 5, ''{basicFeatures}'', 1, 1),
                    (''{standardPlanId}'', ''Standard'', ''Advanced features for growing businesses'', 79.99, 799.99, 50, 25, ''{standardFeatures}'', 1, 2),
                    (''{premiumPlanId}'', ''Premium'', ''Full features for enterprises'', 149.99, 1499.99, 100, 100, ''{premiumFeatures}'', 1, 3)';
                EXCEPTION
                    WHEN OTHERS THEN
                        IF SQLCODE != -955 THEN -- ORA-00955: name is already used by an existing object
                            RAISE;
                        END IF;
                END;
            ";
        }
        
        #endregion
    }
}