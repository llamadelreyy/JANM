# Ngrok Setup Guide for External Access

## Prerequisites
1. Install ngrok
2. Create an ngrok account and get your authtoken
3. Have both frontend and backend servers running locally

## Setup Steps

### 1. Configure Ngrok Authentication
```bash
ngrok config add-authtoken YOUR_AUTH_TOKEN
```

### 2. Start Frontend Tunnel (Port 2002)
In a new terminal:
```bash
ngrok http 2002
```
This will create a URL like `https://your-subdomain.ngrok-free.dev`

### 3. Start Backend Tunnel (Port 11434)
In another terminal:
```bash
ngrok http 11434
```
This will create another URL for the backend.

### 4. Update Frontend Configuration
The Vite config (vite.config.js) is already set up to:
- Allow ngrok domains in allowedHosts
- Include proper CSP headers for ngrok domains
- Handle WebSocket connections for HMR

### 5. Access the Application
1. Use the frontend ngrok URL (e.g., https://your-subdomain.ngrok-free.dev)
2. The application will automatically proxy requests to the backend

## Troubleshooting

### If Chat Shows "Tidak tersambung"
1. Verify both ngrok tunnels are running
2. Check the ngrok console for any connection errors
3. Make sure both frontend and backend servers are running locally
4. Verify the CSP headers allow your ngrok domain

### Common Issues
- WebSocket connection fails: Make sure the CSP headers include `wss://*.ngrok-free.dev`
- Images don't load: Verify the CSP img-src includes ngrok domains
- Backend unreachable: Check that both ngrok tunnels are active

## Security Notes
- Ngrok URLs are public - anyone with the URL can access your application
- Consider using ngrok's authentication features for additional security
- Use HTTPS URLs to ensure secure communication