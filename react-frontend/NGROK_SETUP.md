# Ngrok Setup Guide for External Access

## Prerequisites
1. Install ngrok: `npm install -g ngrok` or download from https://ngrok.com
2. Create an ngrok account and get your authtoken
3. Have both frontend and backend servers running locally

## Quick Start

### 1. Configure Ngrok Authentication
```bash
ngrok config add-authtoken YOUR_AUTH_TOKEN
```

### 2. Update Ngrok Configuration
Edit `vite.config.js` and update the `NGROK_HOST` variable with your ngrok URL:
```javascript
const NGROK_HOST = 'your-actual-subdomain.ngrok-free.dev'
```

### 3. Enable Ngrok Mode
Copy the ngrok environment file to activate ngrok mode:
```bash
cd react-frontend
cp .env.ngrok .env
```

Update `.env` with your actual ngrok host if needed:
```env
VITE_USE_NGROK=true
VITE_NGROK_HOST=your-actual-subdomain.ngrok-free.dev
```

### 4. Start Frontend Tunnel (Port 2002)
In a new terminal:
```bash
ngrok http 2002 --host-header=rewrite
```
This will create a URL like `https://your-subdomain.ngrok-free.dev`

**Important**: Copy the ngrok URL and update both `vite.config.js` and `.env` with this URL.

### 5. Restart Frontend Server
After updating the configuration:
```bash
# Stop the current frontend server (Ctrl+C)
# Then restart it
npm run dev
```

### 6. Access the Application
1. Open the ngrok URL in your browser (e.g., `https://your-subdomain.ngrok-free.dev`)
2. The backend will be accessed through Vite's proxy configuration
3. All API calls will route through `/api/*` proxy to `localhost:3002`

## Architecture

### Local Development (Default)
```
Browser → http://localhost:2002 → Vite Dev Server
                                 ↓
                          Proxy (/api/*) → http://localhost:3002 → Backend
```

### Ngrok External Access
```
Remote Browser → https://xxx.ngrok-free.dev → Ngrok Tunnel → Vite Dev Server (localhost:2002)
                                                             ↓
                                                      Proxy (/api/*) → Backend (localhost:3002)
```

## Troubleshooting

### CORS Errors
The backend is configured to allow all origins (`origin: '*'`), so CORS shouldn't be an issue. If you still see CORS errors:
1. Verify backend is running on port 3002
2. Check backend console for error messages
3. Ensure `VITE_USE_NGROK=true` in `.env`

### WebSocket/HMR Connection Fails
If you see `Firefox can't establish a connection to the server at wss://...`:
1. Verify you've updated `NGROK_HOST` in `vite.config.js`
2. Ensure you've restarted the frontend server after changes
3. Check that ngrok tunnel is running with `--host-header=rewrite`

### Chat Connection Error
If chat shows network errors:
1. Make sure backend is running: `cd backend && npm start`
2. Test backend directly: `curl http://localhost:3002/health`
3. Check browser console for specific error messages
4. Verify `.env` has `VITE_USE_NGROK=true`

### Backend Not Accessible
1. Backend runs on `localhost:3002` (check with `netstat -an | grep 3002`)
2. Vite proxy is configured to forward `/api/*` to backend
3. When accessing through ngrok, requests go: ngrok → Vite → Backend

## Switching Between Local and Ngrok

### For Local Development
```bash
cd react-frontend
cp .env.local .env  # or just set VITE_USE_NGROK=false
npm run dev
```

### For Ngrok Access
```bash
cd react-frontend
cp .env.ngrok .env
# Update NGROK_HOST in vite.config.js and .env
npm run dev
# In another terminal:
ngrok http 2002 --host-header=rewrite
```

## Security Notes
- Ngrok URLs are public - anyone with the URL can access your application
- Consider using ngrok's authentication features for production
- Backend is configured to accept requests from any origin (CORS: `*`)
- Use HTTPS URLs (ngrok provides this by default)
- Keep your ngrok authtoken secure