const express = require('express');
const cors = require('cors');
const axios = require('axios');
require('dotenv').config();

const app = express();
const PORT = process.env.PORT || 3001;
const LLM_URL = process.env.LLM_URL || 'http://60.51.17.97:9501/v1/chat/completions';

// Middleware
app.use(cors());
app.use(express.json({ limit: '10mb' }));

// Health check endpoint
app.get('/health', (req, res) => {
  res.json({ 
    status: 'OK', 
    timestamp: new Date().toISOString(),
    llm_url: LLM_URL 
  });
});

// SSE streaming endpoint for chat
app.post('/api/chat/stream', async (req, res) => {
  try {
    console.log('Received chat stream request:', req.body);

    // Set SSE headers
    res.writeHead(200, {
      'Content-Type': 'text/event-stream',
      'Cache-Control': 'no-cache',
      'Connection': 'keep-alive',
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Headers': 'Cache-Control'
    });

    // Ensure the payload requests streaming
    const payload = {
      ...req.body,
      stream: true
    };

    console.log('Proxying request to LLM:', LLM_URL);

    // Create axios request with streaming
    const response = await axios({
      method: 'POST',
      url: LLM_URL,
      data: payload,
      responseType: 'stream',
      timeout: 300000, // 5 minutes timeout
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'text/event-stream'
      }
    });

    let buffer = '';

    // Handle the streaming response
    response.data.on('data', (chunk) => {
      try {
        buffer += chunk.toString();
        const lines = buffer.split('\n');
        
        // Keep the last potentially incomplete line in the buffer
        buffer = lines.pop() || '';

        for (const line of lines) {
          if (!line.trim()) continue;

          // Normalize to SSE format
          let sseData;
          if (line.startsWith('data: ')) {
            sseData = line;
          } else {
            sseData = `data: ${line}`;
          }

          // Write the SSE data
          res.write(`${sseData}\n\n`);
        }
      } catch (error) {
        console.error('Error processing chunk:', error);
      }
    });

    response.data.on('end', () => {
      try {
        // Process any remaining data in buffer
        if (buffer.trim()) {
          const sseData = buffer.startsWith('data: ') ? buffer : `data: ${buffer}`;
          res.write(`${sseData}\n\n`);
        }
        
        // Send explicit done marker
        res.write('data: [DONE]\n\n');
        res.end();
        console.log('Stream completed successfully');
      } catch (error) {
        console.error('Error ending stream:', error);
        res.end();
      }
    });

    response.data.on('error', (error) => {
      console.error('Stream error:', error);
      res.write(`data: {"error": "Stream error: ${error.message}"}\n\n`);
      res.end();
    });

    // Handle client disconnect
    req.on('close', () => {
      console.log('Client disconnected');
      response.data.destroy();
    });

  } catch (error) {
    console.error('Error in chat stream endpoint:', error);
    
    // Send error as SSE
    const errorData = {
      error: error.message || 'Internal server error',
      status: error.response?.status || 500
    };
    
    res.write(`data: ${JSON.stringify(errorData)}\n\n`);
    res.write('data: [DONE]\n\n');
    res.end();
  }
});

// Handle non-streaming chat (fallback)
app.post('/api/chat', async (req, res) => {
  try {
    console.log('Received non-streaming chat request');

    const payload = {
      ...req.body,
      stream: false
    };

    const response = await axios({
      method: 'POST',
      url: LLM_URL,
      data: payload,
      timeout: 300000,
      headers: {
        'Content-Type': 'application/json'
      }
    });

    res.json(response.data);
  } catch (error) {
    console.error('Error in chat endpoint:', error);
    res.status(error.response?.status || 500).json({
      error: error.message || 'Internal server error'
    });
  }
});

// Error handling middleware
app.use((error, req, res, next) => {
  console.error('Unhandled error:', error);
  res.status(500).json({ error: 'Internal server error' });
});

// Start server
app.listen(PORT, () => {
  console.log(`🚀 Chat backend server running on port ${PORT}`);
  console.log(`📡 LLM URL: ${LLM_URL}`);
  console.log(`🏥 Health check: http://localhost:${PORT}/health`);
  console.log(`💬 Chat stream: http://localhost:${PORT}/api/chat/stream`);
});