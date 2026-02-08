# User Acceptance Testing (UAT) - JPAN WhatsApp AI Chatbot

## Project Overview
This document outlines the User Acceptance Testing procedures for the JPAN WhatsApp AI Chatbot system, which provides intelligent responses using JPAN FAQ database with RAG (Retrieval-Augmented Generation) capabilities.

## Test Environment Setup
- **Application URL**: http://localhost:8092
- **WhatsApp Integration**: Twilio WhatsApp API
- **AI Service**: Ollama/OpenAI-compatible API
- **Database**: JPAN FAQ documents with RAG integration

## Test Cases

| ID | Description | Pass / Fail | Remarks |
|----|-------------|-------------|---------|
| **TC001** | **Navigate to registration page** | | |
| | **Pre-conditions**: User has WhatsApp installed and access to the configured WhatsApp number | | |
| | **Steps**: | | |
| | 1. Open WhatsApp application | | |
| | 2. Send a message to the configured JPAN chatbot WhatsApp number | | |
| | 3. Send "/start" or "hi" command | | |
| | **Expected Result**: User receives welcome message with registration/introduction | | |
| | **Actual Result**: | | |
| **TC002** | **Enter valid email address in registration form** | | |
| | **Pre-conditions**: User has initiated conversation with the chatbot | | |
| | **Steps**: | | |
| | 1. User sends a message containing a valid email format (e.g., "user@example.com") | | |
| | 2. System processes the message through AI service | | |
| | 3. System validates email format and responds appropriately | | |
| | **Expected Result**: System acknowledges valid email and provides appropriate response | | |
| | **Actual Result**: | | |

## Additional Test Cases

| ID | Description | Pass / Fail | Remarks |
|----|-------------|-------------|---------|
| **TC003** | **System Health Check** | | |
| | **Steps**: | | |
| | 1. Navigate to http://localhost:8092/health | | |
| | 2. Verify system status response | | |
| | **Expected Result**: JSON response showing healthy status for all services | | |
| | **Actual Result**: | | |
| **TC004** | **WhatsApp Welcome Message** | | |
| | **Steps**: | | |
| | 1. Send "hi", "hello", or "/start" to the WhatsApp number | | |
| | **Expected Result**: Receive welcome message in Malay with system introduction | | |
| | **Actual Result**: | | |
| **TC005** | **Help Command Functionality** | | |
| | **Steps**: | | |
| | 1. Send "/help" command via WhatsApp | | |
| | **Expected Result**: Receive help message with usage instructions | | |
| | **Actual Result**: | | |
| **TC006** | **Status Command Functionality** | | |
| | **Steps**: | | |
| | 1. Send "/status" command via WhatsApp | | |
| | **Expected Result**: Receive system status including AI service and database status | | |
| | **Actual Result**: | | |
| **TC007** | **AI Query Processing** | | |
| | **Steps**: | | |
| | 1. Send a question about traffic regulations (e.g., "Apa itu lesen memandu?") | | |
| | 2. Wait for AI response | | |
| | **Expected Result**: Receive relevant answer from JPAN FAQ database | | |
| | **Actual Result**: | | |
| **TC008** | **Long Message Handling** | | |
| | **Steps**: | | |
| | 1. Send a complex query that would generate a long response | | |
| | **Expected Result**: Response is split into multiple messages if over 4000 characters | | |
| | **Actual Result**: | | |
| **TC009** | **Session Management** | | |
| | **Steps**: | | |
| | 1. Send multiple messages in sequence | | |
| | 2. Verify conversation context is maintained | | |
| | **Expected Result**: System maintains user session and conversation history | | |
| | **Actual Result**: | | |
| **TC010** | **Error Handling** | | |
| | **Steps**: | | |
| | 1. Simulate AI service unavailability | | |
| | 2. Send a message to the chatbot | | |
| | **Expected Result**: Receive error message in Malay explaining technical issue | | |
| | **Actual Result**: | | |

## API Endpoint Testing

| ID | Description | Pass / Fail | Remarks |
|----|-------------|-------------|---------|
| **TC011** | **Health Endpoint** | | |
| | **Steps**: GET /health | | |
| | **Expected Result**: 200 status with service health information | | |
| | **Actual Result**: | | |
| **TC012** | **Status Endpoint** | | |
| | **Steps**: GET /status | | |
| | **Expected Result**: 200 status with detailed system status | | |
| | **Actual Result**: | | |
| **TC013** | **WhatsApp Webhook** | | |
| | **Steps**: POST /webhook/whatsapp with valid Twilio payload | | |
| | **Expected Result**: 200 status with appropriate response | | |
| | **Actual Result**: | | |
| **TC014** | **Test Send Endpoint** | | |
| | **Steps**: POST /test/send with valid message payload | | |
| | **Expected Result**: 200 status with message ID | | |
| | **Actual Result**: | | |

