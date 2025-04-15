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

// Complete country list with ISO country codes
const COUNTRIES = [
  { code: "AF", name: "Afghanistan" },
  { code: "AL", name: "Albania" },
  { code: "DZ", name: "Algeria" },
  { code: "AD", name: "Andorra" },
  { code: "AO", name: "Angola" },
  { code: "AG", name: "Antigua and Barbuda" },
  { code: "AR", name: "Argentina" },
  { code: "AM", name: "Armenia" },
  { code: "AU", name: "Australia" },
  { code: "AT", name: "Austria" },
  { code: "AZ", name: "Azerbaijan" },
  { code: "BS", name: "Bahamas" },
  { code: "BH", name: "Bahrain" },
  { code: "BD", name: "Bangladesh" },
  { code: "BB", name: "Barbados" },
  { code: "BY", name: "Belarus" },
  { code: "BE", name: "Belgium" },
  { code: "BZ", name: "Belize" },
  { code: "BJ", name: "Benin" },
  { code: "BT", name: "Bhutan" },
  { code: "BO", name: "Bolivia" },
  { code: "BA", name: "Bosnia and Herzegovina" },
  { code: "BW", name: "Botswana" },
  { code: "BR", name: "Brazil" },
  { code: "BN", name: "Brunei" },
  { code: "BG", name: "Bulgaria" },
  { code: "BF", name: "Burkina Faso" },
  { code: "BI", name: "Burundi" },
  { code: "CV", name: "Cabo Verde" },
  { code: "KH", name: "Cambodia" },
  { code: "CM", name: "Cameroon" },
  { code: "CA", name: "Canada" },
  { code: "CF", name: "Central African Republic" },
  { code: "TD", name: "Chad" },
  { code: "CL", name: "Chile" },
  { code: "CN", name: "China" },
  { code: "CO", name: "Colombia" },
  { code: "KM", name: "Comoros" },
  { code: "CG", name: "Congo" },
  { code: "CD", name: "Congo (Democratic Republic)" },
  { code: "CR", name: "Costa Rica" },
  { code: "CI", name: "Côte d'Ivoire" },
  { code: "HR", name: "Croatia" },
  { code: "CU", name: "Cuba" },
  { code: "CY", name: "Cyprus" },
  { code: "CZ", name: "Czech Republic" },
  { code: "DK", name: "Denmark" },
  { code: "DJ", name: "Djibouti" },
  { code: "DM", name: "Dominica" },
  { code: "DO", name: "Dominican Republic" },
  { code: "EC", name: "Ecuador" },
  { code: "EG", name: "Egypt" },
  { code: "SV", name: "El Salvador" },
  { code: "GQ", name: "Equatorial Guinea" },
  { code: "ER", name: "Eritrea" },
  { code: "EE", name: "Estonia" },
  { code: "SZ", name: "Eswatini" },
  { code: "ET", name: "Ethiopia" },
  { code: "FJ", name: "Fiji" },
  { code: "FI", name: "Finland" },
  { code: "FR", name: "France" },
  { code: "GA", name: "Gabon" },
  { code: "GM", name: "Gambia" },
  { code: "GE", name: "Georgia" },
  { code: "DE", name: "Germany" },
  { code: "GH", name: "Ghana" },
  { code: "GR", name: "Greece" },
  { code: "GD", name: "Grenada" },
  { code: "GT", name: "Guatemala" },
  { code: "GN", name: "Guinea" },
  { code: "GW", name: "Guinea-Bissau" },
  { code: "GY", name: "Guyana" },
  { code: "HT", name: "Haiti" },
  { code: "HN", name: "Honduras" },
  { code: "HU", name: "Hungary" },
  { code: "IS", name: "Iceland" },
  { code: "IN", name: "India" },
  { code: "ID", name: "Indonesia" },
  { code: "IR", name: "Iran" },
  { code: "IQ", name: "Iraq" },
  { code: "IE", name: "Ireland" },
  { code: "IL", name: "Israel" },
  { code: "IT", name: "Italy" },
  { code: "JM", name: "Jamaica" },
  { code: "JP", name: "Japan" },
  { code: "JO", name: "Jordan" },
  { code: "KZ", name: "Kazakhstan" },
  { code: "KE", name: "Kenya" },
  { code: "KI", name: "Kiribati" },
  { code: "KP", name: "Korea (North)" },
  { code: "KR", name: "Korea (South)" },
  { code: "KW", name: "Kuwait" },
  { code: "KG", name: "Kyrgyzstan" },
  { code: "LA", name: "Laos" },
  { code: "LV", name: "Latvia" },
  { code: "LB", name: "Lebanon" },
  { code: "LS", name: "Lesotho" },
  { code: "LR", name: "Liberia" },
  { code: "LY", name: "Libya" },
  { code: "LI", name: "Liechtenstein" },
  { code: "LT", name: "Lithuania" },
  { code: "LU", name: "Luxembourg" },
  { code: "MG", name: "Madagascar" },
  { code: "MW", name: "Malawi" },
  { code: "MY", name: "Malaysia" },
  { code: "MV", name: "Maldives" },
  { code: "ML", name: "Mali" },
  { code: "MT", name: "Malta" },
  { code: "MH", name: "Marshall Islands" },
  { code: "MR", name: "Mauritania" },
  { code: "MU", name: "Mauritius" },
  { code: "MX", name: "Mexico" },
  { code: "FM", name: "Micronesia" },
  { code: "MD", name: "Moldova" },
  { code: "MC", name: "Monaco" },
  { code: "MN", name: "Mongolia" },
  { code: "ME", name: "Montenegro" },
  { code: "MA", name: "Morocco" },
  { code: "MZ", name: "Mozambique" },
  { code: "MM", name: "Myanmar" },
  { code: "NA", name: "Namibia" },
  { code: "NR", name: "Nauru" },
  { code: "NP", name: "Nepal" },
  { code: "NL", name: "Netherlands" },
  { code: "NZ", name: "New Zealand" },
  { code: "NI", name: "Nicaragua" },
  { code: "NE", name: "Niger" },
  { code: "NG", name: "Nigeria" },
  { code: "MK", name: "North Macedonia" },
  { code: "NO", name: "Norway" },
  { code: "OM", name: "Oman" },
  { code: "PK", name: "Pakistan" },
  { code: "PW", name: "Palau" },
  { code: "PS", name: "Palestine" },
  { code: "PA", name: "Panama" },
  { code: "PG", name: "Papua New Guinea" },
  { code: "PY", name: "Paraguay" },
  { code: "PE", name: "Peru" },
  { code: "PH", name: "Philippines" },
  { code: "PL", name: "Poland" },
  { code: "PT", name: "Portugal" },
  { code: "QA", name: "Qatar" },
  { code: "RO", name: "Romania" },
  { code: "RU", name: "Russia" },
  { code: "RW", name: "Rwanda" },
  { code: "KN", name: "Saint Kitts and Nevis" },
  { code: "LC", name: "Saint Lucia" },
  { code: "VC", name: "Saint Vincent and the Grenadines" },
  { code: "WS", name: "Samoa" },
  { code: "SM", name: "San Marino" },
  { code: "ST", name: "Sao Tome and Principe" },
  { code: "SA", name: "Saudi Arabia" },
  { code: "SN", name: "Senegal" },
  { code: "RS", name: "Serbia" },
  { code: "SC", name: "Seychelles" },
  { code: "SL", name: "Sierra Leone" },
  { code: "SG", name: "Singapore" },
  { code: "SK", name: "Slovakia" },
  { code: "SI", name: "Slovenia" },
  { code: "SB", name: "Solomon Islands" },
  { code: "SO", name: "Somalia" },
  { code: "ZA", name: "South Africa" },
  { code: "SS", name: "South Sudan" },
  { code: "ES", name: "Spain" },
  { code: "LK", name: "Sri Lanka" },
  { code: "SD", name: "Sudan" },
  { code: "SR", name: "Suriname" },
  { code: "SE", name: "Sweden" },
  { code: "CH", name: "Switzerland" },
  { code: "SY", name: "Syria" },
  { code: "TW", name: "Taiwan" },
  { code: "TJ", name: "Tajikistan" },
  { code: "TZ", name: "Tanzania" },
  { code: "TH", name: "Thailand" },
  { code: "TL", name: "Timor-Leste" },
  { code: "TG", name: "Togo" },
  { code: "TO", name: "Tonga" },
  { code: "TT", name: "Trinidad and Tobago" },
  { code: "TN", name: "Tunisia" },
  { code: "TR", name: "Turkey" },
  { code: "TM", name: "Turkmenistan" },
  { code: "TV", name: "Tuvalu" },
  { code: "UG", name: "Uganda" },
  { code: "UA", name: "Ukraine" },
  { code: "AE", name: "United Arab Emirates" },
  { code: "GB", name: "United Kingdom" },
  { code: "US", name: "United States" },
  { code: "UY", name: "Uruguay" },
  { code: "UZ", name: "Uzbekistan" },
  { code: "VU", name: "Vanuatu" },
  { code: "VA", name: "Vatican City" },
  { code: "VE", name: "Venezuela" },
  { code: "VN", name: "Vietnam" },
  { code: "YE", name: "Yemen" },
  { code: "ZM", name: "Zambia" },
  { code: "ZW", name: "Zimbabwe" },
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
    name: "Standard",
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
      // Get the correct name for the selected plan
      const selectedPlan = PLANS.find(plan => plan.id === formData.planId);
      
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
      };
      
      console.log("API URL:", `${config.apiUrl}/api/Client/register`);
      console.log("Request payload:", requestData);
      
      // First, try to create plans directly via the database service
      try {
        console.log("Attempting to initialize plans first...");
        await fetch(`${config.apiUrl}/api/Install`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            dbType: "MySQL",
            serviceIP: "localhost",
            port: "3306",
            dbName: "saas_base",
            rootUser: "root",
            rootPassword: "admin",
            adminUser: "admin",
            adminPassword: "admin123"
          }),
        });
        console.log("Database initialization complete");
      } catch (err) {
        console.log("Database initialization error (can be ignored):", err);
      }
      
      // Add debugging to see the full request being sent
      const requestDataString = JSON.stringify(requestData);
      console.log("Request data as string:", requestDataString);
      
      // Add plan ID explicitly
      const finalRequestData = {
        ...requestData,
        planName: selectedPlan?.name || "Standard",  // Add explicit plan name
        skipClientPlan: false   // Enable client plan creation
      };
      
      const response = await fetch(`${config.apiUrl}/api/Client/register`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(finalRequestData),
      });
      
      const data = await response.json();
      console.log("Response status:", response.status);
      console.log("Response data:", data);
      
      if (!response.ok) {
        throw new Error(data.message || data.error || JSON.stringify(data) || "Registration failed");
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
              <Box 
                sx={{ 
                  display: 'grid',
                  gridTemplateColumns: {
                    xs: '1fr',            // 1 column on mobile
                    md: 'repeat(3, 1fr)'  // 3 columns on medium and up
                  },
                  gap: 3
                }}
              >
                {PLANS.map((plan) => (
                  <Box key={plan.id}>
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
                        mt: 2, // Add top margin for the Recommended chip
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
                            top: -15,
                            right: 20,
                            fontWeight: 'bold',
                            zIndex: 1,
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
                  </Box>
                ))}
              </Box>
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
            
            <Stack spacing={3}>
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
              
              <TextField
                name="industry"
                label="Industry"
                fullWidth
                value={formData.industry}
                onChange={handleChange}
                placeholder="e.g. Healthcare, Technology, Finance"
              />
            </Stack>
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
            
            <Stack spacing={3}>
              <Box sx={{ display: 'flex', gap: 2 }}>
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
              </Box>
              
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
            </Stack>
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
            
            <Stack spacing={3}>
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
              
              {formData.domainName && (
                <Alert severity="info">
                  Your organization will be accessible at:{" "}
                  <Typography component="span" fontWeight="bold">
                    {formData.domainName}.localhost:5173
                  </Typography>
                </Alert>
              )}
            </Stack>
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
            
            <Stack spacing={3}>
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
              
              <Box>
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
              </Box>
            </Stack>
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