import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      '/CollaborativeShoppingListAPI': {
        target: 'http://localhost:5066',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/CollaborativeShoppingListAPI/, ''),
      },
    },
  },
  //TODO: configure Vitest
  // test: {
  //   globals: true,
  //   environment: 'jsdom'
  // }
})
