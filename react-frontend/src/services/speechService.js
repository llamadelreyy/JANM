class SpeechService {
  constructor() {
    this.isRecording = false
    this.mediaRecorder = null
    this.audioChunks = []
  }

  // Check if browser supports audio recording
  isSupported() {
    return navigator.mediaDevices &&
           navigator.mediaDevices.getUserMedia &&
           window.MediaRecorder
  }

  // Get detailed support information
  getSupportInfo() {
    const hasMediaDevices = !!navigator.mediaDevices
    const hasGetUserMedia = !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia)
    const hasMediaRecorder = !!window.MediaRecorder
    const isLocalhost = window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1'
    const isSecureContext = window.isSecureContext || window.location.protocol === 'https:'
    const isNgrok = window.location.hostname.includes('ngrok')
    const isPortForwarded = window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1' && window.location.protocol === 'http:' && !isNgrok
    
    // Check if all APIs are available
    const hasAllAPIs = hasMediaDevices && hasGetUserMedia && hasMediaRecorder
    
    // Determine if microphone should work
    // ngrok provides HTTPS, so it should work fine
    const shouldWork = hasAllAPIs && (isSecureContext || isLocalhost || isNgrok)
    
    return {
      hasMediaDevices,
      hasGetUserMedia,
      hasMediaRecorder,
      isSecureContext,
      isLocalhost,
      isNgrok,
      isPortForwarded,
      isSupported: shouldWork,
      message: this.getSupportMessage(hasMediaDevices, hasGetUserMedia, hasMediaRecorder, isSecureContext, isLocalhost, isPortForwarded, isNgrok)
    }
  }

  getSupportMessage(hasMediaDevices, hasGetUserMedia, hasMediaRecorder, isSecureContext, isLocalhost, isPortForwarded, isNgrok) {
    if (!hasMediaDevices) return 'Browser tidak menyokong MediaDevices API'
    if (!hasGetUserMedia) return 'Browser tidak menyokong getUserMedia API'
    if (!hasMediaRecorder) return 'Browser tidak menyokong MediaRecorder API'
    
    if (isPortForwarded) {
      return 'Mikrofon diblokir - gunakan localhost, ngrok, atau HTTPS untuk akses mikrofon'
    }
    
    if (!isSecureContext && !isLocalhost && !isNgrok) {
      return 'Mikrofon memerlukan HTTPS, localhost, atau ngrok untuk keselamatan'
    }
    
    if (isNgrok) {
      return 'Mikrofon disokong melalui ngrok HTTPS - klik untuk mula merakam'
    }
    
    return 'Mikrofon disokong - klik untuk mula merakam'
  }

  // Start recording audio
  async startRecording() {
    if (!this.isSupported()) {
      throw new Error('Audio recording is not supported in this browser')
    }

    try {
      // Try to get user media with fallback for different environments
      let stream
      try {
        stream = await navigator.mediaDevices.getUserMedia({
          audio: {
            echoCancellation: true,
            noiseSuppression: true,
            autoGainControl: true
          }
        })
      } catch (error) {
        // Fallback with basic audio constraints
        console.warn('Advanced audio constraints failed, trying basic:', error)
        stream = await navigator.mediaDevices.getUserMedia({ audio: true })
      }

      // Use the most compatible format for Whisper
      let options = {}
      if (MediaRecorder.isTypeSupported('audio/webm;codecs=opus')) {
        options = { mimeType: 'audio/webm;codecs=opus' }
      } else if (MediaRecorder.isTypeSupported('audio/webm')) {
        options = { mimeType: 'audio/webm' }
      } else if (MediaRecorder.isTypeSupported('audio/mp4')) {
        options = { mimeType: 'audio/mp4' }
      }
      
      this.mediaRecorder = new MediaRecorder(stream, options)

      this.audioChunks = []

      this.mediaRecorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
          this.audioChunks.push(event.data)
        }
      }

      this.mediaRecorder.start()
      this.isRecording = true

      return new Promise((resolve, reject) => {
        this.mediaRecorder.onstop = async () => {
          try {
            // Use the same mime type as the recorder
            const mimeType = this.mediaRecorder.mimeType || 'audio/wav'
            const audioBlob = new Blob(this.audioChunks, { type: mimeType })
            
            // Stop all tracks to release microphone
            stream.getTracks().forEach(track => track.stop())
            
            resolve(audioBlob)
          } catch (error) {
            reject(error)
          }
        }

        this.mediaRecorder.onerror = (error) => {
          reject(error)
        }
      })
    } catch (error) {
      throw new Error(`Failed to start recording: ${error.message}`)
    }
  }

  // Stop recording audio
  stopRecording() {
    if (this.mediaRecorder && this.isRecording) {
      this.mediaRecorder.stop()
      this.isRecording = false
    }
  }

  // Convert audio blob to text using your optimized Whisper API
  async transcribeAudio(audioBlob) {
    try {
      // Convert audio to WAV format for your Whisper endpoint
      const wavBlob = await this.convertToWav(audioBlob)
      
      // Your API expects the audio file as 'audio' parameter
      const audioFile = new File([wavBlob], 'audio.wav', { type: 'audio/wav' })
      
      const formData = new FormData()
      formData.append('audio', audioFile)  // Changed from 'file' to 'audio'

      const response = await fetch('/api/whisper', {
        method: 'POST',
        body: formData
      })

      if (!response.ok) {
        const errorText = await response.text()
        throw new Error(`Whisper API error: ${response.status} - ${errorText}`)
      }

      const result = await response.json()
      
      // Your API returns { text, duration, timestamp }
      if (result.text) {
        return result.text.trim()
      } else {
        throw new Error('No transcription text received from Whisper')
      }
    } catch (error) {
      throw new Error(`Transcription failed: ${error.message}`)
    }
  }

  // Convert audio blob to WAV format
  async convertToWav(audioBlob) {
    return new Promise((resolve, reject) => {
      try {
        const audioContext = new (window.AudioContext || window.webkitAudioContext)()
        const fileReader = new FileReader()
        
        fileReader.onload = async (e) => {
          try {
            const arrayBuffer = e.target.result
            const audioBuffer = await audioContext.decodeAudioData(arrayBuffer)
            
            // Convert to WAV
            const wavBuffer = this.audioBufferToWav(audioBuffer)
            const wavBlob = new Blob([wavBuffer], { type: 'audio/wav' })
            
            resolve(wavBlob)
          } catch (error) {
            // If conversion fails, return original blob
            console.warn('Audio conversion failed, using original:', error)
            resolve(audioBlob)
          }
        }
        
        fileReader.onerror = () => {
          // If file reading fails, return original blob
          console.warn('File reading failed, using original')
          resolve(audioBlob)
        }
        
        fileReader.readAsArrayBuffer(audioBlob)
      } catch (error) {
        // If any error occurs, return original blob
        console.warn('Audio processing failed, using original:', error)
        resolve(audioBlob)
      }
    })
  }

  // Convert AudioBuffer to WAV format
  audioBufferToWav(buffer) {
    const length = buffer.length
    const numberOfChannels = buffer.numberOfChannels
    const sampleRate = buffer.sampleRate
    const arrayBuffer = new ArrayBuffer(44 + length * numberOfChannels * 2)
    const view = new DataView(arrayBuffer)
    
    // WAV header
    const writeString = (offset, string) => {
      for (let i = 0; i < string.length; i++) {
        view.setUint8(offset + i, string.charCodeAt(i))
      }
    }
    
    writeString(0, 'RIFF')
    view.setUint32(4, 36 + length * numberOfChannels * 2, true)
    writeString(8, 'WAVE')
    writeString(12, 'fmt ')
    view.setUint32(16, 16, true)
    view.setUint16(20, 1, true)
    view.setUint16(22, numberOfChannels, true)
    view.setUint32(24, sampleRate, true)
    view.setUint32(28, sampleRate * numberOfChannels * 2, true)
    view.setUint16(32, numberOfChannels * 2, true)
    view.setUint16(34, 16, true)
    writeString(36, 'data')
    view.setUint32(40, length * numberOfChannels * 2, true)
    
    // Convert audio data
    let offset = 44
    for (let i = 0; i < length; i++) {
      for (let channel = 0; channel < numberOfChannels; channel++) {
        const sample = Math.max(-1, Math.min(1, buffer.getChannelData(channel)[i]))
        view.setInt16(offset, sample < 0 ? sample * 0x8000 : sample * 0x7FFF, true)
        offset += 2
      }
    }
    
    return arrayBuffer
  }

  // Complete speech-to-text workflow
  async recordAndTranscribe() {
    try {
      const audioBlob = await this.startRecording()
      const transcription = await this.transcribeAudio(audioBlob)
      return transcription
    } catch (error) {
      throw error
    }
  }

  // Get recording status
  getRecordingStatus() {
    return {
      isRecording: this.isRecording,
      isSupported: this.isSupported()
    }
  }
}

// Create and export a singleton instance
const speechService = new SpeechService()
export default speechService