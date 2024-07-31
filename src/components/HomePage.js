import React from 'react';
import { Link } from 'react-router-dom';
import './../css/HomePage.css';

function HomePage() {
    return (
        <div className="home-page">
            <h1>Bine ai venit pe pagina noastră!</h1>
            <div className="image-link-container">
                <Link to="/restaurants" className="home-image-link">
                    <div className="image-background">
                        <div className="image-text">Vezi restaurantele noastre</div>
                    </div>
                </Link>
            </div>
        </div>
    );
}

export default HomePage;
