# Network Setup for External Device Access

## Problem
When you port forward the React app and access it from another device, the AI chat shows "Ollama tak tersambung" (Ollama not connected).

## Root Cause
Ollama by default only listens on localhost (127.0.0.1), which means other devices can't connect to it.

## Solution

### Step 1: Start Ollama with Network Access
Instead of running `ollama serve`, run:

```bash
OLLAMA_HOST=0.0.0.0:11434 OLLAMA_KEEP_ALIVE=-1 ollama serve
```

This makes Ollama:
- Listen on all network interfaces (0.0.0.0), allowing external devices to connect
- Keep models loaded in memory (-1) for faster responses

### Step 2: Verify the Setup
1. Start your React app: `npm run dev`
2. Start Ollama with network access (command above)
3. Port forward your React app to port 2000
4. Access from another device - AI chat should now work automatically

## How It Works
The app automatically detects the hostname you're accessing it from and uses the same hostname for Ollama connections. So if you access the app via `192.168.1.100:2000`, it will try to connect to Ollama at `192.168.1.100:11434`.

## Quick Start Script
You can use the provided script:
```bash
bash start-ollama-network.sh
```

## Troubleshooting
- Make sure port 11434 is not blocked by firewall
- Verify Ollama is running with `OLLAMA_HOST=0.0.0.0:11434`
- Check the AI chat page shows the correct Ollama URL at the bottom