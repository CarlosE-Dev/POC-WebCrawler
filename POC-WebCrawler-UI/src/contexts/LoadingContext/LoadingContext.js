import React, { createContext, useState, useContext } from 'react';

const LoadingContext = createContext();

export function useLoading() {
  return useContext(LoadingContext);
}

export function LoadingProvider({ children }) {
  const [isLoading, setLoading] = useState(false);

  return (
    <LoadingContext.Provider value={{ isLoading, setLoading }}>
      {children}
    </LoadingContext.Provider>
  );
}