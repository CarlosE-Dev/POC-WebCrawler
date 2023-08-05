import React from 'react';
import { useLoading } from '../../contexts/LoadingContext/LoadingContext';
import Loading from '../Loading/Loading';
import './HeaderOffCanvas.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min';

function HeaderOffCanvas({ children }) {
  const { isLoading } = useLoading();

  return (
    <div className="oc-container">
        <button
          className="navbar-toggler btn-canv"
          type="button"
          data-bs-toggle="offcanvas"
          data-bs-target="#offcanvasRight"
          aria-controls="offcanvasRight"
          disabled={isLoading}
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>

        <div className="offcanvas offcanvas-end text-bg-dark" tabIndex="-1" id="offcanvasRight" aria-labelledby="offcanvasRightLabel">
        <div className="offcanvas-header">
            <h5 className="offcanvas-title" id="offcanvasRightLabel">SearchData App</h5>
            <button type="button" className="btn-close" data-bs-dismiss="offcanvas" aria-label="Close" disabled={isLoading}></button>
        </div>
        {!isLoading ? (
          <div className="offcanvas-body">
            {children}
          </div>
        ) : 
        (
          <Loading></Loading>
        )}
        </div>
    </div>
  )
}

export default HeaderOffCanvas;