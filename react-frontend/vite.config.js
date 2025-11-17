import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { resolve } from 'path'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": resolve(process.cwd(), "./src"),
    },
  },
  server: {
    port: 2002,
    host: true,
    allowedHosts: [
      'e7b777cc0b96.ngrok-free.app',
      'sarah-noninclinational-ingrately.ngrok-free.dev',
      'regardless-stake-knife-materials.trycloudflare.com',
      // Allow all ngrok hosts for development
      /\.ngrok-free\.(app|dev)$/,
      /\.ngrok\.io$/,
      /\.ngrok\.app$/
    ],
    proxy: {
      '/chat-api': {
        target: 'http://localhost:3002',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/chat-api/, '/api'),
        configure: (proxy, options) => {
          proxy.on('error', (err, req, res) => {
            console.log('Chat API Proxy error:', err);
          });
          proxy.on('proxyReq', (proxyReq, req, res) => {
            console.log('Proxying chat request to:', proxyReq.path);
          });
        }
      },
      '/api/chat': {
        target: 'http://localhost:3002',
        changeOrigin: true,
        secure: false,
        configure: (proxy, options) => {
          proxy.on('error', (err, req, res) => {
            console.log('Chat API Proxy error:', err);
          });
          proxy.on('proxyReq', (proxyReq, req, res) => {
            console.log('Proxying chat request to:', proxyReq.path);
          });
        }
      },
      '/api/whisper': {
        target: 'http://localhost:3002',
        changeOrigin: true,
        secure: false,
        configure: (proxy, options) => {
          proxy.on('error', (err, req, res) => {
            console.log('Whisper API Proxy error:', err);
          });
          proxy.on('proxyReq', (proxyReq, req, res) => {
            console.log('Proxying whisper request to:', proxyReq.path);
          });
        }
      },
      '/v1': {
        target: 'http://localhost:14501',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/v1/, '/v1'),
        configure: (proxy, options) => {
          proxy.on('error', (err, req, res) => {
            console.log('LLM API Proxy error:', err);
          });
          proxy.on('proxyReq', (proxyReq, req, res) => {
            console.log('Proxying LLM request to:', proxyReq.path);
          });
        }
      },
    },
  },
})