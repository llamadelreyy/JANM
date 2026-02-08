/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        border: "oklch(0.929 0.013 255.508)",
        input: "oklch(0.929 0.013 255.508)",
        ring: "oklch(0.704 0.04 256.788)",
        background: "oklch(1 0 0)",
        foreground: "oklch(0.129 0.042 264.695)",
        primary: {
          DEFAULT: "oklch(0.557 0.149 252.847)",
          foreground: "oklch(0.984 0.003 247.858)",
        },
        secondary: {
          DEFAULT: "oklch(0.968 0.007 247.896)",
          foreground: "oklch(0.208 0.042 265.755)",
        },
        destructive: {
          DEFAULT: "oklch(0.577 0.245 27.325)",
          foreground: "oklch(0.984 0.003 247.858)",
        },
        muted: {
          DEFAULT: "oklch(0.968 0.007 247.896)",
          foreground: "oklch(0.554 0.046 257.417)",
        },
        accent: {
          DEFAULT: "oklch(0.968 0.007 247.896)",
          foreground: "oklch(0.208 0.042 265.755)",
        },
        popover: {
          DEFAULT: "oklch(1 0 0)",
          foreground: "oklch(0.129 0.042 264.695)",
        },
        card: {
          DEFAULT: "oklch(1 0 0)",
          foreground: "oklch(0.129 0.042 264.695)",
        },
      },
      borderRadius: {
        lg: "0.625rem",
        md: "calc(0.625rem - 2px)",
        sm: "calc(0.625rem - 4px)",
      },
      fontFamily: {
        sans: ['Inter', 'sans-serif'],
      },
    },
  },
  plugins: [],
}