# WhatsApp AI Chat Backend

This backend service integrates Twilio WhatsApp with AI services (Ollama/OpenAI-compatible API) and RAG (Retrieval-Augmented Generation) for intelligent responses based on organizational documents.

## Features

- 🤖 AI-powered responses using Ollama/OpenAI-compatible API
- 📚 RAG integration with organizational documents (JPAN FAQ, Road Transport Regulations)
- 💬 WhatsApp integration via Twilio
- 🔄 Session management for user conversations
- 📊 Health monitoring and status endpoints
- 🛡️ Error handling and message chunking for WhatsApp limits

## Prerequisites

- Node.js 18+ 
- Twilio account with WhatsApp Business API access
- Access to Ollama or OpenAI-compatible API endpoint

## Installation

1. Install dependencies:
```bash
npm install
```

2. Configure environment variables in `.env`:
```env
TWILIO_ACCOUNT_SID=your_twilio_account_sid
TWILIO_AUTH_TOKEN=your_twilio_auth_token
TWILIO_WHATSAPP_NUMBER=+1234567890
PORT=8092
DEBUG=True
OPENAI_API_BASE_URL=http://60.51.17.97:11122/v1
OPENAI_MODEL=llm_model
OPENAI_API_KEY=dummy-key
```

3. Ensure document files are available:
   - `../react-frontend/public/JPAN FAQ.txt`
   - `../react-frontend/public/KAEDAH-KAEDAH PENGANGKUTAN JALAN.txt`

## Running the Server

### Development
```bash
npm run dev
```

### Production
```bash
npm start
```

The server will start on the configured port (default: 8092).

## Twilio WhatsApp Setup

1. **Create Twilio Account**: Sign up at [Twilio Console](https://console.twilio.com/)

2. **Enable WhatsApp**: 
   - Go to Messaging > Try it out > Send a WhatsApp message
   - Follow the sandbox setup instructions

3. **Configure Webhook**:
   - In Twilio Console, go to Messaging > Settings > WhatsApp sandbox settings
   - Set webhook URL to: `https://your-domain.com/webhook/whatsapp`
   - For local development, use ngrok: `https://your-ngrok-url.ngrok.io/webhook/whatsapp`

4. **Test Connection**:
   - Send "join [sandbox-keyword]" to your Twilio WhatsApp number
   - Send a test message to verify the integration

## API Endpoints

### WhatsApp Webhook
- **POST** `/webhook/whatsapp` - Receives WhatsApp messages from Twilio

### Health & Status
- **GET** `/health` - Health check with service status
- **GET** `/status` - Detailed system status

### Testing
- **POST** `/test/send` - Send test WhatsApp message
  ```json
  {
    "to": "whatsapp:+1234567890",
    "message": "Test message"
  }
  ```

## WhatsApp Commands

Users can interact with the bot using these commands:

- `/start` or `hi` - Welcome message and introduction
- `/help` - Show available commands and usage examples
- `/status` - Check system status
- Any question - Get AI-powered response with RAG

## Architecture

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   WhatsApp      │    │   Twilio API     │    │   Backend       │
│   User          │◄──►│   Webhook        │◄──►│   Server        │
└─────────────────┘    └──────────────────┘    └─────────────────┘
                                                        │
                                                        ▼
                                               ┌─────────────────┐
                                               │   AI Services   │
                                               │                 │
                                               │ • Ollama API    │
                                               │ • RAG Service   │
                                               │ • Document DB   │
                                               └─────────────────┘
```

## Services

### OllamaService
- Handles communication with Ollama/OpenAI-compatible API
- Manages AI model interactions
- Formats responses for WhatsApp

### RAGService  
- Loads and indexes organizational documents
- Performs semantic search on user queries
- Provides relevant context to AI model

## Message Flow

1. User sends WhatsApp message
2. Twilio forwards message to webhook
3. Backend processes message and extracts user intent
4. RAG service searches relevant documents
5. AI service generates response with context
6. Response is formatted and sent back via Twilio
7. User receives AI-powered answer

## Error Handling

- Connection failures to AI service
- Twilio API errors
- Message length limits (4096 chars for WhatsApp)
- Rate limiting protection
- Session cleanup for inactive users

## Development

### Local Testing with ngrok

1. Install ngrok: `npm install -g ngrok`
2. Start the server: `npm run dev`
3. In another terminal: `ngrok http 8092`
4. Use the ngrok URL in Twilio webhook configuration

### Debugging

Set `DEBUG=True` in `.env` to enable detailed logging.

## Production Deployment

1. Use a process manager like PM2:
```bash
npm install -g pm2
pm2 start server.js --name whatsapp-ai-chat
```

2. Set up reverse proxy (nginx) for HTTPS
3. Configure proper environment variables
4. Set up monitoring and logging

## Security Considerations

- Validate Twilio webhook signatures in production
- Use HTTPS for webhook endpoints
- Implement rate limiting
- Sanitize user inputs
- Secure environment variables

## Troubleshooting

### Common Issues

1. **Twilio webhook not receiving messages**
   - Check webhook URL configuration
   - Verify ngrok tunnel is active (for local dev)
   - Check Twilio console for error logs

2. **AI service connection failed**
   - Verify `OPENAI_API_BASE_URL` is correct
   - Check if AI service is running
   - Test connection using `/status` endpoint

3. **RAG documents not loading**
   - Ensure document files exist in specified paths
   - Check file permissions
   - Verify file encoding (UTF-8)

### Logs

Check server logs for detailed error information:
```bash
# Development
npm run dev

# Production with PM2
pm2 logs whatsapp-ai-chat
```

## License

MIT License