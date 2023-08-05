import React from 'react';
import './Loading.css';

function Loading() {
  return (
    <div className="spinner-container">
      <div className="spinner-border" role="status">
          <span className="sr-only"></span>
      </div>
    </div>
  )
}

export default Loading;