import React, { useState } from "react";

const ProfilePage = () => {
  const [email, setEmail] = useState("");
  const [newEmail, setNewEmail] = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);

  // Optionally fetch current email on mount
  // useEffect(() => { ... }, []);

  const handleEmailChange = (e) => setNewEmail(e.target.value);
  const handlePasswordChange = (e) => setPassword(e.target.value);

  const submitEmailChange = (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage("");
    fetch("/api/Client/change-email", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ newEmail }),
    })
      .then((res) => res.json())
      .then((data) => setMessage(data.message || "Email updated."))
      .catch((err) => setMessage("Error: " + err.message))
      .finally(() => setLoading(false));
  };

  const submitPasswordChange = (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage("");
    fetch("/api/Client/change-password", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ newPassword: password }),
    })
      .then((res) => res.json())
      .then((data) => setMessage(data.message || "Password updated."))
      .catch((err) => setMessage("Error: " + err.message))
      .finally(() => setLoading(false));
  };

  return (
    <div>
      <h2>Profile</h2>
      <form onSubmit={submitEmailChange} style={{ marginBottom: 24 }}>
        <label>Change Email: </label>
        <input
          type="email"
          value={newEmail}
          onChange={handleEmailChange}
          required
        />
        <button type="submit" disabled={loading}>
          Change Email
        </button>
      </form>
      <form onSubmit={submitPasswordChange}>
        <label>Change Password: </label>
        <input
          type="password"
          value={password}
          onChange={handlePasswordChange}
          required
        />
        <button type="submit" disabled={loading}>
          Change Password
        </button>
      </form>
      {message && (
        <div
          style={{
            marginTop: 16,
            color: message.startsWith("Error") ? "red" : "green",
          }}
        >
          {message}
        </div>
      )}
    </div>
  );
};

export default ProfilePage;
