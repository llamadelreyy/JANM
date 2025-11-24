import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { resolve } from 'path'

// Replace with your ngrok host
const NGROK_HOST = 'sarah-noninclinational-ingrately.ngrok-free.dev'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": resolve(process.cwd(), "./src"),
    },
  },
  server: {
    port: 2002,
    host: true, // Allows external devices to connect
    hmr: {
      protocol: 'wss', // Use secure WebSocket for ngrok
      host: NGROK_HOST,
      port: 443,       // Standard HTTPS port, required by ngrok
    },
    allowedHosts: [
      /\.ngrok-free\.(app|dev)$/, // Allow all ngrok hosts
    ],
    proxy: {
      '/ollama': {
        target: 'http://localhost:11434',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/ollama/, '/v1')
      },
      '/remote': {
        target: 'http://60.51.17.97:9501',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/remote/, '/v1')
      }
    },
  },
  headers: {
    '/*': {
      'Content-Security-Policy': [
        // Allow scripts, inline code, evals, and your ngrok domain
        "default-src 'self' http://localhost:11434 http://60.51.17.97:9501 https://*.ngrok-free.dev 'unsafe-eval' 'unsafe-inline'",

        // Fonts from Google Fonts and ngrok domain
        "font-src 'self' https://fonts.googleapis.com https://fonts.gstatic.com https://*.ngrok-free.dev",

        // Images from self, data URIs, SVGs, ngrok, and external assets
        "img-src 'self' data: https://www.w3.org/2000/svg https://ngrok.com https://*.ngrok-free.dev",

        // WebSocket and API connections
        "connect-src 'self' ws://localhost:8081 wss://*.ngrok-free.dev http://localhost:11434 http://60.51.17.97:9501 https://*.ngrok-free.dev"
      ].join('; ')
    }
  }
})
