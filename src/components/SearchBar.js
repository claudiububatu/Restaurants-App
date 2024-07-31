import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import './../css/SearchBar.css'; // importăm fișierul CSS

const getFilteredItems = (query, items) => {
    if (!query || query.length < 2) {
        return [];
    }
    return items.filter((item) => item.firstName.includes(query) || item.lastName.includes(query));
};

function SearchBar() {
    const [query, setQuery] = useState('');
    const [data, setData] = useState([]);
    const navigate = useNavigate();

    useEffect(() => {
        axios.get('https://localhost:7262/api/Person')
            .then((response) => {
                setData(response.data);
            })
            .catch((error) => {
                console.error(`There was an error retrieving the data: ${error}`);
            });
    }, []);

    const filteredItems = getFilteredItems(query, data);

    const handleSuggestionClick = (firstName, lastName) => {
        setQuery(`${firstName} ${lastName}`);
    };

    const handleSearch = (event) => {
        event.preventDefault();
        const [searchFirstName, searchLastName] = query.split(' ');
        axios.get(`https://localhost:7262/Auth/getProfile/${searchFirstName}/${searchLastName}`)
            .then((response) => {
                localStorage.setItem('searchFirstName', searchFirstName);
                localStorage.setItem('searchLastName', searchLastName);
                navigate(`/profile/${searchFirstName}/${searchLastName}`);
            })
            .catch((error) => {
                console.error(`There was an error retrieving the profile data: ${error}`);
            });
    };

    return (
        <div className="search-bar-container">
            <div className="SearchBar">
                <label>Căutare</label>
                <div className="search-container">
                    <input className="search-input" type="text" value={query} onChange={(e) => setQuery(e.target.value)} />
                    <button className="search-button" onClick={handleSearch}>Caută</button> {/* Butonul nou adăugat */}
                </div>
                <ul className="suggestions">
                    {filteredItems.map((value) => (
                        <li className="suggestion" key={value.id} onClick={() => handleSuggestionClick(value.firstName, value.lastName)}>
                            {value.firstName} {value.lastName}
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
}

export default SearchBar;
