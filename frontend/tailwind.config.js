/** @type {import('tailwindcss').Config} */
export default {
  darkMode: ['class'],
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    extend: {
      colors: {
        laerdal: {
          navy:      '#0D1B2A',
          card:      '#101e2e',
          red:       '#C8102E',
          'red-dark':'#A00D25',
        },
        content: {
          bg:      '#f0f4f8',
          surface: '#ffffff',
          border:  '#e5eaef',
        },
        text: {
          primary:   '#1a2a3a',
          secondary: '#8a9faf',
          dim:       '#5a7a90',
        },
      },
      fontFamily: {
        sans: ['Inter', 'Segoe UI', 'system-ui', 'sans-serif'],
      },
    },
  },
  plugins: [],
}
