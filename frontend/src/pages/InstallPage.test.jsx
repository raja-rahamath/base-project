import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom';
import InstallPage from './InstallPage';
import { ThemeProvider } from '@mui/material/styles';
import { lightTheme } from '../theme';
import { act } from 'react-dom/test-utils';

// Mock fetch
global.fetch = jest.fn();

// Mock theme for component testing
const renderWithTheme = (ui) => {
  return render(
    <ThemeProvider theme={lightTheme}>
      {ui}
    </ThemeProvider>
  );
};

describe('InstallPage Component', () => {
  beforeEach(() => {
    fetch.mockClear();
  });

  it('renders properly with all form fields', () => {
    renderWithTheme(<InstallPage />);
    
    // Verify heading is present
    expect(screen.getByText('Database Setup Wizard')).toBeInTheDocument();
    
    // First step should be visible (Database Type selection)
    expect(screen.getByText('Select Database Type')).toBeInTheDocument();
    
    // MySQL should be pre-selected
    expect(screen.getByText('MySQL')).toBeInTheDocument();
  });

  it('allows navigation through steps when valid data is provided', async () => {
    renderWithTheme(<InstallPage />);
    
    // First step (Database Type) is already valid with default selection
    const nextButton = screen.getByText('Next');
    expect(nextButton).not.toBeDisabled();
    
    // Move to step 2
    fireEvent.click(nextButton);
    await waitFor(() => {
      expect(screen.getByText('Server Connection Details')).toBeInTheDocument();
    });
    
    // Step 2 should already be valid with default values
    expect(screen.getByText('Next')).not.toBeDisabled();
    
    // Move to step 3
    fireEvent.click(screen.getByText('Next'));
    await waitFor(() => {
      expect(screen.getByText('Root Credentials')).toBeInTheDocument();
    });
    
    // Step 3 should already be valid with default values
    expect(screen.getByText('Next')).not.toBeDisabled();
    
    // Move to step 4
    fireEvent.click(screen.getByText('Next'));
    await waitFor(() => {
      expect(screen.getByText('New Database and Admin User')).toBeInTheDocument();
    });
    
    // Final step should have a Create Database button
    expect(screen.getByText('Create Database')).toBeInTheDocument();
  });

  it('shows validation errors for invalid inputs', async () => {
    renderWithTheme(<InstallPage />);
    
    // Move to server connection step
    fireEvent.click(screen.getByText('Next'));
    
    // Clear the IP field and add invalid IP
    const ipField = screen.getByLabelText('Server IP or Domain Name');
    fireEvent.change(ipField, { target: { value: '' } });
    fireEvent.change(ipField, { target: { value: '999.999.999.999' } });
    
    // Check for validation error
    expect(await screen.findByText('Please enter a valid IP address or domain name')).toBeInTheDocument();
    
    // Next button should be disabled due to validation error
    expect(screen.getByText('Next')).toBeDisabled();
  });

  it('handles form submission correctly', async () => {
    // Mock a successful response
    fetch.mockResolvedValueOnce({
      ok: true,
      json: async () => ({ success: true, message: 'Database and user created successfully', data: { connectionId: '123' } }),
    });

    renderWithTheme(<InstallPage />);
    
    // Navigate to the last step
    await act(async () => {
      fireEvent.click(screen.getByText('Next')); // Step 1 to 2
      await waitFor(() => expect(screen.getByText('Server Connection Details')).toBeInTheDocument());
      
      fireEvent.click(screen.getByText('Next')); // Step 2 to 3
      await waitFor(() => expect(screen.getByText('Root Credentials')).toBeInTheDocument());
      
      fireEvent.click(screen.getByText('Next')); // Step 3 to 4
      await waitFor(() => expect(screen.getByText('New Database and Admin User')).toBeInTheDocument());
    });
    
    // Submit the form
    await act(async () => {
      fireEvent.click(screen.getByText('Create Database'));
    });
    
    // Verify fetch was called with correct data
    expect(fetch).toHaveBeenCalledTimes(1);
    expect(fetch).toHaveBeenCalledWith(expect.stringContaining('/api/install'), expect.objectContaining({
      method: 'POST',
      headers: expect.objectContaining({
        'Content-Type': 'application/json',
      }),
      body: expect.any(String),
    }));
    
    // Success message should appear
    await waitFor(() => {
      expect(screen.getByText('Database and user created successfully!')).toBeInTheDocument();
    });
  });

  it('handles API errors correctly', async () => {
    // Mock an error response
    fetch.mockResolvedValueOnce({
      ok: false,
      json: async () => ({ success: false, error: 'Database connection failed' }),
    });

    renderWithTheme(<InstallPage />);
    
    // Navigate to the last step
    await act(async () => {
      fireEvent.click(screen.getByText('Next'));
      await waitFor(() => expect(screen.getByText('Server Connection Details')).toBeInTheDocument());
      
      fireEvent.click(screen.getByText('Next'));
      await waitFor(() => expect(screen.getByText('Root Credentials')).toBeInTheDocument());
      
      fireEvent.click(screen.getByText('Next'));
      await waitFor(() => expect(screen.getByText('New Database and Admin User')).toBeInTheDocument());
    });
    
    // Submit the form
    await act(async () => {
      fireEvent.click(screen.getByText('Create Database'));
    });
    
    // Error message should appear
    await waitFor(() => {
      expect(screen.getByText('Database connection failed')).toBeInTheDocument();
    });
  });
});