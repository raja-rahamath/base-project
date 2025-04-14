// Force refresh environment variables
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

// Debug environment
console.log('Environment variables loaded:');
console.log('VITE_API_URL:', import.meta.env.VITE_API_URL);
console.log('Using API URL:', API_URL);

const config = {
    apiUrl: API_URL,
    // Cache-busting parameter to prevent using cached endpoints
    timestamp: new Date().getTime()
};

console.log('Exported config:', config);

export default config;