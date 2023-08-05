import toast from 'react-hot-toast';

export const showSuccessToast = (message) => {
        toast.success(`${message}`, {
        duration: 6000,
        position: 'bottom-center',
        style: {
            background: '#363636',
            color: '#fff'
        }
    });
};

export const showErrorToast = (message) => {
    toast.error(`${message}`, {
        duration: 6000,
        position: 'bottom-center',
        style: {
            background: '#363636',
            color: '#fff'
        }
    });
}