## Configuration Testing

| ID | Description | Pass / Fail | Remarks |
|----|-------------|-------------|---------|
| **TC015** | **Environment Variables** | | |
| | **Steps**: Verify all required environment variables are set | | |
| | **Expected Result**: TWILIO_ACCOUNT_SID, TWILIO_AUTH_TOKEN, TWILIO_WHATSAPP_NUMBER configured | | |
| | **Actual Result**: | | |
| **TC016** | **RAG Document Loading** | | |
| | **Steps**: Verify JPAN FAQ.txt is loaded successfully | | |
| | **Expected Result**: RAG service shows documents loaded in status | | |
| | **Actual Result**: | | |
| **TC017** | **AI Service Connection** | | |
| | **Steps**: Verify connection to Ollama/OpenAI service | | |
| | **Expected Result**: AI service shows connected status | | |
| | **Actual Result**: | | |

## Performance Testing

| ID | Description | Pass / Fail | Remarks |
|----|-------------|-------------|---------|
| **TC018** | **Response Time** | | |
| | **Steps**: Send message and measure response time | | |
| | **Expected Result**: Response received within 30 seconds | | |
| | **Actual Result**: | | |
| **TC019** | **Concurrent Users** | | |
| | **Steps**: Simulate multiple users sending messages simultaneously | | |
| | **Expected Result**: All users receive appropriate responses | | |
| | **Actual Result**: | | |
| **TC020** | **Session Cleanup** | | |
| | **Steps**: Wait for session cleanup interval (1 hour) | | |
| | **Expected Result**: Inactive sessions are removed from memory | | |
| | **Actual Result**: | | |

## Security Testing

| ID | Description | Pass / Fail | Remarks |
|----|-------------|-------------|---------|
| **TC021** | **WhatsApp Validation** | | |
| | **Steps**: Send non-WhatsApp message to webhook | | |
| | **Expected Result**: Message is ignored, no processing occurs | | |
| | **Actual Result**: | | |
| **TC022** | **Input Sanitization** | | |
| | **Steps**: Send message with special characters/potential injection | | |
| | **Expected Result**: Input is safely processed without errors | | |
| | **Actual Result**: | | |

## Test Execution Guidelines

### Pre-Test Setup
1. Ensure all environment variables are configured in `.env` file
2. Start the application: `npm start`
3. Verify services are running: `curl http://localhost:8092/health`
4. Configure Twilio WhatsApp webhook URL
5. Ensure JPAN FAQ.txt contains test data

### Test Data Requirements
- Valid WhatsApp number for testing
- Sample JPAN FAQ content
- Test email addresses (valid and invalid formats)
- Various query types (simple, complex, multilingual)

### Test Execution Notes
- Record actual results for each test case
- Note any deviations from expected behavior
- Capture screenshots for UI-related tests
- Document response times for performance tests
- Log any error messages or unexpected behavior

### Pass/Fail Criteria
- **Pass**: Test case meets all expected results
- **Fail**: Test case does not meet expected results or produces errors
- **Partial**: Test case partially meets expected results with minor issues

### Post-Test Activities
1. Document all failed test cases with detailed error descriptions
2. Prioritize issues based on severity and impact
3. Create bug reports for development team
4. Schedule retesting after fixes are implemented
5. Update test cases based on any requirement changes

## Test Sign-off

| Role | Name | Signature | Date |
|------|------|-----------|------|
| Test Lead | | | |
| Business Analyst | | | |
| Product Owner | | | |
| Development Lead | | | |

## Appendix

### Environment Configuration
```env
TWILIO_ACCOUNT_SID=your_twilio_account_sid_here
TWILIO_AUTH_TOKEN=your_twilio_auth_token_here
TWILIO_WHATSAPP_NUMBER=whatsapp:+your_whatsapp_number_here
PORT=8092
DEBUG=True
MOCK_WHATSAPP_RESPONSES=false
OPENAI_API_BASE_URL=http://60.51.17.97:11122/v1
OPENAI_MODEL=llm_model
OPENAI_API_KEY=dummy-key
```

### Sample Test Messages
- Welcome: "hi", "hello", "/start"
- Help: "/help"
- Status: "/status"
- FAQ Query: "Apa itu lesen memandu?"
- Email Test: "My email is test@example.com"
- Complex Query: "Bagaimana prosedur untuk renew roadtax dan apa dokumen yang diperlukan?"