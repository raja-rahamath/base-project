import React, { useEffect, useState } from "react";

const BillingPage = () => {
  const [records, setRecords] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetch("/api/Billing")
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch billing records");
        return res.json();
      })
      .then(setRecords)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  const downloadInvoice = (id, format) => {
    window.open(`/api/Billing/${id}/invoice?format=${format}`, "_blank");
  };

  if (loading) return <div>Loading billing records...</div>;

  return (
    <div>
      <h2>Billing History</h2>
      <table className="billing-table">
        <thead>
          <tr>
            <th>Invoice #</th>
            <th>Amount</th>
            <th>Period</th>
            <th>Status</th>
            <th>Created</th>
            <th>Paid</th>
            <th>Download</th>
          </tr>
        </thead>
        <tbody>
          {error ? (
            <tr>
              <td colSpan={7} style={{ color: "red", textAlign: "center" }}>
                Error: {error}
              </td>
            </tr>
          ) : records.length === 0 ? (
            <tr>
              <td colSpan={7} style={{ textAlign: "center" }}>
                No billing records found.
              </td>
            </tr>
          ) : (
            records.map((r) => (
              <tr key={r.id}>
                <td>{r.invoiceNumber}</td>
                <td>
                  {r.amount} {r.currency}
                </td>
                <td>
                  {new Date(r.billingPeriodStart).toLocaleDateString()} -{" "}
                  {new Date(r.billingPeriodEnd).toLocaleDateString()}
                </td>
                <td>{r.status}</td>
                <td>{new Date(r.createdAt).toLocaleDateString()}</td>
                <td>
                  {r.paidAt ? new Date(r.paidAt).toLocaleDateString() : "-"}
                </td>
                <td>
                  <button onClick={() => downloadInvoice(r.id, "pdf")}>
                    PDF
                  </button>
                  <button onClick={() => downloadInvoice(r.id, "csv")}>
                    CSV
                  </button>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
};

export default BillingPage;
