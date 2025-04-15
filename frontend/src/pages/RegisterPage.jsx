import React, { useState } from "react";
import {
  TextField,
  Button,
  MenuItem,
  FormControl,
  InputLabel,
  Select,
  Paper,
  Box,
  Typography,
  Container,
  Grid,
  Alert,
  CircularProgress,
  FormHelperText,
  Stepper,
  Step,
  StepLabel,
  Card,
  CardContent,
  CardActions,
  Divider,
  Chip,
  useTheme,
  useMediaQuery,
  Stack,
  Radio,
  RadioGroup,
  FormControlLabel,
  InputAdornment,
  Checkbox,
} from "@mui/material";
import CheckIcon from '@mui/icons-material/Check';
import config from "../config";

// Simple country list
const COUNTRIES = [
  { code: "US", name: "United States" },
  { code: "GB", name: "United Kingdom" },
  { code: "CA", name: "Canada" },
  { code: "IN", name: "India" },
  { code: "AU", name: "Australia" },
  { code: "DE", name: "Germany" },
  { code: "FR", name: "France" },
];

// Company size options
const COMPANY_SIZES = [
  { value: "1-10", label: "1-10 employees" },
  { value: "11-50", label: "11-50 employees" },
  { value: "51-200", label: "51-200 employees" },
  { value: "201-500", label: "201-500 employees" },
  { value: "501-1000", label: "501-1000 employees" },
  { value: "1001+", label: "1001+ employees" },
];

// Plans data
const PLANS = [
  {
    id: "basic",
    name: "Basic",
    price: 29,
    features: [
      "Up to 10 users",
      "5GB storage",
      "Email support",
      "Basic reporting",
    ],
    recommended: false,
  },
  {
    id: "medium",
    name: "Medium",
    price: 79,
    features: [
      "Up to 50 users",
      "25GB storage",
      "Priority email & chat support",
      "Advanced reporting",
      "Custom branding",
    ],
    recommended: true,
  },
  {
    id: "premium",
    name: "Premium",
    price: 149,
    features: [
      "Unlimited users",
      "100GB storage",
      "24/7 phone, email & chat support",
      "Enterprise-grade reporting",
      "Custom branding",
      "API access",
      "Dedicated account manager",
    ],
    recommended: false,
  },
];

