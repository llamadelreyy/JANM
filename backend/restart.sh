#!/bin/bash
echo "Stopping old backend..."
pkill -f "node server.js"
sleep 2
echo "Starting backend on port 3002..."
cd "$(dirname "$0")"
npm start