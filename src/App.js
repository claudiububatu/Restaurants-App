import React, { useState } from 'react';
import { HashRouter as Router, Routes, Route } from 'react-router-dom';
import NavBar from './components/NavBar';
import RegisterForm from './components/RegisterForm';
import LoginForm from './components/LoginForm';
import HomePage from './components/HomePage';
import FavoriteRestaurants from './components/FavoriteRestaurants';
import SearchBar from './components/SearchBar';
import Profile from './components/Profile';
import Friends from './components/Friends';
import RestaurantsPage from './components/RestaurantsPage';
import { AuthContext } from './AuthContext';
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    return (
        <AuthContext.Provider value={{ isAuthenticated, setIsAuthenticated }}>
            <Router>
                <NavBar />
                <div className="main-content">
                    <Routes>
                        <Route path="/register" element={<RegisterForm />} />
                        <Route path="/login" element={<LoginForm />} />
                        <Route path="/favorite-restaurants" element={<FavoriteRestaurants />} />
                        <Route path="/search" element={<SearchBar />} />
                        <Route path="/profile" element={<Profile />} />
                        <Route path="/profile/:firstName/:lastName" element={<Profile />} />
                        <Route path="/friends" element={<Friends />} />
                        <Route path="/restaurants" element={<RestaurantsPage />} />
                        <Route path="/" element={<HomePage />} />
                    </Routes>
                </div>
            </Router>
        </AuthContext.Provider>
    );
}

export default App;