const RegisterPage = () => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const isTablet = useMediaQuery(theme.breakpoints.down('md'));
  
  // Step management
  const [activeStep, setActiveStep] = useState(0);
  
  // Form state
  const [formData, setFormData] = useState({
    // Plan selection
    planId: "medium",
    
    // Company details
    companyName: "",
    companySize: "",
    industry: "",
    
    // Contact details
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    
    // Domain & settings
    domainName: "",
    countryCode: "",
    
    // Account security
    password: "",
    confirmPassword: "",
    
    // Terms
    acceptTerms: false,
  });

  // Validation errors
  const [errors, setErrors] = useState({});
  
  // UI state
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  // Handle form field changes
  const handleChange = (e) => {
    const { name, value, checked, type } = e.target;
    
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
    
    // Clear error when field changes
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: "" }));
    }
  };

  // Validate the current step
  const validateStep = (step) => {
    const newErrors = {};
    let isValid = true;

    switch (step) {
      case 0: // Plan selection
        if (!formData.planId) {
          newErrors.planId = "Please select a plan";
          isValid = false;
        }
        break;
        
      case 1: // Company details
        if (!formData.companyName.trim()) {
          newErrors.companyName = "Company name is required";
          isValid = false;
        }
        
        if (!formData.companySize) {
          newErrors.companySize = "Company size is required";
          isValid = false;
        }
        break;
        
      case 2: // Contact details
        if (!formData.firstName.trim()) {
          newErrors.firstName = "First name is required";
          isValid = false;
        }
        
        if (!formData.lastName.trim()) {
          newErrors.lastName = "Last name is required";
          isValid = false;
        }
        
        if (!formData.email.trim()) {
          newErrors.email = "Email is required";
          isValid = false;
        } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
          newErrors.email = "Please enter a valid email address";
          isValid = false;
        }
        
        if (!formData.phone.trim()) {
          newErrors.phone = "Phone number is required";
          isValid = false;
        }
        break;
        
      case 3: // Domain & settings
        if (!formData.domainName.trim()) {
          newErrors.domainName = "Domain name is required";
          isValid = false;
        } else if (!/^[a-zA-Z0-9]([a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?$/.test(formData.domainName)) {
          newErrors.domainName = "Please enter a valid domain name (letters, numbers, hyphens)";
          isValid = false;
        }
        
        if (!formData.countryCode) {
          newErrors.countryCode = "Country is required";
          isValid = false;
        }
        break;
        
      case 4: // Account security
        if (!formData.password) {
          newErrors.password = "Password is required";
          isValid = false;
        } else if (formData.password.length < 8) {
          newErrors.password = "Password must be at least 8 characters";
          isValid = false;
        }
        
        if (!formData.confirmPassword) {
          newErrors.confirmPassword = "Please confirm your password";
          isValid = false;
        } else if (formData.password !== formData.confirmPassword) {
          newErrors.confirmPassword = "Passwords don't match";
          isValid = false;
        }
        
        if (!formData.acceptTerms) {
          newErrors.acceptTerms = "You must accept the terms and conditions";
          isValid = false;
        }
        break;
    }

    setErrors(newErrors);
    return isValid;
  };

  // Handle next step
  const handleNext = () => {
    if (validateStep(activeStep)) {
      setActiveStep((prevStep) => prevStep + 1);
    }
  };

  // Handle back step
  const handleBack = () => {
    setActiveStep((prevStep) => prevStep - 1);
  };

  // Handle form submission
  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!validateStep(activeStep)) {
      return;
    }
    
    setIsSubmitting(true);
    setError("");
    
    try {
      // Prepare API request payload
      const requestData = {
        // Company information
        companyName: formData.companyName,
        vatNumber: "",
        countryCode: formData.countryCode,
        
        // Contact information
        email: formData.email,
        phone: formData.phone,
        website: "",
        
        // Address (not collected in this form)
        billingAddressLine1: "",
        billingAddressLine2: "",
        city: "",
        state: "",
        postalCode: "",
        
        // Domain information
        domainUrl: `${formData.domainName}.localhost:5173`, 
        databaseName: formData.domainName.toLowerCase().replace(/[^a-z0-9]/g, ''),
        
        // Admin user information
        firstName: formData.firstName,
        lastName: formData.lastName,
        password: formData.password,
        
        // Plan information from selected plan
        billingCycle: 0, // Monthly
        
        // Additional metadata
        companySize: formData.companySize,
        industry: formData.industry || "",
        planId: formData.planId,
      };
      
      console.log("API URL:", `${config.apiUrl}/api/Client/register`);
      console.log("Request payload:", requestData);
      
      const response = await fetch(`${config.apiUrl}/api/Client/register`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(requestData),
      });
      
      const data = await response.json();
      
      if (!response.ok) {
        throw new Error(data.message || data.error || "Registration failed");
      }
      
      setSuccess("Your account has been created successfully!");
      // Allow the success message to be seen before redirecting
      setTimeout(() => {
        window.location.href = "/login";
      }, 3000);
      
    } catch (err) {
      console.error("Registration error:", err);
      setError(err.message || "Registration failed. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  // Steps configuration
  const steps = [
    "Choose plan",
    "Company details",
    "Contact information",
    "Domain setup",
    "Account security",
  ];

  // Render current step content
  const renderStepContent = (step) => {
    switch (step) {
      case 0:
        return (
          <Box sx={{ pt: 2 }}>
            <Typography variant="h5" gutterBottom>
              Choose your plan
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 4 }}>
              Select the plan that best fits your business needs. You can upgrade or downgrade at any time.
            </Typography>
            
            <RadioGroup
              name="planId"
              value={formData.planId}
              onChange={handleChange}
            >
              <Grid container spacing={3}>
                {PLANS.map((plan) => (
                  <Grid item xs={12} md={4} key={plan.id}>
                    <Card 
                      sx={{ 
                        height: '100%',
                        display: 'flex',
                        flexDirection: 'column',
                        borderColor: formData.planId === plan.id ? 'primary.main' : 'divider',
                        borderWidth: formData.planId === plan.id ? 2 : 1,
                        borderStyle: 'solid',
                        position: 'relative',
                        transition: 'all 0.3s ease',
                        '&:hover': {
                          borderColor: 'primary.main',
                          boxShadow: 3,
                          transform: 'translateY(-4px)'
                        }
                      }}
                    >
                      {plan.recommended && (
                        <Chip
                          label="Recommended"
                          color="primary"
                          size="small"
                          sx={{
                            position: 'absolute',
                            top: -10,
                            right: 20,
                            fontWeight: 'bold',
                          }}
                        />
                      )}
                      
                      <CardContent sx={{ flexGrow: 1 }}>
                        <Box sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', mb: 1 }}>
                          <Typography variant="h5" component="div" gutterBottom>
                            {plan.name}
                          </Typography>
                          <FormControlLabel
                            value={plan.id}
                            control={<Radio />}
                            label=""
                            sx={{ m: 0 }}
                          />
                        </Box>
                        
                        <Typography variant="h4" color="primary.main" sx={{ mb: 2, fontWeight: 'bold' }}>
                          ${plan.price}<Typography component="span" variant="body1" color="text.secondary">/mo</Typography>
                        </Typography>
                        
                        <Divider sx={{ mb: 2 }} />
                        
                        <Stack spacing={1}>
                          {plan.features.map((feature, index) => (
                            <Box key={index} sx={{ display: 'flex', alignItems: 'center' }}>
                              <CheckIcon fontSize="small" color="success" sx={{ mr: 1 }} />
                              <Typography variant="body2">
                                {feature}
                              </Typography>
                            </Box>
                          ))}
                        </Stack>
                      </CardContent>
                      
                      <CardActions sx={{ p: 2, pt: 0 }}>
                        <Button 
                          size="small" 
                          onClick={() => setFormData(prev => ({ ...prev, planId: plan.id }))}
                          variant={formData.planId === plan.id ? "contained" : "outlined"}
                          fullWidth
                        >
                          {formData.planId === plan.id ? "Selected" : "Select Plan"}
                        </Button>
                      </CardActions>
                    </Card>
                  </Grid>
                ))}
              </Grid>
            </RadioGroup>
            
            {errors.planId && (
              <Typography color="error" variant="body2" sx={{ mt: 2 }}>
                {errors.planId}
              </Typography>
            )}
          </Box>
        );
        
      case 1:
        return (
          <Box sx={{ pt: 2 }}>
            <Typography variant="h5" gutterBottom>
              Company details
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 4 }}>
              Tell us a bit about your company.
            </Typography>
            
            <Grid container spacing={3}>
              <Grid item xs={12}>
                <TextField
                  name="companyName"
                  label="Company name"
                  fullWidth
                  required
                  value={formData.companyName}
                  onChange={handleChange}
                  error={!!errors.companyName}
                  helperText={errors.companyName}
                />
              </Grid>
              
              <Grid item xs={12} sm={6}>
                <FormControl fullWidth required error={!!errors.companySize}>
                  <InputLabel id="company-size-label">Company size</InputLabel>
                  <Select
                    labelId="company-size-label"
                    name="companySize"
                    value={formData.companySize}
                    onChange={handleChange}
                    label="Company size"
                    sx={{ 
                      height: '56px',
                      '& .MuiSelect-select': {
                        display: 'flex',
                        alignItems: 'center',
                        minHeight: '56px',
                      }
                    }}
                    MenuProps={{
                      PaperProps: {
                        style: {
                          maxHeight: 300
                        }
                      }
                    }}
                  >
                    {COMPANY_SIZES.map((size) => (
                      <MenuItem key={size.value} value={size.value}>
                        {size.label}
                      </MenuItem>
                    ))}
                  </Select>
                  {errors.companySize && (
                    <FormHelperText>{errors.companySize}</FormHelperText>
                  )}
                </FormControl>
              </Grid>
              
              <Grid item xs={12} sm={6}>
                <TextField
                  name="industry"
                  label="Industry"
                  fullWidth
                  value={formData.industry}
                  onChange={handleChange}
                  placeholder="e.g. Healthcare, Technology, Finance"
                />
              </Grid>
            </Grid>
          </Box>
        );
        
      case 2:
        return (
          <Box sx={{ pt: 2 }}>
            <Typography variant="h5" gutterBottom>
              Contact information
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 4 }}>
              Your contact details for account management.
            </Typography>
            
            <Grid container spacing={3}>
              <Grid item xs={12} sm={6}>
                <TextField
                  name="firstName"
                  label="First name"
                  fullWidth
                  required
                  value={formData.firstName}
                  onChange={handleChange}
                  error={!!errors.firstName}
                  helperText={errors.firstName}
                />
              </Grid>
              
              <Grid item xs={12} sm={6}>
                <TextField
                  name="lastName"
                  label="Last name"
                  fullWidth
                  required
                  value={formData.lastName}
                  onChange={handleChange}
                  error={!!errors.lastName}
                  helperText={errors.lastName}
                />
              </Grid>
              
              <Grid item xs={12}>
                <TextField
                  name="email"
                  label="Work email"
                  fullWidth
                  required
                  type="email"
                  value={formData.email}
                  onChange={handleChange}
                  error={!!errors.email}
                  helperText={errors.email}
                />
              </Grid>
              
              <Grid item xs={12}>
                <TextField
                  name="phone"
                  label="Phone number"
                  fullWidth
                  required
                  value={formData.phone}
                  onChange={handleChange}
                  error={!!errors.phone}
                  helperText={errors.phone}
                  placeholder="+1 (555) 123-4567"
                />
              </Grid>
            </Grid>
          </Box>
        );
        
      case 3:
        return (
          <Box sx={{ pt: 2 }}>
            <Typography variant="h5" gutterBottom>
              Domain setup
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 4 }}>
              Choose your unique domain name and location.
            </Typography>
            
            <Grid container spacing={3}>
              <Grid item xs={12}>
                <TextField
                  name="domainName"
                  label="Domain name"
                  fullWidth
                  required
                  value={formData.domainName}
                  onChange={handleChange}
                  error={!!errors.domainName}
                  helperText={errors.domainName || "Choose a unique name for your organization's domain"}
                  InputProps={{
                    endAdornment: <InputAdornment position="end">.localhost:5173</InputAdornment>,
                  }}
                  placeholder="mycompany"
                />
              </Grid>
              
              <Grid item xs={12}>
                <FormControl fullWidth required error={!!errors.countryCode}>
                  <InputLabel id="country-label">Country</InputLabel>
                  <Select
                    labelId="country-label"
                    name="countryCode"
                    value={formData.countryCode}
                    onChange={handleChange}
                    label="Country"
                    sx={{ 
                      height: '56px',
                      '& .MuiSelect-select': {
                        display: 'flex',
                        alignItems: 'center',
                        minHeight: '56px',
                      }
                    }}
                    MenuProps={{
                      PaperProps: {
                        style: {
                          maxHeight: 300
                        }
                      }
                    }}
                  >
                    {COUNTRIES.map((country) => (
                      <MenuItem key={country.code} value={country.code}>
                        {country.name}
                      </MenuItem>
                    ))}
                  </Select>
                  {errors.countryCode && (
                    <FormHelperText>{errors.countryCode}</FormHelperText>
                  )}
                </FormControl>
              </Grid>
              
              {formData.domainName && (
                <Grid item xs={12}>
                  <Alert severity="info">
                    Your organization will be accessible at:{" "}
                    <Typography component="span" fontWeight="bold">
                      {formData.domainName}.localhost:5173
                    </Typography>
                  </Alert>
                </Grid>
              )}
            </Grid>
          </Box>
        );
        
      case 4:
        return (
          <Box sx={{ pt: 2 }}>
            <Typography variant="h5" gutterBottom>
              Account security
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 4 }}>
              Set up your password and agree to terms.
            </Typography>
            
            <Grid container spacing={3}>
              <Grid item xs={12}>
                <TextField
                  name="password"
                  label="Password"
                  fullWidth
                  required
                  type="password"
                  value={formData.password}
                  onChange={handleChange}
                  error={!!errors.password}
                  helperText={errors.password || "Must be at least 8 characters"}
                />
              </Grid>
              
              <Grid item xs={12}>
                <TextField
                  name="confirmPassword"
                  label="Confirm password"
                  fullWidth
                  required
                  type="password"
                  value={formData.confirmPassword}
                  onChange={handleChange}
                  error={!!errors.confirmPassword}
                  helperText={errors.confirmPassword}
                />
              </Grid>
              
              <Grid item xs={12}>
                <FormControlLabel
                  control={
                    <Checkbox
                      name="acceptTerms"
                      checked={formData.acceptTerms}
                      onChange={handleChange}
                      color="primary"
                    />
                  }
                  label="I agree to the Terms of Service and Privacy Policy"
                />
                {errors.acceptTerms && (
                  <Typography color="error" variant="body2">
                    {errors.acceptTerms}
                  </Typography>
                )}
              </Grid>
            </Grid>
          </Box>
        );
        
      default:
        return null;
    }
  };

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Paper
        elevation={3}
        sx={{
          p: { xs: 2, sm: 4 },
          borderRadius: 2,
        }}
      >
        <Typography
          variant="h4"
          component="h1"
          gutterBottom
          align="center"
          sx={{ mb: 4 }}
        >
          Set up your organization
        </Typography>
        
        {error && (
          <Alert severity="error" sx={{ mb: 4 }} onClose={() => setError("")}>
            {error}
          </Alert>
        )}
        
        {success && (
          <Alert severity="success" sx={{ mb: 4 }} onClose={() => setSuccess("")}>
            {success}
          </Alert>
        )}
        
        {/* Stepper */}
        <Stepper
          activeStep={activeStep}
          alternativeLabel={!isMobile}
          orientation={isMobile ? "vertical" : "horizontal"}
          sx={{ mb: 4 }}
        >
          {steps.map((label, index) => (
            <Step key={label}>
              <StepLabel>{label}</StepLabel>
            </Step>
          ))}
        </Stepper>
        
        {/* Step content */}
        <form onSubmit={activeStep === steps.length - 1 ? handleSubmit : (e) => { e.preventDefault(); handleNext(); }}>
          {renderStepContent(activeStep)}
          
          {/* Navigation buttons */}
          <Box
            sx={{
              display: "flex",
              justifyContent: "space-between",
              mt: 4,
              pt: 2,
              borderTop: `1px solid ${theme.palette.divider}`,
            }}
          >
            <Button
              variant="outlined"
              onClick={handleBack}
              disabled={activeStep === 0}
            >
              Back
            </Button>
            
            <Button
              variant="contained"
              type="submit"
              disabled={isSubmitting}
            >
              {isSubmitting ? (
                <CircularProgress size={24} />
              ) : activeStep === steps.length - 1 ? (
                "Create Account"
              ) : (
                "Continue"
              )}
            </Button>
          </Box>
        </form>
      </Paper>
    </Container>
  );
};

export default RegisterPage;