/**
 * WhatsApp AI Chat Backend Server
 * Integrates Twilio WhatsApp with AI services
 */

import express from 'express'
import cors from 'cors'
import bodyParser from 'body-parser'
import dotenv from 'dotenv'
import twilio from 'twilio'
import ollamaService from './services/ollamaService.js'
import ragService from './services/ragService.js'

// Load environment variables
dotenv.config()

const app = express()
const port = process.env.PORT || 8092

// Twilio configuration
const accountSid = process.env.TWILIO_ACCOUNT_SID
const authToken = process.env.TWILIO_AUTH_TOKEN
const whatsappNumber = process.env.TWILIO_WHATSAPP_NUMBER
const mockMode = process.env.MOCK_WHATSAPP_RESPONSES === 'true'

// Only initialize Twilio client if not in mock mode and credentials are provided
let client = null
if (!mockMode) {
  if (!accountSid || !authToken || !whatsappNumber) {
    console.error('Missing required Twilio environment variables')
    console.log('Set MOCK_WHATSAPP_RESPONSES=true in .env to run in mock mode')
    process.exit(1)
  }
  
  try {
    client = twilio(accountSid, authToken)
    console.log('Twilio client initialized successfully')
  } catch (error) {
    console.error('Failed to initialize Twilio client:', error.message)
    console.log('Set MOCK_WHATSAPP_RESPONSES=true in .env to run in mock mode')
    process.exit(1)
  }
} else {
  console.log('Running in MOCK mode - Twilio client not initialized')
}

// Middleware
app.use(cors())
app.use(bodyParser.urlencoded({ extended: false }))
app.use(bodyParser.json())

// Store user sessions (in production, use Redis or database)
const userSessions = new Map()

// Initialize services
async function initializeServices() {
  try {
    console.log('Initializing AI services...')
    
    // Load RAG documents
    await ragService.loadDocuments()
    console.log('RAG service initialized:', ragService.getStatus())
    
    // Check Ollama connection
    const ollamaStatus = await ollamaService.getStatus()
    console.log('Ollama service status:', ollamaStatus)
    
    if (!ollamaStatus.isConnected) {
      console.warn('Warning: Ollama service is not connected')
    }
    
    console.log('Services initialized successfully')
  } catch (error) {
    console.error('Failed to initialize services:', error)
  }
}

// Helper function to get user session
function getUserSession(phoneNumber) {
  if (!userSessions.has(phoneNumber)) {
    userSessions.set(phoneNumber, {
      phoneNumber,
      messages: [],
      lastActivity: new Date(),
      userName: 'Pengguna'
    })
  }
  return userSessions.get(phoneNumber)
}

// Helper function to send WhatsApp message
async function sendWhatsAppMessage(to, message) {
  // If mock responses are enabled, just log the message
  if (mockMode || process.env.MOCK_WHATSAPP_RESPONSES === 'true') {
    console.log('\n🤖 === MOCK WHATSAPP RESPONSE ===')
    console.log(`📱 To: ${to}`)
    console.log(`💬 Message: ${message}`)
    console.log('================================\n')
    return { sid: 'mock-response-sid' }
  }

  // Check if client is available
  if (!client) {
    console.error('Twilio client not initialized')
    throw new Error('Twilio client not available')
  }

  try {
    const response = await client.messages.create({
      body: message,
      from: whatsappNumber,
      to: to
    })
    console.log(`Message sent to ${to}: ${response.sid}`)
    return response
  } catch (error) {
    console.error('Error sending WhatsApp message:', error)
    // Don't throw error in development/testing - just log it
    if (process.env.DEBUG === 'True') {
      console.log('Debug mode: Continuing despite Twilio error')
      console.log('\n🤖 === DEBUG RESPONSE (Twilio Error) ===')
      console.log(`📱 To: ${to}`)
      console.log(`💬 Message: ${message}`)
      console.log('====================================\n')
      return { sid: 'debug-mode-fake-sid' }
    }
    throw error
  }
}

