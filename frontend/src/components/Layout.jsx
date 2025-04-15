import React from "react";
import { Box, AppBar, Toolbar, Typography, Container, useTheme, Button, Stack } from "@mui/material";
import { Link as RouterLink, useLocation } from "react-router-dom";
import Footer from "./Footer";
import StorageIcon from '@mui/icons-material/Storage';

const Layout = ({ children }) => {
  const theme = useTheme();
  const location = useLocation();

  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "column",
        minHeight: "100vh",
      }}
    >
      <AppBar 
        position="static" 
        elevation={0}
        sx={{ 
          backgroundColor: 'background.paper',
          borderBottom: `1px solid ${theme.palette.divider}`,
          color: 'text.primary'
        }}
      >
        <Toolbar>
          <StorageIcon 
            sx={{ 
              fontSize: 28, 
              mr: 1.5, 
              color: 'primary.main' 
            }} 
          />
          <Typography 
            variant="h6" 
            component="div"
            sx={{ 
              flexGrow: 1,
              fontWeight: 600,
              letterSpacing: '-0.5px',
              color: 'inherit',
              display: 'flex',
              alignItems: 'center'
            }}
          >
            RCode Database Manager
          </Typography>
        </Toolbar>
      </AppBar>

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          py: { xs: 2, sm: 4 },
          px: { xs: 2, sm: 3 }
        }}
      >
        <Container maxWidth="lg">
          {children}
        </Container>
      </Box>
      
      <Footer />
    </Box>
  );
};

export default Layout;