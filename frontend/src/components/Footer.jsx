import React from "react";
import { Box, Typography, Divider, Link, Stack, useTheme } from "@mui/material";
import GitHubIcon from '@mui/icons-material/GitHub';
import LinkedInIcon from '@mui/icons-material/LinkedIn';
import TwitterIcon from '@mui/icons-material/Twitter';

const Footer = () => {
  const theme = useTheme();
  const currentYear = new Date().getFullYear();

  return (
    <Box
      component="footer"
      sx={{
        width: "100%",
        py: 3,
        px: { xs: 2, sm: 6 },
        mt: "auto",
        backgroundColor: theme.palette.background.paper,
        borderTop: `1px solid ${theme.palette.divider}`,
        boxShadow: 0,
      }}
    >
      <Box
        sx={{
          display: "flex",
          flexDirection: { xs: "column", md: "row" },
          justifyContent: "space-between",
          alignItems: { xs: "center", md: "flex-start" },
          gap: 3,
          width: "100%"
        }}
      >
        <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: { xs: 'center', md: 'flex-start' } }}>
          <Typography variant="subtitle2" color="text.primary" sx={{ fontWeight: 600, mb: 0.5 }}>
            RCode Database Manager
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            A simple tool to manage database installations
          </Typography>
        </Box>
        <Box>
          <Typography variant="subtitle2" color="text.primary" sx={{ fontWeight: 600, mb: 1.5, textAlign: { xs: 'center', md: 'left' } }}>
            Resources
          </Typography>
          <Stack spacing={1} sx={{ alignItems: { xs: 'center', md: 'flex-start' } }}>
            <Link href="#" color="text.secondary" underline="hover" variant="body2">Documentation</Link>
            <Link href="#" color="text.secondary" underline="hover" variant="body2">API Reference</Link>
            <Link href="#" color="text.secondary" underline="hover" variant="body2">Support</Link>
          </Stack>
        </Box>
        <Box>
          <Typography variant="subtitle2" color="text.primary" sx={{ fontWeight: 600, mb: 1.5, textAlign: { xs: 'center', md: 'left' } }}>
            Follow Us
          </Typography>
          <Stack direction="row" spacing={2} sx={{ justifyContent: 'center', '& .MuiSvgIcon-root': { transition: 'all 0.2s ease-in-out', '&:hover': { color: 'primary.main', transform: 'scale(1.1)' } } }}>
            <Link href="#" color="text.secondary" underline="none"><GitHubIcon /></Link>
            <Link href="#" color="text.secondary" underline="none"><LinkedInIcon /></Link>
            <Link href="#" color="text.secondary" underline="none"><TwitterIcon /></Link>
          </Stack>
        </Box>
      </Box>
      <Divider sx={{ my: 3 }} />
      <Box sx={{ display: "flex", flexDirection: { xs: "column", sm: "row" }, justifyContent: "space-between", alignItems: "center", gap: 1, width: "100%" }}>
        <Typography variant="body2" color="text.secondary" sx={{ textAlign: { xs: 'center', sm: 'left' } }}>
          © {currentYear} RCode Technologies. All rights reserved.
        </Typography>
        <Typography variant="caption" color="text.secondary" sx={{ opacity: 0.8 }}>
          Version 1.0.0
        </Typography>
      </Box>
    </Box>
  );
};

export default Footer;