import { defineConfig } from 'vite'
import VueSetupExtend from 'vite-plugin-vue-setup-extend'
import vue from '@vitejs/plugin-vue'
import { resolve } from 'path' 
 
const pathResolve = (dir: string): any => {  
  return resolve(__dirname, ".", dir)          
}
 
const alias: Record<string, string> = {
  '@': pathResolve("src")
}

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [VueSetupExtend(), vue()],
  resolve: {
    alias
  },
  server: {
    cors: true,
    proxy: { 
      '/api': {
        target: 'http://hamster.love:8080/api/', 
        changeOrigin: true, 
        ws: true,
        rewrite: (path) => path.replace(/^\/api/, '') 
      },
      '/chathub':{
        target: 'http://hamster.love:8080/chathub/', 
        changeOrigin: true, 
        ws: true,
        rewrite: (path) => path.replace(/^\/chathub/, '') 
      }
    }
  }
})

