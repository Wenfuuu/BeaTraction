import { toast as sonnerToast } from 'sonner'

interface ToastOptions {
  description?: string
}

export const toast = {
  success: (title: string, options?: ToastOptions) => {
    sonnerToast.success(title, {
      description: options?.description,
      style: {
        background: '#f0fdf4',
        border: '1px solid #86efac',
        color: '#166534',
      },
    })
  },

  error: (title: string, options?: ToastOptions) => {
    sonnerToast.error(title, {
      description: options?.description,
      style: {
        background: '#fef2f2',
        border: '1px solid #fecaca',
        color: '#991b1b',
      },
    })
  },

  info: (title: string, options?: ToastOptions) => {
    sonnerToast.info(title, {
      description: options?.description,
      style: {
        background: '#eff6ff',
        border: '1px solid #93c5fd',
        color: '#1e40af',
      },
    })
  },

  warning: (title: string, options?: ToastOptions) => {
    sonnerToast.warning(title, {
      description: options?.description,
      style: {
        background: '#fffbeb',
        border: '1px solid #fde047',
        color: '#92400e',
      },
    })
  },
}
