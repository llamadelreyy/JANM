# JPAN WhatsApp AI Chatbot

A WhatsApp AI chatbot that provides intelligent responses using JPAN FAQ database with RAG (Retrieval-Augmented Generation) capabilities.

## Features

- 🤖 AI-powered responses using Ollama/OpenAI-compatible API
- 📚 RAG integration with JPAN FAQ documents
- 💬 WhatsApp integration via Twilio
- 🔄 Session management for user conversations
- 📊 Health monitoring and status endpoints
- 🛡️ Error handling and message chunking for WhatsApp limits

## Quick Start

1. **Install dependencies:**
```bash
npm install
```

2. **Configure environment variables:**
```bash
cp .env.example .env
# Edit .env with your Twilio credentials
```

3. **Add your JPAN FAQ data:**
```bash
# Replace the placeholder content in "JPAN FAQ.txt" with your actual FAQ data
```

4. **Start the server:**
```bash
npm start
```

## Configuration

### Environment Variables (.env)

```env
# Twilio WhatsApp Configuration
TWILIO_ACCOUNT_SID=your_twilio_account_sid_here
TWILIO_AUTH_TOKEN=your_twilio_auth_token_here
TWILIO_WHATSAPP_NUMBER=whatsapp:+your_whatsapp_number_here

# Application Configuration
PORT=8092
DEBUG=True
MOCK_WHATSAPP_RESPONSES=false

# AI Service Configuration
OPENAI_API_BASE_URL=http://60.51.17.97:11122/v1
OPENAI_MODEL=llm_model
OPENAI_API_KEY=dummy-key
```

### Twilio WhatsApp Setup

1. Create a Twilio account at [console.twilio.com](https://console.twilio.com/)
2. Navigate to **Messaging** → **Try it out** → **Send a WhatsApp message**
3. Set webhook URL to: `http://your-server:8092/webhook/whatsapp`
4. Configure your WhatsApp sandbox

## API Endpoints

- **POST** `/webhook/whatsapp` - WhatsApp webhook (for Twilio)
- **GET** `/health` - Health check with service status
- **GET** `/status` - Detailed system status

## WhatsApp Commands

- `/start` or `hi` - Welcome message
- `/help` - Show available commands
- `/status` - Check system status
- Any question - Get AI-powered response with RAG

## File Structure

```
├── server.js              # Main Express server
├── services/
│   ├── ollamaService.js   # AI service integration
│   └── ragService.js      # Document search & retrieval
├── package.json           # Dependencies and scripts
├── .env.example          # Environment template
├── JPAN FAQ.txt          # FAQ database file
└── README.md             # This file
```

## Development

### Local Testing
```bash
npm run dev
```

### Testing with ngrok
```bash
# Install ngrok
npm install -g ngrok

# Start server
npm start

# In another terminal
ngrok http 8092

# Use the ngrok URL in Twilio webhook configuration
```

## Production Deployment

1. Deploy to your server
2. Set up reverse proxy (nginx) for HTTPS
3. Configure proper environment variables
4. Set up monitoring and logging

## Troubleshooting

### Common Issues

1. **Twilio webhook not receiving messages**
   - Check webhook URL configuration
   - Verify server is accessible from internet
   - Check Twilio console for error logs

2. **AI service connection failed**
   - Verify `OPENAI_API_BASE_URL` is correct
   - Check if AI service is running
   - Test connection using `/status` endpoint

3. **RAG documents not loading**
   - Ensure `JPAN FAQ.txt` exists and contains data
   - Check file permissions
   - Verify file encoding (UTF-8)

## License

MIT License