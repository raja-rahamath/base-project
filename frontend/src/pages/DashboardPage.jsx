import React from "react";
import { Card, CardContent, Typography, Grid } from "@mui/material";

const DashboardPage = () => {
  // Demo stats, replace with real API data if available
  const stats = [
    { label: "Active Users", value: 5 },
    { label: "Current Plan", value: "Pro" },
    { label: "Storage Used", value: "2.3 GB / 10 GB" },
    { label: "Next Invoice", value: "2025-05-01" },
  ];

  return (
    <div>
      <Typography variant="h4" gutterBottom>
        Welcome to your SaaS Dashboard
      </Typography>
      <Typography variant="body1" color="text.secondary" gutterBottom>
        Here you can manage your subscription, billing, and account settings.
      </Typography>
      <Grid container spacing={3} sx={{ mt: 2 }}>
        {stats.map((stat) => (
          <Grid item xs={12} sm={6} md={3} key={stat.label}>
            <Card>
              <CardContent>
                <Typography variant="h6" color="text.secondary">
                  {stat.label}
                </Typography>
                <Typography variant="h5" fontWeight={700}>
                  {stat.value}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
    </div>
  );
};

export default DashboardPage;
