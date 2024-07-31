import React, { useState, useContext, useEffect } from 'react';
import axios from 'axios';
import { AuthContext } from './../AuthContext';
import './../css/LoginForm.css'; // importăm fișierul CSS
import { Link, useNavigate } from 'react-router-dom';

function LoginForm() {
    const [form, setForm] = useState({ username: "", parola: "" });
    const [message, setMessage] = useState("");
    const [isSuccessful, setIsSuccessful] = useState(false);
    const { isAuthenticated, setIsAuthenticated } = useContext(AuthContext);
    const navigate = useNavigate();

    useEffect(() => {
        if (isAuthenticated) {
            navigate('/');
        }
    }, [isAuthenticated]);

    const handleChange = e => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = e => {
        e.preventDefault();
        axios.post('https://localhost:7262/Auth/login', {
            Username: form.username,
            Password: form.parola,
        })
            .then(response => {
                console.log(response);
                localStorage.setItem('token', response.data.token);
                setMessage("Autentificare reușită!");
                setIsSuccessful(true);
                setIsAuthenticated(true);
                axios.get('https://localhost:7262/Auth/getProfile', {
                    headers: {
                        'Authorization': `Bearer ${response.data.token}`
                    }
                })
                    .then(profileResponse => {
                        localStorage.setItem('firstName', profileResponse.data.firstName);
                        localStorage.setItem('lastName', profileResponse.data.lastName);
                    })
                    .catch(error => {
                        console.error(`There was an error retrieving the profile data: ${error}`);
                    });
            })
            .catch(error => {
                console.log(error);
                setMessage("A apărut o eroare la autentificare.");
                setIsSuccessful(false);
                setIsAuthenticated(false); 
            });
    };

    return (
        <div className="login-form-container">
            <div className="login-form">
                <form onSubmit={handleSubmit}>
                    <input type="text" name="username" onChange={handleChange} placeholder="Nume de utilizator" required />
                    <input type="password" name="parola" onChange={handleChange} placeholder="Parolă" required />
                    <button type="submit">Autentificare</button>
                </form>
                {message && <p className={`${isSuccessful ? 'success' : 'error'}`}>{message}</p>} {}
            </div>
        </div>
    );
}

export default LoginForm;
