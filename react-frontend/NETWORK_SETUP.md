# Network Setup for External Device Access

## Problem
When you port forward the React app (port 2000) and access it from another device, the AI chat shows "Tidak tersambung" (Not connected) because external devices can't reach the AI backend.

## Root Cause
The AI backend runs on `http://192.168.50.125:5501` which is only accessible from the local network. When you port forward only port 2000, external devices can't reach the AI backend on port 5501.

## Solution - Automatic Proxy Configuration

### How It Works Now
The application now automatically handles network access through a built-in proxy:

1. **Local Development**: Uses direct connection to `http://192.168.50.125:5501/v1`
2. **External Access**: Automatically uses proxy route through the frontend server

### Setup Steps

#### Step 1: Ensure AI Backend is Running
Make sure your AI backend is running on `http://192.168.50.125:5501`

#### Step 2: Start the React App
```bash
cd react-frontend
npm run dev
```

#### Step 3: Port Forward Only Port 2000
You only need to port forward port 2000 (the React app). The AI backend will be automatically proxied through the same port.

#### Step 4: Access from External Device
When you access the app from an external device via `your-external-ip:2000`, the AI chat will automatically work because:
- Frontend requests to `/v1/*` are proxied to `http://192.168.50.125:5501/v1/*`
- No additional port forwarding needed

## Technical Details

### Vite Proxy Configuration
The `vite.config.js` now includes:
```javascript
proxy: {
  '/v1': {
    target: 'http://192.168.50.125:5501',
    changeOrigin: true,
    secure: false,
  }
}
```

### Automatic URL Detection
The `ollamaService.js` automatically detects:
- **Development mode**: Uses direct backend URL
- **Production/External access**: Uses proxy route through frontend

### Manual Configuration
If needed, you can still manually configure the AI backend URL:
1. Click "Config URL" in the AI chat
2. Enter custom URL (e.g., `http://your-server-ip:5501/v1`)
3. Click "Update"

## Troubleshooting

### If AI Chat Still Shows "Tidak tersambung"
1. Check that the AI backend is running on `http://192.168.50.125:5501`
2. Verify the React app started successfully with `npm run dev`
3. Check the current URL in AI chat by clicking "Config URL"
4. Try manually setting the URL to `/v1` (proxy route)

### Network Connectivity
- Only port 2000 needs to be port forwarded
- AI backend (port 5501) should be accessible from the React app server
- External devices connect through the proxy automatically

### Debug Information
The AI chat page shows the current backend URL being used. Click "Config URL" to see:
- Current URL being used
- Option to manually override if needed

## Quick Test
1. Start the React app: `npm run dev`
2. Port forward port 2000
3. Access from external device: `your-external-ip:2000`
4. Go to AI Chat page
5. AI should connect automatically through the proxy