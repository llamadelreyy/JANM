#!/bin/bash
echo "=== Stopping all services ==="
pkill -f "node server.js"
pkill -f "vite"
sleep 2

echo "=== Starting backend on port 3002 ==="
cd backend
npm start &
BACKEND_PID=$!
sleep 3

echo "=== Starting frontend on port 2002 ==="
cd ../react-frontend  
npm run dev &
FRONTEND_PID=$!

echo ""
echo "✅ Services started:"
echo "   Backend PID: $BACKEND_PID"
echo "   Frontend PID: $FRONTEND_PID"
echo ""
echo "Access at:"
echo "   Local: http://localhost:2002"
echo "   Ngrok: https://sarah-noninclinational-ingrately.ngrok-free.dev"
echo ""
echo "Press Ctrl+C to stop all services"
wait