// Helper function to format AI response for WhatsApp
function formatForWhatsApp(text) {
  // WhatsApp has a 1600 character limit per message
  const maxLength = 1500
  
  if (text.length <= maxLength) {
    return [text]
  }
  
  // Split long messages into chunks
  const chunks = []
  let currentChunk = ''
  const lines = text.split('\n')
  
  for (const line of lines) {
    if ((currentChunk + line + '\n').length > maxLength) {
      if (currentChunk) {
        chunks.push(currentChunk.trim())
        currentChunk = line + '\n'
      } else {
        // Line itself is too long, split it
        const words = line.split(' ')
        let currentLine = ''
        for (const word of words) {
          if ((currentLine + word + ' ').length > maxLength) {
            if (currentLine) {
              chunks.push(currentLine.trim())
              currentLine = word + ' '
            } else {
              chunks.push(word)
            }
          } else {
            currentLine += word + ' '
          }
        }
        if (currentLine) {
          currentChunk = currentLine + '\n'
        }
      }
    } else {
      currentChunk += line + '\n'
    }
  }
  
  if (currentChunk) {
    chunks.push(currentChunk.trim())
  }
  
  return chunks
}

// WhatsApp webhook endpoint
app.post('/webhook/whatsapp', async (req, res) => {
  try {
    const { Body, From, To } = req.body
    
    console.log(`Received message from ${From}: ${Body}`)
    
    // Validate that this is a WhatsApp message
    if (!From || !From.startsWith('whatsapp:')) {
      console.log('Not a WhatsApp message, ignoring')
      return res.status(200).send('OK')
    }
    
    const phoneNumber = From
    const userMessage = Body?.trim()
    
    if (!userMessage) {
      console.log('Empty message, ignoring')
      return res.status(200).send('OK')
    }
    
    // Get or create user session
    const session = getUserSession(phoneNumber)
    session.lastActivity = new Date()
    
    // Add user message to session
    session.messages.push({
      type: 'user',
      content: userMessage,
      timestamp: new Date()
    })
    
    // Handle special commands
    if (userMessage.toLowerCase() === '/start' || userMessage.toLowerCase() === 'hi' || userMessage.toLowerCase() === 'hello') {
      const welcomeMessage = `Selamat datang! 👋

Saya adalah AROS Chatbot, pembantu AI yang boleh membantu anda dengan soalan berkaitan dengan Jabatan Perkhidmatan Awam Negeri Sabah!

Hantar soalan anda dan saya akan cuba membantu! 🚗`
      
      try {
        await sendWhatsAppMessage(phoneNumber, welcomeMessage)
      } catch (sendError) {
        console.error('Failed to send welcome message:', sendError)
      }
      return res.status(200).send(welcomeMessage)
    }
    
    if (userMessage.toLowerCase() === '/help') {
      const helpMessage = `🤖 *Panduan Penggunaan*

Saya boleh membantu dengan:
• Soalan tentang peraturan jalan raya
• Maklumat JPAN
• Prosedur trafik PDRM

Contoh soalan:
• "Apa itu lesen memandu?"
• "Bagaimana nak renew roadtax?"
• "Prosedur saman trafik"

Hantar /start untuk mula semula
Hantar /status untuk semak status sistem`
      
      try {
        await sendWhatsAppMessage(phoneNumber, helpMessage)
      } catch (sendError) {
        console.error('Failed to send help message:', sendError)
      }
      return res.status(200).send(helpMessage)
    }
    
    if (userMessage.toLowerCase() === '/status') {
      const ollamaStatus = await ollamaService.getStatus()
      const ragStatus = ragService.getStatus()
      
      const statusMessage = `📊 *Status Sistem*

🤖 AI Service: ${ollamaStatus.isConnected ? '✅ Connected' : '❌ Disconnected'}
📚 Database: ${ragStatus.isLoaded ? '✅ Loaded' : '❌ Not Loaded'}

Model: ${ollamaStatus.model}
Database Files: ${ragStatus.hasJpanFAQ ? 'JPAN FAQ ✅' : 'JPAN FAQ ❌'} ${ragStatus.hasRoadTransportRegulations ? 'Road Transport ✅' : 'Road Transport ❌'}`
      
      try {
        await sendWhatsAppMessage(phoneNumber, statusMessage)
      } catch (sendError) {
        console.error('Failed to send status message:', sendError)
      }
      return res.status(200).send(statusMessage)
    }
    
    // Send typing indicator (optional)
    console.log('Processing AI request...')
    
    try {
      // Get AI response
      const aiResponse = await ollamaService.sendMessage(userMessage)
      
      // Add AI response to session
      session.messages.push({
        type: 'bot',
        content: aiResponse,
        timestamp: new Date()
      })
      
      // Format response for WhatsApp
      const messageChunks = formatForWhatsApp(aiResponse)
      
      // Send response chunks
      for (let i = 0; i < messageChunks.length; i++) {
        const chunk = messageChunks[i]
        const prefix = messageChunks.length > 1 ? `(${i + 1}/${messageChunks.length}) ` : ''
        try {
          await sendWhatsAppMessage(phoneNumber, prefix + chunk)
        } catch (sendError) {
          console.error(`Failed to send chunk ${i + 1}:`, sendError)
          // Continue with next chunk
        }
        
        // Add small delay between chunks to avoid rate limiting
        if (i < messageChunks.length - 1) {
          await new Promise(resolve => setTimeout(resolve, 1000))
        }
      }
      
      // Return the AI response in HTTP response
      return res.status(200).send(aiResponse)
      
    } catch (error) {
      console.error('Error processing AI request:', error)
      
      const errorMessage = `Maaf, terdapat masalah teknikal. Sila cuba lagi sebentar lagi. 🔧

Jika masalah berterusan, sila hubungi pentadbir sistem.`
      
      try {
        await sendWhatsAppMessage(phoneNumber, errorMessage)
      } catch (sendError) {
        console.error('Failed to send error message:', sendError)
        // Continue without throwing - this is just for user notification
      }
      
      return res.status(200).send(errorMessage)
    }
    
  } catch (error) {
    console.error('Error in WhatsApp webhook:', error)
    res.status(500).send('Internal Server Error')
  }
})

