import React, { useContext } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import './../css/NavBar.css';
import { AuthContext } from './../AuthContext';

function NavBar() {
    const { isAuthenticated, setIsAuthenticated } = useContext(AuthContext);
    const navigate = useNavigate();
    const location = useLocation();

    const handleLogout = () => {
        setIsAuthenticated(false);
        const firstName = localStorage.getItem('searchFirstName');
        const lastName = localStorage.getItem('searchLastName');

        if (firstName || lastName) {
            localStorage.removeItem('searchFirstName');
            localStorage.removeItem('searchLastName');
        }
        localStorage.removeItem('token');
        localStorage.removeItem('firstName');
        localStorage.removeItem('lastName');
        navigate('/');
    };

    const getNavLinkClass = (path) => {
        return location.pathname === path ? 'nav-item active' : 'nav-item';
    };

    return (
        <nav className="navbar navbar-expand-lg navbar-custom">
            <div className="container">
                <Link className="navbar-brand logo" to="/">IPSMDB</Link>
                <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                    <span className="navbar-toggler-icon"></span>
                </button>
                <div className="collapse navbar-collapse justify-content-end" id="navbarNav">
                    <ul className="navbar-nav">
                        <li className={getNavLinkClass('/')}><Link className="nav-link" to="/">Acasă</Link></li>
                        {!isAuthenticated && <li className={getNavLinkClass('/register')}><Link className="nav-link" to="/register">Înregistrare</Link></li>}
                        {!isAuthenticated && <li className={getNavLinkClass('/login')}><Link className="nav-link" to="/login">Autentificare</Link></li>}
                        {isAuthenticated && <li className={getNavLinkClass('/favorite-restaurants')}><Link className="nav-link" to="/favorite-restaurants">Restaurante preferate</Link></li>}
                        {isAuthenticated && <li className={getNavLinkClass('/search')}><Link className="nav-link" to="/search">Caută</Link></li>}
                        {isAuthenticated && <li className={getNavLinkClass('/profile')}><Link className="nav-link" to="/profile">Profil</Link></li>}
                        {isAuthenticated && <li className={getNavLinkClass('/friends')}><Link className="nav-link" to="/friends">Prietenii mei</Link></li>}
                        {isAuthenticated && <li className={getNavLinkClass('/restaurants')}><Link className="nav-link" to="/restaurants">Restaurante</Link></li>}
                        {isAuthenticated && <li className="nav-item"><Link className="nav-link" to="/" onClick={handleLogout}>Logout</Link></li>}
                    </ul>
                </div>
            </div>
        </nav>
    );
}

export default NavBar;
