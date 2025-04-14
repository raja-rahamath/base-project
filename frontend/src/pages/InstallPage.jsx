import React, { useState, useEffect } from "react";
import {
  TextField,
  Button,
  MenuItem,
  Select,
  InputLabel,
  FormControl,
  Paper,
  Box,
  Typography,
  Container,
  Divider,
  CircularProgress,
  Alert,
  Stack,
  InputAdornment,
  Stepper,
  Step,
  StepLabel,
  useTheme,
  useMediaQuery,
  Chip,
  Tooltip,
  IconButton,
  Backdrop,
} from "@mui/material";
import HelpOutlineIcon from '@mui/icons-material/HelpOutline';
import StorageIcon from '@mui/icons-material/Storage';
import PublicIcon from '@mui/icons-material/Public';
import RouterIcon from '@mui/icons-material/Router';
import DatabaseIcon from '@mui/icons-material/Storage';
import PersonIcon from '@mui/icons-material/Person';
import LockIcon from '@mui/icons-material/Lock';
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import ArrowForwardIcon from '@mui/icons-material/ArrowForward';
import config from "../config";

// Debug configuration
console.log("ENV Variables:", import.meta.env);
console.log("VITE_API_URL:", import.meta.env.VITE_API_URL);
console.log("Config API URL:", config.apiUrl);

// Default values for testing
const defaultValues = {
  dbType: "MySQL",
  serviceIP: "localhost",
  port: "3306",
  dbName: "myapp",
  rootUser: "root",
  rootPassword: "rsquare-dev",
  adminUser: "appadmin",
  adminPassword: "admin123",
};