// Health check endpoint
app.get('/health', async (req, res) => {
  try {
    const ollamaStatus = await ollamaService.getStatus()
    const ragStatus = ragService.getStatus()
    
    res.json({
      status: 'healthy',
      timestamp: new Date().toISOString(),
      services: {
        ollama: ollamaStatus,
        rag: ragStatus
      },
      sessions: {
        active: userSessions.size,
        users: Array.from(userSessions.keys())
      }
    })
  } catch (error) {
    res.status(500).json({
      status: 'unhealthy',
      error: error.message,
      timestamp: new Date().toISOString()
    })
  }
})

// Status endpoint
app.get('/status', async (req, res) => {
  try {
    const ollamaStatus = await ollamaService.getStatus()
    const ragStatus = ragService.getStatus()
    
    res.json({
      ollama: ollamaStatus,
      rag: ragStatus,
      twilio: {
        accountSid: accountSid ? 'configured' : 'missing',
        whatsappNumber: whatsappNumber || 'not configured'
      },
      sessions: userSessions.size
    })
  } catch (error) {
    res.status(500).json({ error: error.message })
  }
})

// Send test message endpoint (for testing)
app.post('/test/send', async (req, res) => {
  try {
    const { to, message } = req.body
    
    if (!to || !message) {
      return res.status(400).json({ error: 'Missing to or message parameter' })
    }
    
    const response = await sendWhatsAppMessage(to, message)
    res.json({ success: true, messageId: response.sid })
  } catch (error) {
    res.status(500).json({ error: error.message })
  }
})

// Clean up old sessions (run every hour)
setInterval(() => {
  const now = new Date()
  const oneHourAgo = new Date(now.getTime() - 60 * 60 * 1000)
  
  for (const [phoneNumber, session] of userSessions.entries()) {
    if (session.lastActivity < oneHourAgo) {
      userSessions.delete(phoneNumber)
      console.log(`Cleaned up session for ${phoneNumber}`)
    }
  }
}, 60 * 60 * 1000) // Run every hour

// Start server
app.listen(port, async () => {
  console.log(`WhatsApp AI Chat Backend running on port ${port}`)
  console.log(`Webhook URL: http://localhost:${port}/webhook/whatsapp`)
  console.log(`Health check: http://localhost:${port}/health`)
  console.log(`Status: http://localhost:${port}/status`)
  
  // Initialize services
  await initializeServices()
  
  console.log('Server ready to receive WhatsApp messages! 🚀')
})