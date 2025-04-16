import React from "react";
import {
  Box,
  AppBar,
  Toolbar,
  Typography,
  Drawer,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  IconButton,
  Divider,
  useTheme,
} from "@mui/material";
import DashboardIcon from "@mui/icons-material/Dashboard";
import ReceiptIcon from "@mui/icons-material/Receipt";
import AssignmentIcon from "@mui/icons-material/Assignment";
import PersonIcon from "@mui/icons-material/Person";
import LogoutIcon from "@mui/icons-material/Logout";
import MenuIcon from "@mui/icons-material/Menu";
import StorageIcon from "@mui/icons-material/Storage";
import { Link, useLocation } from "react-router-dom";
import Footer from "./Footer";
import { useAuth } from "../auth";

const drawerWidth = 220;

const menuItems = [
  { text: "Dashboard", icon: <DashboardIcon />, path: "/dashboard" },
  { text: "Billing", icon: <ReceiptIcon />, path: "/billing" },
  { text: "Plan", icon: <AssignmentIcon />, path: "/plan" },
  { text: "Profile", icon: <PersonIcon />, path: "/profile" },
];

const Layout = ({ children }) => {
  const theme = useTheme();
  const location = useLocation();
  const { logout } = useAuth();

  return (
    <Box
      sx={{
        display: "flex",
        minHeight: "100vh",
        bgcolor: "background.default",
      }}
    >
      <Drawer
        variant="permanent"
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          [`& .MuiDrawer-paper`]: {
            width: drawerWidth,
            boxSizing: "border-box",
            bgcolor: "background.paper",
            borderRight: `1px solid ${theme.palette.divider}`,
          },
        }}
      >
        <Toolbar sx={{ justifyContent: "center", py: 2 }}>
          <StorageIcon sx={{ fontSize: 32, color: "primary.main", mr: 1 }} />
          <Typography variant="h6" fontWeight={700} color="primary.main">
            SaaS Portal
          </Typography>
        </Toolbar>
        <Divider />
        <List>
          {menuItems.map((item) => (
            <ListItem
              button
              key={item.text}
              component={Link}
              to={item.path}
              selected={location.pathname === item.path}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItem>
          ))}
          <Divider sx={{ my: 1 }} />
          <ListItem button onClick={logout}>
            <ListItemIcon>
              <LogoutIcon color="error" />
            </ListItemIcon>
            <ListItemText
              primary="Logout"
              primaryTypographyProps={{ color: "error.main" }}
            />
          </ListItem>
        </List>
      </Drawer>
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          flexGrow: 1,
          minHeight: "100vh",
          ml: `${drawerWidth}px`,
          width: `calc(100% - ${drawerWidth}px)`,
        }}
      >
        <AppBar
          position="fixed"
          sx={{
            width: `calc(100% - ${drawerWidth}px)`,
            ml: `${drawerWidth}px`,
            bgcolor: "background.paper",
            color: "text.primary",
            boxShadow: 0,
            borderBottom: `1px solid ${theme.palette.divider}`,
          }}
          elevation={0}
        >
          <Toolbar>
            <Typography variant="h6" fontWeight={600} sx={{ flexGrow: 1 }}>
              {menuItems.find((item) => location.pathname.startsWith(item.path))
                ?.text || "Dashboard"}
            </Typography>
          </Toolbar>
        </AppBar>
        <Toolbar />
        <Box sx={{ mt: 2, flexGrow: 1, width: "100%" }}>{children}</Box>
        <Footer />
      </Box>
    </Box>
  );
};

export default Layout;
