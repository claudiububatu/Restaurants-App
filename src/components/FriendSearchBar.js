import React, { useState } from 'react';

function FriendSearchBar({ friends, onSearch }) {
    const [query, setQuery] = useState('');

    return (
        <div className="SearchBar">
            <label>Cautare</label>
            <div className="search-container">
                <input className="search-input" type="text" value={query} onChange={(e) => { setQuery(e.target.value); onSearch(e.target.value); }} />
            </div>
        </div>
    );
}

export default FriendSearchBar;