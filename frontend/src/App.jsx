import React, { useState, useMemo } from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import {
  ThemeProvider,
  CssBaseline,
  IconButton,
  Box,
  useMediaQuery,
} from "@mui/material";
import DarkModeIcon from "@mui/icons-material/DarkMode";
import LightModeIcon from "@mui/icons-material/LightMode";
import InstallPage from "./pages/InstallPage";
import RegisterPage from "./pages/RegisterPage";
import LoginPage from "./pages/LoginPage";
import Layout from "./components/Layout";
import { lightTheme, darkTheme } from "./theme";
import "./App.css";

function App() {
  // Use system preference as initial theme
  const prefersDarkMode = useMediaQuery("(prefers-color-scheme: dark)");
  const [mode, setMode] = useState(prefersDarkMode ? "dark" : "light");
  
  const theme = useMemo(
    () => (mode === "light" ? lightTheme : darkTheme),
    [mode]
  );

  const toggleTheme = () => {
    setMode((prevMode) => (prevMode === "light" ? "dark" : "light"));
  };

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Router>
        <Layout>
          <Box
            sx={{
              position: "fixed",
              top: 16,
              right: 16,
              zIndex: 1100,
              bgcolor: "background.paper",
              borderRadius: "50%",
              boxShadow: 2,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              width: 40,
              height: 40,
              transition: "all 0.2s ease-in-out",
              "&:hover": {
                transform: "scale(1.1)",
                boxShadow: 3,
              },
            }}
          >
            <IconButton
              onClick={toggleTheme}
              color="primary"
              aria-label="toggle theme"
              size="small"
            >
              {mode === "light" ? (
                <DarkModeIcon fontSize="small" />
              ) : (
                <LightModeIcon fontSize="small" />
              )}
            </IconButton>
          </Box>

          <Routes>
            <Route path="/" element={<RegisterPage />} />
            <Route path="/install" element={<InstallPage />} />
            <Route path="/login" element={<LoginPage />} />
            {/* Add more routes here as needed */}
          </Routes>
        </Layout>
      </Router>
    </ThemeProvider>
  );
}

export default App;