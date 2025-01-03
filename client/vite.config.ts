import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  //TODO: configure Vitest
  // test: {
  //   globals: true,
  //   environment: 'jsdom'
  // }
})
