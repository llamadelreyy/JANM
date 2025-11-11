const express = require('express');
const cors = require('cors');
const axios = require('axios');
const multer = require('multer');
const FormData = require('form-data');
require('dotenv').config();

const app = express();
const PORT = process.env.PORT || 3001;
const LLM_URL = process.env.LLM_URL || 'http://60.51.17.97:14501/v1/chat/completions';
const WHISPER_URL = process.env.WHISPER_URL || 'http://localhost:14801/v1/audio/transcriptions';

// Middleware
app.use(cors({
  origin: true, // Allow all origins for development
  credentials: true,
  methods: ['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS'],
  allowedHeaders: ['Content-Type', 'Authorization', 'Accept', 'Cache-Control']
}));
app.use(express.json({ limit: '10mb' }));

// Configure multer for file uploads
const upload = multer({
  storage: multer.memoryStorage(),
  limits: {
    fileSize: 10 * 1024 * 1024, // 10MB limit
  },
});

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

// Whisper Speech-to-Text endpoint
app.post('/api/whisper', upload.single('audio'), async (req, res) => {
  try {
    console.log('=== WHISPER REQUEST RECEIVED ===');
    console.log('Headers:', req.headers);
    console.log('File info:', req.file ? {
      fieldname: req.file.fieldname,
      originalname: req.file.originalname,
      mimetype: req.file.mimetype,
      size: req.file.size
    } : 'No file');

    if (!req.file) {
      console.log('ERROR: No audio file provided');
      return res.status(400).json({ error: 'No audio file provided' });
    }

    // Create FormData to forward to Whisper service
    const formData = new FormData();
    formData.append('file', req.file.buffer, {
      filename: 'audio.wav',
      contentType: req.file.mimetype || 'audio/wav'
    });
    formData.append('model', 'stt_model');
    formData.append('language', 'ms'); // Force Malay language
    formData.append('response_format', 'json');

    console.log('Forwarding to Whisper service:', WHISPER_URL);

    // Forward to Whisper service
    const response = await axios({
      method: 'POST',
      url: WHISPER_URL,
      data: formData,
      headers: {
        ...formData.getHeaders(),
      },
      timeout: 30000, // 30 seconds timeout
    });

    console.log('Whisper response received:', response.data);
    res.json(response.data);

  } catch (error) {
    console.error('Error in Whisper endpoint:', error);
    
    if (error.response) {
      // Forward the error from Whisper service
      res.status(error.response.status).json({
        error: error.response.data?.error || error.message
      });
    } else {
      res.status(500).json({
        error: error.message || 'Whisper transcription failed'
      });
    }
  }
});

// Error handling middleware
app.use((error, req, res, next) => {
  console.error('Unhandled error:', error);
  res.status(500).json({ error: 'Internal server error' });
});

// Start server
app.listen(PORT, '0.0.0.0', () => {
  console.log(`🚀 Chat backend server running on port ${PORT}`);
  console.log(`📡 LLM URL: ${LLM_URL}`);
  console.log(`🎤 Whisper URL: ${WHISPER_URL}`);
  console.log(` Health check: http://0.0.0.0:${PORT}/health`);
  console.log(`💬 Chat stream: http://0.0.0.0:${PORT}/api/chat/stream`);
  console.log(`🎙️ Whisper endpoint: http://0.0.0.0:${PORT}/api/whisper`);
  console.log(`� Server accessible on all network interfaces`);
});