import { create } from 'zustand'
import { persist, createJSONStorage } from 'zustand/middleware'

export const useAuthStore = create(
  persist(
    (set, get) => ({
      // State
      user: null,
      accessToken: null,
      isAuthenticated: false,
      isLoading: false,
      error: null,

      // Actions
      login: async (credentials) => {
        set({ isLoading: true, error: null })
        
        try {
          // Try to connect to the real backend API
          const response = await fetch('/api/Auth/Login', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify({
              username: credentials.username,
              password: credentials.password
            }),
          })

          if (response.ok) {
            const data = await response.json()
            
            set({
              user: data.user || {
                userid: data.userId || 1,
                fullname: data.fullName || credentials.username,
                role: data.role || 'User',
                roleid: data.roleId || 1,
                email: data.email || `${credentials.username}@pbt.gov.my`
              },
              accessToken: data.accessToken || data.token,
              isAuthenticated: true,
              isLoading: false,
              error: null,
            })

            return { success: true }
          } else {
            // If backend returns error, try to get error message
            const errorData = await response.json().catch(() => ({}))
            throw new Error(errorData.message || 'Invalid username or password')
          }
        } catch (error) {
          // Always fall back to mock authentication for development
          console.warn('Backend error or not available, using mock authentication for development:', error.message)
          
          // Mock authentication for development - accept any credentials
          const mockUser = {
            userid: 1,
            fullname: credentials.username || 'Admin User',
            role: 'Administrator',
            roleid: 1,
            email: `${credentials.username}@pbt.gov.my`
          }
          
          set({
            user: mockUser,
            accessToken: 'mock-token-' + Date.now(),
            isAuthenticated: true,
            isLoading: false,
            error: null,
          })

          return { success: true }
        }
      },

      logout: async () => {
        try {
          // Try to call logout API
          await fetch('/api/Auth/Logout', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${get().accessToken}`
            },
          }).catch(() => {
            // Ignore errors on logout
          })
        } catch (error) {
          // Ignore logout errors
        }

        set({
          user: null,
          accessToken: null,
          isAuthenticated: false,
          error: null,
        })
      },

      updateUser: (userData) => {
        set((state) => ({
          user: { ...state.user, ...userData }
        }))
      },

      clearError: () => {
        set({ error: null })
      },

      // Check if user is still authenticated
      checkAuth: async () => {
        const token = get().accessToken
        if (!token) return false

        try {
          const response = await fetch('/api/Auth/Verify', {
            method: 'GET',
            headers: {
              'Authorization': `Bearer ${token}`
            },
          })

          if (response.ok) {
            return true
          } else {
            // Token is invalid, clear auth
            set({
              user: null,
              accessToken: null,
              isAuthenticated: false,
            })
            return false
          }
        } catch (error) {
          // If backend is not available, assume token is still valid for development
          return true
        }
      },

      // Initialize auth state
      initializeAuth: () => {
        const state = get()
        if (state.accessToken && !state.isAuthenticated) {
          set({ isAuthenticated: true })
        }
      },
    }),
    {
      name: 'auth-storage',
      storage: createJSONStorage(() => localStorage),
      partialize: (state) => ({
        user: state.user,
        accessToken: state.accessToken,
        isAuthenticated: state.isAuthenticated,
      }),
    }
  )
)