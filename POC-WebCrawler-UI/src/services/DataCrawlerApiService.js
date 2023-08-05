import axios from 'axios';

const API_BASE_URL = 'https://localhost:8080/api/v1/crawler/';

const apiService = axios.create({
  baseURL: API_BASE_URL,
});

export const Execute = async (endpoint, params = '') => {
  try {
    if(params !== ''){
      const response = await apiService.get(endpoint, {
        params: params
    });

      return response.data;
    }
    const response = await apiService.get(endpoint);
    return response.data;
  } 
  catch (error) {
    console.error(error);
    throw error;
  }
};