const InstallPage = () => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  
  // Form state
  const [dbType, setDbType] = useState(defaultValues.dbType);
  const [serviceIP, setServiceIP] = useState(defaultValues.serviceIP);
  const [port, setPort] = useState(defaultValues.port);
  const [dbName, setDbName] = useState(defaultValues.dbName);
  const [rootUser, setRootUser] = useState(defaultValues.rootUser);
  const [rootPassword, setRootPassword] = useState(defaultValues.rootPassword);
  const [adminUser, setAdminUser] = useState(defaultValues.adminUser);
  const [adminPassword, setAdminPassword] = useState(defaultValues.adminPassword);
  
  // Validation state
  const [ipError, setIpError] = useState("");
  const [portError, setPortError] = useState("");
  const [dbNameError, setDbNameError] = useState("");
  const [rootUserError, setRootUserError] = useState("");
  const [adminUserError, setAdminUserError] = useState("");
  const [adminPasswordError, setAdminPasswordError] = useState("");
  
  // UI state
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [activeStep, setActiveStep] = useState(0);
  const [completed, setCompleted] = useState({});

  // Default port when DB type changes
  useEffect(() => {
    switch (dbType) {
      case "MySQL":
        setPort("3306");
        break;
      case "Postgres":
        setPort("5432");
        break;
      case "MSSQL":
        setPort("1433");
        break;
      case "Oracle":
        setPort("1521");
        break;
      default:
        setPort("");
    }
  }, [dbType]);

  const validateIPAddress = (ip) => {
    // Accept localhost
    if (ip === "localhost") {
      return true;
    }

    // Check if it's a domain name (contains letters)
    if (/^[a-zA-Z0-9][a-zA-Z0-9-]{1,61}[a-zA-Z0-9](?:\.[a-zA-Z]{2,})+$/.test(ip)) {
      return true;
    }

    // IP address validation
    const ipv4Regex = /^(\d{1,3}\.){3}\d{1,3}$/;
    if (!ipv4Regex.test(ip)) {
      return false;
    }

    const parts = ip.split(".");
    return parts.every((part) => {
      const num = parseInt(part, 10);
      return num >= 0 && num <= 255;
    });
  };

  const validatePort = (port) => {
    // Check if it's a number
    if (!/^\d+$/.test(port)) {
      return false;
    }

    const portNum = parseInt(port, 10);
    return portNum >= 0 && portNum <= 65535;
  };

  const validateDatabaseName = (name) => {
    if (!name) return false;

    // Check length (most databases have a limit of 64 characters)
    if (name.length > 64) {
      return false;
    }

    // Check for valid characters (letters, numbers, underscore)
    if (!/^[a-zA-Z0-9_]+$/.test(name)) {
      return false;
    }

    // Check if it starts with a letter or underscore
    if (!/^[a-zA-Z_]/.test(name)) {
      return false;
    }

    // Check for reserved words (basic check)
    const reservedWords = [
      "database",
      "table",
      "select",
      "insert",
      "update",
      "delete",
      "create",
      "drop",
      "user",
      "password",
      "master",
      "system",
    ];
    if (reservedWords.includes(name.toLowerCase())) {
      return false;
    }

    return true;
  };

  const validateUsername = (username) => {
    if (!username) return false;
    return /^[a-zA-Z0-9_]+$/.test(username) && username.length <= 32;
  };

  const validatePassword = (password) => {
    if (!password) return false;
    return password.length >= 6;
  };

  const handleIPChange = (e) => {
    const value = e.target.value;
    setServiceIP(value);

    if (value && !validateIPAddress(value)) {
      setIpError("Please enter a valid IP address or domain name");
    } else {
      setIpError("");
    }
  };

  const handlePortChange = (e) => {
    const value = e.target.value;
    setPort(value);

    if (value && !validatePort(value)) {
      setPortError("Please enter a valid port number (0-65535)");
    } else {
      setPortError("");
    }
  };

  const handleDbNameChange = (e) => {
    const value = e.target.value;
    setDbName(value);

    if (value && !validateDatabaseName(value)) {
      setDbNameError(
        "Database name must start with a letter or underscore, contain only letters, numbers, and underscores, and be less than 64 characters"
      );
    } else {
      setDbNameError("");
    }
  };

  const handleRootUserChange = (e) => {
    const value = e.target.value;
    setRootUser(value);

    if (value && !validateUsername(value)) {
      setRootUserError(
        "Username must contain only letters, numbers, and underscores"
      );
    } else {
      setRootUserError("");
    }
  };

  const handleAdminUserChange = (e) => {
    const value = e.target.value;
    setAdminUser(value);

    if (value && !validateUsername(value)) {
      setAdminUserError(
        "Username must contain only letters, numbers, and underscores"
      );
    } else {
      setAdminUserError("");
    }
  };

  const handleAdminPasswordChange = (e) => {
    const value = e.target.value;
    setAdminPassword(value);

    if (value && !validatePassword(value)) {
      setAdminPasswordError("Password must be at least 6 characters");
    } else {
      setAdminPasswordError("");
    }
  };

  const isStepValid = (step) => {
    switch (step) {
      case 0: // Database Type
        return !!dbType;
      case 1: // Server Connection
        return (
          !!serviceIP && 
          !!port && 
          !ipError && 
          !portError
        );
      case 2: // Root Credentials
        return (
          !!rootUser && 
          !!rootPassword && 
          !rootUserError
        );
      case 3: // Admin User Setup
        return (
          !!dbName && 
          !!adminUser && 
          !!adminPassword && 
          !dbNameError && 
          !adminUserError && 
          !adminPasswordError
        );
      default:
        return false;
    }
  };

  const handleNext = () => {
    setCompleted({
      ...completed,
      [activeStep]: true
    });
    setActiveStep((prevStep) => prevStep + 1);
  };

  const handleBack = () => {
    setActiveStep((prevStep) => prevStep - 1);
  };

  const handleSubmit = async () => {
    if (
      ipError ||
      portError ||
      dbNameError ||
      rootUserError ||
      adminUserError ||
      adminPasswordError
    ) {
      return;
    }

    setIsLoading(true);
    setError("");
    setSuccess("");

    const connectionDetails = {
      dbType,
      serviceIP,
      port,
      dbName,
      rootUser,
      rootPassword,
      adminUser,
      adminPassword,
    };

    try {
      // Enhanced debugging
      console.log("Environment check at submit time:");
      console.log("- import.meta.env.VITE_API_URL:", import.meta.env.VITE_API_URL);
      console.log("- config.apiUrl:", config.apiUrl);
      console.log("- Full ENV:", import.meta.env);
      
      const apiUrl = config.apiUrl;
      console.log("Using API URL:", apiUrl);
      
      // Add cache busting parameter
      const fullUrl = `${apiUrl}/api/install?t=${config.timestamp}`;
      console.log("Full request URL with cache busting:", fullUrl);
      
      const response = await fetch(fullUrl, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },
        body: JSON.stringify(connectionDetails),
      });

      console.log("Response status:", response.status);
      console.log("Response URL (actual URL used):", response.url);

      const responseData = await response.json();

      if (!response.ok) {
        console.error("Error response:", responseData);
        throw new Error(
          responseData.error || `HTTP error! status: ${response.status}`
        );
      }

      console.log("Success response:", responseData);

      setSuccess("Database and user created successfully!");
      setCompleted({
        ...completed,
        [activeStep]: true
      });
      
      // Optionally reset form or keep it as is
      // resetForm();
    } catch (error) {
      console.error("Error during installation:", error);
      setError(error.message);
    } finally {
      setIsLoading(false);
    }
  };

  const resetForm = () => {
    setDbType("");
    setServiceIP("");
    setPort("");
    setDbName("");
    setRootUser("");
    setRootPassword("");
    setAdminUser("");
    setAdminPassword("");
    setActiveStep(0);
    setCompleted({});
  };

  const steps = [
    {
      label: "Database Type",
      description: "Select the type of database you want to create",
    },
    {
      label: "Server Connection",
      description: "Configure the server connection details",
    },
    {
      label: "Root Credentials",
      description: "Enter the root user credentials",
    },
    {
      label: "Admin User Setup",
      description: "Set up the admin user and database",
    },
  ];

  const getStepContent = (step) => {
    switch (step) {
      case 0:
        return (
          <Box sx={{ mt: 4 }}>
            <Typography variant="h6" gutterBottom>
              Select Database Type
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              Choose the type of database you want to create. This will determine the connection settings and SQL syntax used.
            </Typography>
            
            <FormControl fullWidth sx={{ mt: 2 }}>
              <InputLabel id="db-type-label">Database Type</InputLabel>
              <Select
                labelId="db-type-label"
                value={dbType}
                onChange={(e) => setDbType(e.target.value)}
                label="Database Type"
                startAdornment={
                  <InputAdornment position="start">
                    <DatabaseIcon color="primary" />
                  </InputAdornment>
                }
              >
                <MenuItem value="MySQL">MySQL</MenuItem>
                <MenuItem value="Postgres">PostgreSQL</MenuItem>
                <MenuItem value="MSSQL">Microsoft SQL Server</MenuItem>
                <MenuItem value="Oracle">Oracle Database</MenuItem>
              </Select>
            </FormControl>

            {dbType && (
              <Box sx={{ mt: 4, p: 2, bgcolor: 'background.default', borderRadius: 2 }}>
                <Typography variant="subtitle2" sx={{ fontWeight: 600, mb: 1 }}>
                  About {dbType}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  {dbType === "MySQL" && "MySQL is an open-source relational database management system. It is widely used for web applications and is the 'M' in the LAMP stack."}
                  {dbType === "Postgres" && "PostgreSQL is a powerful, open source object-relational database system known for reliability, feature robustness, and performance."}
                  {dbType === "MSSQL" && "Microsoft SQL Server is a relational database management system developed by Microsoft, known for its integration with other Microsoft products."}
                  {dbType === "Oracle" && "Oracle Database is a multi-model database management system produced and marketed by Oracle Corporation, known for its robustness in enterprise environments."}
                </Typography>
                
                <Chip 
                  label={`Default Port: ${port}`} 
                  variant="outlined" 
                  size="small" 
                  color="primary"
                  sx={{ mt: 2 }}
                />
              </Box>
            )}
          </Box>
        );
      case 1:
        return (
          <Box sx={{ mt: 4 }}>
            <Typography variant="h6" gutterBottom>
              Server Connection Details
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              Enter the host and port information for your {dbType} server.
            </Typography>
            
            <Stack spacing={3}>
              <TextField
                fullWidth
                label="Server IP or Domain Name"
                value={serviceIP}
                onChange={handleIPChange}
                error={!!ipError}
                helperText={ipError}
                placeholder="e.g., localhost or 192.168.1.1"
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <PublicIcon color={ipError ? "error" : "primary"} />
                    </InputAdornment>
                  ),
                }}
              />

              <TextField
                fullWidth
                label="Port"
                value={port}
                onChange={handlePortChange}
                error={!!portError}
                helperText={portError || `Standard port for ${dbType}`}
                placeholder={`e.g., ${port}`}
                inputProps={{
                  inputMode: "numeric",
                  pattern: "[0-9]*",
                }}
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <RouterIcon color={portError ? "error" : "primary"} />
                    </InputAdornment>
                  ),
                }}
              />
            </Stack>

            <Alert severity="info" sx={{ mt: 4 }}>
              Make sure your database server is running and accessible from this machine.
            </Alert>
          </Box>
        );
      case 2:
        return (
          <Box sx={{ mt: 4 }}>
            <Typography variant="h6" gutterBottom>
              Root Credentials
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              Enter the credentials for an existing user with administrative privileges on the {dbType} server.
            </Typography>
            
            <Stack spacing={3}>
              <TextField
                fullWidth
                label="Root Username"
                value={rootUser}
                onChange={handleRootUserChange}
                error={!!rootUserError}
                helperText={rootUserError || `The admin user for your ${dbType} server`}
                placeholder={dbType === "Oracle" ? "e.g., system" : "e.g., root"}
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <PersonIcon color={rootUserError ? "error" : "primary"} />
                    </InputAdornment>
                  ),
                }}
              />

              <TextField
                fullWidth
                label="Root Password"
                type="password"
                value={rootPassword}
                onChange={(e) => setRootPassword(e.target.value)}
                placeholder="Enter root password"
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <LockIcon color="primary" />
                    </InputAdornment>
                  ),
                }}
              />
            </Stack>

            <Alert severity="warning" sx={{ mt: 4 }}>
              These credentials will only be used to create the new database and user. They are not stored permanently.
            </Alert>
          </Box>
        );
      case 3:
        return (
          <Box sx={{ mt: 4 }}>
            <Typography variant="h6" gutterBottom>
              New Database and Admin User
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              Configure the new database and user to be created.
            </Typography>
            
            <Stack spacing={3}>
              <TextField
                fullWidth
                label="Database Name"
                value={dbName}
                onChange={handleDbNameChange}
                error={!!dbNameError}
                helperText={dbNameError}
                placeholder="e.g., my_application"
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <StorageIcon color={dbNameError ? "error" : "primary"} />
                    </InputAdornment>
                  ),
                }}
              />

              <TextField
                fullWidth
                label="Admin Username"
                value={adminUser}
                onChange={handleAdminUserChange}
                error={!!adminUserError}
                helperText={adminUserError}
                placeholder="e.g., app_admin"
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <PersonIcon color={adminUserError ? "error" : "primary"} />
                    </InputAdornment>
                  ),
                }}
              />

              <TextField
                fullWidth
                label="Admin Password"
                type="password"
                value={adminPassword}
                onChange={handleAdminPasswordChange}
                error={!!adminPasswordError}
                helperText={adminPasswordError || "Password should be at least 6 characters long"}
                placeholder="Enter a strong password"
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <LockIcon color={adminPasswordError ? "error" : "primary"} />
                    </InputAdornment>
                  ),
                }}
              />
            </Stack>

            <Alert severity="info" sx={{ mt: 4 }}>
              This user will have full permissions to the new database.
            </Alert>
          </Box>
        );
      default:
        return "Unknown step";
    }
  };

  return (
    <Box sx={{ py: 4 }}>
      <Typography
        variant="h4"
        component="h1"
        gutterBottom
        align="center"
        sx={{
          color: "primary.main",
          fontWeight: 700,
          mb: 5,
        }}
      >
        Database Setup Wizard
      </Typography>

      <Paper
        elevation={3}
        sx={{
          maxWidth: 800,
          mx: "auto",
          borderRadius: 3,
          overflow: "hidden",
        }}
      >
        <Box
          sx={{
            p: 3,
            bgcolor: "primary.main",
            color: "white",
          }}
        >
          <Typography 
            variant="h5" 
            sx={{ 
              fontWeight: 600, 
              display: 'flex', 
              alignItems: 'center',
              gap: 1 
            }}
          >
            <StorageIcon /> Create New Database
          </Typography>
          <Typography variant="body2" sx={{ mt: 1, opacity: 0.9 }}>
            This wizard will help you set up a new database and admin user for your application.
          </Typography>
        </Box>

        <Box sx={{ p: { xs: 2, md: 4 } }}>
          {error && (
            <Alert 
              severity="error" 
              sx={{ mb: 3 }}
              onClose={() => setError("")}
            >
              {error}
            </Alert>
          )}

          {success && (
            <Alert 
              severity="success" 
              sx={{ mb: 3 }}
              onClose={() => setSuccess("")}
              icon={<CheckCircleOutlineIcon fontSize="inherit" />}
            >
              {success}
            </Alert>
          )}

          <Stepper 
            activeStep={activeStep} 
            alternativeLabel={!isMobile}
            orientation={isMobile ? "vertical" : "horizontal"}
            sx={{ mb: 4 }}
          >
            {steps.map((step, index) => (
              <Step key={step.label} completed={completed[index]}>
                <StepLabel>
                  {step.label}
                  {isMobile && (
                    <Typography variant="caption" display="block" color="text.secondary">
                      {step.description}
                    </Typography>
                  )}
                </StepLabel>
              </Step>
            ))}
          </Stepper>

          {getStepContent(activeStep)}

          <Box sx={{ display: "flex", justifyContent: "space-between", mt: 4, pt: 2, borderTop: `1px solid ${theme.palette.divider}` }}>
            <Button
              variant="outlined"
              color="primary"
              onClick={handleBack}
              disabled={activeStep === 0}
              startIcon={<ArrowBackIcon />}
            >
              Back
            </Button>
            
            <Box>
              {activeStep === steps.length - 1 ? (
                <Button
                  variant="contained"
                  color="primary"
                  onClick={handleSubmit}
                  disabled={
                    isLoading ||
                    !isStepValid(activeStep)
                  }
                  sx={{
                    px: 4,
                    py: 1,
                    position: "relative",
                  }}
                >
                  {isLoading ? (
                    <>
                      <CircularProgress
                        size={24}
                        sx={{
                          color: "white",
                          position: "absolute",
                          top: "50%",
                          left: "50%",
                          marginTop: "-12px",
                          marginLeft: "-12px",
                        }}
                      />
                      <span style={{ opacity: 0 }}>Create Database</span>
                    </>
                  ) : (
                    "Create Database"
                  )}
                </Button>
              ) : (
                <Button
                  variant="contained"
                  color="primary"
                  onClick={handleNext}
                  disabled={!isStepValid(activeStep)}
                  endIcon={<ArrowForwardIcon />}
                >
                  Next
                </Button>
              )}
            </Box>
          </Box>
        </Box>
      </Paper>

      <Box sx={{ maxWidth: 800, mx: "auto", mt: 4 }}>
        <Alert severity="info" sx={{ display: 'flex', alignItems: 'flex-start' }}>
          <HelpOutlineIcon sx={{ mt: 0.5, mr: 1 }} />
          <div>
            <Typography variant="body2" sx={{ fontWeight: 500, mb: 0.5 }}>
              Need help with database setup?
            </Typography>
            <Typography variant="body2">
              Check out our documentation for more information on database setup and configuration. If you're experiencing issues, contact our support team.
            </Typography>
          </div>
        </Alert>
      </Box>
      
      <Backdrop
        sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1 }}
        open={isLoading}
      >
        <CircularProgress color="inherit" />
      </Backdrop>
    </Box>
  );
};

export default InstallPage;