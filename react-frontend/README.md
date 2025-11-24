# UPSI GPT Frontend

## Network Setup for External Access

### Local Development
1. Start the AI backend on port 5501
2. Start the React app:
```bash
cd react-frontend
npm run dev
```
The app will run on port 2000 and automatically proxy requests to the AI backend.

### Accessing from External Devices

1. Make sure the React app is running on your development machine
2. Port forward ONLY port 2000 to external devices
3. Access the app from external devices using:
```
http://your-external-ip:2000
```

The app will automatically proxy all AI requests through port 2000, so you don't need to port forward the AI backend port (5501).

### Troubleshooting

If you see "Tidak tersambung" (Not connected):
1. Verify the AI backend is running on port 5501
2. Check that you're accessing the app through port 2000
3. Make sure you're using the correct external IP address

### Technical Details

- Frontend runs on port 2000
- AI backend runs on port 5501
- All AI requests are proxied through the frontend server
- WebSocket for hot reloading runs on port 8081
- CSP headers are configured to allow necessary connections