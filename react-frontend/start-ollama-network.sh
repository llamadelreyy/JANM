#!/bin/bash

# Script to start Ollama with network access
# This allows other devices to connect to Ollama

echo "Starting Ollama with network access..."
echo "This will allow other devices to connect to Ollama on port 11434"

# Set Ollama to listen on all network interfaces and keep models loaded
export OLLAMA_HOST=0.0.0.0:11434
export OLLAMA_KEEP_ALIVE=-1

# Start Ollama
ollama serve

echo "Ollama is now accessible from other devices on your network"
echo "Other devices can connect to: http://[YOUR_SERVER_IP]:11434"