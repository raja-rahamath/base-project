import React, { useEffect, useState } from "react";
import {
  Typography,
  Box,
  Button,
  Card,
  CardContent,
  Grid,
  Alert,
} from "@mui/material";

const PlanPage = () => {
  const [plans, setPlans] = useState([]);
  const [currentPlan, setCurrentPlan] = useState({
    name: "Pro",
    billingCycle: "Annual",
    price: 120,
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedPlan, setSelectedPlan] = useState("");
  const [billingCycle, setBillingCycle] = useState(0); // 0 = Monthly, 1 = Annual
  const [confirming, setConfirming] = useState(false);
  const [message, setMessage] = useState("");

  useEffect(() => {
    fetch("/api/Plan")
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch plans");
        return res.json();
      })
      .then((data) => setPlans(data))
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  const handleChangePlan = () => {
    setConfirming(true);
  };

  const confirmChangePlan = () => {
    setMessage("");
    fetch("/api/Plan/change", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        PlanId: selectedPlan,
        BillingCycle: billingCycle,
      }),
    })
      .then((res) => res.json())
      .then((data) => setMessage(data.message || "Plan changed successfully."))
      .catch((err) => setMessage("Error: " + err.message))
      .finally(() => setConfirming(false));
  };

  const handleCancelSubscription = () => {
    if (
      !window.confirm(
        "Are you sure you want to cancel your subscription? This action cannot be undone."
      )
    )
      return;
    setMessage("");
    fetch("/api/Plan/cancel", { method: "POST" })
      .then((res) => res.json())
      .then((data) => setMessage(data.message || "Subscription cancelled."))
      .catch((err) => setMessage("Error: " + err.message));
  };

  if (loading) return <div>Loading plans...</div>;

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Plan Management
      </Typography>
      <Box mb={3}>
        <Alert severity="info">
          <b>Current Plan:</b> {currentPlan.name} ({currentPlan.billingCycle}) -
          ${currentPlan.price}/yr
        </Alert>
      </Box>
      <Grid container spacing={3}>
        {plans.map((plan) => (
          <Grid item xs={12} sm={6} md={4} key={plan.id}>
            <Card
              variant={selectedPlan === plan.id ? "outlined" : undefined}
              sx={{
                borderColor:
                  selectedPlan === plan.id ? "primary.main" : undefined,
              }}
            >
              <CardContent>
                <Typography variant="h6">{plan.name}</Typography>
                <Typography color="text.secondary">
                  {plan.description}
                </Typography>
                <Typography sx={{ mt: 1 }}>
                  <b>${plan.monthlyPrice}/mo</b> or{" "}
                  <b>${plan.annualPrice}/yr</b>
                </Typography>
                <Typography
                  variant="body2"
                  color="text.secondary"
                  sx={{ mt: 1 }}
                >
                  Max Users: {plan.maxUsers}
                </Typography>
                <Button
                  variant={selectedPlan === plan.id ? "contained" : "outlined"}
                  sx={{ mt: 2 }}
                  onClick={() => setSelectedPlan(plan.id)}
                  fullWidth
                >
                  {selectedPlan === plan.id ? "Selected" : "Select"}
                </Button>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
      <Box mt={3}>
        <Typography>Billing Cycle:</Typography>
        <Button
          variant={billingCycle === 0 ? "contained" : "outlined"}
          onClick={() => setBillingCycle(0)}
          sx={{ mr: 2 }}
        >
          Monthly
        </Button>
        <Button
          variant={billingCycle === 1 ? "contained" : "outlined"}
          onClick={() => setBillingCycle(1)}
        >
          Annual
        </Button>
      </Box>
      <Box mt={3}>
        <Button
          variant="contained"
          color="primary"
          disabled={!selectedPlan}
          onClick={handleChangePlan}
        >
          Change Plan
        </Button>
        <Button
          variant="outlined"
          color="error"
          sx={{ ml: 2 }}
          onClick={handleCancelSubscription}
        >
          Cancel Subscription
        </Button>
      </Box>
      {confirming && (
        <Alert severity="warning" sx={{ mt: 3 }}>
          Are you sure you want to change your plan?
          <Box mt={2}>
            <Button
              onClick={confirmChangePlan}
              variant="contained"
              color="primary"
            >
              Yes, Change Plan
            </Button>
            <Button onClick={() => setConfirming(false)} sx={{ ml: 2 }}>
              Cancel
            </Button>
          </Box>
        </Alert>
      )}
      {message && (
        <Alert
          sx={{ mt: 3 }}
          severity={message.startsWith("Error") ? "error" : "success"}
        >
          {message}
        </Alert>
      )}
      {error && (
        <Alert sx={{ mt: 3 }} severity="error">
          {error}
        </Alert>
      )}
    </Box>
  );
};

export default PlanPage;
