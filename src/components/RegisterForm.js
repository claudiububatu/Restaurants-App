import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import './../css/RegisterForm.css';

function RegisterForm() {
    const [form, setForm] = useState({ username: "", nume: "", prenume: "", email: "", parola: "", validareParola: "", role: "User" });
    const [message, setMessage] = useState("");
    const [isSuccessful, setIsSuccessful] = useState(false);
    const navigate = useNavigate();

    const handleChange = e => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = e => {
        e.preventDefault();
        if (form.parola === form.validareParola) {
            axios.post('https://localhost:7262/Auth/register', {
                Username: form.username,
                Email: form.email,
                Password: form.parola,
                FirstName: form.prenume,
                LastName: form.nume,
                Role: form.role
            })
                .then(response => {
                    console.log(response);
                    setMessage("Contul a fost înregistrat cu succes!");
                    setIsSuccessful(true);
                    setTimeout(() => {
                        navigate('/login');
                    }, 2000);
                })
                .catch(error => {
                    console.log(error);
                    setMessage("A apărut o eroare la înregistrare.");
                    setIsSuccessful(false);
                });
        } else {
            setMessage('Parolele nu se potrivesc!');
        }
    };

    return (
        <div className="register-form-container">
            <div className="register-form">
                <form onSubmit={handleSubmit}>
                    <input type="text" name="username" onChange={handleChange} placeholder="Nume de utilizator" required />
                    <input type="text" name="prenume" onChange={handleChange} placeholder="Prenume" required />
                    <input type="text" name="nume" onChange={handleChange} placeholder="Nume" required />
                    <input type="email" name="email" onChange={handleChange} placeholder="Email" required />
                    <input type="password" name="parola" onChange={handleChange} placeholder="Parolă" required />
                    <input type="password" name="validareParola" onChange={handleChange} placeholder="Validare parolă" required />
                    <input type="hidden" name="role" value="User" />
                    <button type="submit">Înregistrează-te</button>
                </form>
                {message && <p className={`message ${isSuccessful ? 'success' : 'error'}`}>{message}</p>}
            </div>
        </div>
    );
}

export default RegisterForm;
