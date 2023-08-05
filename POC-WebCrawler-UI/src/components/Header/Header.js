import './Header.css';
import { BsSearch } from "react-icons/bs";
import React from 'react';
import { useLoading } from '../../contexts/LoadingContext/LoadingContext';
import { Execute } from '../../services/DataCrawlerApiService';
import HeaderOffCanvas from '../OffCanvas/HeaderOffCanvas';
import '@fortawesome/fontawesome-free/css/all.css';
import { showSuccessToast, showErrorToast } from '../../services/ToastService';

const Header = () => {
    const { isLoading, setLoading } = useLoading();
    
    const handleSubmit = async (e) => {
        setLoading(true);

        Execute('')
        .then((data) => {
            setLoading(false);
            showSuccessToast(data);
        })
        .catch((e) => {
            setLoading(false);
            showErrorToast('An error occurred during your request.');
        })
    };

    return (
        <nav className="navbar navbar-dark bg-dark">
            <div className="nav-container">
                <span className="navbar-brand" href="home" disabled={isLoading}> <BsSearch className="search-icon"/> SearchData</span>
            </div>
            <div className="nav-container">
                <HeaderOffCanvas>
                    <div className="children-container">
                     <span className="btn-label">Execute Crawler</span>
                    <button className="btn btn-primary btn-exec" onClick={handleSubmit} disabled={isLoading}>
                        <i className="fa-solid fa-play"></i>
                     </button>
                    </div>
                </HeaderOffCanvas>
            </div>
        </nav>
    );
};
export default Header;