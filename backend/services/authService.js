/**
 * Simple Authentication Service
 * Handles user registration and login
 */

import fs from 'fs/promises'
import path from 'path'
import crypto from 'crypto'

const USERS_FILE = path.join(process.cwd(), './data/users.json')

class AuthService {
  constructor() {
    this.users = []
    this.initialize()
  }

  async initialize() {
    try {
      // Ensure data directory exists
      const dataDir = path.dirname(USERS_FILE)
      try {
        await fs.access(dataDir)
      } catch {
        await fs.mkdir(dataDir, { recursive: true })
      }

      // Load users from file
      try {
        const data = await fs.readFile(USERS_FILE, 'utf8')
        this.users = JSON.parse(data)
      } catch (error) {
        // File doesn't exist, create empty users file
        this.users = []
        await this.saveUsers()
      }
    } catch (error) {
      console.error('Failed to initialize auth service:', error)
      this.users = []
    }
  }

  async saveUsers() {
    const dataDir = path.dirname(USERS_FILE)
    await fs.mkdir(dataDir, { recursive: true })
    await fs.writeFile(USERS_FILE, JSON.stringify(this.users, null, 2))
  }

  hashPassword(password) {
    return crypto.createHash('sha256').update(password).digest('hex')
  }

  generateToken() {
    return crypto.randomBytes(32).toString('hex')
  }

  async register(username, email, password) {
    // Check if user already exists
    if (this.users.find(u => u.username === username || u.email === email)) {
      throw new Error('User already exists')
    }

    const user = {
      id: Date.now().toString(),
      username,
      email,
      password: this.hashPassword(password),
      token: this.generateToken(),
      createdAt: new Date().toISOString()
    }

    this.users.push(user)
    await this.saveUsers()

    return {
      id: user.id,
      username: user.username,
      email: user.email,
      token: user.token
    }
  }

  async login(email, password) {
    const user = this.users.find(u => u.email === email && u.password === this.hashPassword(password))
    
    if (!user) {
      throw new Error('Invalid credentials')
    }

    // Generate new token
    user.token = this.generateToken()
    await this.saveUsers()

    return {
      id: user.id,
      username: user.username,
      email: user.email,
      token: user.token
    }
  }

  validateToken(token) {
    const user = this.users.find(u => u.token === token)
    if (!user) {
      return null
    }
    return {
      id: user.id,
      username: user.username,
      email: user.email
    }
  }

  async logout(token) {
    const user = this.users.find(u => u.token === token)
    if (user) {
      user.token = null
      await this.saveUsers()
    }
    return true
  }
}

const authService = new AuthService()
export default authService